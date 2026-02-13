<script setup lang="ts">
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
    emptyMessage: "No results found.",
    filterInputType: "textarea",
    filterPlaceholder:
      "Search, use RSQL filters (e.g. title==chair; available==true).",
    page: 1,
    pageSize: 20,
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

const pageSizes = [10, 20, 50, 100].map((value) => ({
  label: `${value} / page`,
  value,
}))

const safeTotalPages = computed(() =>
  Math.max(1, Number(props.totalPages) || 1),
)

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
  <div class="flex flex-col gap-5">
    <header class="space-y-1">
      <h2 class="text-foreground text-base font-semibold">
        {{ title }}
      </h2>
      <p v-if="description" class="text-muted text-sm">
        {{ description }}
      </p>
    </header>

    <div class="flex flex-col gap-3">
      <UFormField>
        <UTextarea
          v-if="filterInputType === 'textarea'"
          v-model="filterInput"
          :placeholder="filterPlaceholder"
          size="md"
          :rows="3"
          class="w-full"
        />
        <UInput
          v-else
          v-model="filterInput"
          :placeholder="filterPlaceholder"
          size="lg"
          class="w-full"
          @keydown.enter.prevent="applySearch"
        />
      </UFormField>
      <UButton
        color="primary"
        :loading="loading"
        class="w-full"
        @click="applySearch"
      >
        Search
      </UButton>
    </div>

    <div class="flex min-h-0 flex-1 flex-col gap-3">
      <StatusMessages
        v-if="error || loading || !items.length"
        :error="error"
        :loading="loading ? 'Loading results.' : false"
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
              {{ item[itemTitleKey] || "Untitled" }}
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

    <div
      class="border-default flex flex-wrap items-center justify-between gap-3 border-t pt-3"
    >
      <div class="text-muted text-sm">
        Page {{ page }} of {{ safeTotalPages }} - {{ totalCount }} total
      </div>
      <div class="flex items-center gap-2">
        <UFormField label="Page Size">
          <USelectMenu
            :items="pageSizes"
            :model-value="pageSize"
            value-key="value"
            label-key="label"
            placeholder="Select size"
            class="min-w-34"
            @update:model-value="updatePageSize"
          />
        </UFormField>
        <div class="flex items-center gap-1">
          <UButton
            variant="outline"
            :disabled="page <= 1"
            @click="updatePage(page - 1)"
          >
            Prev
          </UButton>
          <UButton
            variant="outline"
            :disabled="page >= safeTotalPages"
            @click="updatePage(page + 1)"
          >
            Next
          </UButton>
        </div>
      </div>
    </div>
  </div>
</template>
