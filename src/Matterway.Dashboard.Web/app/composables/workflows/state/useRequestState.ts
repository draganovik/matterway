type RequestState = {
  loading: boolean
  error: string
  success: string
  empty: string
}

export function useRequestState(empty = "", loading = false) {
  return reactive<RequestState>({
    loading,
    error: "",
    success: "",
    empty,
  })
}
