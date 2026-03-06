<script setup lang="ts">
import { useCatalogArticlesPage } from "~/composables/features/catalog/useCatalogArticlesPage"

definePageMeta({
  title: "Articles",
  service: "catalog",
  permissions: ["observer", "operator", "manager"],
})

const {
  canEdit,
  listState,
  articles,
  filter,
  pagination,
  changePage,
  changePageSize,
  selectedId,
  selectedArticle,
  articleState,
  createModalOpen,
  selectArticle,
  updateSelectedArticle,
  handleArticleCreated,
  searchArticles,
} = useCatalogArticlesPage()
</script>

<template>
  <UDashboardPanel
    id="catalog-articles"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Articles">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
          <div
            class="flex shrink-0 flex-wrap items-center justify-between gap-3"
          >
            <div>
              <h2 class="text-foreground text-base font-semibold">
                Browse Articles
              </h2>
              <p class="text-muted text-sm">
                Search existing articles and enrich a selected article with
                images and details.
              </p>
            </div>

            <UButton
              color="primary"
              :disabled="!canEdit"
              @click="createModalOpen = true"
            >
              Create New
            </UButton>
          </div>

          <EntitiesSplitView
            class="min-h-0 flex-1"
            list-class="overflow-hidden"
            detail-class="overflow-y-auto"
            :detail-loading="articleState.loading"
          >
            <template #list>
              <CatalogArticlesListView
                :items="articles"
                :selected-id="selectedId"
                :filter="filter"
                :loading="listState.loading"
                :error="listState.error"
                :empty-message="listState.empty"
                :page="pagination.page"
                :page-size="pagination.pageSize"
                :total-count="pagination.totalCount"
                :total-pages="pagination.totalPages"
                @update:filter="(value) => (filter = value)"
                @search="searchArticles"
                @update:page="changePage"
                @update:page-size="changePageSize"
                @select="selectArticle"
              />
            </template>

            <template #detail>
              <CatalogArticlesInformationPanel
                :article="selectedArticle"
                :error="articleState.error"
                :can-edit="canEdit"
                @update:article="updateSelectedArticle"
              />
            </template>
          </EntitiesSplitView>
        </div>

        <CatalogArticlesModalInformationView
          v-model:open="createModalOpen"
          :can-edit="canEdit"
          @created="handleArticleCreated"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
