<script setup lang="ts">
import { useCatalogDetailsPage } from "~/composables/features/catalog/useCatalogDetailsPage"

definePageMeta({
  title: "Detalji",
})

const {
  canEdit,
  listState,
  saveState,
  removeState,
  details,
  filter,
  pagination,
  selectedSlug,
  selectedDetail,
  createModalOpen,
  deleteConfirmOpen,
  form,
  canDelete,
  beginCreate,
  requestRemoveDetail,
  selectDetail,
  searchDetails,
  changePage,
  changePageSize,
  saveDetail,
  removeDetail,
  handleDetailCreated,
} = useCatalogDetailsPage()
</script>

<template>
  <UDashboardPanel
    id="catalog-details"
    :ui="{
      body: 'min-h-0 overflow-hidden py-3',
    }"
  >
    <template #header>
      <UDashboardNavbar title="Detalji">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #trailing>
          <PageInfoTooltip
            text="Kreirajte i održavajte definicije detalja koje se koriste u vrednostima detalja artikala."
          />
        </template>

        <template #right>
          <UButton color="primary" :disabled="!canEdit" @click="beginCreate">
            Novi detalj
          </UButton>
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <EntitiesSplitView class="h-full min-h-0">
          <template #list>
            <CatalogDetailsListView
              :items="details"
              :selected-id="selectedSlug"
              :filter="filter"
              :loading="listState.loading"
              :error="listState.error"
              :empty-message="listState.empty"
              :page="pagination.page"
              :page-size="pagination.pageSize"
              :total-count="pagination.totalCount"
              :total-pages="pagination.totalPages"
              @update:filter="(value) => (filter = value)"
              @search="searchDetails"
              @update:page="changePage"
              @update:page-size="changePageSize"
              @select="selectDetail"
            />
          </template>

          <template #detail>
            <CatalogDetailsInformationPanel
              :detail="selectedDetail"
              :can-edit="canEdit"
              :can-delete="canDelete"
              :slug="form.slug"
              :title="form.title"
              :unit="form.unit"
              :save-loading="saveState.loading"
              :error="saveState.error"
              :success="saveState.success"
              @update:slug="(value) => (form.slug = value)"
              @update:title="(value) => (form.title = value)"
              @update:unit="(value) => (form.unit = value)"
              @save="saveDetail"
              @remove="requestRemoveDetail"
            />
          </template>
        </EntitiesSplitView>

        <CatalogDetailsCreateModal
          v-model:open="createModalOpen"
          :can-edit="canEdit"
          @created="handleDetailCreated"
        />

        <ConfirmDeleteModal
          v-model:open="deleteConfirmOpen"
          title="Obriši detalj"
          description="Definicija detalja će biti trajno uklonjena."
          :subject="
            selectedDetail
              ? `${selectedDetail.title || 'Detalj bez naziva'} · ${selectedDetail.slug || ''}`
              : ''
          "
          confirm-label="Obriši detalj"
          :loading="removeState.loading"
          :error="removeState.error"
          @confirm="removeDetail"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
