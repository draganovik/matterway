<script setup lang="ts">
import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import type { QueryArticleResponse } from "~/types/catalog"

withDefaults(
  defineProps<{
    items?: QueryArticleResponse[]
    selectedCode?: string | null
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
    selectedCode: null,
    filter: "",
    loading: false,
    error: "",
    emptyMessage: "",
    page: 1,
    pageSize: DEFAULT_PAGINATION_PAGE_SIZE,
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
    title="Artikli"
    description="Koristite RSQL filtere da pronađete artikle po nazivu, šifri ili atributima."
    :items="items"
    item-key="code"
    item-title-key="title"
    item-subtitle-key="code"
    :selected-id="selectedCode"
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
