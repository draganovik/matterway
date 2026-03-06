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
  pages,
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
</script>

<template>
  <div class="grid gap-6 lg:grid-cols-[22rem_minmax(0,1fr)]">
    <div class="space-y-5">
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

      <ArticlesBrowseSearchSummaryPanel
        :total-count="meta?.totalCount ?? 0"
        :page-size="pagination.pageSize"
        :page-options="pageOptions"
        @update:page-size="changePageSize"
      />
    </div>

    <div class="space-y-5">
      <StatusMessages v-if="error" :error="error" />

      <div
        v-if="showInitialSkeleton"
        class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3"
      >
        <USkeleton
          v-for="n in pagination.pageSize"
          :key="`article-skeleton-${n}`"
          class="h-96"
        />
      </div>

      <EmptyState
        v-else-if="!items.length"
        title="Nema odgovarajućih artikala"
        description="Prilagodite filtere ili ih poništite za pregled celog kataloga."
        icon="i-lucide-search-x"
      >
        <UButton color="neutral" variant="soft" @click="resetFilters"
          >Poništi filtere</UButton
        >
      </EmptyState>

      <ArticlesBrowseListView
        v-else
        :items="items"
        :is-refreshing="isRefreshing"
      />

      <ArticlesBrowsePaginationPanel
        :page="pagination.page"
        :pages="pages"
        :total-pages="meta?.totalPages ?? 0"
        @go-to-page="goToPage"
      />
    </div>
  </div>
</template>
