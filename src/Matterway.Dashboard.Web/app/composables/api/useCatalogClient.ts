import { buildQuery } from "~/utils/http"
import { useApiClient } from "~/composables/api/useApiClient"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import type { ApiResult } from "~/types/common/api"
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
  ExportCatalogArchiveResponse,
  GetArticleResponse,
  ImportCatalogArchiveResponse,
  PutDetailRequest,
  PutDetailResponse,
  QueryArticleResponse,
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
import type { PaginationResponse } from "~/types/common/pagination"

const ADMIN_ARTICLES_PATH = "admin/articles"
const PUBLIC_ARTICLES_PATH = "public/articles"
const ADMIN_DETAILS_PATH = "admin/details"
const ADMIN_DISCOUNTS_PATH = "admin/discounts"

type AnyRecord = Record<string, unknown>

function asRecord(value: unknown): AnyRecord {
  return value && typeof value === "object" ? (value as AnyRecord) : {}
}

function asArray(value: unknown): unknown[] {
  return Array.isArray(value) ? value : []
}

function asNumber(value: unknown) {
  if (typeof value === "number") return value

  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : undefined
}

function asString(value: unknown) {
  return typeof value === "string" ? value : undefined
}

function readCode(source: AnyRecord) {
  return String(source.code ?? "")
}

function mapQueryArticleResponse(payload: unknown): QueryArticleResponse {
  const source = asRecord(payload)
  return {
    code: readCode(source),
    title: asString(source.title) ?? null,
    basePrice: asNumber(source.basePrice) ?? null,
    price: asNumber(source.price) ?? null,
    discount: source.discount ? (source.discount as QueryArticleResponse["discount"]) : null,
    description: asString(source.description) ?? null,
    thumbnailUrl: asString(source.thumbnailUrl) ?? null,
    thumbnailAlt: asString(source.thumbnailAlt) ?? null,
    isAvailable: Boolean(source.isAvailable),
  }
}

function mapGetArticleResponse(payload: unknown): GetArticleResponse {
  const source = asRecord(payload)
  return {
    code: readCode(source),
    title: asString(source.title) ?? null,
    basePrice: asNumber(source.basePrice) ?? null,
    price: asNumber(source.price) ?? null,
    discount: source.discount ? (source.discount as GetArticleResponse["discount"]) : null,
    description: asString(source.description) ?? null,
    details: asArray(source.details) as GetArticleResponse["details"],
    images: asArray(source.images) as GetArticleResponse["images"],
    createdAt: asString(source.createdAt) ?? null,
    updatedAt: asString(source.updatedAt) ?? null,
    isAvailable: Boolean(source.isAvailable),
  }
}

function mapArticleMutationResponse(
  payload: unknown,
): CreateArticleResponse | UpdateArticleResponse {
  const source = asRecord(payload)
  return {
    code: readCode(source),
    title: asString(source.title) ?? null,
    basePrice: asNumber(source.basePrice) ?? null,
    price: asNumber(source.price) ?? null,
    description: asString(source.description) ?? null,
    createdAt: asString(source.createdAt) ?? null,
    updatedAt: asString(source.updatedAt) ?? null,
    isAvailable: Boolean(source.isAvailable),
  }
}

export function useCatalogClient() {
  const api = useApiClient()
  const auth = useAuthSessionStore()
  const config = useRuntimeConfig()

  async function queryArticles(params: QueryArticlesParams) {
    const query = buildQuery({
      Filter: params.filter || undefined,
      Page: params.page,
      PageSize: params.pageSize,
    })
    const response = await api.request<PaginationResponse<Record<string, unknown>>>(
      "catalog",
      `${PUBLIC_ARTICLES_PATH}${query}`,
    )

    if (!response.ok || !response.data) {
      return response as ApiResult<QueryArticlesResponse>
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
    const response = await api.request<Record<string, unknown>>(
      "catalog",
      `${PUBLIC_ARTICLES_PATH}/${code}`,
    )

    if (!response.ok || !response.data) {
      return response as ApiResult<GetArticleResponse>
    }

    return {
      ...response,
      data: mapGetArticleResponse(response.data),
    } satisfies ApiResult<GetArticleResponse>
  }

  async function createArticle(payload: CreateArticleRequest) {
    const response = await api.request<Record<string, unknown>>("catalog", ADMIN_ARTICLES_PATH, {
      method: "POST",
      body: JSON.stringify(payload),
    })

    if (!response.ok || !response.data) {
      return response as ApiResult<CreateArticleResponse>
    }

    return {
      ...response,
      data: mapArticleMutationResponse(response.data) as CreateArticleResponse,
    } satisfies ApiResult<CreateArticleResponse>
  }

  async function updateArticle(code: string, payload: UpdateArticleRequest) {
    const response = await api.request<Record<string, unknown>>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )

    if (!response.ok || !response.data) {
      return response as ApiResult<UpdateArticleResponse>
    }

    return {
      ...response,
      data: mapArticleMutationResponse(response.data) as UpdateArticleResponse,
    } satisfies ApiResult<UpdateArticleResponse>
  }

  async function deleteArticle(code: string) {
    return api.request("catalog", `${ADMIN_ARTICLES_PATH}/${code}`, {
      method: "DELETE",
    })
  }

  async function addArticleImage(
    code: string,
    payload: AddArticleImageRequest,
  ) {
    const formData = new FormData()
    formData.append("orderIndex", String(payload.orderIndex))
    formData.append("file", payload.file)
    if (payload.imageAlt) formData.append("imageAlt", payload.imageAlt)

    return api.request<AddArticleImageResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/images`,
      {
        method: "POST",
        body: formData,
      },
    )
  }

  async function updateArticleImage(
    code: string,
    orderIndex: number | string,
    payload: UpdateArticleImageRequest,
  ) {
    return api.request<UpdateArticleImageResponse>(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/images/${orderIndex}`,
      {
        method: "PATCH",
        body: JSON.stringify(payload),
      },
    )
  }

  async function removeArticleImage(
    code: string,
    orderIndex: number | string,
  ) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/images/${orderIndex}`,
      {
        method: "DELETE",
      },
    )
  }

  async function addArticleDetail(
    code: string,
    payload: AddArticleDetailRequest,
  ) {
    return api.request(
      "catalog",
      `${ADMIN_ARTICLES_PATH}/${code}/details`,
      {
        method: "POST",
        body: JSON.stringify(payload),
      },
    )
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
    const baseUrl = config.public.catalogApiBaseUrl
    if (!baseUrl) {
      return {
        ok: false,
        status: 0,
        error: "Missing catalog API base URL.",
      }
    }

    const endpoint = `${baseUrl.replace(/\/+$/, "")}/api/admin/v1/catalog/archive/export`
    const token = auth.getAccessToken()
    const headers = new Headers({ Accept: "application/zip" })
    if (token) headers.set("Authorization", token)

    const runFetch = () =>
      fetch(endpoint, {
        method: "GET",
        headers,
      })

    let response = await runFetch()

    if (response.status === 401) {
      await auth.refreshTokens()
      const refreshedToken = auth.getAccessToken()
      if (refreshedToken) {
        headers.set("Authorization", refreshedToken)
        response = await runFetch()
      }
    }

    if (!response.ok) {
      const contentType = response.headers.get("content-type") || ""
      const payload = contentType.includes("application/json")
        ? await response.json().catch(() => null)
        : await response.text().catch(() => null)
      const error =
        payload?.title ||
        payload?.detail ||
        (typeof payload === "string" ? payload : null) ||
        "Unable to export catalog archive."

      return { ok: false, status: response.status, error }
    }

    const blob = await response.blob()
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
