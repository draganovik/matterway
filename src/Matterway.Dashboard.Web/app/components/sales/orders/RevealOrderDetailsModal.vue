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

    loadState.error = result.error || 'Unable to reveal order details.'
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
        <h3 class="text-foreground text-base font-semibold">Order Details</h3>
        <p class="text-muted text-sm">Revealed order details for {{ displayLabel }}.</p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing order details.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Order not found"
          description="The selected order could not be loaded."
        />

        <template v-else-if="order">
          <div class="space-y-2">
            <h4 class="text-foreground text-sm font-semibold">Overview</h4>
            <dl class="grid gap-3 sm:grid-cols-2">
              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">ID</dt>
                <dd class="text-foreground mt-1 text-sm font-medium font-mono break-all">
                  {{ order.id }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Customer ID</dt>
                <dd class="text-foreground mt-1 text-sm font-medium font-mono break-all">
                  {{ order.customerId || '-' }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Type</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.type }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Placed At</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ formatDateTime(order.placedAt) }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2">
                <dt class="text-muted text-xs">Total Amount</dt>
                <dd class="text-foreground mt-1 text-sm font-semibold">
                  {{ formatMoney(order.totalAmount) }}
                </dd>
              </div>
            </dl>
          </div>

          <div class="space-y-2">
            <h4 class="text-foreground text-sm font-semibold">Delivery Info</h4>
            <dl v-if="order.deliveryInfo" class="grid gap-3 sm:grid-cols-2">
              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Country</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.country || '-' }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">City</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.city || '-' }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Zip Code</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.zipCode || '-' }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Contact Phone</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.contactPhone || '-' }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2">
                <dt class="text-muted text-xs">Address Line 1</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.addressLine1 || '-' }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2">
                <dt class="text-muted text-xs">Address Line 2</dt>
                <dd class="text-foreground mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.addressLine2 || '-' }}
                </dd>
              </div>
            </dl>

            <div
              v-else
              class="border-default/70 bg-background text-muted rounded-md border px-3 py-3 text-sm"
            >
              Delivery info is not available.
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
