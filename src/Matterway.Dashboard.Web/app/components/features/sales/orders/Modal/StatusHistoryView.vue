<script setup lang="ts">
import { useOrderReveal } from '~/composables/useOrderReveal'
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

const { order, notFound, loadState, displayLabel } = useOrderReveal({
  isOpen,
  orderId: toRef(props, 'orderId'),
  orderLabel: toRef(props, 'orderLabel'),
  revealErrorMessage: 'Unable to reveal status history.'
})
</script>

<template>
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-3xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Status History</h3>
        <p class="text-muted text-sm">
          Revealed status history for {{ displayLabel }}.
        </p>
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
                <p class="text-foreground text-sm font-medium">
                  {{ entry.status }}
                </p>
                <p class="text-muted text-xs">Entry {{ index + 1 }}</p>
              </div>

              <p class="text-muted mt-1 text-xs">
                Changed: {{ formatDateTime(entry.changedAt) }}
              </p>
              <p class="text-muted mt-1 text-xs">
                Note: {{ entry.note || '-' }}
              </p>
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
