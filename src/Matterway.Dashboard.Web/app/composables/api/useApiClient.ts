import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { buildServiceApiPathFromRequestPath } from "~/utils/apiProxy"
import type { ApiResult, ApiService } from "~/types/common/api"

type ApiRequestOptions = Exclude<Parameters<typeof $fetch.raw>[1], undefined>

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
  const serviceUnavailableMessage =
    "Usluga trenutno nije dostupna. Pokušajte ponovo kasnije."

  async function request<T>(
    service: ApiService,
    path: string,
    options: ApiRequestOptions = {},
  ): Promise<ApiResult<T>> {
    await auth.initialize()

    const requestPath = buildServiceApiPathFromRequestPath(service, path)
    if (!requestPath) {
      return {
        ok: false,
        status: 0,
        error:
          "API putanja mora da počne vrstom endpointa: self, admin, public ili system.",
      }
    }
    const headers = new Headers(options.headers || {})
    if (!headers.has("Accept")) headers.set("Accept", "application/json")

    const accessToken = auth.getAccessToken()
    if (accessToken) headers.set("Authorization", accessToken)

    const runFetch = async () =>
      $fetch.raw<T>(requestPath, {
        ...options,
        headers,
        ignoreResponseError: true,
      })

    let response
    try {
      response = await runFetch()
    } catch {
      return {
        ok: false,
        status: 0,
        error: serviceUnavailableMessage,
      }
    }

    if (response.status === 401 && !path.startsWith("public/")) {
      await auth.refreshTokens()
      const refreshedToken = auth.getAccessToken()
      if (refreshedToken) {
        headers.set("Authorization", refreshedToken)
        try {
          response = await runFetch()
        } catch {
          return {
            ok: false,
            status: 0,
            error: serviceUnavailableMessage,
          }
        }
      }
    }

    if (response.status === 204) {
      return { ok: true, status: response.status }
    }

    const payload = response._data as unknown

    if (response.ok) {
      return { ok: true, status: response.status, data: payload as T }
    }

    const validationErrors = getValidationErrors(payload)
    const baseError = getErrorMessage(payload) || "Zahtev nije uspeo."
    const validationMessage = formatValidationErrors(validationErrors)
    return {
      ok: false,
      status: response.status,
      error: validationMessage
        ? `${baseError} ${validationMessage}`
        : baseError,
    }
  }

  return { request }
}
