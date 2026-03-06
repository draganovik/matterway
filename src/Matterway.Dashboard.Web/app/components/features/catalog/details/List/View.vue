<script setup lang="ts">
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
    pageSize: 20,
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
    title="Detail Definitions"
    description="Search by title and select one to update or remove."
    :items="items"
    item-key="slug"
    item-title-key="title"
    item-subtitle-key="slug"
    :selected-id="selectedId"
    :filter="filter"
    filter-input-type="input"
    filter-placeholder="Search detail definitions by title (e.g. screen size)."
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
