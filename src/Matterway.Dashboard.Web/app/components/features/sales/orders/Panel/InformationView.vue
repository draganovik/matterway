<script setup lang="ts">
import type { OrderResponse, OrderStatusResponse } from '~/types/sales'
import { formatDateTime, formatMoney } from '~/utils/formatters'
import {
  paymentSumOf,
  paymentsBalanced,
  quantitySumOf
} from '~/utils/salesOrderMetrics'

const props = withDefaults(
  defineProps<{
    order?: OrderResponse | null
    loading?: boolean
    error?: string
    canManageStatuses?: boolean
  }>(),
  {
    order: null,
    loading: false,
    error: '',
    canManageStatuses: false
  }
)

const emit = defineEmits<{
  (
    event:
      | 'revealDetails'
      | 'revealStatusHistory'
      | 'createStatus'
      | 'revealPayments'
      | 'revealItems'
  ): void
}>()

function latestStatusOf(order: OrderResponse | null | undefined) {
  const entries = order?.statusHistory || []
  return entries.length > 0 ? (entries[entries.length - 1] ?? null) : null
}

const latestStatus = computed<OrderStatusResponse | null>(() =>
  latestStatusOf(props.order)
)

const paymentSum = computed(() => paymentSumOf(props.order))
const paymentBalanced = computed(() => paymentsBalanced(props.order))
const itemCount = computed(() => props.order?.items?.length || 0)
const quantitySum = computed(() => quantitySumOf(props.order))
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-foreground text-base font-semibold">
        {{ order ? 'Order Summary' : 'Order Viewer' }}
      </h3>
      <p class="text-muted text-sm">
        Reveal loads a fresh snapshot from the Sales API for each selected
        section.
      </p>
    </div>

    <StatusMessages
      v-if="loading || error"
      :loading="loading ? 'Loading order.' : false"
      :error="error"
    />

    <EntitiesEmptyState
      v-else-if="!order"
      title="Nothing selected"
      description="Select an order from the list to inspect details."
    />

    <div v-else class="space-y-4">
      <div class="text-muted font-mono text-sm break-all">
        Order ID: {{ order.id }}
      </div>

      <div class="grid gap-3 sm:grid-cols-3">
        <div class="border-default/70 rounded-lg border px-3 py-2">
          <p class="text-muted text-xs">Type</p>
          <p class="text-foreground mt-1 text-sm font-medium">
            {{ order.type }}
          </p>
        </div>

        <div class="border-default/70 rounded-lg border px-3 py-2">
          <p class="text-muted text-xs">Placed At</p>
          <p class="text-foreground mt-1 text-sm font-medium">
            {{ formatDateTime(order.placedAt) }}
          </p>
        </div>

        <div class="border-default/70 rounded-lg border px-3 py-2">
          <p class="text-muted text-xs">Total Amount</p>
          <p class="text-foreground mt-1 text-sm font-semibold">
            {{ formatMoney(order.totalAmount) }}
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

        <UButton variant="outline" @click="emit('revealDetails')">
          Reveal Details
        </UButton>
      </div>

      <div class="border-default/70 space-y-2 rounded-lg border p-3">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <h4 class="text-foreground text-sm font-semibold">Status History</h4>

          <div class="flex items-center gap-2">
            <UBadge color="neutral" variant="subtle" class="font-normal">
              {{ order.statusHistory?.length || 0 }} entries
            </UBadge>
            <UButton
              size="xs"
              variant="ghost"
              @click="emit('revealStatusHistory')"
            >
              Reveal Status History
            </UButton>
            <UButton
              color="primary"
              variant="outline"
              :disabled="!canManageStatuses"
              @click="emit('createStatus')"
            >
              Update Status
            </UButton>
          </div>
        </div>

        <div
          v-if="latestStatus"
          class="bg-background border-default/60 rounded-md border px-3 py-2"
        >
          <p class="text-foreground text-sm font-medium">
            {{ latestStatus.status }}
          </p>
          <p class="text-muted mt-1 text-xs">
            Changed: {{ formatDateTime(latestStatus.changedAt) }}
          </p>
          <p class="text-muted mt-1 text-xs">
            Note: {{ latestStatus.note || '-' }}
          </p>
        </div>

        <p v-else class="text-muted text-sm">No status history available.</p>
      </div>

      <div class="border-default/70 space-y-2 rounded-lg border p-3">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <h4 class="text-foreground text-sm font-semibold">Payments</h4>

          <div class="flex items-center gap-2">
            <UBadge
              :color="paymentBalanced ? 'success' : 'error'"
              variant="subtle"
              class="font-normal"
            >
              {{ paymentBalanced ? 'Amounts Match' : 'Amounts Do Not Match' }}
            </UBadge>
            <UButton size="xs" variant="ghost" @click="emit('revealPayments')">
              Reveal Payments
            </UButton>
          </div>
        </div>

        <p class="text-muted text-sm">
          Total: {{ formatMoney(order.totalAmount) }}
        </p>
        <p class="text-muted text-sm">
          Sum of payments: {{ formatMoney(paymentSum) }}
        </p>
      </div>

      <div class="border-default/70 space-y-2 rounded-lg border p-3">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <h4 class="text-foreground text-sm font-semibold">Items</h4>

          <UButton size="xs" variant="ghost" @click="emit('revealItems')">
            Reveal Items
          </UButton>
        </div>

        <p class="text-muted text-sm">Item rows: {{ itemCount }}</p>
        <p class="text-muted text-sm">Total quantity: {{ quantitySum }}</p>
      </div>
    </div>
  </div>
</template>
