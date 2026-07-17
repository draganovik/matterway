<script setup lang="ts">
import { useCatalogArticlesPage } from "~/composables/features/catalog/useCatalogArticlesPage"

definePageMeta({
  title: "Artikli",
})

const {
  canEdit,
  listState,
  articles,
  filter,
  pagination,
  changePage,
  changePageSize,
  selectedCode,
  selectedArticle,
  articleState,
  removeState,
  createModalOpen,
  deleteConfirmOpen,
  selectArticle,
  updateSelectedArticle,
  requestRemoveArticle,
  removeArticle,
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
      <UDashboardNavbar title="Artikli">
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
                Pregled artikala
              </h2>
              <p class="text-muted text-sm">
                Pretražite postojeće artikle i dopunite izabrani artikal slikama
                i detaljima.
              </p>
            </div>

            <UButton
              color="primary"
              :disabled="!canEdit"
              @click="createModalOpen = true"
            >
              Novi artikal
            </UButton>
          </div>

          <EntitiesSplitView
            class="min-h-0 flex-1"
            :detail-loading="articleState.loading"
          >
            <template #list>
              <CatalogArticlesListView
                :items="articles"
                :selected-code="selectedCode"
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
                @remove="requestRemoveArticle"
              />
            </template>
          </EntitiesSplitView>
        </div>

        <CatalogArticlesModalInformationView
          v-model:open="createModalOpen"
          :can-edit="canEdit"
          @created="handleArticleCreated"
        />

        <ConfirmDeleteModal
          v-model:open="deleteConfirmOpen"
          title="Obriši artikal"
          description="Artikal će biti trajno obrisan zajedno sa povezanim slikama."
          :subject="
            selectedArticle
              ? `${selectedArticle.title || 'Artikal bez naziva'} · ${selectedArticle.code}`
              : ''
          "
          confirm-label="Obriši artikal"
          :loading="removeState.loading"
          :error="removeState.error"
          @confirm="removeArticle"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
