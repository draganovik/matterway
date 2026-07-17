export type PaginationMeta = {
  totalCount: number
  totalPages: number
  currentPage: number
  pageSize: number
}

export type PaginationResponse<T> = {
  meta: PaginationMeta
  data: T[]
  links: unknown
}
