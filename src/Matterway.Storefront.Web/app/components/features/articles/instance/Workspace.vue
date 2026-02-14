<script setup lang="ts">
import { useCatalogApi } from "~/composables/useCatalogApi"
import type { CatalogArticle } from "~/types/catalog/articles"

const catalogApi = useCatalogApi()
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

watch(
  () => route.params.slug,
  () => {
    void loadArticle()
  },
  { immediate: true },
)
</script>

<template>
  <div class="space-y-6">
    <StatusMessages v-if="error" :error="error" />

    <div v-if="loading" class="flex flex-col gap-6 lg:flex-row">
      <div class="flex flex-col gap-6 lg:w-2/5 lg:flex-none">
        <USkeleton class="aspect-4/3" />
        <USkeleton class="h-72" />
      </div>
      <div class="flex flex-col gap-6 lg:w-3/5 lg:flex-none">
        <USkeleton class="h-56" />
        <USkeleton class="h-72" />
      </div>
    </div>

    <template v-else-if="article">
      <div class="flex flex-col gap-6 lg:flex-row">
        <div class="flex flex-col gap-6 lg:w-2/5 lg:flex-none">
          <ArticlesInstanceImageCarousel :images="gallery" />
          <ArticlesInstanceDescription :description="article.description" />
        </div>

        <div class="flex flex-col gap-6 lg:w-3/5 lg:flex-none">
          <ArticlesInstanceOverview :article="article" />
          <ArticlesInstanceDetailsTable :details="article.articleDetails" />
        </div>
      </div>
    </template>
  </div>
</template>
