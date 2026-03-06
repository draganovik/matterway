import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import type { ApiResult, ApiService } from "~/types/common/api"

const validEndpointKinds = new Set(["self", "admin", "public", "system"])

function getBaseUrl(service: ApiService) {
  const config = useRuntimeConfig()
  if (service === "catalog") return config.public.catalogApiBaseUrl
  if (service === "customers") return config.public.customersApiBaseUrl
  if (service === "identity") return config.public.identityApiBaseUrl
  if (service === "sales") return config.public.salesApiBaseUrl
  return null
}

function getValidationErrors(
  payload: unknown,
): Record<string, string[]> | undefined {
  if (!payload || typeof payload !== "object") return undefined
  const payloadWithErrors = payload as { errors?: unknown }
  if (!payloadWithErrors.errors || typeof payloadWithErrors.errors !== "object")
    return undefined
  return payloadWithErrors.errors as Record<string, string[]>
}

function formatValidationErrors(errors?: Record<string, string[]>) {
  if (!errors) return ""
  return Object.entries(errors)
    .map(([field, messages]) => `${field}: ${messages.join(" ")}`)
    .join(" | ")
}

function getErrorMessage(payload: unknown) {
  if (typeof payload === "string") {
    const message = payload.trim()
    return message || null
  }

  if (!payload || typeof payload !== "object") return null

  const candidate = payload as {
    title?: unknown
    detail?: unknown
    message?: unknown
  }

  if (typeof candidate.detail === "string" && candidate.detail.trim()) {
    return candidate.detail.trim()
  }

  if (typeof candidate.title === "string" && candidate.title.trim()) {
    return candidate.title.trim()
  }

  if (typeof candidate.message === "string" && candidate.message.trim()) {
    return candidate.message.trim()
  }

  return null
}

export function useApiClient() {
  const auth = useAuthSessionStore()

  async function request<T>(
    service: ApiService,
    path: string,
    options: RequestInit = {},
    allowUnauthorized = false,
  ): Promise<ApiResult<T>> {
    if (!auth.isInitialized.value) {
      await auth.initialize()
    }

    const baseUrl = getBaseUrl(service)
    if (!baseUrl) {
      return {
        ok: false,
        status: 0,
        error: `Missing API base URL for service: ${service}.`,
      }
    }

    const normalizedPath = path.replace(/^\/+/, "")
    const [endpointKind, ...resourcePath] = normalizedPath.split("/")
    if (!endpointKind || !validEndpointKinds.has(endpointKind)) {
      return {
        ok: false,
        status: 0,
        error: "API path must start with one of: self, admin, public, system.",
      }
    }
    if (resourcePath.length === 0) {
      return {
        ok: false,
        status: 0,
        error: "API path must include a resource after the first segment.",
      }
    }
    const url = `${baseUrl}/api/${endpointKind}/v1/${resourcePath.join("/")}`
    const headers = new Headers(options.headers || {})
    if (!headers.has("Accept")) headers.set("Accept", "application/json")
    if (!headers.has("Content-Type") && !(options.body instanceof FormData)) {
      headers.set("Content-Type", "application/json")
    }

    const accessToken = auth.getAccessToken()
    if (accessToken) headers.set("Authorization", accessToken)

    const runFetch = async () =>
      fetch(url, {
        ...options,
        headers,
      })

    let response = await runFetch()

    if (response.status === 401 && !allowUnauthorized) {
      await auth.refreshTokens()
      const refreshedToken = auth.getAccessToken()
      if (refreshedToken) headers.set("Authorization", refreshedToken)
      response = await runFetch()
    }

    if (response.status === 204) {
      return { ok: true, status: response.status }
    }

    const contentType = response.headers.get("content-type") || ""
    const isJson = contentType.includes("json")
    const payload = isJson
      ? await response.json().catch(() => null)
      : await response.text().catch(() => null)

    if (response.ok) {
      return { ok: true, status: response.status, data: payload as T }
    }

    const validationErrors = getValidationErrors(payload)
    const baseError = getErrorMessage(payload) || "Request failed."
    const validationMessage = formatValidationErrors(validationErrors)
    return {
      ok: false,
      status: response.status,
      error: validationMessage
        ? `${baseError} ${validationMessage}`
        : baseError,
      validationErrors,
    }
  }

  return { request }
}
