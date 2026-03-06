<script setup lang="ts">
import type { CatalogArticle } from "~/types/catalog/articles"

const props = defineProps<{
  items: CatalogArticle[]
}>()
</script>

<template>
  <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
    <OverviewListItem
      v-for="article in props.items"
      :key="article.id"
      :article="article"
    />
  </div>
</template>
