<script setup lang="ts">
const props = withDefaults(defineProps<{
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
  filterInputType?: 'textarea' | 'input'
  filterPlaceholder?: string
  page?: number
  pageSize?: number
  totalCount?: number
  totalPages?: number
}>(), {
  description: '',
  itemKey: 'id',
  itemTitleKey: 'title',
  itemSubtitleKey: '',
  selectedId: null,
  filter: '',
  loading: false,
  error: '',
  emptyMessage: 'No results found.',
  filterInputType: 'textarea',
  filterPlaceholder: 'Search, use RSQL filters (e.g. title==chair; available==true).',
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1
})

const emit = defineEmits<{
  (event: 'update:filter' | 'select', value: string): void
  (event: 'update:page' | 'update:pageSize', value: number): void
  (event: 'search'): void
}>()

const filterInput = ref(props.filter)

watch(
  () => props.filter,
  (value) => {
    filterInput.value = value || ''
  }
)

const pageSizes = [10, 20, 50, 100].map(value => ({ label: `${value} / page`, value }))

const safeTotalPages = computed(() => Math.max(1, Number(props.totalPages) || 1))

function applySearch() {
  emit('update:filter', filterInput.value.trim())
  emit('search')
}

function selectItem(item: Record<string, unknown>) {
  const key = String(item[props.itemKey] ?? '')
  if (key) emit('select', key)
}

function updatePage(value: number) {
  const next = Math.min(Math.max(1, value), safeTotalPages.value)
  if (next !== props.page) emit('update:page', next)
}

function updatePageSize(value: number) {
  emit('update:pageSize', value)
}
</script>

<template>
  <div class="flex flex-col gap-5">
    <header class="space-y-1">
      <h2 class="text-base font-semibold text-foreground">
        {{ title }}
      </h2>
      <p
        v-if="description"
        class="text-sm text-muted"
      >
        {{ description }}
      </p>
    </header>

    <div class="flex flex-col gap-3">
      <UTextarea
        v-if="filterInputType === 'textarea'"
        v-model="filterInput"
        :placeholder="filterPlaceholder"
        size="md"
        :rows="3"
      />
      <UInput
        v-else
        v-model="filterInput"
        :placeholder="filterPlaceholder"
        size="lg"
        class="w-full"
        @keydown.enter.prevent="applySearch"
      />
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
      <div
        v-if="error"
        class="rounded-lg border border-red-200/60 bg-red-50/60 px-4 py-3 text-sm text-red-600"
      >
        {{ error }}
      </div>
      <div
        v-else-if="loading"
        class="rounded-lg border border-default bg-background px-4 py-6 text-center text-sm text-muted"
      >
        Loading results.
      </div>
      <div
        v-else-if="!items.length"
        class="rounded-lg border border-default bg-background px-4 py-6 text-center text-sm text-muted"
      >
        {{ emptyMessage }}
      </div>
      <div
        v-else
        class="flex flex-col gap-2"
      >
        <button
          v-for="item in items"
          :key="String(item[itemKey])"
          type="button"
          class="flex w-full min-h-19 flex-col gap-2 rounded-xl border border-transparent bg-background px-4 py-3 text-left transition"
          :class="selectedId === String(item[itemKey])
            ? 'border-primary/40 bg-primary/5'
            : 'hover:border-default hover:bg-muted/40'"
          @click="selectItem(item)"
        >
          <slot
            name="item"
            :item="item"
            :selected="selectedId === String(item[itemKey])"
          >
            <div class="text-base font-medium text-foreground">
              {{ item[itemTitleKey] || 'Untitled' }}
            </div>
            <div
              v-if="itemSubtitleKey && item[itemSubtitleKey]"
              class="text-sm text-muted"
            >
              {{ item[itemSubtitleKey] }}
            </div>
          </slot>
        </button>
      </div>
    </div>

    <div class="flex flex-wrap items-center justify-between gap-3 border-t border-default pt-3">
      <div class="text-sm text-muted">
        Page {{ page }} of {{ safeTotalPages }} - {{ totalCount }} total
      </div>
      <div class="flex items-center gap-2">
        <USelectMenu
          :items="pageSizes"
          :model-value="pageSize"
          value-key="value"
          label-key="label"
          class="min-w-34"
          @update:model-value="updatePageSize"
        />
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
