<script setup lang="ts">
import { useSalesApi } from '~/composables/useSalesApi'
import type { OrderResponse } from '~/types/sales'
import { useRequestState } from '~/composables/useRequestState'
import { formatDateTime, formatMoney } from '~/utils/formatters'

const props = withDefaults(
  defineProps<{
    orderId?: string | null
    orderLabel?: string
  }>(),
  {
    orderId: null,
    orderLabel: ''
  }
)

const isOpen = defineModel<boolean>('open', { required: true })

const api = useSalesApi()
const loadState = useRequestState()

const order = ref<OrderResponse | null>(null)
const notFound = ref(false)
let closeResetTimer: ReturnType<typeof setTimeout> | null = null

const displayLabel = computed(() => props.orderLabel.trim() || props.orderId || 'Selected order')

function toAmount(value: number | string | null | undefined) {
  if (value === null || value === undefined || value === '') return 0
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : 0
}

function roundCurrency(value: number) {
  return Math.round((value + Number.EPSILON) * 100) / 100
}

const paymentSum = computed(() =>
  roundCurrency((order.value?.payments || []).reduce((sum, row) => sum + toAmount(row.amount), 0))
)

const isPaymentBalanced = computed(() => {
  const totalAmount = roundCurrency(toAmount(order.value?.totalAmount))
  return Math.abs(totalAmount - paymentSum.value) < 0.01
})

function resetModalState() {
  loadState.loading = false
  loadState.error = ''
  order.value = null
  notFound.value = false
}

async function loadOrder() {
  const orderId = props.orderId?.trim()
  if (!orderId) {
    loadState.error = 'Select an order first.'
    order.value = null
    notFound.value = false
    return
  }

  loadState.loading = true
  loadState.error = ''
  order.value = null
  notFound.value = false

  const result = await api.getOrderById(orderId)

  loadState.loading = false

  if (!result.ok) {
    if (result.status === 404) {
      notFound.value = true
      return
    }

    loadState.error = result.error || 'Unable to reveal payments.'
    return
  }

  if (!result.data) {
    notFound.value = true
    return
  }

  order.value = result.data
}

watch([isOpen, () => props.orderId], ([open]) => {
  if (!open) {
    if (closeResetTimer) clearTimeout(closeResetTimer)
    closeResetTimer = setTimeout(() => {
      resetModalState()
      closeResetTimer = null
    }, 200)
    return
  }

  if (closeResetTimer) {
    clearTimeout(closeResetTimer)
    closeResetTimer = null
  }

  void loadOrder()
})

onBeforeUnmount(() => {
  if (closeResetTimer) clearTimeout(closeResetTimer)
})
</script>

<template>
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-4xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Payments</h3>
        <p class="text-muted text-sm">Revealed payments for {{ displayLabel }}.</p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing payments.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Order not found"
          description="The selected order could not be loaded."
        />

        <template v-else-if="order">
          <div class="space-y-2">
            <h4 class="text-foreground text-sm font-semibold">Reconciliation</h4>

            <div class="border-default/70 rounded-md border px-3 py-2">
              <div class="flex flex-wrap items-center justify-between gap-2">
                <div class="text-sm">
                  <p class="text-muted text-xs">Order Amount vs Payments Sum</p>
                  <p class="text-foreground font-medium">
                    {{ formatMoney(order.totalAmount) }} vs {{ formatMoney(paymentSum) }}
                  </p>
                </div>
                <UBadge :color="isPaymentBalanced ? 'success' : 'error'" variant="subtle">
                  {{ isPaymentBalanced ? 'Amounts Match' : 'Amounts Do Not Match' }}
                </UBadge>
              </div>
            </div>
          </div>

          <div class="space-y-2">
            <div class="flex items-center justify-between gap-3">
              <h4 class="text-foreground text-sm font-semibold">Registered Payments</h4>
              <UBadge color="neutral" variant="subtle" class="font-normal">
                {{ order.payments?.length || 0 }} payments
              </UBadge>
            </div>

            <div
              v-if="!order.payments?.length"
              class="border-default/70 bg-background text-muted rounded-md border px-3 py-3 text-sm"
            >
              No payments registered for this order.
            </div>

            <div v-else class="space-y-2">
              <div
                v-for="payment in order.payments"
                :key="payment.id"
                class="border-default/70 rounded-md border px-3 py-2"
              >
                <dl class="grid gap-2 sm:grid-cols-2">
                  <div>
                    <dt class="text-muted text-xs">ID</dt>
                    <dd class="text-foreground mt-1 text-sm font-mono break-all">{{ payment.id }}</dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Status</dt>
                    <dd class="text-foreground mt-1 text-sm">{{ payment.status }}</dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Provider</dt>
                    <dd class="text-foreground mt-1 text-sm">{{ payment.provider || '-' }}</dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Reference ID</dt>
                    <dd class="text-foreground mt-1 text-sm font-mono break-all">
                      {{ payment.referenceId || '-' }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Amount</dt>
                    <dd class="text-foreground mt-1 text-sm font-medium">
                      {{ formatMoney(payment.amount) }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Created At</dt>
                    <dd class="text-foreground mt-1 text-sm">
                      {{ formatDateTime(payment.createdAt) }}
                    </dd>
                  </div>
                </dl>
              </div>
            </div>
          </div>
        </template>
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-end">
        <UButton variant="ghost" @click="isOpen = false">Close</UButton>
      </div>
    </template>
  </UModal>
</template>
