import {
  decodeJwtPayload,
  getJwtArrayClaim,
  getJwtStringClaim,
  type JwtPayload,
} from "~/utils/jwt"
import type {
  AuthSession,
  LoginPayload,
  LoginResponse,
} from "~/types/auth/session"

const refreshCookieName = "mw_storefront_refresh"
const authPath = "/api/public/v1/auth"
const authJsonHeaders = {
  "Content-Type": "application/json",
  accept: "application/json",
} as const

type AuthRuntimeState = {
  refreshPromise: Promise<void> | null
  initPromise: Promise<void> | null
}

function translateAuthMessage(message: string) {
  const normalized = message.trim()

  if (!normalized) return normalized
  if (normalized === "Email is required.") return "Imejl adresa je obavezna."
  if (normalized === "Invalid email format.") {
    return "Imejl adresa nije u ispravnom formatu."
  }
  if (normalized === "Password is required.") return "Lozinka je obavezna."
  if (normalized === "One or more validation errors occurred.") {
    return "Proverite unesene podatke."
  }

  return normalized
}

function extractAuthError(payload: unknown) {
  if (!payload || typeof payload !== "object") return null

  const candidate = payload as {
    title?: unknown
    detail?: unknown
    message?: unknown
    errors?: unknown
  }

  if (candidate.errors && typeof candidate.errors === "object") {
    const entries = Object.entries(candidate.errors as Record<string, unknown>)
    const messages = entries
      .flatMap(([field, value]) => {
        if (!Array.isArray(value)) return []
        const label =
          field === "Email" ? "Imejl" : field === "Password" ? "Lozinka" : field
        return value
          .filter(
            (item): item is string =>
              typeof item === "string" && item.trim().length > 0,
          )
          .map((item) => `${label}: ${translateAuthMessage(item)}`)
      })
      .filter(Boolean)

    if (messages.length) {
      return `${translateAuthMessage("One or more validation errors occurred.")} ${messages.join(" | ")}`
    }
  }

  const directMessage =
    (typeof candidate.detail === "string" && candidate.detail) ||
    (typeof candidate.title === "string" && candidate.title) ||
    (typeof candidate.message === "string" && candidate.message) ||
    ""

  return directMessage ? translateAuthMessage(directMessage) : null
}

function useAuthRuntimeState() {
  const nuxtApp = useNuxtApp() as ReturnType<typeof useNuxtApp> & {
    _mwStorefrontAuthRuntime?: AuthRuntimeState
  }

  if (!nuxtApp._mwStorefrontAuthRuntime) {
    nuxtApp._mwStorefrontAuthRuntime = {
      refreshPromise: null,
      initPromise: null,
    }
  }

  return nuxtApp._mwStorefrontAuthRuntime
}

function useSessionState() {
  return useState<AuthSession>("storefront-auth-session", () => ({
    accessToken: null,
    tokenType: "Bearer",
    created: null,
    expires: null,
    refreshExpires: null,
  }))
}

function parseDate(value: string | null | undefined): Date | null {
  if (!value) return null
  const parsed = new Date(value)
  return Number.isNaN(parsed.getTime()) ? null : parsed
}

function getCookieOptions() {
  const secure = import.meta.client
    ? window.location.protocol === "https:"
    : useRequestURL().protocol === "https:"
  return {
    sameSite: "lax" as const,
    secure,
    path: "/",
  }
}

export function useAuthSessionStore() {
  const config = useRuntimeConfig()
  const runtime = useAuthRuntimeState()
  const session = useSessionState()
  const refreshCookie = useCookie<string | null>(refreshCookieName, {
    ...getCookieOptions(),
    default: () => null,
  })
  const refreshTimer = useState<ReturnType<typeof setTimeout> | null>(
    "storefront-auth-refresh-timer",
    () => null,
  )
  const isInitialized = useState("storefront-auth-is-initialized", () => false)

  const payload = computed<JwtPayload | null>(() => {
    if (!session.value.accessToken) return null
    return decodeJwtPayload(session.value.accessToken)
  })

  const role = computed(
    () =>
      getJwtStringClaim(payload.value, "role") ||
      getJwtStringClaim(
        payload.value,
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
      ),
  )

  const customerId = computed(
    () =>
      getJwtStringClaim(payload.value, "sub") ||
      getJwtStringClaim(
        payload.value,
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      ),
  )

  const permissions = computed(() => getJwtArrayClaim(payload.value, "perm"))

  const isCustomer = computed(() => role.value?.toLowerCase() === "customer")

  const isLoggedIn = computed(
    () => Boolean(session.value.accessToken) && !isAccessExpired(),
  )

  function getAuthBaseUrl() {
    return `${config.public.identityApiBaseUrl}${authPath}`
  }

  function authFetch(
    path: "login" | "logout" | "refresh",
    options: RequestInit,
  ) {
    return fetch(`${getAuthBaseUrl()}/${path}`, options)
  }

  function authJsonPost(
    path: "login" | "refresh",
    body: Record<string, unknown>,
  ) {
    return authFetch(path, {
      method: "POST",
      headers: authJsonHeaders,
      body: JSON.stringify(body),
    })
  }

  function clearRefreshTimer() {
    if (refreshTimer.value) {
      clearTimeout(refreshTimer.value)
      refreshTimer.value = null
    }
  }

  function writeRefreshCookie(token: string | null, expires: Date | null) {
    refreshCookie.value = token
    if (!import.meta.client) return
    if (!token) {
      document.cookie = `${refreshCookieName}=; Max-Age=0; Path=/; SameSite=Lax`
      return
    }
    const attrs = [
      `${refreshCookieName}=${encodeURIComponent(token)}`,
      "Path=/",
      "SameSite=Lax",
    ]
    if (expires) attrs.push(`Expires=${expires.toUTCString()}`)
    if (window.location.protocol === "https:") attrs.push("Secure")
    document.cookie = attrs.join("; ")
  }

  function setSession(data: LoginResponse) {
    session.value = {
      accessToken: data.token,
      tokenType: data.tokenType || "Bearer",
      created: data.created,
      expires: data.expires,
      refreshExpires: data.refreshExpires,
    }
    const refreshExpires = parseDate(data.refreshExpires)
    writeRefreshCookie(data.refreshToken, refreshExpires)
    scheduleRefresh()
  }

  function clearSession() {
    session.value = {
      accessToken: null,
      tokenType: "Bearer",
      created: null,
      expires: null,
      refreshExpires: null,
    }
    writeRefreshCookie(null, null)
    clearRefreshTimer()
  }

  function getAccessToken() {
    if (!session.value.accessToken) return null
    return `${session.value.tokenType} ${session.value.accessToken}`
  }

  function scheduleRefresh() {
    if (!import.meta.client) return
    clearRefreshTimer()
    const created = parseDate(session.value.created)
    const expires = parseDate(session.value.expires)
    if (!created || !expires) return
    const lifetimeMs = Math.max(0, expires.getTime() - created.getTime())
    if (!lifetimeMs) return
    const delay = Math.floor(lifetimeMs * (2 / 3))
    if (delay <= 0) return
    refreshTimer.value = setTimeout(() => {
      void refreshTokens()
    }, delay)
  }

  async function login(credentials: LoginPayload) {
    const response = await authJsonPost("login", credentials)

    if (!response.ok) {
      if (response.status === 401) {
        throw new Error("Imejl adresa ili lozinka nisu ispravni.")
      }
      const payload = await response.json().catch(() => null)
      throw new Error(extractAuthError(payload) || "Prijava nije uspela.")
    }

    const data = (await response.json()) as LoginResponse
    setSession(data)
    if (!isCustomer.value) {
      clearSession()
      throw new Error("Za pristup prodavnici potrebna je uloga kupca.")
    }
  }

  async function logout() {
    const token = getAccessToken()
    if (token) {
      await authFetch("logout", {
        method: "POST",
        headers: {
          Authorization: token,
        },
      }).catch(() => null)
    }
    clearSession()
  }

  async function refreshTokens() {
    if (runtime.refreshPromise) return runtime.refreshPromise
    const refreshToken = refreshCookie.value
    if (!refreshToken) return
    runtime.refreshPromise = (async () => {
      try {
        const response = await authJsonPost("refresh", { refreshToken })

        if (!response.ok) {
          clearSession()
          return
        }

        const data = (await response.json()) as LoginResponse
        setSession(data)
        if (!isCustomer.value) {
          clearSession()
        }
      } catch {
        clearSession()
      }
    })()

    try {
      await runtime.refreshPromise
    } finally {
      runtime.refreshPromise = null
    }
  }

  function isAccessExpired() {
    const expires = parseDate(session.value.expires)
    if (!expires) return true
    return expires.getTime() <= Date.now()
  }

  async function initialize() {
    if (runtime.initPromise) return runtime.initPromise
    runtime.initPromise = (async () => {
      if (session.value.accessToken) {
        if (!isAccessExpired()) {
          scheduleRefresh()
          isInitialized.value = true
          return
        }
        session.value.accessToken = null
      }

      if (refreshCookie.value) {
        try {
          await refreshTokens()
        } catch {
          clearSession()
        }
      } else {
        clearRefreshTimer()
      }

      isInitialized.value = true
    })()

    try {
      await runtime.initPromise
    } finally {
      runtime.initPromise = null
    }
  }

  return {
    session,
    payload,
    role,
    customerId,
    permissions,
    isCustomer,
    isLoggedIn,
    login,
    logout,
    refreshTokens,
    getAccessToken,
    initialize,
    isAccessExpired,
    isInitialized,
  }
}
