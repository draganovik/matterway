<script setup lang="ts">
import type { CatalogArticle } from "~/types/catalog"

const props = defineProps<{
  items: CatalogArticle[]
  isRefreshing: boolean
}>()
</script>

<template>
  <div class="@container">
    <div
      class="grid grid-cols-1 gap-4 transition-opacity @lg:grid-cols-2 @3xl:grid-cols-3"
      :class="{ 'opacity-70': props.isRefreshing }"
    >
      <ArticlesBrowseListItem
        v-for="article in props.items"
        :key="article.code"
        :article="article"
      />
    </div>
  </div>
</template>
