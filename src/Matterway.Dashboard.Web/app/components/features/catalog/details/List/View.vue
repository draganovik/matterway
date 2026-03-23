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
    pageSize?: number
  }>(),
  {
    items: () => [],
    selectedId: null,
    filter: "",
    loading: false,
    error: "",
    emptyMessage: "",
    pageSize: DEFAULT_PAGINATION_PAGE_SIZE,
  },
)

const emit = defineEmits<{
  "update:filter": [value: string]
  search: []
  "update:page-size": [value: number]
  select: [value: string]
}>()
</script>

<template>
  <EntitiesListPanel
    class="details-list-panel"
    title="Definicije detalja"
    description="Pretražite po nazivu i izaberite stavku za izmenu ili brisanje."
    :items="items"
    item-key="slug"
    item-title-key="title"
    item-subtitle-key="slug"
    :selected-id="selectedId"
    :filter="filter"
    filter-input-type="input"
    filter-placeholder="Pretražite definicije detalja po nazivu (npr. veličina ekrana)."
    :loading="loading"
    :error="error"
    :empty-message="emptyMessage"
    :page="1"
    :page-size="pageSize"
    :total-count="items.length"
    :total-pages="1"
    @update:filter="emit('update:filter', $event)"
    @search="emit('search')"
    @update:page-size="emit('update:page-size', $event)"
    @select="emit('select', $event)"
  >
    <template #item="{ item }">
      <CatalogDetailsListItem :item="item" />
    </template>
  </EntitiesListPanel>
</template>

<style scoped>
.details-list-panel :deep(button.min-h-19) {
  min-height: 3.5rem;
  padding-top: 0.5rem;
  padding-bottom: 0.5rem;
}
</style>
