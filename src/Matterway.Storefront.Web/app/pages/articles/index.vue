<script setup lang="ts">
import { useArticlesBrowsePage } from "~/composables/features/articles/useArticlesBrowsePage"

definePageMeta({
  title: "Artikli",
  public: true,
})

const {
  filters,
  pagination,
  items,
  meta,
  error,
  showInitialSkeleton,
  isRefreshing,
  detailDefinitions,
  detailDefinitionsLoading,
  pageOptions,
  submitFilters,
  resetFilters,
  addDetailFilter,
  removeDetailFilter,
  onDetailFilterSlugChange,
  setSearch,
  setMinPrice,
  setMaxPrice,
  setDetailFilterValue,
  setDetailFilterMin,
  setDetailFilterMax,
  goToPage,
  changePageSize,
} = useArticlesBrowsePage()

const resultsSection = ref<HTMLElement | null>(null)
const scrollAfterLoad = ref(false)

watch(
  items,
  () => {
    if (!scrollAfterLoad.value) return

    scrollAfterLoad.value = false
    resultsSection.value?.scrollIntoView({ block: "start" })
  },
  { flush: "post" },
)

function handlePageChange(page: number) {
  if (page === pagination.page) return
  if (
    page < 1 ||
    ((meta.value?.totalPages ?? 0) && page > (meta.value?.totalPages ?? 0))
  ) {
    return
  }

  scrollAfterLoad.value = true
  goToPage(page)
}
</script>

<template>
  <div
    class="mx-auto grid w-full max-w-6xl gap-4 lg:grid-cols-[22rem_minmax(0,1fr)] lg:gap-2"
  >
    <div
      class="lg:border-default flex flex-col gap-4 lg:sticky lg:top-[5.0625rem] lg:h-[calc(100dvh-6.0625rem)] lg:max-h-[56rem] lg:min-h-0 lg:border-e lg:pe-4"
    >
      <div class="lg:min-h-0 lg:flex-1">
        <ArticlesBrowseSearchFilterPanel
          :filters="filters"
          :detail-definitions="detailDefinitions"
          :detail-definitions-loading="detailDefinitionsLoading"
          @submit="submitFilters"
          @reset="resetFilters"
          @add-detail-filter="addDetailFilter"
          @remove-detail-filter="removeDetailFilter"
          @set-search="setSearch"
          @set-min-price="setMinPrice"
          @set-max-price="setMaxPrice"
          @set-detail-filter-slug="onDetailFilterSlugChange"
          @set-detail-filter-value="setDetailFilterValue"
          @set-detail-filter-min="setDetailFilterMin"
          @set-detail-filter-max="setDetailFilterMax"
        />
      </div>

      <ArticlesBrowsePaginationPanel
        class="mt-auto hidden lg:block"
        :page="pagination.page"
        :page-size="pagination.pageSize"
        :page-options="pageOptions"
        :total-count="meta?.totalCount ?? 0"
        :total-pages="meta?.totalPages ?? 0"
        @go-to-page="handlePageChange"
        @update:page-size="changePageSize"
      />
    </div>

    <div ref="resultsSection" class="scroll-mt-20 space-y-4">
      <StatusMessages v-if="error" :error="error" />

      <div
        v-if="showInitialSkeleton"
        class="mx-auto flex max-w-[49.5rem] flex-wrap justify-start gap-3"
      >
        <USkeleton
          v-for="n in pagination.pageSize"
          :key="`article-skeleton-${n}`"
          class="h-96 w-full sm:w-64 sm:flex-none"
        />
      </div>

      <EmptyState
        v-else-if="!items.length"
        title="Nema odgovarajućih artikala"
        description="Prilagodite filtere ili ih poništite da biste videli ceo katalog."
        icon="i-lucide-search-x"
      >
        <UButton color="neutral" variant="soft" @click="resetFilters"
          >Poništi filtere</UButton
        >
      </EmptyState>

      <ArticlesBrowseListView
        v-else
        class="mx-auto max-w-[49.5rem] !justify-start"
        :items="items"
        :is-refreshing="isRefreshing"
      />

      <ArticlesBrowsePaginationPanel
        class="lg:hidden"
        :page="pagination.page"
        :page-size="pagination.pageSize"
        :page-options="pageOptions"
        :total-count="meta?.totalCount ?? 0"
        :total-pages="meta?.totalPages ?? 0"
        @go-to-page="handlePageChange"
        @update:page-size="changePageSize"
      />
    </div>
  </div>
</template>
