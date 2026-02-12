<script setup lang="ts">
import type { CustomerResponse } from '~/types/customers'

type CustomerForm = {
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

const form = defineModel<CustomerForm>({ required: true })

withDefaults(
  defineProps<{
    customer?: CustomerResponse | null
    loading?: boolean
    error?: string
    canEdit?: boolean
    saveLoading?: boolean
    removeLoading?: boolean
    saveError?: string
    removeError?: string
    saveSuccess?: string
    removeSuccess?: string
  }>(),
  {
    customer: null,
    loading: false,
    error: '',
    canEdit: false,
    saveLoading: false,
    removeLoading: false,
    saveError: '',
    removeError: '',
    saveSuccess: '',
    removeSuccess: ''
  }
)

const emit = defineEmits<{
  (event: 'save' | 'remove' | 'revealAddress'): void
}>()
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-foreground text-base font-semibold">
        {{ customer ? 'Edit Customer' : 'Customer Editor' }}
      </h3>
      <p class="text-muted text-sm">
        {{
          canEdit
            ? 'Operator permission is required for create, update, and delete.'
            : 'Read-only mode: operator permission required for changes.'
        }}
      </p>
    </div>

    <StatusMessages
      v-if="loading || error"
      :loading="loading ? 'Loading customer.' : false"
      :error="error"
    />

    <EntitiesEmptyState
      v-else-if="!customer"
      title="Nothing selected"
      description="Select a customer from the list to start editing."
    />

    <div v-else class="grid gap-4">
      <div class="text-muted text-sm">
        System User ID: {{ customer.systemUserId }}
      </div>

      <CustomersCustomersPanelInformationForm
        v-model="form"
        :disabled="!canEdit"
        :show-system-user-id="false"
      />

      <div class="flex flex-wrap items-center gap-3">
        <UButton
          color="primary"
          :loading="saveLoading"
          :disabled="!canEdit"
          @click="emit('save')"
        >
          Update Customer
        </UButton>

        <UButton
          color="error"
          variant="ghost"
          :loading="removeLoading"
          :disabled="!canEdit"
          @click="emit('remove')"
        >
          Delete Customer
        </UButton>
      </div>

      <div
        class="border-default/70 flex flex-wrap items-start justify-between gap-3 rounded-lg border p-3"
      >
        <div class="space-y-1">
          <h4 class="text-foreground text-sm font-semibold">Address</h4>
          <p class="text-muted text-sm">
            Reveal and manage the selected customer's address.
          </p>
        </div>

        <UButton variant="outline" @click="emit('revealAddress')">
          Reveal Address
        </UButton>
      </div>

      <StatusMessages
        :error="saveError || removeError"
        :success="saveSuccess || removeSuccess"
      />
    </div>
  </div>
</template>
