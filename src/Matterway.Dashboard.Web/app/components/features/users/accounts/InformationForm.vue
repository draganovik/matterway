<script setup lang="ts">
type SystemUserForm = {
  email: string
  password: string
}

const { disabled = false } = defineProps<{
  disabled?: boolean
}>()

const form = defineModel<SystemUserForm>({ required: true })
</script>

<template>
  <div class="grid gap-4">
    <UFormField label="Imejl" help="Opciono. Ostavite prazno ako ne menjate.">
      <UInput
        v-model="form.email"
        type="email"
        placeholder="ime.prezime@domen.com"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

    <UFormField
      label="Lozinka"
      help="Opciono. Ako je unesete, mora imati najmanje 6 karaktera."
    >
      <UInput
        v-model="form.password"
        type="password"
        placeholder="Nova lozinka"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>
  </div>
</template>
