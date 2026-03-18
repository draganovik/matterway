<script setup lang="ts">
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
    pageSize: 20,
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
</script>

<template>
  <EntitiesListPanel
    title="Nalozi"
    description="Filtrirajte po ulozi, koristite paginaciju ili unesite tačan ID naloga da biste prikazali samo taj nalog."
    :items="items"
    item-key="id"
    item-title-key="email"
    item-subtitle-key="id"
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
        <USelectMenu
          :items="roleFilterOptions"
          :model-value="roleFilter"
          value-key="value"
          label-key="label"
          placeholder="Sve uloge"
          class="w-full"
          @update:model-value="emit('update:role-filter', $event)"
        />
      </UFormField>
    </template>

    <template #item="{ item }">
      <UsersAccountsListItem :item="item" />
    </template>
  </EntitiesListPanel>
</template>
