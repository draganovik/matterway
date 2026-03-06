<script setup lang="ts">
import { useUsersCustomersPage } from "~/composables/features/users/useUsersCustomersPage"

definePageMeta({
  title: "Customers",
  service: "customers",
  permissions: ["observer", "operator", "manager"],
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
  handleCustomerCreated,
} = useUsersCustomersPage()
</script>

<template>
  <UDashboardPanel
    id="users-customers"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Customers">
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
                Manage Customers
              </h2>
              <p class="text-muted text-sm">
                Browse customer profiles, edit selected records, and remove
                invalid entries.
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
                :remove-loading="removeState.loading"
                :save-error="saveState.error"
                :remove-error="removeState.error"
                :save-success="saveState.success"
                :remove-success="removeState.success"
                @save="saveCustomer"
                @remove="removeCustomer"
                @reveal-address="revealAddress"
              />
            </template>
          </EntitiesSplitView>
        </div>

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
      </div>
    </template>
  </UDashboardPanel>
</template>
