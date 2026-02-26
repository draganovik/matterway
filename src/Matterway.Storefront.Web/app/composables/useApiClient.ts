import { useAuthSession } from "~/composables/useAuthSession"
import type { ApiResult, ApiService } from "~/types/common/api"

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

export function useApiClient() {
  const auth = useAuthSession()
  const validEndpointKinds = new Set(["self", "admin", "public", "system"])

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
        error: `Nedostaje API osnovni URL za servis: ${service}.`,
      }
    }

    const normalizedPath = path.replace(/^\/+/, "")
    const [endpointKind, ...resourcePath] = normalizedPath.split("/")
    if (!endpointKind || !validEndpointKinds.has(endpointKind)) {
      return {
        ok: false,
        status: 0,
        error:
          "API putanja mora početi segmentom: self, admin, public ili system.",
      }
    }
    if (resourcePath.length === 0) {
      return {
        ok: false,
        status: 0,
        error: "API putanja mora imati resurs nakon prvog segmenta.",
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
    const isJson = contentType.includes("application/json")
    const payload = isJson
      ? await response.json().catch(() => null)
      : await response.text().catch(() => null)

    if (response.ok) {
      return { ok: true, status: response.status, data: payload as T }
    }

    const validationErrors = getValidationErrors(payload)
    const payloadError =
      typeof payload === "object" && payload
        ? ((payload as { title?: string; detail?: string }).title ??
          (payload as { title?: string; detail?: string }).detail)
        : null
    const baseError = payloadError || "Zahtev nije uspeo."
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
