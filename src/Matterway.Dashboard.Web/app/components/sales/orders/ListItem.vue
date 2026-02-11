<script setup lang="ts">
import type { OrderResponse } from '~/types/sales'
import { formatDateTime, formatMoney } from '~/utils/formatters'

const { item } = defineProps<{
  item: OrderResponse
}>()

function toAmount(value: number | string | null | undefined) {
  if (value === null || value === undefined || value === '') return 0
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : 0
}

function roundCurrency(value: number) {
  return Math.round((value + Number.EPSILON) * 100) / 100
}

const latestStatus = computed(() => {
  const entries = item.statusHistory || []
  return entries.length > 0 ? entries[entries.length - 1] : null
})

const paymentSum = computed(() =>
  roundCurrency((item.payments || []).reduce((sum, row) => sum + toAmount(row.amount), 0))
)

const isPaymentBalanced = computed(() => {
  const total = roundCurrency(toAmount(item.totalAmount))
  return Math.abs(total - paymentSum.value) < 0.01
})
</script>

<template>
  <div class="flex items-start justify-between gap-3">
    <div class="min-w-0">
      <p class="text-foreground truncate text-base font-medium font-mono">
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
      <UBadge v-if="latestStatus" color="primary" variant="subtle" class="font-normal">
        {{ latestStatus.status }}
      </UBadge>
    </div>
  </div>
</template>
