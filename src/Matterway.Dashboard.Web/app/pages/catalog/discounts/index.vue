<script setup lang="ts">
import { useCatalogDiscountsPage } from "~/composables/features/catalog/useCatalogDiscountsPage"

definePageMeta({
  title: "Popusti",
})

const {
  canEdit,
  listState,
  submitState,
  deleteState,
  deleteConfirmOpen,
  discountFilter,
  listPage,
  listPageSize,
  selectedKey,
  selectedDiscount,
  createModalOpen,
  form,
  selectedArticleCodes,
  filteredDiscountCount,
  filteredDiscountPages,
  visibleDiscounts,
  updateDiscountFilter,
  searchDiscounts,
  changeListPage,
  changeListPageSize,
  beginCreate,
  selectDiscount,
  saveDiscount,
  removeDiscount,
  requestRemoveDiscount,
  handleDiscountCreated,
} = useCatalogDiscountsPage()
</script>

<template>
  <UDashboardPanel
    id="catalog-discounts"
    :ui="{
      body: 'min-h-0 overflow-hidden py-3',
    }"
  >
    <template #header>
      <UDashboardNavbar title="Popusti">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #trailing>
          <PageInfoTooltip
            text="Izaberite postojeći popust sa liste ili kreirajte novi kod, pa zatim ažurirajte povezane artikle i period važenja."
          />
        </template>

        <template #right>
          <UButton color="primary" :disabled="!canEdit" @click="beginCreate">
            Novi popust
          </UButton>
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <EntitiesSplitView class="h-full min-h-0">
          <template #list>
            <CatalogDiscountsListView
              :items="visibleDiscounts"
              :selected-id="selectedKey"
              :filter="discountFilter"
              :loading="listState.loading"
              :error="listState.error"
              :empty-message="listState.empty"
              :page="listPage"
              :page-size="listPageSize"
              :total-count="filteredDiscountCount"
              :total-pages="filteredDiscountPages"
              @update:filter="updateDiscountFilter"
              @search="searchDiscounts"
              @update:page="changeListPage"
              @update:page-size="changeListPageSize"
              @select="selectDiscount"
            />
          </template>

          <template #detail>
            <CatalogDiscountsInformationPanel
              v-model="form"
              v-model:article-codes="selectedArticleCodes"
              :selected="Boolean(selectedDiscount)"
              :can-edit="canEdit"
              :save-loading="submitState.loading"
              :delete-loading="deleteState.loading"
              :error="
                submitState.error ||
                (!deleteConfirmOpen ? deleteState.error : '')
              "
              :success="submitState.success || deleteState.success"
              @save="saveDiscount"
              @remove="requestRemoveDiscount"
            />
          </template>
        </EntitiesSplitView>
      </div>

      <CatalogDiscountsCreateModal
        v-model:open="createModalOpen"
        :can-edit="canEdit"
        @created="handleDiscountCreated"
      />

      <ConfirmDeleteModal
        v-model:open="deleteConfirmOpen"
        title="Obriši popust"
        description="Brisanje uklanja sve zapise za ovaj kod popusta."
        :subject="`Kod popusta: ${
          String(form.code || '')
            .trim()
            .toUpperCase() || 'nije unet'
        }`"
        confirm-label="Obriši popust"
        :loading="deleteState.loading"
        :error="deleteState.error"
        @confirm="removeDiscount"
      />
    </template>
  </UDashboardPanel>
</template>
