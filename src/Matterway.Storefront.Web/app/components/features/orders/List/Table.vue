<script setup lang="ts">
import type { SalesOrder } from "~/types/sales/orders"

const props = defineProps<{
  orders: SalesOrder[]
}>()

const emit = defineEmits<{
  "reveal-status-history": [order: SalesOrder]
  "reveal-items": [order: SalesOrder]
}>()
</script>

<template>
  <UCard
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
            v-for="order in props.orders"
            :key="order.id"
            :order="order"
            @reveal-status-history="emit('reveal-status-history', $event)"
            @reveal-items="emit('reveal-items', $event)"
          />
        </tbody>
      </table>
    </div>
  </UCard>
</template>
