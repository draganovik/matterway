<script setup lang="ts">
import type { OrderResponse } from "~/types/sales"
import { formatDateTime, formatMoney } from "~/utils/formatters"
import { formatOrderStatus } from "~/utils/labels"
import { paymentsBalanced } from "~/utils/salesOrderMetrics"

const { item } = defineProps<{
  item: OrderResponse
}>()

const latestStatus = computed(() => {
  const entries = item.statusHistory || []
  return entries.length > 0 ? entries[entries.length - 1] : null
})

const isPaymentBalanced = computed(() => paymentsBalanced(item))
</script>

<template>
  <div class="flex w-full items-end justify-between gap-3">
    <div class="min-w-0 space-y-1">
      <p
        class="text-foreground font-mono text-sm leading-snug font-medium break-all"
      >
        {{ item.id || "Bez ID-ja" }}
      </p>
      <p class="text-muted truncate text-xs">
        Kreirano: {{ formatDateTime(item.placedAt) }}
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
        {{ isPaymentBalanced ? "Plaćeno" : "Nije plaćeno" }}
      </UBadge>
      <UBadge
        v-if="latestStatus"
        color="primary"
        variant="subtle"
        class="font-normal"
      >
        {{ formatOrderStatus(latestStatus.status) }}
      </UBadge>
    </div>
  </div>
</template>
