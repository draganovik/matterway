<script setup lang="ts">
import { useArticleBrowser } from "~/composables/features/useArticleBrowser";

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
} = useArticleBrowser();
</script>

<template>
  <div class="grid gap-6 lg:grid-cols-[22rem_minmax(0,1fr)]">
    <div class="space-y-5">
      <FeaturesArticlesBrowseFiltersCard
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

      <FeaturesArticlesBrowseResultsToolbar
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

      <CommonEmptyState
        v-else-if="!items.length"
        title="Nema odgovarajućih artikala"
        description="Prilagodite filtere ili ih poništite za pregled celog kataloga."
        icon="i-lucide-search-x"
      >
        <UButton color="neutral" variant="soft" @click="resetFilters"
          >Poništi filtere</UButton
        >
      </CommonEmptyState>

      <div
        v-else
        class="grid gap-4 transition-opacity sm:grid-cols-2 xl:grid-cols-3"
        :class="{ 'opacity-70': isRefreshing }"
      >
        <FeaturesArticlesBrowseListItem
          v-for="article in items"
          :key="article.id"
          :article="article"
        />
      </div>

      <div
        v-if="(meta?.totalPages ?? 0) > 1"
        class="flex flex-wrap items-center gap-2"
      >
        <UButton
          color="neutral"
          variant="soft"
          :disabled="pagination.page <= 1"
          @click="goToPage(pagination.page - 1)"
        >
          Prethodna
        </UButton>

        <UButton
          v-for="page in pages"
          :key="`page-${page}`"
          :variant="page === pagination.page ? 'solid' : 'soft'"
          :color="page === pagination.page ? 'primary' : 'neutral'"
          @click="goToPage(page)"
        >
          {{ page }}
        </UButton>

        <UButton
          color="neutral"
          variant="soft"
          :disabled="pagination.page >= (meta?.totalPages ?? 0)"
          @click="goToPage(pagination.page + 1)"
        >
          Sledeća
        </UButton>
      </div>
    </div>
  </div>
</template>
