<script setup lang="ts">
withDefaults(
  defineProps<{
    items?: Array<Record<string, unknown>>
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

function asDiscountItem(item: Record<string, unknown>) {
  return item as {
    code: string
    percentage: number | string
    validFrom: string
    validTo?: string | null
    articleCodes: string[]
  }
}
</script>

<template>
  <EntitiesListPanel
    title="Popusti"
    description="Pretražite postojeće popuste po kodu, datumu ili procentu."
    :items="items"
    item-key="key"
    item-title-key="code"
    :selected-id="selectedId"
    :filter="filter"
    filter-input-type="input"
    filter-placeholder="Pretražite postojeće popuste po kodu ili vrednosti."
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
      <CatalogDiscountsListItem :item="asDiscountItem(item)" />
    </template>
  </EntitiesListPanel>
</template>
