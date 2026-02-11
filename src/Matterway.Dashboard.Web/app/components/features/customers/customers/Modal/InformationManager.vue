<script setup lang="ts">
import { useCustomersApi } from '~/composables/useCustomersApi'
import type { CustomerResponse } from '~/types/customers'
import { useRequestState } from '~/composables/useRequestState'
import { useResetOnModalOpen } from '~/composables/useResetOnModalOpen'

type CustomerCreateForm = {
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

const { canEdit = false } = defineProps<{
  canEdit?: boolean
}>()

const emit = defineEmits<{
  created: [customer: CustomerResponse]
}>()

const isOpen = defineModel<boolean>('open', { required: true })

const api = useCustomersApi()
const createState = useRequestState()

const form = ref<CustomerCreateForm>({
  systemUserId: '',
  firstName: '',
  lastName: '',
  birthDate: '',
  defaultAddressId: ''
})

function resetForm() {
  form.value = {
    systemUserId: '',
    firstName: '',
    lastName: '',
    birthDate: '',
    defaultAddressId: ''
  }
  createState.error = ''
}

useResetOnModalOpen(isOpen, resetForm)

async function createCustomer() {
  createState.error = ''
  if (!canEdit) return

  const payload = {
    systemUserId: form.value.systemUserId.trim(),
    firstName: form.value.firstName.trim(),
    lastName: form.value.lastName.trim(),
    birthDate: form.value.birthDate.trim(),
    defaultAddressId: form.value.defaultAddressId.trim() || null
  }

  if (
    !payload.systemUserId ||
    !payload.firstName ||
    !payload.lastName ||
    !payload.birthDate
  ) {
    createState.error = 'Fill in all required fields before creating.'
    return
  }

  createState.loading = true
  const result = await api.createCustomer(payload)
  createState.loading = false

  if (!result.ok || !result.data) {
    createState.error = result.error || 'Unable to create customer.'
    return
  }

  emit('created', result.data)
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          Create New Customer
        </h3>
        <p class="text-muted text-sm">
          Create a customer profile linked to an existing system user.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <CustomersCustomersPanelInformationForm
          v-model="form"
          :disabled="!canEdit || createState.loading"
        />
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
          :disabled="!canEdit"
          @click="createCustomer"
        >
          Create Customer
        </UButton>
      </div>
    </template>
  </UModal>
</template>
