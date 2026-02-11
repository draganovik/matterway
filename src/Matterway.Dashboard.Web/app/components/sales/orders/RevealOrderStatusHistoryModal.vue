<script setup lang="ts">
import { useSalesApi } from '~/composables/useSalesApi'
import type { OrderResponse } from '~/types/sales'
import { useRequestState } from '~/composables/useRequestState'
import { formatDateTime } from '~/utils/formatters'

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

    loadState.error = result.error || 'Unable to reveal status history.'
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
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-3xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Status History</h3>
        <p class="text-muted text-sm">Revealed status history for {{ displayLabel }}.</p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing status history.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Order not found"
          description="The selected order could not be loaded."
        />

        <template v-else-if="order">
          <div class="flex items-center justify-between gap-3">
            <h4 class="text-foreground text-sm font-semibold">Timeline</h4>
            <UBadge color="neutral" variant="subtle" class="font-normal">
              {{ order.statusHistory?.length || 0 }} entries
            </UBadge>
          </div>

          <EntitiesEmptyState
            v-if="!order.statusHistory?.length"
            title="No status history"
            description="No status transitions have been registered for this order."
          />

          <div v-else class="space-y-2">
            <div
              v-for="(entry, index) in order.statusHistory"
              :key="`${entry.changedAt}:${entry.status}:${index}`"
              class="border-default/70 rounded-md border px-3 py-2"
            >
              <div class="flex items-start justify-between gap-2">
                <p class="text-foreground text-sm font-medium">{{ entry.status }}</p>
                <p class="text-muted text-xs">Entry {{ index + 1 }}</p>
              </div>

              <p class="text-muted mt-1 text-xs">Changed: {{ formatDateTime(entry.changedAt) }}</p>
              <p class="text-muted mt-1 text-xs">Note: {{ entry.note || '-' }}</p>
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
