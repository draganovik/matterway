export type RequestState = {
  loading: boolean
  error: string
  success: string
  empty: string
}

export function useRequestState(initial?: Partial<RequestState>) {
  return reactive<RequestState>({
    loading: false,
    error: '',
    success: '',
    empty: '',
    ...initial
  })
}
