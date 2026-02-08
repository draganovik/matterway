export type ApiService = 'catalog'

export type ApiResult<T> = {
  ok: boolean
  status: number
  data?: T
  error?: string
  validationErrors?: Record<string, string[]>
}
