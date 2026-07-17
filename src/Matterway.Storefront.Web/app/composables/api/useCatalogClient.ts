import { useApiClient } from "~/composables/api/useApiClient"
import {
  mapCatalogArticleDetail,
  mapCatalogArticleListItem,
  type CatalogArticle,
  type CatalogArticleDetailResponse,
  type CatalogArticleListResponse,
  type CatalogDetailDefinition,
} from "~/types/catalog"
import type { PaginatedPayload, PaginationMeta } from "~/types/common/api"

export function useCatalogClient() {
  const api = useApiClient()

  async function browseArticles(params: {
    page: number
    pageSize: number
    filter?: string
  }): Promise<{
    items: CatalogArticle[]
    meta: PaginationMeta | null
    error?: string
  }> {
    const query = new URLSearchParams({
      page: String(params.page),
      pageSize: String(params.pageSize),
    })

    const normalizedFilter = params.filter?.trim()
    if (normalizedFilter) {
      query.set("filter", normalizedFilter)
    }

    const response = await api.request<
      PaginatedPayload<CatalogArticleListResponse>
    >("catalog", `public/articles?${query.toString()}`, { method: "GET" })

    if (!response.ok) {
      return { items: [], meta: null, error: response.error }
    }

    const payload = response.data
    const items = (payload?.data ?? []).map((item) =>
      mapCatalogArticleListItem(item),
    )

    return {
      items,
      meta: payload?.meta ?? null,
    }
  }

  async function getArticle(code: string): Promise<{
    item: CatalogArticle | null
    error?: string
  }> {
    const response = await api.request<CatalogArticleDetailResponse>(
      "catalog",
      `public/articles/${encodeURIComponent(code)}`,
      { method: "GET" },
    )

    if (!response.ok) {
      return { item: null, error: response.error }
    }

    return {
      item: response.data ? mapCatalogArticleDetail(response.data) : null,
    }
  }

  async function queryDetailDefinitions(): Promise<{
    items: CatalogDetailDefinition[]
    error?: string
  }> {
    const items: CatalogDetailDefinition[] = []
    const pageSize = 200
    let page = 1

    while (true) {
      const query = new URLSearchParams({
        page: String(page),
        pageSize: String(pageSize),
      })

      const response = await api.request<
        PaginatedPayload<CatalogDetailDefinition>
      >("catalog", `public/details?${query.toString()}`, { method: "GET" })

      if (!response.ok) {
        return { items: [], error: response.error }
      }

      if (response.status === 204 || !response.data) {
        return { items }
      }

      items.push(...(response.data.data ?? []))

      const totalPages = Math.max(
        1,
        Number(response.data.meta?.totalPages) || 1,
      )
      if (page >= totalPages) {
        return { items }
      }

      page += 1
    }
  }

  return {
    browseArticles,
    getArticle,
    queryDetailDefinitions,
  }
}
