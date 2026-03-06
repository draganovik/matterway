import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { useCartStore } from "~/composables/stores/useCartStore"

export function useOverviewPage() {
  const catalogApi = useCatalogClient()
  const cart = useCartStore()

  const loading = ref(true)
  const error = ref("")
  const articles = ref(
    [] as Awaited<ReturnType<typeof catalogApi.browseArticles>>["items"],
  )
  const totalCount = ref(0)

  async function loadOverview() {
    loading.value = true
    error.value = ""

    const result = await catalogApi.browseArticles({
      page: 1,
      pageSize: 8,
    })

    if (result.error) {
      error.value = result.error
    }

    articles.value = result.items
    totalCount.value = result.meta?.totalCount ?? result.items.length
    loading.value = false
  }

  async function initialize() {
    await loadOverview()
  }

  const featured = computed(() => articles.value.slice(0, 4))
  const featuredCount = computed(() => featured.value.length)

  return {
    cart,
    loading,
    error,
    totalCount,
    featured,
    featuredCount,
    loadOverview,
    initialize,
  }
}
