import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { CatalogArticle } from "~/types/catalog/articles"

export function useArticlesInstancePage() {
  const catalogApi = useCatalogClient()
  const route = useRoute()

  const article = ref<CatalogArticle | null>(null)
  const error = ref("")
  const loading = ref(true)

  const pageTitle = computed(() => {
    const title = article.value?.title?.trim()
    return title && title.length ? title : "Artikal"
  })

  useHead({
    title: pageTitle,
  })

  const gallery = computed(() => {
    if (!article.value) return []
    const images = [
      article.value.thumbnailImage,
      ...(article.value.articleImages ?? []),
    ]
      .filter((item) => item?.imageUrl)
      .map((item, index) => ({
        id: item?.id || `${index}`,
        url: item?.imageUrl as string,
        alt: item?.imageAlt || article.value?.title || "Slika artikla",
      }))

    const unique = new Map<string, (typeof images)[number]>()
    images.forEach((img) => {
      if (!unique.has(img.url)) unique.set(img.url, img)
    })
    return Array.from(unique.values())
  })

  async function loadArticle() {
    const slug = route.params.slug
    if (typeof slug !== "string" || !slug.trim()) {
      article.value = null
      loading.value = false
      error.value = "Artikal nije pronađen."
      return
    }

    loading.value = true
    error.value = ""

    const result = await catalogApi.getArticle(slug)
    article.value = result.item
    if (result.error) error.value = result.error
    if (!result.item && !result.error) error.value = "Artikal nije pronađen."

    loading.value = false
  }

  async function initialize() {
    await loadArticle()
  }

  watch(
    () => route.params.slug,
    () => {
      void loadArticle()
    },
  )

  return {
    article,
    error,
    loading,
    gallery,
    initialize,
  }
}
