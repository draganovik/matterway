import {
  decodeJwtPayload,
  getJwtStringClaim,
  type JwtPayload,
} from "~/utils/jwt"
import { getApiErrorMessage } from "~/utils/apiErrors"
import { buildServiceApiPath } from "~/utils/apiProxy"
import type {
  AuthSession,
  LoginPayload,
  LoginResponse,
} from "~/types/auth/session"

const refreshCookieName = "mw_storefront_refresh"
const refreshLockName = "mw-storefront-auth-refresh"
const refreshRetryDelayMs = 30_000
const authBasePath = buildServiceApiPath("identity", "public", "auth")
const authJsonHeaders = {
  accept: "application/json",
} as const

type AuthRuntimeState = {
  refreshPromise: Promise<void> | null
  initPromise: Promise<void> | null
  refreshTimer: ReturnType<typeof setTimeout> | null
}

function extractAuthError(payload: unknown) {
  if (!payload || typeof payload !== "object") return null

  const candidate = payload as { errors?: unknown }

  if (candidate.errors && typeof candidate.errors === "object") {
    const entries = Object.entries(candidate.errors as Record<string, unknown>)
    const messages = entries
      .flatMap(([, value]) => {
        if (!Array.isArray(value)) return []
        return value
          .filter(
            (item): item is string =>
              typeof item === "string" && item.trim().length > 0,
          )
          .map((item) => item.trim())
      })
      .filter(Boolean)

    if (messages.length) {
      return messages.join(" | ")
    }
  }

  return getApiErrorMessage(payload)
}

function useAuthRuntimeState() {
  const nuxtApp = useNuxtApp() as ReturnType<typeof useNuxtApp> & {
    _mwStorefrontAuthRuntime?: AuthRuntimeState
  }

  if (!nuxtApp._mwStorefrontAuthRuntime) {
    nuxtApp._mwStorefrontAuthRuntime = {
      refreshPromise: null,
      initPromise: null,
      refreshTimer: null,
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
  const runtime = useAuthRuntimeState()
  const session = useSessionState()
  const refreshTokenCookie = useCookie<string | null>(refreshCookieName, {
    ...getCookieOptions(),
    default: () => null,
  })
  const isInitialized = useState("storefront-auth-is-initialized", () => false)
  const customerFirstName = useState<string | null>(
    "storefront-customer-first-name",
    () => null,
  )
  const hasRefreshSession = computed(() => Boolean(refreshTokenCookie.value))

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

  const isCustomer = computed(() => role.value?.toLowerCase() === "customer")

  const isLoggedIn = computed(
    () => Boolean(session.value.accessToken) && !isAccessExpired(),
  )

  function authFetch(
    path: "login" | "logout" | "refresh",
    options: Exclude<Parameters<typeof $fetch.raw>[1], undefined>,
  ) {
    return $fetch.raw(`${authBasePath}/${path}`, {
      ...options,
      ignoreResponseError: true,
    })
  }

  function authJsonPost(
    path: "login" | "refresh",
    body: Record<string, unknown>,
  ) {
    return authFetch(path, {
      method: "POST",
      headers: authJsonHeaders,
      body,
    })
  }

  function clearRefreshTimer() {
    if (runtime.refreshTimer) {
      clearTimeout(runtime.refreshTimer)
      runtime.refreshTimer = null
    }
  }

  function readRefreshToken() {
    if (import.meta.client) {
      const prefix = `${refreshCookieName}=`
      const rawValue = document.cookie
        .split("; ")
        .find((cookie) => cookie.startsWith(prefix))
        ?.slice(prefix.length)

      if (!rawValue) return null
      try {
        return decodeURIComponent(rawValue)
      } catch {
        return rawValue
      }
    }

    return refreshTokenCookie.value
  }

  function writeRefreshCookie(token: string | null, expires: Date | null) {
    if (!import.meta.client) {
      refreshTokenCookie.value = token
      return
    }
    if (!token) {
      document.cookie = `${refreshCookieName}=; Max-Age=0; Path=/; SameSite=Lax`
      refreshCookie(refreshCookieName)
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
    refreshCookie(refreshCookieName)
  }

  function setSession(data: LoginResponse) {
    session.value = {
      accessToken: data.token,
      tokenType: data.tokenType || "Bearer",
      created: data.created,
      expires: data.expires,
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
    }
    writeRefreshCookie(null, null)
    clearRefreshTimer()
    customerFirstName.value = null
  }

  function setCustomerFirstName(value: string | null | undefined) {
    customerFirstName.value = value?.trim() || null
  }

  function getAccessToken() {
    if (!session.value.accessToken || isAccessExpired()) return null
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
    const refreshAt = created.getTime() + Math.floor(lifetimeMs * (2 / 3))
    const delay = Math.max(0, refreshAt - Date.now())
    runtime.refreshTimer = setTimeout(() => {
      void refreshTokens()
    }, delay)
  }

  function scheduleRefreshRetry() {
    if (!import.meta.client || !readRefreshToken()) return
    clearRefreshTimer()
    runtime.refreshTimer = setTimeout(() => {
      void refreshTokens()
    }, refreshRetryDelayMs)
  }

  async function withRefreshLock(callback: () => Promise<void>) {
    if (!import.meta.client || !("locks" in navigator)) {
      await callback()
      return
    }

    await navigator.locks.request(refreshLockName, callback)
  }

  async function login(credentials: LoginPayload) {
    const response = await authJsonPost("login", credentials).catch(() => null)

    if (!response) {
      throw new Error("Prijava trenutno nije moguća. Pokušajte ponovo.")
    }

    if (!response.ok) {
      if (response.status === 401) {
        throw new Error("Imejl adresa ili lozinka nisu ispravni.")
      }
      throw new Error(
        extractAuthError(response._data) || "Prijava nije uspela.",
      )
    }

    const data = response._data as LoginResponse | undefined
    if (!data) {
      throw new Error("Prijava nije uspela.")
    }
    setSession(data)
    if (!isCustomer.value) {
      clearSession()
      throw new Error("Za pristup prodavnici potrebna je uloga kupca.")
    }
  }

  async function logout() {
    await initialize()
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
    runtime.refreshPromise = withRefreshLock(async () => {
      const refreshToken = readRefreshToken()
      if (!refreshToken) return

      const response = await authJsonPost("refresh", { refreshToken }).catch(
        () => null,
      )

      if (!response) {
        scheduleRefreshRetry()
        return
      }

      if (!response.ok) {
        if (
          response.status === 408 ||
          response.status === 425 ||
          response.status === 429 ||
          response.status >= 500
        ) {
          scheduleRefreshRetry()
          return
        }

        clearSession()
        return
      }

      const data = response._data as LoginResponse | undefined
      if (!data) {
        scheduleRefreshRetry()
        return
      }

      setSession(data)
      if (!isCustomer.value) clearSession()
    })

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
      }

      if (readRefreshToken()) {
        try {
          await refreshTokens()
        } catch {
          scheduleRefreshRetry()
        }
      } else {
        clearSession()
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
    role,
    customerId,
    customerFirstName,
    isCustomer,
    isLoggedIn,
    hasRefreshSession,
    login,
    logout,
    refreshTokens,
    getAccessToken,
    setCustomerFirstName,
    initialize,
    isInitialized,
  }
}
