import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { useCartStore } from "~/composables/stores/useCartStore"
import type { CatalogArticle } from "~/types/catalog"

export const OVERVIEW_CATEGORY_FILTERS = {
  light: "category==light",
  camera: "category==camera",
} as const

export function useOverviewPage() {
  const catalogApi = useCatalogClient()
  const cart = useCartStore()

  const loading = ref(true)
  const error = ref("")
  const latest = ref<CatalogArticle[]>([])
  const lights = ref<CatalogArticle[]>([])
  const cameras = ref<CatalogArticle[]>([])
  const totalCount = ref(0)

  async function loadOverview() {
    loading.value = true
    error.value = ""

    const [latestResult, lightsResult, camerasResult] = await Promise.all([
      catalogApi.browseArticles({ page: 1, pageSize: 4 }),
      catalogApi.browseArticles({
        page: 1,
        pageSize: 4,
        filter: OVERVIEW_CATEGORY_FILTERS.light,
      }),
      catalogApi.browseArticles({
        page: 1,
        pageSize: 4,
        filter: OVERVIEW_CATEGORY_FILTERS.camera,
      }),
    ])

    error.value =
      [latestResult, lightsResult, camerasResult].find((result) => result.error)
        ?.error ?? ""
    latest.value = latestResult.items
    lights.value = lightsResult.items
    cameras.value = camerasResult.items
    totalCount.value =
      latestResult.meta?.totalCount ?? latestResult.items.length
    loading.value = false
  }

  return {
    cart,
    loading,
    error,
    totalCount,
    latest,
    lights,
    cameras,
    loadOverview,
  }
}
