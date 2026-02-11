<script setup lang="ts">
import { useSalesApi } from '~/composables/useSalesApi'
import type { OrderResponse } from '~/types/sales'
import { useRequestState } from '~/composables/useRequestState'
import { useWorkspacePagination } from '~/composables/useWorkspacePagination'

const api = useSalesApi()

const listState = useRequestState({ empty: 'No orders found.' })
const detailState = useRequestState()

const orders = ref<OrderResponse[]>([])
const filter = ref('')
const {
  pagination,
  resetTotals,
  applyMeta,
  changePage,
  changePageSize,
  searchWithPageReset,
  watchPagination
} = useWorkspacePagination({ pageSize: 20 })

const selectedId = ref<string | null>(null)
const selectedOrder = ref<OrderResponse | null>(null)

const revealDetailsModalOpen = ref(false)
const revealStatusHistoryModalOpen = ref(false)
const revealPaymentsModalOpen = ref(false)
const revealItemsModalOpen = ref(false)

function clearSelection() {
  selectedId.value = null
  selectedOrder.value = null
  revealDetailsModalOpen.value = false
  revealStatusHistoryModalOpen.value = false
  revealPaymentsModalOpen.value = false
  revealItemsModalOpen.value = false
  detailState.error = ''
}

async function loadOrders() {
  listState.loading = true
  listState.error = ''

  const result = await api.queryOrders({
    page: pagination.page,
    pageSize: pagination.pageSize,
    customerId: filter.value.trim() || undefined
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || 'Unable to load orders.'
    orders.value = []
    resetTotals()
    clearSelection()
    return
  }

  if (result.status === 204 || !result.data) {
    orders.value = []
    resetTotals()
    clearSelection()
    return
  }

  orders.value = result.data.data || []
  applyMeta(result.data.meta, orders.value.length)

  if (!selectedId.value) return

  const match =
    orders.value.find((item) => item.id === selectedId.value) || null
  if (!match) clearSelection()
}

async function loadOrder(orderId: string) {
  detailState.loading = true
  detailState.error = ''

  const result = await api.getOrderById(orderId)
  detailState.loading = false

  if (!result.ok || !result.data) {
    detailState.error = result.error || 'Unable to load order details.'
    selectedOrder.value = null
    return
  }

  selectedOrder.value = result.data
  orders.value = orders.value.map((item) =>
    item.id === result.data?.id ? result.data : item
  )
}

function searchOrders() {
  searchWithPageReset(loadOrders)
}

function selectOrder(orderId: string) {
  selectedId.value = orderId
  revealDetailsModalOpen.value = false
  revealStatusHistoryModalOpen.value = false
  revealPaymentsModalOpen.value = false
  revealItemsModalOpen.value = false
  void loadOrder(orderId)
}

function revealDetails() {
  if (!selectedOrder.value?.id) return
  revealDetailsModalOpen.value = true
}

function revealStatusHistory() {
  if (!selectedOrder.value?.id) return
  revealStatusHistoryModalOpen.value = true
}

function revealPayments() {
  if (!selectedOrder.value?.id) return
  revealPaymentsModalOpen.value = true
}

function revealItems() {
  if (!selectedOrder.value?.id) return
  revealItemsModalOpen.value = true
}

const selectedOrderLabel = computed(
  () => selectedOrder.value?.id || 'Selected order'
)

watchPagination(loadOrders)

onMounted(() => {
  void loadOrders()
})
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="flex shrink-0 flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-foreground text-base font-semibold">Manage Orders</h2>
        <p class="text-muted text-sm">
          Browse orders and reveal details, status history, payments, or items
          by order ID.
        </p>
      </div>
    </div>

    <EntitiesSplitView
      class="min-h-0 flex-1"
      list-class="overflow-y-auto"
      detail-class="overflow-y-auto"
    >
      <template #list>
        <EntitiesListPanel
          title="Orders"
          description="Use pagination and optionally filter by exact Customer ID."
          :items="orders"
          item-key="id"
          item-title-key="id"
          item-subtitle-key="customerId"
          :selected-id="selectedId"
          :filter="filter"
          filter-input-type="input"
          filter-placeholder="Optional exact Customer ID (GUID)."
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
        >
          <template #item="{ item }">
            <SalesOrdersListItem :item="item as OrderResponse" />
          </template>
        </EntitiesListPanel>
      </template>

      <template #detail>
        <SalesOrdersPanelInformationView
          :order="selectedOrder"
          :loading="detailState.loading"
          :error="detailState.error"
          @reveal-details="revealDetails"
          @reveal-status-history="revealStatusHistory"
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
</template>
