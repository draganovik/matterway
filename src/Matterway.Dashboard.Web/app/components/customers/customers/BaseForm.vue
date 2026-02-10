<script setup lang="ts">
type CustomerForm = {
  systemUserId?: string
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

const { disabled = false, showSystemUserId = true } = defineProps<{
  disabled?: boolean
  showSystemUserId?: boolean
}>()

const form = defineModel<CustomerForm>({ required: true })
</script>

<template>
  <div class="grid gap-4">
    <UFormField
      v-if="showSystemUserId"
      label="System User ID"
      required
      help="Existing Identity user ID."
    >
      <UInput
        v-model="form.systemUserId"
        placeholder="00000000-0000-0000-0000-000000000000"
        :disabled="disabled"
        class="w-full font-mono"
      />
    </UFormField>

    <div class="grid gap-4 md:grid-cols-2">
      <UFormField label="First Name" required>
        <UInput
          v-model="form.firstName"
          placeholder="Ana"
          :disabled="disabled"
          class="w-full"
        />
      </UFormField>

      <UFormField label="Last Name" required>
        <UInput
          v-model="form.lastName"
          placeholder="Jovanovic"
          :disabled="disabled"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField label="Birth Date" required>
      <UInput
        v-model="form.birthDate"
        type="date"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

  </div>
</template>
