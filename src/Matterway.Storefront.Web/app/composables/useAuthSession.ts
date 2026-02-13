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
const authPath = "/api/v1.0/public/auth"
const authJsonHeaders = {
  "Content-Type": "application/json",
  accept: "application/json",
} as const

let refreshPromise: Promise<void> | null = null
let initPromise: Promise<void> | null = null

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
    : false
  return {
    sameSite: "lax" as const,
    secure,
    path: "/",
  }
}

export function useAuthSession() {
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
    const config = useRuntimeConfig()
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
      const payload = await response.json().catch(() => null)
      throw new Error(payload?.title || "Prijava nije uspela.")
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
    if (refreshPromise) return refreshPromise
    const refreshToken = refreshCookie.value
    if (!refreshToken) return
    refreshPromise = (async () => {
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
      await refreshPromise
    } finally {
      refreshPromise = null
    }
  }

  function isAccessExpired() {
    const expires = parseDate(session.value.expires)
    if (!expires) return true
    return expires.getTime() <= Date.now()
  }

  async function initialize() {
    if (isInitialized.value) return
    if (initPromise) return initPromise
    initPromise = (async () => {
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
      }
      isInitialized.value = true
    })()

    try {
      await initPromise
    } finally {
      initPromise = null
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
