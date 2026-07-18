<script setup lang="ts">
import type { CatalogArticle } from "~/types/catalog"

const props = defineProps<{
  items: CatalogArticle[]
  isRefreshing: boolean
  carousel?: boolean
  ariaLabel?: string
}>()
</script>

<template>
  <ul
    class="flex items-stretch gap-3 transition-opacity"
    :class="[
      props.carousel
        ? 'snap-x snap-mandatory scroll-px-4 overflow-x-auto overscroll-x-contain scroll-smooth px-4 pb-2 sm:scroll-px-5 sm:px-5 lg:scroll-px-6 lg:px-6 min-[75rem]:scroll-px-[2.875rem] min-[75rem]:px-[2.875rem]'
        : 'flex-wrap justify-center',
      props.isRefreshing ? 'opacity-70 delay-150' : 'opacity-100 delay-0',
    ]"
    :aria-label="props.ariaLabel"
  >
    <li
      v-for="article in props.items"
      :key="article.code"
      :class="
        props.carousel
          ? 'w-[min(16rem,calc(100vw-2rem))] flex-none snap-start snap-always'
          : 'w-full sm:w-64 sm:flex-none'
      "
    >
      <ArticlesBrowseListItem :article="article" class="h-full" />
    </li>
  </ul>
</template>
