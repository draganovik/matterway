<script setup lang="ts">
import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import type { SystemUserResponse } from "~/types/identity"

type RoleFilterValue = "all" | "customers" | "employees"

withDefaults(
  defineProps<{
    items?: SystemUserResponse[]
    roleFilter?: RoleFilterValue
    roleFilterOptions?: Array<{ label: string; value: RoleFilterValue }>
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
    roleFilter: "all",
    roleFilterOptions: () => [],
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
  "update:role-filter": [value: RoleFilterValue]
  "update:filter": [value: string]
  search: []
  "update:page": [value: number]
  "update:page-size": [value: number]
  select: [value: string]
}>()

function updateRoleFilter(value: string | number | null | undefined) {
  emit("update:role-filter", String(value || "all") as RoleFilterValue)
}
</script>

<template>
  <EntitiesListPanel
    :items="items"
    item-key="id"
    :selected-id="selectedId"
    :filter="filter"
    filter-input-type="input"
    filter-placeholder="Opciono: tačan ID naloga (GUID)."
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
    <template #search-controls>
      <UFormField label="Uloga" class="w-full">
        <USelect
          :items="roleFilterOptions"
          :model-value="roleFilter"
          placeholder="Sve uloge"
          class="w-full"
          @update:model-value="updateRoleFilter($event)"
        />
      </UFormField>
    </template>

    <template #item="{ item }">
      <UsersAccountsListItem :item="item" />
    </template>
  </EntitiesListPanel>
</template>
