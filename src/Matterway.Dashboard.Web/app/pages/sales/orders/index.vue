<script setup lang="ts">
import { useSalesOrdersPage } from "~/composables/features/sales/useSalesOrdersPage"

definePageMeta({
  title: "Orders",
  service: "sales",
  permissions: ["observer", "operator", "manager"],
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
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Orders">
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
                Manage Orders
              </h2>
              <p class="text-muted text-sm">
                Browse orders and reveal details, status history, payments, or
                items by order ID.
              </p>
            </div>
          </div>

          <EntitiesSplitView
            class="min-h-0 flex-1"
            list-class="overflow-hidden"
            detail-class="overflow-y-auto"
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
                @update:filter="(value) => (filter = value)"
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
        </div>

        <SalesOrdersModalDetailsView
          v-model:open="revealDetailsModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />

        <SalesOrdersModalStatusHistoryView
          v-model:open="revealStatusHistoryModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />

        <SalesOrdersModalCreateStatusView
          v-model:open="createStatusModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
          :can-edit="canManageStatuses"
          @created="handleStatusCreated"
        />

        <SalesOrdersModalPaymentsView
          v-model:open="revealPaymentsModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />

        <SalesOrdersModalItemsView
          v-model:open="revealItemsModalOpen"
          :order-id="selectedOrder?.id || null"
          :order-label="selectedOrderLabel"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
