<script setup lang="ts">
import { useUsersCustomersPage } from "~/composables/features/users/useUsersCustomersPage"

definePageMeta({
  title: "Kupci",
})

const {
  canEdit,
  listState,
  detailState,
  saveState,
  removeState,
  customers,
  filter,
  pagination,
  selectedId,
  selectedCustomer,
  createModalOpen,
  deleteConfirmOpen,
  addressModalOpen,
  form,
  selectedCustomerName,
  changePage,
  changePageSize,
  searchCustomers,
  selectCustomer,
  revealAddress,
  handleAddressSaved,
  saveCustomer,
  removeCustomer,
  requestRemoveCustomer,
  handleCustomerCreated,
} = useUsersCustomersPage()
</script>

<template>
  <UDashboardPanel
    id="users-customers"
    :ui="{
      body: 'min-h-0 overflow-hidden py-3',
    }"
  >
    <template #header>
      <UDashboardNavbar title="Kupci">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #trailing>
          <PageInfoTooltip
            text="Pregledajte profile kupaca, uređujte izabrane zapise i uklanjajte neispravne unose."
          />
        </template>

        <template #right>
          <UButton
            color="primary"
            :disabled="!canEdit"
            @click="createModalOpen = true"
          >
            Novi kupac
          </UButton>
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <EntitiesSplitView
          class="h-full min-h-0"
          :detail-loading="detailState.loading"
        >
          <template #list>
            <UsersCustomersListView
              :items="customers"
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
              @search="searchCustomers"
              @update:page="changePage"
              @update:page-size="changePageSize"
              @select="selectCustomer"
            />
          </template>

          <template #detail>
            <UsersCustomersInformationPanel
              v-model="form"
              :customer="selectedCustomer"
              :error="detailState.error"
              :can-edit="canEdit"
              :save-loading="saveState.loading"
              :save-error="saveState.error"
              :save-success="saveState.success"
              @save="saveCustomer"
              @remove="requestRemoveCustomer"
              @reveal-address="revealAddress"
            />
          </template>
        </EntitiesSplitView>

        <UsersCustomersModalInformationView
          v-model:open="createModalOpen"
          :can-edit="canEdit"
          @created="handleCustomerCreated"
        />

        <UsersCustomersModalAddressView
          v-model:open="addressModalOpen"
          :customer-id="selectedCustomer?.systemUserId || null"
          :customer-name="selectedCustomerName"
          :can-edit="canEdit"
          @saved="handleAddressSaved"
        />

        <ConfirmDeleteModal
          v-model:open="deleteConfirmOpen"
          title="Obriši kupca"
          description="Kupac i njegova podrazumevana adresa će biti trajno uklonjeni."
          :subject="
            selectedCustomer
              ? `${selectedCustomerName} · ${selectedCustomer.systemUserId}`
              : ''
          "
          confirm-label="Obriši kupca"
          :loading="removeState.loading"
          :error="removeState.error"
          @confirm="removeCustomer"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
