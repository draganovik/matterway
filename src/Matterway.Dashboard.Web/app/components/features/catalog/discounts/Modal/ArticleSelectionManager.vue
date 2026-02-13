<script setup lang="ts">
import { useCatalogApi } from "~/composables/useCatalogApi"
import type { QueryArticleResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/useRequestState"
import { parseNumberOr } from "~/utils/numbers"

const props = withDefaults(
  defineProps<{
    open?: boolean
    selectedIds?: string[]
    canEdit?: boolean
  }>(),
  {
    open: false,
    selectedIds: () => [],
    canEdit: false,
  },
)

const emit = defineEmits<{
  (event: "update:open", value: boolean): void
  (event: "submit", value: string[]): void
}>()

const isOpen = computed({
  get: () => props.open,
  set: (value: boolean) => emit("update:open", value),
})

const api = useCatalogApi()
const listState = useRequestState({ empty: "No articles found." })

const articles = ref<QueryArticleResponse[]>([])
const filter = ref("")
const selectedArticleIds = ref<string[]>([])
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1,
})

const pageSizes = [10, 20, 50, 100].map((value) => ({
  label: `${value} / page`,
  value,
}))

const selectedOnPageCount = computed(
  () =>
    articles.value.filter(
      (item) => item.id && selectedArticleIds.value.includes(item.id),
    ).length,
)
const allCurrentPageSelected = computed(
  () =>
    articles.value.length > 0 &&
    selectedOnPageCount.value === articles.value.length,
)

watch(
  () => props.open,
  (open) => {
    if (!open) return
    selectedArticleIds.value = [...new Set(props.selectedIds)]
    pagination.page = 1
    void loadArticles()
  },
)

watch([() => pagination.page, () => pagination.pageSize], () => {
  if (!isOpen.value) return
  void loadArticles()
})

async function loadArticles() {
  listState.loading = true
  listState.error = ""

  const result = await api.queryArticles({
    filter: filter.value.trim() || undefined,
    page: pagination.page,
    pageSize: pagination.pageSize,
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || "Unable to load articles."
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
  pagination.totalCount = parseNumberOr(
    result.data.meta?.totalCount,
    articles.value.length,
  )
  pagination.totalPages = Math.max(
    1,
    parseNumberOr(result.data.meta?.totalPages, 1),
  )
  pagination.page = Math.max(
    1,
    parseNumberOr(result.data.meta?.currentPage, pagination.page),
  )
  pagination.pageSize = Math.max(
    1,
    parseNumberOr(result.data.meta?.pageSize, pagination.pageSize),
  )
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
    selectedArticleIds.value = selectedArticleIds.value.filter(
      (item) => item !== id,
    )
    return
  }
  selectedArticleIds.value = [...selectedArticleIds.value, id]
}

function togglePageSelection() {
  const currentIds = articles.value
    .map((item) => item.id)
    .filter((id): id is string => Boolean(id))

  if (!currentIds.length) return

  if (allCurrentPageSelected.value) {
    selectedArticleIds.value = selectedArticleIds.value.filter(
      (id) => !currentIds.includes(id),
    )
    return
  }

  const next = new Set(selectedArticleIds.value)
  currentIds.forEach((id) => next.add(id))
  selectedArticleIds.value = [...next]
}

function clearSelection() {
  selectedArticleIds.value = []
}

function submitSelection() {
  emit("submit", [...new Set(selectedArticleIds.value)])
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Select Articles</h3>
        <p class="text-muted text-sm">
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
            {{ allCurrentPageSelected ? "Unselect Page" : "Select Page" }}
          </UButton>
          <UButton
            variant="ghost"
            :disabled="!selectedArticleIds.length"
            @click="clearSelection"
          >
            Clear
          </UButton>
        </div>

        <div class="max-h-75 space-y-2 overflow-y-auto">
          <StatusMessages
            v-if="listState.error || listState.loading || !articles.length"
            :error="listState.error"
            :loading="listState.loading ? 'Loading articles.' : false"
            :empty="
              !listState.loading && !listState.error && !articles.length
                ? listState.empty
                : false
            "
          />
          <div v-else class="space-y-2">
            <EntitiesListItem
              v-for="article in articles"
              :key="article.id"
              :selected="isSelected(article.id)"
              @click="toggleSelection(article.id)"
            >
              <CatalogDiscountsListArticleItem
                :item="article"
                :selected="isSelected(article.id)"
              />
            </EntitiesListItem>
          </div>
        </div>

        <div
          class="border-default flex flex-wrap items-center justify-between gap-3 border-t pt-3"
        >
          <div class="text-muted text-sm">
            Page {{ pagination.page }} of {{ pagination.totalPages }} -
            {{ pagination.totalCount }} total
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
        <UButton variant="ghost" @click="isOpen = false"> Cancel </UButton>
        <UButton color="primary" :disabled="!canEdit" @click="submitSelection">
          Save Selection ({{ selectedArticleIds.length }})
        </UButton>
      </div>
    </template>
  </UModal>
</template>
