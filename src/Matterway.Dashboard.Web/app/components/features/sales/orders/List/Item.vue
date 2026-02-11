<script setup lang="ts">
import type { OrderResponse } from '~/types/sales'
import { formatDateTime, formatMoney } from '~/utils/formatters'
import { paymentsBalanced } from '~/utils/salesOrderMetrics'

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
  <div class="flex items-start justify-between gap-3">
    <div class="min-w-0">
      <p class="text-foreground truncate font-mono text-base font-medium">
        {{ item.id || 'No ID' }}
      </p>
      <p class="text-muted truncate text-xs">
        Customer: {{ item.customerId || 'N/A' }}
      </p>
      <p class="text-muted truncate text-xs">
        Placed: {{ formatDateTime(item.placedAt) }}
      </p>
    </div>

    <div class="flex shrink-0 flex-col items-end gap-1">
      <UBadge color="neutral" variant="subtle" class="font-normal">
        {{ formatMoney(item.totalAmount) }}
      </UBadge>
      <UBadge
        :color="isPaymentBalanced ? 'success' : 'warning'"
        variant="subtle"
        class="font-normal"
      >
        {{ isPaymentBalanced ? 'Payments Match' : 'Payment Mismatch' }}
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
