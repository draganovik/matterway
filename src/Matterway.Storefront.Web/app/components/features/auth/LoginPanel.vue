<script setup lang="ts">
type LoginFormModel = {
  email: string
  password: string
}

const model = defineModel<LoginFormModel>({ required: true })

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
    hero-title="Prijava za kupce"
    hero-description="Prijavite se da biste naručivali i pratili svoje porudžbine."
    card-title="Dobro došli nazad"
    card-description="Unesite podatke za prijavu."
  >
    <form class="space-y-4" @submit.prevent="emit('submit')">
      <UFormField label="Imejl" required>
        <UInput
          v-model="model.email"
          type="email"
          placeholder="ime.prezime@domen.com"
          autocomplete="email"
          class="w-full"
          required
        />
      </UFormField>

      <UFormField label="Lozinka" required>
        <UInput
          v-model="model.password"
          type="password"
          placeholder="••••••••"
          autocomplete="current-password"
          class="w-full"
          required
          @keydown.enter.exact="submitFormOnEnter"
        />
      </UFormField>

      <StatusMessages v-if="props.error" :error="props.error" />

      <div
        class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between"
      >
        <UButton type="submit" color="primary" :loading="props.loading">{{
          props.loading ? "Prijava" : "Prijavi se"
        }}</UButton>
        <NuxtLink to="/register" class="text-primary text-sm hover:underline"
          >Registrujte se</NuxtLink
        >
      </div>
    </form>
  </AuthShell>
</template>
