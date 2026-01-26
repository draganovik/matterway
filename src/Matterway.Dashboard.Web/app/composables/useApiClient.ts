import { useAuthSession } from '~/composables/useAuthSession'

type ApiService = 'identity' | 'catalog' | 'customers' | 'sales'

type ApiResult<T> = {
  ok: boolean
  status: number
  data?: T
  error?: string
  validationErrors?: Record<string, string[]>
}

function getBaseUrl(service: ApiService) {
  const config = useRuntimeConfig()
  switch (service) {
    case 'identity':
      return config.public.identityApiBaseUrl
    case 'catalog':
      return config.public.catalogApiBaseUrl
    case 'customers':
      return config.public.customersApiBaseUrl
    case 'sales':
      return config.public.salesApiBaseUrl
    default:
      return config.public.identityApiBaseUrl
  }
}

function getValidationErrors(payload: unknown): Record<string, string[]> | undefined {
  if (!payload || typeof payload !== 'object') return undefined
  if (!payload.errors || typeof payload.errors !== 'object') return undefined
  return payload.errors as Record<string, string[]>
}

function formatValidationErrors(errors?: Record<string, string[]>) {
  if (!errors) return ''
  return Object.entries(errors)
    .map(([field, messages]) => `${field}: ${messages.join(' ')}`)
    .join(' | ')
}

export function useApiClient() {
  const auth = useAuthSession()

  async function request<T>(
    service: ApiService,
    path: string,
    options: RequestInit = {},
    allowUnauthorized = false
  ): Promise<ApiResult<T>> {
    if (!auth.isInitialized.value) {
      await auth.initialize()
    }
    const baseUrl = getBaseUrl(service)
    const url = `${baseUrl}/api/v1.0/${path}`
    const headers = new Headers(options.headers || {})
    if (!headers.has('Accept')) headers.set('Accept', 'application/json')
    if (!headers.has('Content-Type') && !(options.body instanceof FormData)) {
      headers.set('Content-Type', 'application/json')
    }

    const accessToken = auth.getAccessToken()
    if (accessToken) headers.set('Authorization', accessToken)

    const runFetch = async () =>
      fetch(url, {
        ...options,
        headers
      })

    let response = await runFetch()

    if (response.status === 401 && !allowUnauthorized) {
      await auth.refreshTokens()
      const refreshedToken = auth.getAccessToken()
      if (refreshedToken) headers.set('Authorization', refreshedToken)
      response = await runFetch()
    }

    if (response.status === 204) {
      return { ok: true, status: response.status }
    }

    const contentType = response.headers.get('content-type') || ''
    const isJson = contentType.includes('application/json')
    const payload = isJson ? await response.json().catch(() => null) : await response.text().catch(() => null)

    if (response.ok) {
      return { ok: true, status: response.status, data: payload as T }
    }

    const validationErrors = getValidationErrors(payload)
    const baseError = payload?.title || payload?.detail || 'Request failed.'
    const validationMessage = formatValidationErrors(validationErrors)
    return {
      ok: false,
      status: response.status,
      error: validationMessage ? `${baseError} ${validationMessage}` : baseError,
      validationErrors
    }
  }

  return { request }
}
