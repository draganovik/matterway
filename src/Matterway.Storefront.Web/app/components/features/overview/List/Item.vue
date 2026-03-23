<script setup lang="ts">
import type { CatalogArticle } from "~/types/catalog"

const props = defineProps<{
  article: CatalogArticle
}>()
</script>

<template>
  <ArticlesBrowseListItem :article="props.article" />
</template>
