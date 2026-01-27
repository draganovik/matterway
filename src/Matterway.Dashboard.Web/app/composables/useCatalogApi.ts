import { buildQuery } from '~/utils/http'
import { useApiClient } from '~/composables/useApiClient'

export type ApiResult<T> = {
  ok: boolean
  status: number
  data?: T
  error?: string
  validationErrors?: Record<string, string[]>
}

export type ArticleDiscountProperty = {
  percentage: number | string
  validFrom: string
  validTo?: string | null
}

export type ArticleImageProperty = {
  id: string
  orderIndex: number | string
  imageUrl?: string | null
  imageAlt?: string | null
}

export type ArticleDetailProperty = {
  detailSlug?: string | null
  title?: string | null
  value?: string | null
}

export type ArticleSpecificationProperty = {
  specificationSlug?: string | null
  title?: string | null
  value?: number | string | null
  unit?: string | null
}

export type QueryArticleResponse = {
  id: string
  code?: string | null
  title?: string | null
  basePrice?: number | string | null
  price?: number | string | null
  discount?: ArticleDiscountProperty | null
  description?: string | null
  thumbnailUrl?: string | null
  thumbnailAlt?: string | null
  isAvailable: boolean
}

export type GetArticleByIdResponse = {
  id: string
  code?: string | null
  title?: string | null
  basePrice?: number | string | null
  price?: number | string | null
  discount?: ArticleDiscountProperty | null
  description?: string | null
  details?: ArticleDetailProperty[] | null
  specifications?: ArticleSpecificationProperty[] | null
  images?: ArticleImageProperty[] | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

export type PaginationMeta = {
  totalCount?: number | string
  totalPages?: number | string
  currentPage?: number | string
  pageSize?: number | string
}

export type PaginationResponse<T> = {
  meta: PaginationMeta
  data: T[]
  links: unknown
}

export type CreateArticleRequest = {
  articleCode: string
  title: string
  basePrice: number | string
  description: string
  isAvailable?: boolean
}

export type UpdateArticleRequest = {
  articleCode?: string | null
  title?: string | null
  basePrice?: number | string | null
  description?: string | null
  isAvailable?: boolean
}

export type CreateArticleResponse = {
  id: string
  articleCode?: string | null
  title?: string | null
  basePrice?: number | string | null
  price?: number | string | null
  description?: string | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

export type UpdateArticleResponse = {
  id: string
  articleCode?: string | null
  title?: string | null
  basePrice?: number | string | null
  price?: number | string | null
  description?: string | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

export type AddArticleDetailRequest = {
  detailSlug: string
  value: string
}

export type UpdateArticleDetailRequest = {
  value: string
}

export type AddArticleSpecificationRequest = {
  specificationSlug: string
  value: number | string
}

export type UpdateArticleSpecificationRequest = {
  value: number | string
}

export type QueryDetailResponse = {
  slug?: string | null
  title?: string | null
}

export type QuerySpecificationResponse = {
  slug?: string | null
  title?: string | null
  unit?: string | null
}

export function useCatalogApi() {
  const api = useApiClient()

  async function queryArticles(params: { filter?: string, page: number, pageSize: number }) {
    const query = buildQuery({
      Filter: params.filter || undefined,
      Page: params.page,
      PageSize: params.pageSize
    })
    return api.request<PaginationResponse<QueryArticleResponse>>('catalog', `public/articles${query}`)
  }

  async function getArticleById(id: string) {
    return api.request<GetArticleByIdResponse>('catalog', `public/articles/${id}`)
  }

  async function createArticle(payload: CreateArticleRequest) {
    return api.request<CreateArticleResponse>('catalog', 'admin/articles', {
      method: 'POST',
      body: JSON.stringify(payload)
    })
  }

  async function updateArticle(id: string, payload: UpdateArticleRequest) {
    return api.request<UpdateArticleResponse>('catalog', `admin/articles/${id}`,
      {
        method: 'PATCH',
        body: JSON.stringify(payload)
      }
    )
  }

  async function deleteArticle(id: string) {
    return api.request('catalog', `admin/articles/${id}`,
      {
        method: 'DELETE'
      }
    )
  }

  async function addArticleImage(articleId: string, payload: { orderIndex: number | string, file: File, imageAlt?: string }) {
    const formData = new FormData()
    formData.append('orderIndex', String(payload.orderIndex))
    formData.append('file', payload.file)
    if (payload.imageAlt) formData.append('imageAlt', payload.imageAlt)
    return api.request('catalog', `admin/articles/${articleId}/images`, {
      method: 'POST',
      body: formData
    })
  }

  async function updateArticleImage(articleId: string, orderIndex: number | string, payload: { imageAlt?: string, orderIndex?: number | string }) {
    return api.request('catalog', `admin/articles/${articleId}/images/${orderIndex}`,
      {
        method: 'PATCH',
        body: JSON.stringify(payload)
      }
    )
  }

  async function removeArticleImage(articleId: string, orderIndex: number | string) {
    return api.request('catalog', `admin/articles/${articleId}/images/${orderIndex}`,
      {
        method: 'DELETE'
      }
    )
  }

  async function addArticleDetail(articleId: string, payload: AddArticleDetailRequest) {
    return api.request('catalog', `admin/articles/${articleId}/details`, {
      method: 'POST',
      body: JSON.stringify(payload)
    })
  }

  async function updateArticleDetail(articleId: string, detailSlug: string, payload: UpdateArticleDetailRequest) {
    return api.request('catalog', `admin/articles/${articleId}/details/${detailSlug}`,
      {
        method: 'PATCH',
        body: JSON.stringify(payload)
      }
    )
  }

  async function removeArticleDetail(articleId: string, detailSlug: string) {
    return api.request('catalog', `admin/articles/${articleId}/details/${detailSlug}`,
      {
        method: 'DELETE'
      }
    )
  }

  async function addArticleSpecification(articleId: string, payload: AddArticleSpecificationRequest) {
    return api.request('catalog', `admin/articles/${articleId}/specifications`, {
      method: 'POST',
      body: JSON.stringify(payload)
    })
  }

  async function updateArticleSpecification(articleId: string, specificationSlug: string, payload: UpdateArticleSpecificationRequest) {
    return api.request('catalog', `admin/articles/${articleId}/specifications/${specificationSlug}`,
      {
        method: 'PATCH',
        body: JSON.stringify(payload)
      }
    )
  }

  async function removeArticleSpecification(articleId: string, specificationSlug: string) {
    return api.request('catalog', `admin/articles/${articleId}/specifications/${specificationSlug}`,
      {
        method: 'DELETE'
      }
    )
  }

  async function queryDetails(params: { limit: number, titleLike?: string }) {
    const query = buildQuery({
      Limit: params.limit,
      TitleLike: params.titleLike || undefined
    })
    return api.request<QueryDetailResponse[]>('catalog', `admin/details${query}`)
  }

  async function querySpecifications(params: { limit: number, titleLike?: string }) {
    const query = buildQuery({
      Limit: params.limit,
      TitleLike: params.titleLike || undefined
    })
    return api.request<QuerySpecificationResponse[]>('catalog', `admin/specifications${query}`)
  }

  return {
    queryArticles,
    getArticleById,
    createArticle,
    updateArticle,
    deleteArticle,
    addArticleImage,
    updateArticleImage,
    removeArticleImage,
    addArticleDetail,
    updateArticleDetail,
    removeArticleDetail,
    addArticleSpecification,
    updateArticleSpecification,
    removeArticleSpecification,
    queryDetails,
    querySpecifications
  }
}
