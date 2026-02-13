<script setup lang="ts">
import type { OrderResponse } from "~/types/sales"
import { formatDateTime, formatMoney } from "~/utils/formatters"
import { paymentsBalanced } from "~/utils/salesOrderMetrics"

const { item } = defineProps<{
  item: OrderResponse
}>()

const latestStatus = computed(() => {
  const entries = item.statusHistory || []
  return entries.length > 0 ? entries[entries.length - 1] : null
})

const isPaymentBalanced = computed(() => paymentsBalanced(item))

const shortOrderId = computed(() => {
  const value = String(item.id || "").trim()
  if (!value) return "No ID"
  const segments = value.split("-").filter(Boolean)
  return segments.length ? segments[segments.length - 1] : value
})
</script>

<template>
  <div class="flex w-full items-end justify-between gap-3">
    <div class="min-w-0 space-y-1">
      <p class="text-foreground truncate font-mono text-base font-medium">
        {{ shortOrderId }}
      </p>
      <p class="text-muted truncate text-xs">
        Placed: {{ formatDateTime(item.placedAt) }}
      </p>
      <UBadge color="neutral" variant="subtle" class="w-fit font-normal">
        {{ formatMoney(item.totalAmount) }}
      </UBadge>
    </div>

    <div class="flex shrink-0 flex-col items-end gap-1">
      <UBadge
        :color="isPaymentBalanced ? 'success' : 'neutral'"
        variant="subtle"
        class="font-normal"
      >
        {{ isPaymentBalanced ? "Paid" : "Unpaid" }}
      </UBadge>
      <UBadge
        v-if="latestStatus"
        color="primary"
        variant="subtle"
        class="font-normal"
      >
        {{ latestStatus.status }}
      </UBadge>
    </div>
  </div>
</template>
