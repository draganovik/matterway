<script setup lang="ts">
import { useCatalogApi } from '~/composables/useCatalogApi'
import type { QueryArticleResponse } from '~/types/catalog'
import { useRequestState } from '~/composables/useRequestState'

const props = withDefaults(defineProps<{
  open?: boolean
  selectedIds?: string[]
  canEdit?: boolean
}>(), {
  open: false,
  selectedIds: () => [],
  canEdit: false
})

const emit = defineEmits<{
  (event: 'update:open', value: boolean): void
  (event: 'submit', value: string[]): void
}>()

const isOpen = computed({
  get: () => props.open,
  set: (value: boolean) => emit('update:open', value)
})

const api = useCatalogApi()
const listState = useRequestState({ empty: 'No articles found.' })

const articles = ref<QueryArticleResponse[]>([])
const filter = ref('')
const selectedArticleIds = ref<string[]>([])
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1
})

function parseNumber(value: number | string | undefined, fallback: number) {
  if (value === undefined || value === null || value === '') return fallback
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : fallback
}

const pageSizes = [10, 20, 50, 100].map(value => ({ label: `${value} / page`, value }))

const selectedOnPageCount = computed(() =>
  articles.value.filter(item => item.id && selectedArticleIds.value.includes(item.id)).length
)
const allCurrentPageSelected = computed(() =>
  articles.value.length > 0 && selectedOnPageCount.value === articles.value.length
)

watch(
  () => props.open,
  (open) => {
    if (!open) return
    selectedArticleIds.value = [...new Set(props.selectedIds)]
    pagination.page = 1
    void loadArticles()
  }
)

watch([() => pagination.page, () => pagination.pageSize], () => {
  if (!isOpen.value) return
  void loadArticles()
})

async function loadArticles() {
  listState.loading = true
  listState.error = ''

  const result = await api.queryArticles({
    filter: filter.value.trim() || undefined,
    page: pagination.page,
    pageSize: pagination.pageSize
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || 'Unable to load articles.'
    articles.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    return
  }

  if (result.status === 204 || !result.data) {
    articles.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    return
  }

  articles.value = result.data.data || []
  pagination.totalCount = parseNumber(result.data.meta?.totalCount, articles.value.length)
  pagination.totalPages = Math.max(1, parseNumber(result.data.meta?.totalPages, 1))
  pagination.page = Math.max(1, parseNumber(result.data.meta?.currentPage, pagination.page))
  pagination.pageSize = Math.max(1, parseNumber(result.data.meta?.pageSize, pagination.pageSize))
}

function searchArticles() {
  if (pagination.page !== 1) {
    pagination.page = 1
    return
  }
  void loadArticles()
}

function changePage(page: number) {
  pagination.page = page
}

function changePageSize(value: number) {
  const next = Math.max(1, Number(value) || 20)
  if (next === pagination.pageSize) return
  pagination.pageSize = next
  pagination.page = 1
}

function isSelected(id: string) {
  return selectedArticleIds.value.includes(id)
}

function toggleSelection(id: string) {
  if (isSelected(id)) {
    selectedArticleIds.value = selectedArticleIds.value.filter(item => item !== id)
    return
  }
  selectedArticleIds.value = [...selectedArticleIds.value, id]
}

function togglePageSelection() {
  const currentIds = articles.value
    .map(item => item.id)
    .filter((id): id is string => Boolean(id))

  if (!currentIds.length) return

  if (allCurrentPageSelected.value) {
    selectedArticleIds.value = selectedArticleIds.value.filter(id => !currentIds.includes(id))
    return
  }

  const next = new Set(selectedArticleIds.value)
  currentIds.forEach(id => next.add(id))
  selectedArticleIds.value = [...next]
}

function clearSelection() {
  selectedArticleIds.value = []
}

function submitSelection() {
  emit('submit', [...new Set(selectedArticleIds.value)])
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-base font-semibold text-foreground">
          Select Articles
        </h3>
        <p class="text-sm text-muted">
          Search and select the articles attached to this discount.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UTextarea
          v-model="filter"
          placeholder="Search with RSQL filters (e.g. title==chair;available==true)."
          :rows="3"
        />

        <div class="flex flex-wrap items-center gap-2">
          <UButton
            color="primary"
            :loading="listState.loading"
            @click="searchArticles"
          >
            Search
          </UButton>
          <UButton
            variant="outline"
            :disabled="!articles.length"
            @click="togglePageSelection"
          >
            {{ allCurrentPageSelected ? 'Unselect Page' : 'Select Page' }}
          </UButton>
          <UButton
            variant="ghost"
            :disabled="!selectedArticleIds.length"
            @click="clearSelection"
          >
            Clear
          </UButton>
        </div>

        <div class="max-h-75 overflow-y-auto space-y-2">
          <div
            v-if="listState.error"
            class="rounded-lg border border-red-200/60 bg-red-50/60 px-4 py-3 text-sm text-red-600"
          >
            {{ listState.error }}
          </div>
          <div
            v-else-if="listState.loading"
            class="rounded-lg border border-default bg-background px-4 py-6 text-center text-sm text-muted"
          >
            Loading articles.
          </div>
          <div
            v-else-if="!articles.length"
            class="rounded-lg border border-default bg-background px-4 py-6 text-center text-sm text-muted"
          >
            {{ listState.empty }}
          </div>
          <div
            v-else
            class="space-y-2"
          >
            <button
              v-for="article in articles"
              :key="article.id"
              type="button"
              class="w-full rounded-xl border px-4 py-3 text-left transition"
              :class="isSelected(article.id)
                ? 'border-primary/40 bg-primary/5'
                : 'border-transparent bg-background hover:border-default hover:bg-muted/40'"
              @click="toggleSelection(article.id)"
            >
              <CatalogDiscountsListItem
                :item="article"
                :selected="isSelected(article.id)"
              />
            </button>
          </div>
        </div>

        <div class="flex flex-wrap items-center justify-between gap-3 border-t border-default pt-3">
          <div class="text-sm text-muted">
            Page {{ pagination.page }} of {{ pagination.totalPages }} - {{ pagination.totalCount }} total
          </div>
          <div class="flex items-center gap-2">
            <USelectMenu
              :items="pageSizes"
              :model-value="pagination.pageSize"
              value-key="value"
              label-key="label"
              class="min-w-34"
              @update:model-value="changePageSize"
            />
            <div class="flex items-center gap-1">
              <UButton
                variant="outline"
                :disabled="pagination.page <= 1"
                @click="changePage(pagination.page - 1)"
              >
                Prev
              </UButton>
              <UButton
                variant="outline"
                :disabled="pagination.page >= pagination.totalPages"
                @click="changePage(pagination.page + 1)"
              >
                Next
              </UButton>
            </div>
          </div>
        </div>
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :disabled="!canEdit"
          @click="submitSelection"
        >
          Save Selection ({{ selectedArticleIds.length }})
        </UButton>
      </div>
    </template>
  </UModal>
</template>
