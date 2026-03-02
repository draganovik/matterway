<script setup lang="ts">
import type { SalesOrder, SalesOrderStatus } from "~/types/sales/orders"
import { formatDate, formatMoney } from "~/utils/formatters"

const { order } = defineProps<{
  order: SalesOrder
}>()

const emit = defineEmits<{
  "reveal-status-history": [order: SalesOrder]
  "reveal-items": [order: SalesOrder]
}>()

function toTimestamp(value?: string | null) {
  if (!value) return 0
  const timestamp = new Date(value).getTime()
  return Number.isNaN(timestamp) ? 0 : timestamp
}

function resolveOrderDate(value: SalesOrder) {
  return formatDate(value.placedAt || value.createdAt || null)
}

const latestStatus = computed<SalesOrderStatus | null>(() => {
  const entries = order.statusHistory ?? []
  let latest: SalesOrderStatus | null = null
  let latestTimestamp = -1

  for (const entry of entries) {
    const entryTimestamp = toTimestamp(entry.changedAt)
    if (!latest || entryTimestamp >= latestTimestamp) {
      latest = entry
      latestTimestamp = entryTimestamp
    }
  }

  return latest
})

function revealStatusHistory() {
  emit("reveal-status-history", order)
}

function revealItems() {
  emit("reveal-items", order)
}
</script>

<template>
  <tr class="border-default border-t">
    <td class="px-3 py-3 font-medium">{{ order.id }}</td>
    <td class="text-muted px-3 py-3 whitespace-nowrap">
      {{ resolveOrderDate(order) }}
    </td>
    <td class="px-3 py-3 whitespace-nowrap">
      <div class="flex items-center gap-2">
        <UBadge color="neutral" variant="subtle" class="font-normal">
          {{ order.items?.length ?? 0 }} stavki
        </UBadge>
        <UButton
          variant="ghost"
          icon="i-lucide-package-search"
          @click="revealItems"
        />
      </div>
    </td>
    <td class="px-3 py-3">
      {{ order.deliveryInfo?.addressLine1 || "-" }}
    </td>
    <td class="px-3 py-3 whitespace-nowrap">
      <div class="flex items-center gap-2">
        <UBadge
          :color="latestStatus ? 'primary' : 'neutral'"
          variant="subtle"
          class="font-normal"
        >
          {{ latestStatus?.status || "Nema statusa" }}
        </UBadge>
        <UButton
          variant="ghost"
          icon="i-lucide-history"
          @click="revealStatusHistory"
        />
      </div>
    </td>
    <td class="px-3 py-3 text-right font-semibold">
      {{ formatMoney(order.totalAmount || 0) }}
    </td>
  </tr>
</template>
