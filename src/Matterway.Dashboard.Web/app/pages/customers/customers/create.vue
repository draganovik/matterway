<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer',
  action: 'create'
})

const api = useApiClient()

const createForm = reactive({
  systemUserId: '',
  firstName: '',
  lastName: '',
  birthDate: '',
  defaultAddressId: ''
})
const createState = useRequestState()

async function createCustomer() {
  createState.error = ''
  createState.success = ''
  if (!createForm.systemUserId) {
    createState.error = 'System User Id is required.'
    return
  }
  createState.loading = true
  try {
    const result = await api.request(
      'customers',
      'admin/customers',
      {
        method: 'POST',
        body: JSON.stringify({
          systemUserId: createForm.systemUserId,
          firstName: createForm.firstName,
          lastName: createForm.lastName,
          birthDate: createForm.birthDate,
          defaultAddressId: createForm.defaultAddressId || null
        })
      }
    )
    if (!result.ok) {
      createState.error = result.error || 'Failed to create customer.'
      return
    }
    createState.success = 'Customer created.'
  } catch (err) {
    createState.error = err instanceof Error ? err.message : 'Failed to create customer.'
  } finally {
    createState.loading = false
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
            Create customer
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createCustomer"
        >
          <UFormField
            label="System User Id"
            required
          >
            <UInput
              v-model="createForm.systemUserId"
              placeholder="GUID"
            />
          </UFormField>
          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="First Name"
              required
            >
              <UInput v-model="createForm.firstName" />
            </UFormField>
            <UFormField
              label="Last Name"
              required
            >
              <UInput v-model="createForm.lastName" />
            </UFormField>
          </div>
          <UFormField
            label="Birth Date"
            required
          >
            <UInput
              v-model="createForm.birthDate"
              type="date"
            />
          </UFormField>
          <UFormField label="Default Address Id">
            <UInput
              v-model="createForm.defaultAddressId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="createState.loading"
          >
            Create Customer
          </UButton>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Requirements
          </h3>
        </template>
        <p class="text-sm text-muted">
          Customer must reference an existing system user (Identity service).
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
