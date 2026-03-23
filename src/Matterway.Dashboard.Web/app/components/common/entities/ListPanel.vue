<script setup lang="ts">
import {
  DEFAULT_PAGINATION_PAGE_SIZE,
  PAGINATION_PAGE_SIZE_OPTIONS,
} from "~/constants/pagination"

const props = withDefaults(
  defineProps<{
    title: string
    description?: string
    items: Array<Record<string, unknown>>
    itemKey?: string
    itemTitleKey?: string
    itemSubtitleKey?: string
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
    description: "",
    itemKey: "id",
    itemTitleKey: "title",
    itemSubtitleKey: "",
    selectedId: null,
    filter: "",
    loading: false,
    error: "",
    emptyMessage: "Nema rezultata.",
    filterInputType: "textarea",
    filterPlaceholder:
      "Pretraga uz RSQL filtere (npr. title==chair; available==true).",
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
  <div class="flex h-full min-h-0 flex-col gap-4">
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
        />
      </template>

      <div class="flex flex-wrap items-end gap-3 [&>*]:min-w-0 [&>*]:flex-1">
        <slot name="search-controls" />
        <UFormField label="Po stranici" class="w-full">
          <USelectMenu
            :items="pageSizes"
            :model-value="pageSize"
            value-key="value"
            label-key="label"
            placeholder="Izaberi broj"
            class="w-full"
            @update:model-value="updatePageSize"
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

    <div class="min-h-0 flex-1 overflow-y-auto">
      <StatusMessages
        v-if="error || loading || !items.length"
        :error="error"
        :loading="loading ? 'Učitavanje rezultata.' : false"
        :empty="!loading && !error && !items.length ? emptyMessage : false"
      />
      <div v-else class="flex flex-col gap-2">
        <EntitiesListItem
          v-for="item in items"
          :key="String(item[itemKey])"
          :selected="selectedId === String(item[itemKey])"
          class="flex min-h-19 flex-col gap-2"
          @click="selectItem(item)"
        >
          <slot
            name="item"
            :item="item"
            :selected="selectedId === String(item[itemKey])"
          >
            <div class="text-foreground text-base font-medium">
              {{ item[itemTitleKey] || "Bez naslova" }}
            </div>
            <div
              v-if="itemSubtitleKey && item[itemSubtitleKey]"
              class="text-muted text-sm"
            >
              {{ item[itemSubtitleKey] }}
            </div>
          </slot>
        </EntitiesListItem>
      </div>
    </div>

    <div class="border-default shrink-0 border-t pt-3">
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
