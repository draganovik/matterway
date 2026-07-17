<script setup lang="ts">
import type { CatalogArticle } from "~/types/catalog"

const props = defineProps<{
  items: CatalogArticle[]
  isRefreshing: boolean
}>()
</script>

<template>
  <div
    class="flex flex-wrap items-stretch justify-center gap-3 transition-opacity"
    :class="{ 'opacity-70': props.isRefreshing }"
  >
    <ArticlesBrowseListItem
      v-for="article in props.items"
      :key="article.code"
      :article="article"
      class="w-full sm:w-60 sm:flex-none"
    />
  </div>
</template>
