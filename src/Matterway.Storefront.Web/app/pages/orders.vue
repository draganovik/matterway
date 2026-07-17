<script setup lang="ts">
import { useOrdersPage } from "~/composables/features/orders/useOrdersPage"

definePageMeta({
  title: "Moje porudžbine",
})

const {
  loading,
  error,
  orders,
  currentPage,
  pageSize,
  totalPages,
  totalCount,
  statusHistoryOpen,
  itemsOpen,
  selectedOrder,
  openStatusHistory,
  openItems,
  loadOrders,
  changePage,
} = useOrdersPage()

await loadOrders()
</script>

<template>
  <div class="space-y-4">
    <header class="border-default border-b pb-3">
      <h1 class="text-xl font-semibold">Moje porudžbine</h1>
    </header>

    <StatusMessages v-if="error" :error="error" />

    <UCard v-if="loading">
      <USkeleton class="h-8" />
      <USkeleton class="mt-2 h-8" />
      <USkeleton class="mt-2 h-8" />
    </UCard>

    <EmptyState
      v-else-if="!orders.length"
      title="Još nemate porudžbine"
      description="Kada napravite prvu porudžbinu, pojaviće se ovde."
      icon="i-lucide-package-open"
    />

    <OrdersListTable
      v-else
      :orders="orders"
      @reveal-status-history="openStatusHistory"
      @reveal-items="openItems"
    />

    <div
      v-if="!loading && !error && orders.length"
      class="border-default bg-elevated flex flex-wrap items-center justify-between gap-3 rounded-md border px-3 py-2"
    >
      <p class="text-muted text-sm">
        Strana {{ currentPage }} od {{ Math.max(totalPages, 1) }} •
        {{ totalCount }} porudžbina
      </p>
      <UPagination
        :page="currentPage"
        :items-per-page="pageSize"
        :total="totalCount"
        :sibling-count="1"
        show-controls
        @update:page="changePage"
      />
    </div>

    <OrdersModalStatusHistoryView
      v-model:open="statusHistoryOpen"
      :order="selectedOrder"
    />
    <OrdersModalItemsView v-model:open="itemsOpen" :order="selectedOrder" />
  </div>
</template>
