<script setup lang="ts">
import { useSalesOrdersPage } from "~/composables/features/sales/useSalesOrdersPage"

definePageMeta({
  title: "Porudžbine",
})

const {
  canManageStatuses,
  listState,
  detailState,
  orders,
  filter,
  pagination,
  selectedId,
  selectedOrder,
  revealDetailsModalOpen,
  revealStatusHistoryModalOpen,
  createStatusModalOpen,
  revealPaymentsModalOpen,
  revealItemsModalOpen,
  selectedOrderLabel,
  changePage,
  changePageSize,
  searchOrders,
  selectOrder,
  revealDetails,
  revealStatusHistory,
  openCreateStatus,
  handleStatusCreated,
  revealPayments,
  revealItems,
} = useSalesOrdersPage()
</script>

<template>
  <UDashboardPanel
    id="sales-orders"
    :ui="{
      body: 'min-h-0 overflow-hidden py-3',
    }"
  >
    <template #header>
      <UDashboardNavbar title="Porudžbine">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #trailing>
          <PageInfoTooltip
            text="Pregledajte porudžbine i otvarajte detalje, istoriju statusa, uplate i stavke za izabranu porudžbinu."
          />
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
            <SalesOrdersListView
              :items="orders"
              :selected-id="selectedId"
              :filter="filter"
              :loading="listState.loading"
              :error="listState.error"
              :empty-message="listState.empty"
              :page="pagination.page"
              :page-size="pagination.pageSize"
              :total-count="pagination.totalCount"
              :total-pages="pagination.totalPages"
              @update:filter="
                (value) => {
                  filter = value
                }
              "
              @search="searchOrders"
              @update:page="changePage"
              @update:page-size="changePageSize"
              @select="selectOrder"
            />
          </template>

          <template #detail>
            <SalesOrdersInformationPanel
              :order="selectedOrder"
              :error="detailState.error"
              :can-manage-statuses="canManageStatuses"
              @reveal-details="revealDetails"
              @reveal-status-history="revealStatusHistory"
              @create-status="openCreateStatus"
              @reveal-payments="revealPayments"
              @reveal-items="revealItems"
            />
          </template>
        </EntitiesSplitView>

        <SalesOrdersDetailsModal
          v-model:open="revealDetailsModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />

        <SalesOrdersStatusHistoryModal
          v-model:open="revealStatusHistoryModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />

        <SalesOrdersCreateStatusModal
          v-model:open="createStatusModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
          :can-edit="canManageStatuses"
          @created="handleStatusCreated"
        />

        <SalesOrdersPaymentsModal
          v-model:open="revealPaymentsModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />

        <SalesOrdersItemsModal
          v-model:open="revealItemsModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
