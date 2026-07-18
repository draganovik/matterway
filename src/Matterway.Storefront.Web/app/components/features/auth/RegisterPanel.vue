<script setup lang="ts">
type RegisterFormModel = {
  firstName: string
  lastName: string
  birthDate: string
  email: string
  password: string
  confirmPassword: string
}

const model = defineModel<RegisterFormModel>({ required: true })

const props = defineProps<{
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  submit: []
}>()
</script>

<template>
  <AuthShell
    hero-title="Kreiranje naloga"
    hero-description="Registrujte se da biste brže završili kupovinu i pratili svoje porudžbine."
    card-title="Kreiraj nalog"
    card-description="Nalog je namenjen kupcima."
  >
    <form class="space-y-4" @submit.prevent="emit('submit')">
      <div class="grid gap-4 sm:grid-cols-2">
        <UFormField label="Ime" required>
          <UInput
            v-model="model.firstName"
            placeholder="npr. Petar"
            required
            autocomplete="given-name"
            class="w-full"
          />
        </UFormField>
        <UFormField label="Prezime" required>
          <UInput
            v-model="model.lastName"
            placeholder="npr. Petrović"
            required
            autocomplete="family-name"
            class="w-full"
          />
        </UFormField>
      </div>

      <UFormField label="Datum rođenja" required>
        <UInput
          v-model="model.birthDate"
          type="date"
          placeholder="YYYY-MM-DD"
          required
          class="w-full"
        />
      </UFormField>

      <UFormField label="Imejl" required>
        <UInput
          v-model="model.email"
          type="email"
          placeholder="ime.prezime@domen.com"
          required
          autocomplete="email"
          class="w-full"
        />
      </UFormField>

      <div class="grid gap-4 sm:grid-cols-2">
        <UFormField label="Lozinka" required>
          <UInput
            v-model="model.password"
            type="password"
            placeholder="Unesite lozinku"
            required
            class="w-full"
          />
        </UFormField>

        <UFormField label="Potvrdi lozinku" required>
          <UInput
            v-model="model.confirmPassword"
            type="password"
            placeholder="Ponovite lozinku"
            required
            class="w-full"
            @keydown.enter.exact="submitFormOnEnter"
          />
        </UFormField>
      </div>

      <StatusMessages v-if="props.error" :error="props.error" />

      <div
        class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between"
      >
        <UButton type="submit" color="primary" :loading="props.loading">{{
          props.loading ? "Registracija" : "Registruj se"
        }}</UButton>
        <NuxtLink to="/login" class="text-primary text-sm hover:underline"
          >Prijavite se</NuxtLink
        >
      </div>
    </form>
  </AuthShell>
</template>
