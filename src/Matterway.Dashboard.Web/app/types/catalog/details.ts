export type QueryDetailsParams = {
  limit: number
  titleLike?: string
}

export type QueryDetailResponse = {
  slug?: string | null
  title?: string | null
  unit?: string | null
}

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
