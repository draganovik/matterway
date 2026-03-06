import { useApiClient } from "~/composables/api/useApiClient"
import {
  mapCatalogArticleDetail,
  mapCatalogArticleListItem,
  type CatalogArticle,
} from "~/types/catalog/articles"
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
      PaginatedPayload<Record<string, unknown>>
    >("catalog", `public/articles?${query.toString()}`, { method: "GET" }, true)

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
    const response = await api.request<Record<string, unknown>>(
      "catalog",
      `public/articles/${encodeURIComponent(code)}`,
      { method: "GET" },
      true,
    )

    if (!response.ok) {
      return { item: null, error: response.error }
    }

    return {
      item: response.data ? mapCatalogArticleDetail(response.data) : null,
    }
  }

  return {
    browseArticles,
    getArticle,
  }
}
