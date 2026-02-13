import { buildQuery } from "~/utils/http"
import { useApiClient } from "~/composables/useApiClient"
import type {
  AddArticleDetailRequest,
  AddArticleImageRequest,
  AddArticleImageResponse,
  CreateArticleRequest,
  CreateArticleResponse,
  CreateDiscountRequest,
  CreatedDiscountResponse,
  DeleteDetailResponse,
  DeleteDiscountResponse,
  GetArticleByIdResponse,
  PutDetailRequest,
  PutDetailResponse,
  QueryArticlesParams,
  QueryArticlesResponse,
  QueryDetailResponse,
  QueryDetailsParams,
  QueryDiscountResponse,
  UpdateArticleDetailRequest,
  UpdateArticleImageRequest,
  UpdateArticleImageResponse,
  UpdateArticleRequest,
  UpdateArticleResponse,
  UpdateDiscountRequest,
  UpdatedDiscountResponse,
} from "~/types/catalog"

const ADMIN_ARTICLES_PATH = "admin/articles"
const PUBLIC_ARTICLES_PATH = "public/articles"
const ADMIN_DETAILS_PATH = "admin/details"
const ADMIN_DISCOUNTS_PATH = "admin/discounts"

export function useCatalogApi() {
  const api = useApiClient()

  async function queryArticles(params: QueryArticlesParams) {
    const query = buildQuery({
      Filter: params.filter || undefined,
      Page: params.page,
      PageSize: params.pageSize,
    })
    return api.request<QueryArticlesResponse>(
      "catalog",
      `${PUBLIC_ARTICLES_PATH}${query}`,
    )
  }

  async function getArticleById(id: string) {
    return api.request<GetArticleByIdResponse>(
      "catalog",
      `${PUBLIC_ARTICLES_PATH}/${id}`,
    )
  }

  async function createArticle(payload: CreateArticleRequest) {
    return api.request<CreateArticleResponse>("catalog", ADMIN_ARTICLES_PATH, {
      method: "POST",
      body: JSON.stringify(payload),
    })
  }

  async function updateArticle(id: string, payload: UpdateArticleRequest) {
    return api.request<UpdateArticleResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${id}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )
  }

  async function deleteArticle(id: string) {
    return api.request("catalog", `${ADMIN_ARTICLES_PATH}/${id}`, {
      method: "DELETE",
    })
  }

  async function addArticleImage(
    articleId: string,
    payload: AddArticleImageRequest,
  ) {
    const formData = new FormData()
    formData.append("orderIndex", String(payload.orderIndex))
    formData.append("file", payload.file)
    if (payload.imageAlt) formData.append("imageAlt", payload.imageAlt)

    return api.request<AddArticleImageResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${articleId}/images`,
      {
        method: "POST",
        body: formData,
      },
    )
  }

  async function updateArticleImage(
    articleId: string,
    orderIndex: number | string,
    payload: UpdateArticleImageRequest,
  ) {
    return api.request<UpdateArticleImageResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${articleId}/images/${orderIndex}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )
  }

  async function removeArticleImage(
    articleId: string,
    orderIndex: number | string,
  ) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${articleId}/images/${orderIndex}`,
      {
        method: "DELETE",
      },
    )
  }

  async function addArticleDetail(
    articleId: string,
    payload: AddArticleDetailRequest,
  ) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${articleId}/details`,
      {
        method: "POST",
        body: JSON.stringify(payload),
      },
    )
  }

  async function updateArticleDetail(
    articleId: string,
    detailSlug: string,
    payload: UpdateArticleDetailRequest,
  ) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${articleId}/details/${detailSlug}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )
  }

  async function removeArticleDetail(articleId: string, detailSlug: string) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${articleId}/details/${detailSlug}`,
      {
        method: "DELETE",
      },
    )
  }

  async function queryDetails(params: QueryDetailsParams) {
    const query = buildQuery({
      Limit: params.limit,
      TitleLike: params.titleLike || undefined,
    })
    return api.request<QueryDetailResponse[]>(
      "catalog",
      `${ADMIN_DETAILS_PATH}${query}`,
    )
  }

  async function putDetail(slug: string, payload: PutDetailRequest) {
    return api.request<PutDetailResponse>(
      "catalog",
      `${ADMIN_DETAILS_PATH}/${encodeURIComponent(slug)}`,
      {
        method: "PUT",
        body: JSON.stringify(payload),
      },
    )
  }

  async function deleteDetail(slug: string) {
    return api.request<DeleteDetailResponse>(
      "catalog",
      `${ADMIN_DETAILS_PATH}/${encodeURIComponent(slug)}`,
      {
        method: "DELETE",
      },
    )
  }

  async function createDiscounts(payload: CreateDiscountRequest) {
    return api.request<CreatedDiscountResponse[]>(
      "catalog",
      ADMIN_DISCOUNTS_PATH,
      {
        method: "POST",
        body: JSON.stringify(payload),
      },
    )
  }

  async function queryDiscounts() {
    return api.request<QueryDiscountResponse[]>("catalog", ADMIN_DISCOUNTS_PATH)
  }

  async function updateDiscount(code: string, payload: UpdateDiscountRequest) {
    return api.request<UpdatedDiscountResponse>(
      "catalog",
      `${ADMIN_DISCOUNTS_PATH}/${encodeURIComponent(code)}`,
      {
        method: "PUT",
        body: JSON.stringify(payload),
      },
    )
  }

  async function deleteDiscount(code: string) {
    return api.request<DeleteDiscountResponse>(
      "catalog",
      `${ADMIN_DISCOUNTS_PATH}/${encodeURIComponent(code)}`,
      {
        method: "DELETE",
      },
    )
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
    queryDetails,
    putDetail,
    deleteDetail,
    queryDiscounts,
    createDiscounts,
    updateDiscount,
    deleteDiscount,
  }
}
