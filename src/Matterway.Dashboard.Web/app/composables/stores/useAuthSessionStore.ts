import {
  decodeJwtPayload,
  getJwtArrayClaim,
  getJwtStringClaim,
  type JwtPayload,
} from "~/utils/jwt"
import { buildServiceApiPath } from "~/utils/apiProxy"
import type { AuthSession, LoginResponse } from "~/types/auth/session"
import type { PermissionLevel } from "~/types/services/definitions"

const refreshCookieName = "mw_refresh"
const authBasePath = buildServiceApiPath("identity", "public", "auth")
const authJsonHeaders = {
  "Content-Type": "application/json",
  accept: "application/json",
} as const

const allPermissions: PermissionLevel[] = ["observer", "operator", "manager"]

type AuthRuntimeState = {
  refreshPromise: Promise<void> | null
  initPromise: Promise<void> | null
}

function useAuthRuntimeState() {
  const nuxtApp = useNuxtApp() as ReturnType<typeof useNuxtApp> & {
    _mwDashboardAuthRuntime?: AuthRuntimeState
  }

  if (!nuxtApp._mwDashboardAuthRuntime) {
    nuxtApp._mwDashboardAuthRuntime = {
      refreshPromise: null,
      initPromise: null,
    }
  }

  return nuxtApp._mwDashboardAuthRuntime
}

function useSessionState() {
  return useState<AuthSession>("auth-session", () => ({
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

function normalizePermissionLevel(raw: string): PermissionLevel | null {
  const normalized = raw.trim().toLowerCase()
  if (normalized === "observer") return "observer"
  if (normalized === "operator") return "operator"
  if (normalized === "manager") return "manager"
  return null
}

export function useAuthSessionStore() {
  const runtime = useAuthRuntimeState()
  const session = useSessionState()
  const refreshCookie = useCookie<string | null>(refreshCookieName, {
    ...getCookieOptions(),
    default: () => null,
  })
  const refreshTimer = useState<ReturnType<typeof setTimeout> | null>(
    "auth-refresh-timer",
    () => null,
  )
  const isInitialized = useState("auth-is-initialized", () => false)

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

  const permissions = computed(() => getJwtArrayClaim(payload.value, "perm"))

  const isEmployee = computed(() => role.value?.toLowerCase() === "employee")

  const isLoggedIn = computed(
    () => Boolean(session.value.accessToken) && !isAccessExpired(),
  )

  function authFetch(
    path: "login" | "logout" | "refresh",
    options: RequestInit,
  ) {
    return $fetch.raw(`${authBasePath}/${path}`, {
      ...options,
      ignoreResponseError: true,
    } as Parameters<typeof $fetch.raw>[1])
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

  async function login(email: string, password: string) {
    const response = await authJsonPost("login", { email, password }).catch(
      () => null,
    )

    if (!response) {
      throw new Error("Prijava trenutno nije moguća. Pokušajte ponovo.")
    }

    if (!response.ok) {
      throw new Error(
        (response._data as { title?: string } | null)?.title ||
          "Prijava nije uspela.",
      )
    }

    const data = response._data as LoginResponse | undefined
    if (!data) {
      throw new Error("Prijava nije uspela.")
    }
    setSession(data)
    if (!isEmployee.value) {
      clearSession()
      throw new Error("Za pristup administraciji potrebna je uloga zaposlenog.")
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
        const response = await authJsonPost("refresh", { refreshToken }).catch(
          () => null,
        )

        if (!response || !response.ok) {
          clearSession()
          return
        }

        const data = response._data as LoginResponse | undefined
        if (!data) {
          clearSession()
          return
        }
        setSession(data)
        if (!isEmployee.value) {
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

  function hasPermission(
    service: string,
    allowedLevels: PermissionLevel[] = allPermissions,
  ) {
    if (!isEmployee.value) return false
    const allowed = new Set(allowedLevels)
    const normalizedService = service.toLowerCase()
    return permissions.value.some((perm) => {
      const [permService, permLevel] = perm.split(":", 2)
      if (!permService || !permLevel) return false
      if (permService.toLowerCase() !== normalizedService) return false
      const normalizedLevel = normalizePermissionLevel(permLevel)
      return normalizedLevel !== null && allowed.has(normalizedLevel)
    })
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
    permissions,
    isEmployee,
    isLoggedIn,
    login,
    logout,
    refreshTokens,
    getAccessToken,
    hasPermission,
    initialize,
    isAccessExpired,
    isInitialized,
  }
}
