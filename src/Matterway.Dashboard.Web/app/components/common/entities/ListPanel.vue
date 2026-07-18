<script setup lang="ts">
import {
  DEFAULT_PAGINATION_PAGE_SIZE,
  PAGINATION_PAGE_SIZE_OPTIONS,
} from "~/constants/pagination"

const props = withDefaults(
  defineProps<{
    items: Array<Record<string, unknown>>
    itemKey?: string
    selectedId?: string | null
    filter?: string
    loading?: boolean
    error?: string
    emptyMessage?: string
    filterInputType?: "textarea" | "input"
    filterPlaceholder?: string
    page?: number
    pageSize?: number
    totalCount?: number
    totalPages?: number
  }>(),
  {
    itemKey: "id",
    selectedId: null,
    filter: "",
    loading: false,
    error: "",
    emptyMessage: "Nema rezultata.",
    filterInputType: "textarea",
    filterPlaceholder:
      "Pretraga uz RSQL filtere (npr. title==NAS; available==true).",
    page: 1,
    pageSize: DEFAULT_PAGINATION_PAGE_SIZE,
    totalCount: 0,
    totalPages: 1,
  },
)

const emit = defineEmits<{
  (event: "update:filter" | "select", value: string): void
  (event: "update:page" | "update:pageSize", value: number): void
  (event: "search"): void
}>()

const filterInput = ref(props.filter)

watch(
  () => props.filter,
  (value) => {
    filterInput.value = value || ""
  },
)

const pageSizes = PAGINATION_PAGE_SIZE_OPTIONS.map((value) => ({
  label: String(value),
  value,
}))

const safeTotal = computed(() =>
  Math.max(0, Number(props.totalCount) || props.items.length || 0),
)

const safeTotalPages = computed(() =>
  Math.max(1, Number(props.totalPages) || 1),
)

const normalizedFilterPlaceholder = computed(() => {
  const placeholder = (props.filterPlaceholder || "").trim()
  if (!placeholder) return "Pretraga"
  return placeholder
})

function applySearch() {
  emit("update:filter", filterInput.value.trim())
  emit("search")
}

function selectItem(item: Record<string, unknown>) {
  const key = String(item[props.itemKey] ?? "")
  if (key) emit("select", key)
}

function updatePage(value: number) {
  const next = Math.min(Math.max(1, value), safeTotalPages.value)
  if (next !== props.page) emit("update:page", next)
}

function updatePageSize(value: number) {
  emit("update:pageSize", value)
}
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-3">
    <div class="shrink-0 space-y-3">
      <div v-if="filterInputType === 'input'">
        <UInput
          v-model="filterInput"
          :placeholder="normalizedFilterPlaceholder"
          size="lg"
          class="w-full"
          @keydown.enter.prevent="applySearch"
        />
      </div>

      <template v-else>
        <UTextarea
          v-model="filterInput"
          :placeholder="normalizedFilterPlaceholder"
          size="md"
          :rows="3"
          class="w-full"
          @keydown.enter.exact.prevent="applySearch"
        />
      </template>

      <div class="flex flex-wrap items-end gap-3 [&>*]:min-w-0 [&>*]:flex-1">
        <slot name="search-controls" />
        <UFormField label="Po stranici" class="w-full">
          <USelect
            :items="pageSizes"
            :model-value="pageSize"
            placeholder="Izaberi broj"
            class="w-full"
            @update:model-value="updatePageSize(Number($event))"
          />
        </UFormField>
        <UButton
          color="primary"
          size="lg"
          :loading="loading"
          loading-icon="i-lucide-loader-2"
          :disabled="loading"
          class="w-full justify-center"
          @click="applySearch"
        >
          {{ loading ? "Pretraga" : "Pretraži" }}
        </UButton>
      </div>
    </div>

    <div class="border-muted min-h-0 flex-1 overflow-y-auto border-y p-1">
      <StatusMessages
        v-if="error || !items.length"
        :error="error"
        :loading="loading ? 'Učitavanje rezultata.' : false"
        :empty="!loading && !error && !items.length ? emptyMessage : false"
      />
      <div v-else class="flex flex-col">
        <EntitiesListItem
          v-for="item in items"
          :key="String(item[itemKey])"
          :selected="selectedId === String(item[itemKey])"
          class="flex flex-col"
          @click="selectItem(item)"
        >
          <slot
            name="item"
            :item="item"
            :selected="selectedId === String(item[itemKey])"
          />
        </EntitiesListItem>
      </div>
    </div>

    <div class="shrink-0">
      <div class="flex justify-center">
        <UPagination
          :page="page"
          :items-per-page="pageSize"
          :total="safeTotal"
          :sibling-count="1"
          show-controls
          @update:page="updatePage"
        />
      </div>
    </div>
  </div>
</template>
