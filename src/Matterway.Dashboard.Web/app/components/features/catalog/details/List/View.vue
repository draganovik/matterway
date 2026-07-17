<script setup lang="ts">
import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import type { QueryDetailResponse } from "~/types/catalog"

withDefaults(
  defineProps<{
    items?: QueryDetailResponse[]
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
    :items="items"
    item-key="slug"
    :selected-id="selectedId"
    :filter="filter"
    filter-input-type="input"
    filter-placeholder="Pretražite definicije detalja po nazivu (npr. veličina ekrana)."
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
      <CatalogDetailsListItem :item="item" />
    </template>
  </EntitiesListPanel>
</template>
