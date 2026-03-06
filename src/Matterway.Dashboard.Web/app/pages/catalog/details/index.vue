<script setup lang="ts">
import { useCatalogDetailsPage } from "~/composables/features/catalog/useCatalogDetailsPage"

definePageMeta({
  title: "Details",
  service: "catalog",
  permissions: ["observer", "operator", "manager"],
})

const {
  canEdit,
  listState,
  saveState,
  removeState,
  details,
  filter,
  limit,
  selectedSlug,
  selectedDetail,
  createModalOpen,
  form,
  canDelete,
  beginCreate,
  selectDetail,
  searchDetails,
  updateLimit,
  saveDetail,
  removeDetail,
  handleDetailCreated,
} = useCatalogDetailsPage()
</script>

<template>
  <UDashboardPanel
    id="catalog-details"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Details">
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
                Manage Details
              </h2>
              <p class="text-muted text-sm">
                Create and maintain detail definitions used across article
                detail values.
              </p>
            </div>

            <UButton color="primary" :disabled="!canEdit" @click="beginCreate">
              Create New
            </UButton>
          </div>

          <EntitiesSplitView
            class="min-h-0 flex-1"
            list-class="overflow-hidden"
            detail-class="overflow-y-auto"
          >
            <template #list>
              <CatalogDetailsListView
                :items="details"
                :selected-id="selectedSlug"
                :filter="filter"
                :loading="listState.loading"
                :error="listState.error"
                :empty-message="listState.empty"
                :page-size="limit"
                @update:filter="(value) => (filter = value)"
                @search="searchDetails"
                @update:page-size="updateLimit"
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
                :remove-loading="removeState.loading"
                :error="saveState.error || removeState.error"
                :success="saveState.success || removeState.success"
                @update:slug="(value) => (form.slug = value)"
                @update:title="(value) => (form.title = value)"
                @update:unit="(value) => (form.unit = value)"
                @save="saveDetail"
                @remove="removeDetail"
              />
            </template>
          </EntitiesSplitView>
        </div>

        <CatalogDetailsModalDetailView
          v-model:open="createModalOpen"
          :can-edit="canEdit"
          @created="handleDetailCreated"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
