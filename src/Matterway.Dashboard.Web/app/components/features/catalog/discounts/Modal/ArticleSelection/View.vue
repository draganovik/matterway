<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { QueryArticleResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
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

const api = useCatalogClient()
const listState = useRequestState({ empty: "Nema artikala." })

const articles = ref<QueryArticleResponse[]>([])
const filter = ref("")
const selectedArticleCodes = ref<string[]>([])
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1,
})

const pageSizes = [10, 20, 50, 100].map((value) => ({
  label: `${value} / strana`,
  value,
}))

const safeTotal = computed(() =>
  Math.max(0, Number(pagination.totalCount) || 0),
)

const selectedOnPageCount = computed(
  () =>
    articles.value.filter(
      (item) => item.code && selectedArticleCodes.value.includes(item.code),
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
    selectedArticleCodes.value = [...new Set(props.selectedIds)]
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
    filter: String(filter.value ?? "").trim() || undefined,
    page: pagination.page,
    pageSize: pagination.pageSize,
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || "Učitavanje artikala nije uspelo."
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
  const next = Math.min(Math.max(1, page), Math.max(1, pagination.totalPages))
  pagination.page = next
}

function changePageSize(value: number) {
  const next = Math.max(1, Number(value) || 20)
  if (next === pagination.pageSize) return
  pagination.pageSize = next
  pagination.page = 1
}

function isSelected(id: string) {
  return selectedArticleCodes.value.includes(id)
}

function toggleSelection(id: string) {
  if (isSelected(id)) {
    selectedArticleCodes.value = selectedArticleCodes.value.filter(
      (item) => item !== id,
    )
    return
  }
  selectedArticleCodes.value = [...selectedArticleCodes.value, id]
}

function togglePageSelection() {
  const currentIds = articles.value
    .map((item) => item.code)
    .filter((id): id is string => Boolean(id))

  if (!currentIds.length) return

  if (allCurrentPageSelected.value) {
    selectedArticleCodes.value = selectedArticleCodes.value.filter(
      (id) => !currentIds.includes(id),
    )
    return
  }

  const next = new Set(selectedArticleCodes.value)
  currentIds.forEach((id) => next.add(id))
  selectedArticleCodes.value = [...next]
}

function clearSelection() {
  selectedArticleCodes.value = []
}

function submitSelection() {
  emit("submit", [...new Set(selectedArticleCodes.value)])
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Izbor artikala</h3>
        <p class="text-muted text-sm">
          Pretražite i izaberite artikle povezane sa ovim popustom.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField label="Pretraga">
          <UTextarea
            v-model="filter"
            placeholder="Pretraga pomoću RSQL filtera (npr. title==chair;available==true)."
            :rows="3"
            class="w-full"
          />
        </UFormField>

        <div class="flex flex-wrap items-center gap-2">
          <UButton
            color="primary"
            :loading="listState.loading"
            @click="searchArticles"
          >
            {{ listState.loading ? "Pretraga" : "Pretraži" }}
          </UButton>
          <UButton
            variant="outline"
            :disabled="!articles.length"
            @click="togglePageSelection"
          >
            {{ allCurrentPageSelected ? "Ukloni prikazane" : "Izaberi prikazane" }}
          </UButton>
          <UButton
            variant="ghost"
            :disabled="!selectedArticleCodes.length"
            @click="clearSelection"
          >
            Očisti
          </UButton>
        </div>

        <CatalogDiscountsModalArticleSelectionListView
          :items="articles"
          :selected-ids="selectedArticleCodes"
          :loading="listState.loading"
          :error="listState.error"
          :empty-message="listState.empty"
          @toggle="toggleSelection"
        />

        <div
          class="border-default flex flex-wrap items-center justify-between gap-3 border-t pt-3"
        >
          <div class="text-muted text-sm">
            Strana {{ pagination.page }} od {{ pagination.totalPages }} -
            ukupno {{ pagination.totalCount }}
          </div>
          <div class="flex items-center gap-2">
            <UPagination
              :page="pagination.page"
              :items-per-page="pagination.pageSize"
              :total="safeTotal"
              :sibling-count="1"
              show-controls
              @update:page="changePage"
            />
            <UFormField label="Po stranici">
              <USelectMenu
                :items="pageSizes"
                :model-value="pagination.pageSize"
                value-key="value"
                label-key="label"
                placeholder="Izaberi broj"
                class="min-w-34"
                @update:model-value="changePageSize"
              />
            </UFormField>
          </div>
        </div>
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton variant="ghost" @click="isOpen = false"> Otkaži </UButton>
        <UButton color="primary" :disabled="!canEdit" @click="submitSelection">
          Sačuvaj izbor ({{ selectedArticleCodes.length }})
        </UButton>
      </div>
    </template>
  </UModal>
</template>
