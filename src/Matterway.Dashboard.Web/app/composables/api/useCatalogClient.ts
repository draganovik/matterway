import { buildQuery } from "~/utils/http"
import { useApiClient } from "~/composables/api/useApiClient"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { buildServiceApiPath } from "~/utils/apiProxy"
import { buildCatalogImageUrl } from "~/utils/catalogImages"
import type { ApiResult } from "~/types/common/api"
import type {
  AddArticleDetailRequest,
  AddArticleImageRequest,
  ArticleImageProperty,
  ArticleImagesMutationResponse,
  CreateArticleRequest,
  CreateArticleResponse,
  CreateDiscountRequest,
  CreatedDiscountResponse,
  DeleteArticleResponse,
  DeleteDetailResponse,
  DeleteDiscountResponse,
  ExportCatalogArchiveResponse,
  GetArticleResponse,
  ImportCatalogArchiveResponse,
  PutDetailRequest,
  PutDetailResponse,
  QueryArticleResponse,
  QueryArticlesParams,
  QueryArticlesResponse,
  QueryDetailsParams,
  QueryDetailsResponse,
  QueryDiscountResponse,
  UpdateArticleDetailRequest,
  UpdateArticleImageRequest,
  UpdateArticleRequest,
  UpdateArticleResponse,
  UpdateDiscountRequest,
  UpdatedDiscountResponse,
} from "~/types/catalog"

const ADMIN_ARTICLES_PATH = "admin/articles"
const PUBLIC_ARTICLES_PATH = "public/articles"
const PUBLIC_DETAILS_PATH = "public/details"
const ADMIN_DETAILS_PATH = "admin/details"
const ADMIN_DISCOUNTS_PATH = "admin/discounts"

function mapArticleImageProperty(
  image: ArticleImageProperty,
): ArticleImageProperty {
  return {
    ...image,
    imageUrl: buildCatalogImageUrl(image.imageUrl, image.id),
  }
}

function mapArticleImages(images?: ArticleImageProperty[] | null) {
  return images?.map(mapArticleImageProperty) ?? images
}

function mapQueryArticleResponse(
  article: QueryArticleResponse,
): QueryArticleResponse {
  return {
    ...article,
    thumbnailUrl: buildCatalogImageUrl(article.thumbnailUrl),
  }
}

export function useCatalogClient() {
  const api = useApiClient()
  const auth = useAuthSessionStore()

  async function queryArticles(params: QueryArticlesParams) {
    const query = buildQuery({
      Filter: params.filter || undefined,
      Page: params.page,
      PageSize: params.pageSize,
    })
    const response = await api.request<QueryArticlesResponse>(
      "catalog",
      `${PUBLIC_ARTICLES_PATH}${query}`,
    )

    if (!response.ok || !response.data) {
      return response
    }

    return {
      ...response,
      data: {
        ...response.data,
        data: (response.data.data ?? []).map(mapQueryArticleResponse),
      },
    } satisfies ApiResult<QueryArticlesResponse>
  }

  async function getArticle(code: string) {
    const response = await api.request<GetArticleResponse>(
      "catalog",
      `${PUBLIC_ARTICLES_PATH}/${code}`,
    )

    if (!response.ok || !response.data) {
      return response
    }

    return {
      ...response,
      data: {
        ...response.data,
        images: mapArticleImages(response.data.images),
      },
    } satisfies ApiResult<GetArticleResponse>
  }

  async function createArticle(payload: CreateArticleRequest) {
    return api.request<CreateArticleResponse>("catalog", ADMIN_ARTICLES_PATH, {
      method: "POST",
      body: JSON.stringify(payload),
    })
  }

  async function updateArticle(code: string, payload: UpdateArticleRequest) {
    return api.request<UpdateArticleResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )
  }

  async function deleteArticle(code: string) {
    return api.request<DeleteArticleResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}`,
      {
        method: "DELETE",
      },
    )
  }

  async function addArticleImage(
    code: string,
    payload: AddArticleImageRequest,
  ) {
    const formData = new FormData()
    formData.append("orderIndex", String(payload.orderIndex))
    formData.append("file", payload.file)
    if (payload.imageAlt) formData.append("imageAlt", payload.imageAlt)

    const response = await api.request<ArticleImagesMutationResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/images`,
      {
        method: "POST",
        body: formData,
      },
    )

    if (!response.ok || !response.data) {
      return response
    }

    return {
      ...response,
      data: {
        ...response.data,
        images: mapArticleImages(response.data.images),
      },
    } satisfies ApiResult<ArticleImagesMutationResponse>
  }

  async function updateArticleImage(
    code: string,
    orderIndex: number | string,
    payload: UpdateArticleImageRequest,
  ) {
    const response = await api.request<ArticleImagesMutationResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/images/${orderIndex}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )

    if (!response.ok || !response.data) {
      return response
    }

    return {
      ...response,
      data: {
        ...response.data,
        images: mapArticleImages(response.data.images),
      },
    } satisfies ApiResult<ArticleImagesMutationResponse>
  }

  async function removeArticleImage(code: string, orderIndex: number | string) {
    const response = await api.request<ArticleImagesMutationResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/images/${orderIndex}`,
      {
        method: "DELETE",
      },
    )

    if (!response.ok || !response.data) {
      return response
    }

    return {
      ...response,
      data: {
        ...response.data,
        images: mapArticleImages(response.data.images),
      },
    } satisfies ApiResult<ArticleImagesMutationResponse>
  }

  async function addArticleDetail(
    code: string,
    payload: AddArticleDetailRequest,
  ) {
    return api.request("catalog", `${ADMIN_ARTICLES_PATH}/${code}/details`, {
      method: "POST",
      body: JSON.stringify(payload),
    })
  }

  async function updateArticleDetail(
    code: string,
    detailSlug: string,
    payload: UpdateArticleDetailRequest,
  ) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/details/${detailSlug}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )
  }

  async function removeArticleDetail(code: string, detailSlug: string) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/details/${detailSlug}`,
      {
        method: "DELETE",
      },
    )
  }

  async function queryDetails(params: QueryDetailsParams) {
    const query = buildQuery({
      Page: params.page,
      PageSize: params.pageSize,
      TitleLike: params.titleLike || undefined,
    })
    return api.request<QueryDetailsResponse>(
      "catalog",
      `${PUBLIC_DETAILS_PATH}${query}`,
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
        body: JSON.stringify({
          ...payload,
          articleCodes: payload.articleCodes,
        }),
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
        body: JSON.stringify({
          ...payload,
          articleCodes: payload.articleCodes,
        }),
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

  async function importCatalogArchive(file: File) {
    const formData = new FormData()
    formData.append("file", file)

    return api.request<ImportCatalogArchiveResponse>(
      "catalog",
      "admin/catalog/archive/import",
      {
        method: "POST",
        body: formData,
      },
    )
  }

  async function exportCatalogArchive(): Promise<
    ApiResult<ExportCatalogArchiveResponse>
  > {
    await auth.initialize()
    const endpoint = buildServiceApiPath(
      "catalog",
      "admin",
      "catalog/archive/export",
    )
    const token = auth.getAccessToken()
    const headers = new Headers({ Accept: "application/zip" })
    if (token) headers.set("Authorization", token)

    const runFetch = () =>
      $fetch.raw<ArrayBuffer>(endpoint, {
        method: "GET",
        headers,
        responseType: "arrayBuffer",
        ignoreResponseError: true,
      })

    let response
    try {
      response = await runFetch()
    } catch {
      return {
        ok: false,
        status: 0,
        error: "Izvoz arhive kataloga trenutno nije dostupan.",
      }
    }

    if (response.status === 401) {
      await auth.refreshTokens()
      const refreshedToken = auth.getAccessToken()
      if (refreshedToken) {
        headers.set("Authorization", refreshedToken)
        try {
          response = await runFetch()
        } catch {
          return {
            ok: false,
            status: 0,
            error: "Izvoz arhive kataloga trenutno nije dostupan.",
          }
        }
      }
    }

    if (!response.ok) {
      const contentType = response.headers.get("content-type") || ""
      const payload = decodeDownloadErrorPayload(
        response._data as ArrayBuffer | undefined,
        contentType,
      )
      const error =
        getErrorMessage(payload) || "Izvoz arhive kataloga nije uspeo."

      return { ok: false, status: response.status, error }
    }

    const blob = new Blob([response._data ?? new ArrayBuffer(0)], {
      type: response.headers.get("content-type") || "application/zip",
    })
    const contentDisposition = response.headers.get("content-disposition") || ""
    const fileName = resolveDownloadFileName(contentDisposition)

    return {
      ok: true,
      status: response.status,
      data: {
        fileName,
        blob,
      },
    }
  }

  return {
    queryArticles,
    getArticle,
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
    importCatalogArchive,
    exportCatalogArchive,
  }
}

function resolveDownloadFileName(contentDisposition: string) {
  const fallbackName = buildTimestampedArchiveName()

  const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i)
  if (utf8Match && utf8Match[1]) {
    try {
      return ensureTimestampedArchiveName(decodeURIComponent(utf8Match[1]))
    } catch {
      return ensureTimestampedArchiveName(utf8Match[1])
    }
  }

  const plainMatch = contentDisposition.match(/filename="?([^"]+)"?/i)
  if (plainMatch && plainMatch[1]) {
    return ensureTimestampedArchiveName(plainMatch[1])
  }

  return fallbackName
}

function ensureTimestampedArchiveName(fileName: string) {
  const trimmed = (fileName || "").trim()
  if (!trimmed) return buildTimestampedArchiveName()

  const safe = trimmed.toLowerCase().endsWith(".zip")
    ? trimmed
    : `${trimmed}.zip`

  if (/\d{8}-\d{6}/.test(safe)) return safe

  const extension = ".zip"
  const base = safe.slice(0, -extension.length)
  const stamp = formatArchiveTimestamp(new Date())
  return `${base}-${stamp}${extension}`
}

function buildTimestampedArchiveName(date = new Date()) {
  return `catalog-archive-${formatArchiveTimestamp(date)}.zip`
}

function formatArchiveTimestamp(date: Date) {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, "0")
  const day = String(date.getDate()).padStart(2, "0")
  const hour = String(date.getHours()).padStart(2, "0")
  const minute = String(date.getMinutes()).padStart(2, "0")
  const second = String(date.getSeconds()).padStart(2, "0")
  return `${year}${month}${day}-${hour}${minute}${second}`
}

function decodeDownloadErrorPayload(
  payload: ArrayBuffer | undefined,
  contentType: string,
) {
  if (!payload) return null

  const text = new TextDecoder().decode(payload)
  if (!text.trim()) return null

  if (contentType.includes("json")) {
    try {
      return JSON.parse(text) as unknown
    } catch {
      return text
    }
  }

  return text
}

function getErrorMessage(payload: unknown) {
  if (typeof payload === "string") {
    const message = payload.trim()
    return message || null
  }

  if (!payload || typeof payload !== "object") return null

  const candidate = payload as {
    title?: unknown
    detail?: unknown
    message?: unknown
  }

  if (typeof candidate.detail === "string" && candidate.detail.trim()) {
    return candidate.detail.trim()
  }

  if (typeof candidate.title === "string" && candidate.title.trim()) {
    return candidate.title.trim()
  }

  if (typeof candidate.message === "string" && candidate.message.trim()) {
    return candidate.message.trim()
  }

  return null
}
