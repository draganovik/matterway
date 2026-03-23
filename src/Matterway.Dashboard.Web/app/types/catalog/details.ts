import type { PaginationResponse } from "../common/pagination"

export type QueryDetailsParams = {
  page: number
  pageSize: number
  titleLike?: string
}

export type QueryDetailResponse = {
  slug?: string | null
  title?: string | null
  unit?: string | null
}

export type QueryDetailsResponse = PaginationResponse<QueryDetailResponse>

export type PutDetailRequest = {
  title: string
  unit?: string | null
}

export type PutDetailResponse = {
  slug?: string | null
  title?: string | null
  unit?: string | null
}

export type DeleteDetailResponse = {
  slug: string
  message: string
}
