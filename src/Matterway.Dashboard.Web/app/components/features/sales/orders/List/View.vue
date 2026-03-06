<script setup lang="ts">
import type { OrderResponse } from "~/types/sales"

withDefaults(
  defineProps<{
    items?: OrderResponse[]
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

function asOrderItem(item: Record<string, unknown>) {
  return item as OrderResponse
}
</script>

<template>
  <EntitiesListPanel
    title="Orders"
    description="Use pagination and optionally filter by exact Customer ID."
    :items="items"
    item-key="id"
    item-title-key="id"
    item-subtitle-key="customerId"
    :selected-id="selectedId"
    :filter="filter"
    filter-input-type="input"
    filter-placeholder="Optional exact Customer ID (GUID)."
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
      <SalesOrdersListItem :item="asOrderItem(item)" />
    </template>
  </EntitiesListPanel>
</template>
