<script setup lang="ts">
import { useRegisterCustomer } from "~/composables/useRegisterCustomer"
import { useCart } from "~/composables/useCart"

const registerCustomer = useRegisterCustomer()
const cart = useCart()
const route = useRoute()

const model = reactive({
  firstName: "",
  lastName: "",
  birthDate:
    new Date(new Date().setFullYear(new Date().getFullYear() - 18))
      .toISOString()
      .split("T")[0] || "",
  email: "",
  password: "",
  confirmPassword: "",
})
const loading = ref(false)
const error = ref("")

function resolveNextRoute() {
  const next = route.query.next
  if (typeof next === "string" && next.startsWith("/")) {
    return next
  }
  return "/"
}

async function submit() {
  error.value = ""

  if (model.password !== model.confirmPassword) {
    error.value = "Lozinke se ne podudaraju."
    return
  }

  loading.value = true
  try {
    await registerCustomer.registerAndSignIn({
      firstName: model.firstName,
      lastName: model.lastName,
      birthDate: model.birthDate,
      email: model.email,
      password: model.password,
    })

    await cart.clear()
    await navigateTo(resolveNextRoute())
  } catch (err) {
    error.value =
      err instanceof Error ? err.message : "Registracija nije uspela."
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div
    class="mx-auto grid max-w-6xl items-center gap-8 lg:grid-cols-[1.1fr_0.9fr]"
  >
    <div class="hidden space-y-4 lg:block">
      <p class="text-primary text-xs tracking-[0.4em] uppercase">Matterway</p>
      <h1 class="text-4xl font-semibold">Kreiranje kupčevog naloga</h1>
      <p class="text-muted max-w-md text-sm">
        Registrujte se da završite kupovinu i pristupite prethodnim
        porudžbinama.
      </p>
    </div>

    <UCard class="border-default border shadow-lg">
      <template #header>
        <div class="space-y-1">
          <p class="text-primary text-xs tracking-[0.3em] uppercase">
            Registracija
          </p>
          <h2 class="text-2xl font-semibold">Kreiraj nalog</h2>
          <p class="text-muted text-sm">Pristup samo za kupce.</p>
        </div>
      </template>

      <form class="space-y-4" @submit.prevent="submit">
        <div class="grid gap-4 sm:grid-cols-2">
          <UInput
            v-model="model.firstName"
            label="Ime"
            required
            autocomplete="given-name"
          />
          <UInput
            v-model="model.lastName"
            label="Prezime"
            required
            autocomplete="family-name"
          />
        </div>

        <UInput
          v-model="model.birthDate"
          type="date"
          label="Datum rođenja"
          required
        />
        <UInput
          v-model="model.email"
          type="email"
          label="Imejl"
          required
          autocomplete="email"
        />

        <div class="grid gap-4 sm:grid-cols-2">
          <UInput
            v-model="model.password"
            type="password"
            label="Lozinka"
            required
          />
          <UInput
            v-model="model.confirmPassword"
            type="password"
            label="Potvrdi lozinku"
            required
          />
        </div>

        <StatusMessages v-if="error" :error="error" />

        <div
          class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between"
        >
          <UButton type="submit" color="primary" :loading="loading"
            >Registruj se</UButton
          >
          <NuxtLink to="/login" class="text-primary text-sm hover:underline"
            >Već imate nalog?</NuxtLink
          >
        </div>
      </form>
    </UCard>
  </div>
</template>
