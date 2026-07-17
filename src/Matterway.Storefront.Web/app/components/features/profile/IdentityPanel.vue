<script setup lang="ts">
type IdentityForm = {
  firstName: string
  lastName: string
  birthDate: string
}

withDefaults(
  defineProps<{
    loading?: boolean
    disabled?: boolean
    error?: string
    success?: string
  }>(),
  {
    loading: false,
    disabled: false,
    error: "",
    success: "",
  },
)

const form = defineModel<IdentityForm>({ required: true })

const emit = defineEmits<{
  save: []
}>()
</script>

<template>
  <UCard>
    <template #header>
      <h2 class="text-base font-semibold">Lični podaci</h2>
    </template>

    <form class="space-y-4" @submit.prevent="emit('save')">
      <div class="grid gap-4 sm:grid-cols-2">
        <UFormField label="Ime" required>
          <UInput
            v-model="form.firstName"
            placeholder="npr. Petar"
            autocomplete="given-name"
            class="w-full"
            :disabled="disabled || loading"
            required
          />
        </UFormField>

        <UFormField label="Prezime" required>
          <UInput
            v-model="form.lastName"
            placeholder="npr. Petrović"
            autocomplete="family-name"
            class="w-full"
            :disabled="disabled || loading"
            required
          />
        </UFormField>
      </div>

      <UFormField label="Datum rođenja" required>
        <UInput
          v-model="form.birthDate"
          type="date"
          placeholder="YYYY-MM-DD"
          class="w-full"
          :disabled="disabled || loading"
          required
        />
      </UFormField>

      <StatusMessages :error="error" :success="success" />

      <UButton
        type="submit"
        color="primary"
        :loading="loading"
        :disabled="disabled"
      >
        {{ loading ? "Čuvanje podataka" : "Sačuvaj podatke" }}
      </UButton>
    </form>
  </UCard>
</template>
