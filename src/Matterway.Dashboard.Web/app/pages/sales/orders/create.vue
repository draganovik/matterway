<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer',
  action: 'create'
})

const api = useApiClient()

const statusOptions = [
  'Created',
  'Confirmed',
  'Paid',
  'Shipped',
  'Delivered',
  'Canceled'
]

const statusForm = reactive({
  orderId: '',
  status: '',
  note: ''
})
const statusState = useRequestState()

async function addStatus() {
  statusState.error = ''
  statusState.success = ''
  if (!statusForm.orderId || !statusForm.status) {
    statusState.error = 'Order Id and status are required.'
    return
  }
  statusState.loading = true
  try {
    const result = await api.request(
      'sales',
      `admin/orders/${statusForm.orderId}/statuses`,
      {
        method: 'POST',
        body: JSON.stringify({
          status: statusForm.status,
          note: statusForm.note || null
        })
      }
    )
    if (!result.ok) {
      statusState.error = result.error || 'Failed to add status.'
      return
    }
    statusState.success = 'Status added.'
  } catch (err) {
    statusState.error = err instanceof Error ? err.message : 'Failed to add status.'
  } finally {
    statusState.loading = false
  }
}
</script>

<template>
  <FeatureShell>
    <div
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Add order status
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="addStatus"
        >
          <UFormField
            label="Order Id"
            required
          >
            <UInput
              v-model="statusForm.orderId"
              placeholder="GUID"
            />
          </UFormField>
          <UFormField
            label="Status"
            required
          >
            <USelectMenu
              v-model="statusForm.status"
              :items="statusOptions"
            />
          </UFormField>
          <UFormField label="Note">
            <UTextarea
              v-model="statusForm.note"
              :rows="3"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="statusState.loading"
          >
            Add Status
          </UButton>
          <FormStatus
            :error="statusState.error"
            :success="statusState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Workflow
          </h3>
        </template>
        <p class="text-sm text-muted">
          Status updates are appended to the order history and affect downstream fulfillment.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
