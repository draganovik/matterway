<script setup lang="ts">
import { useOrdersPage } from "~/composables/features/orders/useOrdersPage"

definePageMeta({
  title: "Moje porudžbine",
})

const {
  loading,
  error,
  orders,
  statusHistoryOpen,
  itemsOpen,
  selectedOrder,
  openStatusHistory,
  openItems,
  initialize,
} = useOrdersPage()

onMounted(() => {
  void initialize()
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

    <OrdersListTable
      v-else
      :orders="orders"
      @reveal-status-history="openStatusHistory"
      @reveal-items="openItems"
    />

    <OrdersModalStatusHistoryView
      v-model:open="statusHistoryOpen"
      :order="selectedOrder"
    />
    <OrdersModalItemsView v-model:open="itemsOpen" :order="selectedOrder" />
  </div>
</template>
