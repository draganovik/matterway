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
      label="ID sistemskog korisnika"
      required
      help="Postojeći ID korisnika iz Identity servisa."
    >
      <UInput
        v-model="form.systemUserId"
        placeholder="00000000-0000-0000-0000-000000000000"
        :disabled="disabled"
        class="w-full font-mono"
      />
    </UFormField>

    <div class="grid gap-4 md:grid-cols-2">
      <UFormField label="Ime" required>
        <UInput
          v-model="form.firstName"
          placeholder="Ana"
          :disabled="disabled"
          class="w-full"
        />
      </UFormField>

      <UFormField label="Prezime" required>
        <UInput
          v-model="form.lastName"
          placeholder="Jovanovic"
          :disabled="disabled"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField label="Datum rođenja" required>
      <UInput
        v-model="form.birthDate"
        type="date"
        placeholder="YYYY-MM-DD"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>
  </div>
</template>
