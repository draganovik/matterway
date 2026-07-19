import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { getApiErrorMessage } from "~/utils/apiErrors"
import { buildServiceApiPathFromRequestPath } from "~/utils/apiProxy"
import type { ApiResult, ApiService } from "~/types/common/api"

type ApiRequestOptions = Exclude<Parameters<typeof $fetch.raw>[1], undefined>

const serviceUnavailableMessage =
  "Usluga trenutno nije dostupna. Pokušajte ponovo kasnije."

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
  const auth = useAuthSessionStore()

  async function requestRaw<T>(
    requestPath: string,
    options: ApiRequestOptions = {},
    retryUnauthorized = true,
  ) {
    await auth.initialize()

    const headers = new Headers(options.headers || {})
    if (!headers.has("Accept")) headers.set("Accept", "application/json")

    const accessToken = auth.getAccessToken()
    if (accessToken) headers.set("Authorization", accessToken)

    const runFetch = () =>
      $fetch.raw<T>(requestPath, {
        ...options,
        headers,
        ignoreResponseError: true,
      })

    let response = await runFetch().catch(() => null)
    if (response?.status !== 401 || !retryUnauthorized) return response

    await auth.refreshTokens()
    const refreshedToken = auth.getAccessToken()
    if (!refreshedToken) return response

    headers.set("Authorization", refreshedToken)
    response = await runFetch().catch(() => null)
    return response
  }

  async function request<T>(
    service: ApiService,
    path: string,
    options: ApiRequestOptions = {},
  ): Promise<ApiResult<T>> {
    const requestPath = buildServiceApiPathFromRequestPath(service, path)
    if (!requestPath) {
      return {
        ok: false,
        status: 0,
        error: "Zahtev nije ispravan.",
      }
    }

    const response = await requestRaw<T>(
      requestPath,
      options,
      !path.startsWith("public/"),
    )
    if (!response) {
      return {
        ok: false,
        status: 0,
        error: serviceUnavailableMessage,
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
    const baseError = getApiErrorMessage(payload) || "Zahtev nije uspeo."
    const validationMessage = formatValidationErrors(validationErrors)
    return {
      ok: false,
      status: response.status,
      error: validationMessage
        ? `${baseError} ${validationMessage}`
        : baseError,
    }
  }

  return { request, requestRaw }
}
