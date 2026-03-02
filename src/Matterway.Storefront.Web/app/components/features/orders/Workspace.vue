<script setup lang="ts">
import { useSalesApi } from "~/composables/useSalesApi"
import type { SalesOrder } from "~/types/sales/orders"

const salesApi = useSalesApi()

const loading = ref(true)
const error = ref("")
const orders = ref<SalesOrder[]>([])
const statusHistoryOpen = ref(false)
const itemsOpen = ref(false)
const selectedOrder = ref<SalesOrder | null>(null)

watch([statusHistoryOpen, itemsOpen], ([isStatusHistoryOpen, isItemsOpen]) => {
  if (!isStatusHistoryOpen && !isItemsOpen) selectedOrder.value = null
})

function openStatusHistory(order: SalesOrder) {
  selectedOrder.value = order
  statusHistoryOpen.value = true
}

function openItems(order: SalesOrder) {
  selectedOrder.value = order
  itemsOpen.value = true
}

async function loadOrders() {
  loading.value = true
  error.value = ""

  const response = await salesApi.listSelfOrders(1, 20)
  if (!response.ok) {
    error.value = response.error || "Učitavanje porudžbina nije uspelo."
    orders.value = []
    loading.value = false
    return
  }

  orders.value = response.data?.data ?? []
  loading.value = false
}

onMounted(() => {
  void loadOrders()
})
</script>

<template>
  <div class="space-y-6">
    <UCard class="border-default bg-elevated/60 border">
      <div>
        <p class="text-primary text-xs tracking-[0.3em] uppercase">
          Porudžbine
        </p>
        <h1 class="text-2xl font-semibold">Moje prethodne porudžbine</h1>
      </div>
    </UCard>

    <StatusMessages v-if="error" :error="error" />

    <UCard v-if="loading" class="border-default bg-default border">
      <USkeleton class="h-8" />
      <USkeleton class="mt-2 h-8" />
      <USkeleton class="mt-2 h-8" />
    </UCard>

    <EmptyState
      v-else-if="!orders.length"
      title="Još uvek nema porudžbina"
      description="Ovde će se prikazati vaše završene porudžbine."
      icon="i-lucide-package-open"
    />

    <UCard
      v-else
      :ui="{ body: 'p-0 sm:p-0' }"
      class="border-default bg-default overflow-hidden border"
    >
      <div class="overflow-x-auto">
        <table class="w-full text-left text-sm">
          <thead class="bg-elevated text-muted">
            <tr>
              <th class="px-3 py-2 font-medium">Porudžbina</th>
              <th class="px-3 py-2 font-medium">Datum</th>
              <th class="px-3 py-2 font-medium">Stavke</th>
              <th class="px-3 py-2 font-medium">Adresa</th>
              <th class="px-3 py-2 font-medium">Status</th>
              <th class="px-3 py-2 text-right font-medium">Iznos</th>
            </tr>
          </thead>
          <tbody>
            <OrdersListItem
              v-for="order in orders"
              :key="order.id"
              :order="order"
              @reveal-status-history="openStatusHistory"
              @reveal-items="openItems"
            />
          </tbody>
        </table>
      </div>
    </UCard>
  </div>

  <OrdersModalStatusHistoryView
    v-model:open="statusHistoryOpen"
    :order="selectedOrder"
  />
  <OrdersModalItemsView v-model:open="itemsOpen" :order="selectedOrder" />
</template>
