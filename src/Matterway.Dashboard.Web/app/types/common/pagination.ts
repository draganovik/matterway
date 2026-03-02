type PaginationNumber = number | string

export type PaginationMeta = {
  totalCount?: PaginationNumber
  totalPages?: PaginationNumber
  currentPage?: PaginationNumber
  pageSize?: PaginationNumber
}

export type PaginationResponse<T> = {
  meta: PaginationMeta
  data: T[]
  links: unknown
}
