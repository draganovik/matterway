<script setup lang="ts">
import { useSalesApi } from '~/composables/useSalesApi'
import type { OrderResponse, OrderStatusResponse } from '~/types/sales'
import { useRequestState } from '~/composables/useRequestState'
import { parseNumberOr } from '~/utils/numbers'
import { formatDateTime, formatMoney } from '~/utils/formatters'

const api = useSalesApi()

const listState = useRequestState({ empty: 'No orders found.' })
const detailState = useRequestState()

const orders = ref<OrderResponse[]>([])
const filter = ref('')
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1
})

const selectedId = ref<string | null>(null)
const selectedOrder = ref<OrderResponse | null>(null)

const revealDetailsModalOpen = ref(false)
const revealStatusHistoryModalOpen = ref(false)
const revealPaymentsModalOpen = ref(false)
const revealItemsModalOpen = ref(false)

function toAmount(value: number | string | null | undefined) {
  if (value === null || value === undefined || value === '') return 0
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : 0
}

function roundCurrency(value: number) {
  return Math.round((value + Number.EPSILON) * 100) / 100
}

function latestStatusOf(order: OrderResponse | null | undefined) {
  const entries = order?.statusHistory || []
  return entries.length > 0 ? (entries[entries.length - 1] ?? null) : null
}

function paymentSumOf(order: OrderResponse | null | undefined) {
  return roundCurrency((order?.payments || []).reduce((sum, row) => sum + toAmount(row.amount), 0))
}

function paymentsBalanced(order: OrderResponse | null | undefined) {
  const total = roundCurrency(toAmount(order?.totalAmount))
  return Math.abs(total - paymentSumOf(order)) < 0.01
}

function quantitySumOf(order: OrderResponse | null | undefined) {
  return roundCurrency((order?.items || []).reduce((sum, row) => sum + toAmount(row.quantity), 0))
}

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
    pagination.totalCount = 0
    pagination.totalPages = 1
    clearSelection()
    return
  }

  if (result.status === 204 || !result.data) {
    orders.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    clearSelection()
    return
  }

  orders.value = result.data.data || []
  pagination.totalCount = parseNumberOr(result.data.meta?.totalCount, orders.value.length)
  pagination.totalPages = Math.max(1, parseNumberOr(result.data.meta?.totalPages, 1))
  pagination.page = Math.max(1, parseNumberOr(result.data.meta?.currentPage, pagination.page))
  pagination.pageSize = Math.max(1, parseNumberOr(result.data.meta?.pageSize, pagination.pageSize))

  if (!selectedId.value) return

  const match = orders.value.find((item) => item.id === selectedId.value) || null
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
  if (pagination.page !== 1) {
    pagination.page = 1
    return
  }

  void loadOrders()
}

function selectOrder(orderId: string) {
  selectedId.value = orderId
  revealDetailsModalOpen.value = false
  revealStatusHistoryModalOpen.value = false
  revealPaymentsModalOpen.value = false
  revealItemsModalOpen.value = false
  void loadOrder(orderId)
}

function changePage(page: number) {
  pagination.page = page
}

function changePageSize(pageSize: number) {
  pagination.pageSize = pageSize
  pagination.page = 1
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

const selectedLatestStatus = computed<OrderStatusResponse | null>(() =>
  latestStatusOf(selectedOrder.value)
)

const selectedPaymentSum = computed(() => paymentSumOf(selectedOrder.value))

const selectedPaymentBalanced = computed(() =>
  paymentsBalanced(selectedOrder.value)
)

const selectedItemCount = computed(() => selectedOrder.value?.items?.length || 0)

const selectedQuantitySum = computed(() => quantitySumOf(selectedOrder.value))

const selectedOrderLabel = computed(() => selectedOrder.value?.id || 'Selected order')

watch([() => pagination.page, () => pagination.pageSize], () => {
  void loadOrders()
})

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
          Browse orders and reveal details, status history, payments, or items by order ID.
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
        <div class="space-y-4">
          <div class="space-y-1">
            <h3 class="text-foreground text-base font-semibold">
              {{ selectedOrder ? 'Order Summary' : 'Order Viewer' }}
            </h3>
            <p class="text-muted text-sm">
              Reveal loads a fresh snapshot from the Sales API for each selected section.
            </p>
          </div>

          <StatusMessages
            v-if="detailState.loading || detailState.error"
            :loading="detailState.loading ? 'Loading order.' : false"
            :error="detailState.error"
          />

          <EntitiesEmptyState
            v-else-if="!selectedOrder"
            title="Nothing selected"
            description="Select an order from the list to inspect details."
          />

          <div v-else class="space-y-4">
            <div class="text-muted text-sm font-mono break-all">
              Order ID: {{ selectedOrder.id }}
            </div>

            <div class="grid gap-3 sm:grid-cols-3">
              <div class="border-default/70 rounded-lg border px-3 py-2">
                <p class="text-muted text-xs">Type</p>
                <p class="text-foreground mt-1 text-sm font-medium">{{ selectedOrder.type }}</p>
              </div>

              <div class="border-default/70 rounded-lg border px-3 py-2">
                <p class="text-muted text-xs">Placed At</p>
                <p class="text-foreground mt-1 text-sm font-medium">
                  {{ formatDateTime(selectedOrder.placedAt) }}
                </p>
              </div>

              <div class="border-default/70 rounded-lg border px-3 py-2">
                <p class="text-muted text-xs">Total Amount</p>
                <p class="text-foreground mt-1 text-sm font-semibold">
                  {{ formatMoney(selectedOrder.totalAmount) }}
                </p>
              </div>
            </div>

            <div
              class="border-default/70 flex flex-wrap items-start justify-between gap-3 rounded-lg border p-3"
            >
              <div class="space-y-1">
                <h4 class="text-foreground text-sm font-semibold">Order Details</h4>
                <p class="text-muted text-sm">
                  Reveal overview and delivery fields from get-order-by-id.
                </p>
              </div>

              <UButton variant="outline" @click="revealDetails">Reveal Details</UButton>
            </div>

            <div class="border-default/70 space-y-2 rounded-lg border p-3">
              <div class="flex flex-wrap items-start justify-between gap-3">
                <h4 class="text-foreground text-sm font-semibold">Status History</h4>

                <div class="flex items-center gap-2">
                  <UBadge color="neutral" variant="subtle" class="font-normal">
                    {{ selectedOrder.statusHistory?.length || 0 }} entries
                  </UBadge>
                  <UButton size="xs" variant="ghost" @click="revealStatusHistory">
                    Reveal Status History
                  </UButton>
                </div>
              </div>

              <div
                v-if="selectedLatestStatus"
                class="bg-background border-default/60 rounded-md border px-3 py-2"
              >
                <p class="text-foreground text-sm font-medium">
                  {{ selectedLatestStatus.status }}
                </p>
                <p class="text-muted mt-1 text-xs">
                  Changed: {{ formatDateTime(selectedLatestStatus.changedAt) }}
                </p>
                <p class="text-muted mt-1 text-xs">Note: {{ selectedLatestStatus.note || '-' }}</p>
              </div>

              <p v-else class="text-muted text-sm">No status history available.</p>
            </div>

            <div class="border-default/70 space-y-2 rounded-lg border p-3">
              <div class="flex flex-wrap items-start justify-between gap-3">
                <h4 class="text-foreground text-sm font-semibold">Payments</h4>

                <div class="flex items-center gap-2">
                  <UBadge
                    :color="selectedPaymentBalanced ? 'success' : 'error'"
                    variant="subtle"
                    class="font-normal"
                  >
                    {{ selectedPaymentBalanced ? 'Amounts Match' : 'Amounts Do Not Match' }}
                  </UBadge>
                  <UButton size="xs" variant="ghost" @click="revealPayments">
                    Reveal Payments
                  </UButton>
                </div>
              </div>

              <p class="text-muted text-sm">
                Total: {{ formatMoney(selectedOrder.totalAmount) }}
              </p>
              <p class="text-muted text-sm">
                Sum of payments: {{ formatMoney(selectedPaymentSum) }}
              </p>
            </div>

            <div class="border-default/70 space-y-2 rounded-lg border p-3">
              <div class="flex flex-wrap items-start justify-between gap-3">
                <h4 class="text-foreground text-sm font-semibold">Items</h4>

                <UButton size="xs" variant="ghost" @click="revealItems">
                  Reveal Items
                </UButton>
              </div>

              <p class="text-muted text-sm">
                Item rows: {{ selectedItemCount }}
              </p>
              <p class="text-muted text-sm">
                Total quantity: {{ selectedQuantitySum }}
              </p>
            </div>
          </div>
        </div>
      </template>
    </EntitiesSplitView>
  </div>

  <SalesOrdersRevealOrderDetailsModal
    v-model:open="revealDetailsModalOpen"
    :order-id="selectedOrder?.id || null"
    :order-label="selectedOrderLabel"
  />

  <SalesOrdersRevealOrderStatusHistoryModal
    v-model:open="revealStatusHistoryModalOpen"
    :order-id="selectedOrder?.id || null"
    :order-label="selectedOrderLabel"
  />

  <SalesOrdersRevealOrderPaymentsModal
    v-model:open="revealPaymentsModalOpen"
    :order-id="selectedOrder?.id || null"
    :order-label="selectedOrderLabel"
  />

  <SalesOrdersRevealOrderItemsModal
    v-model:open="revealItemsModalOpen"
    :order-id="selectedOrder?.id || null"
    :order-label="selectedOrderLabel"
  />
</template>
