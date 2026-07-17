export type RequestState = {
  loading: boolean
  error: string
  success: string
  empty: string
}

export function useRequestState(empty = "") {
  return reactive<RequestState>({
    loading: false,
    error: "",
    success: "",
    empty,
  })
}
