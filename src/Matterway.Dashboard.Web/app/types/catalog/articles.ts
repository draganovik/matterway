import type { PaginationResponse } from "../common/pagination"
import type {
  ArticleDetailProperty,
  ArticleDiscountProperty,
  ArticleImageProperty,
  NumberInput,
} from "./shared"

export type QueryArticlesParams = {
  filter?: string
  page: number
  pageSize: number
}

export type QueryArticleResponse = {
  code: string
  title?: string | null
  basePrice?: NumberInput | null
  price?: NumberInput | null
  discount?: ArticleDiscountProperty | null
  description?: string | null
  thumbnailUrl?: string | null
  thumbnailAlt?: string | null
  isAvailable: boolean
}

export type GetArticleResponse = {
  code: string
  title?: string | null
  basePrice?: NumberInput | null
  price?: NumberInput | null
  discount?: ArticleDiscountProperty | null
  description?: string | null
  details?: ArticleDetailProperty[] | null
  images?: ArticleImageProperty[] | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

export type QueryArticlesResponse = PaginationResponse<QueryArticleResponse>

export type CreateArticleRequest = {
  code: string
  title: string
  basePrice: NumberInput
  description: string
  isAvailable?: boolean
}

export type UpdateArticleRequest = {
  code?: string | null
  title?: string | null
  basePrice?: NumberInput | null
  description?: string | null
  isAvailable?: boolean
}

export type CreateArticleResponse = {
  code: string
  title?: string | null
  basePrice?: NumberInput | null
  price?: NumberInput | null
  description?: string | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

export type UpdateArticleResponse = {
  code: string
  title?: string | null
  basePrice?: NumberInput | null
  price?: NumberInput | null
  description?: string | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

export type DeleteArticleResponse = {
  code: string
  message?: string | null
}

export type AddArticleImageRequest = {
  orderIndex: NumberInput
  file: File
  imageAlt?: string
}

export type UpdateArticleImageRequest = {
  imageAlt?: string
  orderIndex?: NumberInput
}

export type ArticleImagesMutationResponse = {
  images?: ArticleImageProperty[] | null
  createdImageId?: string | null
  updatedImageId?: string | null
  deletedImageId?: string | null
}

export type PutArticleDetailRequest = {
  textValue?: string | null
  numericValue?: NumberInput | null
}
