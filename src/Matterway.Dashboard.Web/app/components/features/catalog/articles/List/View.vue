<script setup lang="ts">
import type { QueryArticleResponse } from "~/types/catalog"

withDefaults(
  defineProps<{
    items?: QueryArticleResponse[]
    selectedId?: string | null
    filter?: string
    loading?: boolean
    error?: string
    emptyMessage?: string
    page?: number
    pageSize?: number
    totalCount?: number
    totalPages?: number
  }>(),
  {
    items: () => [],
    selectedId: null,
    filter: "",
    loading: false,
    error: "",
    emptyMessage: "",
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 1,
  },
)

const emit = defineEmits<{
  "update:filter": [value: string]
  search: []
  "update:page": [value: number]
  "update:page-size": [value: number]
  select: [value: string]
}>()
</script>

<template>
  <EntitiesListPanel
    class="articles-list-panel"
    title="Articles"
    description="Use RSQL filters to locate articles by title, code, or attributes."
    :items="items"
    item-key="id"
    item-title-key="title"
    item-subtitle-key="code"
    :selected-id="selectedId"
    :filter="filter"
    :loading="loading"
    :error="error"
    :empty-message="emptyMessage"
    :page="page"
    :page-size="pageSize"
    :total-count="totalCount"
    :total-pages="totalPages"
    @update:filter="emit('update:filter', $event)"
    @search="emit('search')"
    @update:page="emit('update:page', $event)"
    @update:page-size="emit('update:page-size', $event)"
    @select="emit('select', $event)"
  >
    <template #item="{ item }">
      <CatalogArticlesListItem :item="item" />
    </template>
  </EntitiesListPanel>
</template>

<style scoped>
.articles-list-panel :deep(textarea) {
  min-height: 6rem;
  max-height: 12rem;
  overflow-y: auto;
}
</style>
