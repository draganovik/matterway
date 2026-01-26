<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer',
  action: 'update'
})

const api = useApiClient()

const updateForm = reactive({
  systemUserId: '',
  firstName: '',
  lastName: '',
  birthDate: '',
  defaultAddressId: ''
})
const updateState = useRequestState()

async function updateCustomer() {
  updateState.error = ''
  updateState.success = ''
  if (!updateForm.systemUserId) {
    updateState.error = 'System User Id is required.'
    return
  }
  updateState.loading = true
  try {
    const result = await api.request(
      'customers',
      `admin/customers/${updateForm.systemUserId}`,
      {
        method: 'PATCH',
        body: JSON.stringify({
          firstName: updateForm.firstName,
          lastName: updateForm.lastName,
          birthDate: updateForm.birthDate,
          defaultAddressId: updateForm.defaultAddressId || null
        })
      }
    )
    if (!result.ok) {
      updateState.error = result.error || 'Failed to update customer.'
      return
    }
    updateState.success = 'Customer updated.'
  } catch (err) {
    updateState.error = err instanceof Error ? err.message : 'Failed to update customer.'
  } finally {
    updateState.loading = false
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
            Update customer
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateCustomer"
        >
          <UFormField
            label="System User Id"
            required
          >
            <UInput
              v-model="updateForm.systemUserId"
              placeholder="GUID"
            />
          </UFormField>
          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="First Name"
              required
            >
              <UInput v-model="updateForm.firstName" />
            </UFormField>
            <UFormField
              label="Last Name"
              required
            >
              <UInput v-model="updateForm.lastName" />
            </UFormField>
          </div>
          <UFormField
            label="Birth Date"
            required
          >
            <UInput
              v-model="updateForm.birthDate"
              type="date"
            />
          </UFormField>
          <UFormField label="Default Address Id">
            <UInput
              v-model="updateForm.defaultAddressId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update Customer
          </UButton>
          <FormStatus
            :error="updateState.error"
            :success="updateState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Tip
          </h3>
        </template>
        <p class="text-sm text-muted">
          SystemUserId must match the customer record being updated.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
