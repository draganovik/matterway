<script setup lang="ts">
import { useResetOnModalOpen } from '~/composables/useResetOnModalOpen'
import { useRequestState } from '~/composables/useRequestState'
import { useSalesApi } from '~/composables/useSalesApi'
import type { OrderStatusType } from '~/types/sales'

const props = withDefaults(
  defineProps<{
    orderId?: string | null
    orderLabel?: string
    canEdit?: boolean
  }>(),
  {
    orderId: null,
    orderLabel: '',
    canEdit: false
  }
)

const emit = defineEmits<{
  (event: 'created'): void
}>()

const isOpen = defineModel<boolean>('open', { required: true })
const api = useSalesApi()
const createState = useRequestState()

const statusOptions: Array<{ label: string; value: OrderStatusType }> = [
  { label: 'Processing', value: 'Processing' },
  { label: 'Reserved', value: 'Reserved' },
  { label: 'Delivery', value: 'Delivery' },
  { label: 'Completed', value: 'Completed' },
  { label: 'Cancelled', value: 'Cancelled' }
]

const status = ref<OrderStatusType | ''>('')
const note = ref('')

const displayLabel = computed(
  () => props.orderLabel?.trim() || props.orderId?.trim() || 'Selected order'
)

const canSubmit = computed(
  () =>
    props.canEdit &&
    Boolean(props.orderId?.trim()) &&
    Boolean(String(status.value).trim()) &&
    !createState.loading
)

function resetForm() {
  status.value = ''
  note.value = ''
  createState.error = ''
  createState.success = ''
}

useResetOnModalOpen(isOpen, resetForm)

async function createStatus() {
  createState.error = ''
  if (!props.canEdit) return

  const orderId = props.orderId?.trim()
  if (!orderId) {
    createState.error = 'Select an order first.'
    return
  }

  const statusValue = String(status.value).trim()
  if (!statusValue) {
    createState.error = 'Status is required.'
    return
  }

  createState.loading = true

  const result = await api.addOrderStatus(orderId, {
    status: statusValue as OrderStatusType,
    note: note.value.trim() || null
  })

  createState.loading = false

  if (!result.ok) {
    createState.error = result.error || 'Unable to create status entry.'
    return
  }

  emit('created')
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Create Status</h3>
        <p class="text-muted text-sm">
          Add a new status transition for {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField label="Status" required>
          <USelectMenu
            v-model="status"
            :items="statusOptions"
            value-key="value"
            label-key="label"
            :disabled="!canEdit || createState.loading"
            placeholder="Select status"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Note">
          <UTextarea
            v-model="note"
            :rows="3"
            placeholder="Optional note for this transition."
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <StatusMessages :error="createState.error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="createState.loading"
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canSubmit"
          @click="createStatus"
        >
          Create Status
        </UButton>
      </div>
    </template>
  </UModal>
</template>
