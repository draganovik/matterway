<script setup lang="ts">
import { useSalesApi } from '~/composables/useSalesApi'
import type { OrderItemResponse, OrderResponse } from '~/types/sales'
import { useRequestState } from '~/composables/useRequestState'
import { formatMoney } from '~/utils/formatters'

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

function lineTotal(item: OrderItemResponse) {
  return roundCurrency(toAmount(item.unitPrice) * toAmount(item.quantity))
}

const itemCount = computed(() => order.value?.items?.length || 0)

const quantitySum = computed(() =>
  roundCurrency((order.value?.items || []).reduce((sum, item) => sum + toAmount(item.quantity), 0))
)

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

    loadState.error = result.error || 'Unable to reveal order items.'
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
        <h3 class="text-foreground text-base font-semibold">Order Items</h3>
        <p class="text-muted text-sm">Revealed order items for {{ displayLabel }}.</p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing order items.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Order not found"
          description="The selected order could not be loaded."
        />

        <template v-else-if="order">
          <div class="grid gap-3 sm:grid-cols-2">
            <div class="border-default/70 rounded-md border px-3 py-2">
              <p class="text-muted text-xs">Item Rows</p>
              <p class="text-foreground mt-1 text-sm font-semibold">{{ itemCount }}</p>
            </div>

            <div class="border-default/70 rounded-md border px-3 py-2">
              <p class="text-muted text-xs">Total Quantity</p>
              <p class="text-foreground mt-1 text-sm font-semibold">{{ quantitySum }}</p>
            </div>
          </div>

          <div
            v-if="!order.items?.length"
            class="border-default/70 bg-background text-muted rounded-md border px-3 py-3 text-sm"
          >
            No items in this order.
          </div>

          <div v-else class="space-y-2">
            <div
              v-for="item in order.items"
              :key="item.id"
              class="border-default/70 rounded-md border px-3 py-2"
            >
              <dl class="grid gap-2 sm:grid-cols-2">
                <div>
                  <dt class="text-muted text-xs">ID</dt>
                  <dd class="text-foreground mt-1 text-sm font-mono break-all">{{ item.id }}</dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Article ID</dt>
                  <dd class="text-foreground mt-1 text-sm font-mono break-all">
                    {{ item.articleId }}
                  </dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Article Title</dt>
                  <dd class="text-foreground mt-1 text-sm">{{ item.articleTitle || '-' }}</dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Quantity</dt>
                  <dd class="text-foreground mt-1 text-sm">{{ item.quantity }}</dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Unit Price</dt>
                  <dd class="text-foreground mt-1 text-sm">{{ formatMoney(item.unitPrice) }}</dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Line Total</dt>
                  <dd class="text-foreground mt-1 text-sm font-medium">
                    {{ formatMoney(lineTotal(item)) }}
                  </dd>
                </div>
              </dl>
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
