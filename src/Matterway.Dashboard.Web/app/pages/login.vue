<script setup lang="ts">
import { useAuthLoginPage } from "~/composables/features/auth/useAuthLoginPage"

definePageMeta({
  layout: false,
  public: true,
  title: "Prijava",
})

const { model, error, loading, initialize, submit } = useAuthLoginPage()

await initialize()
</script>

<template>
  <div class="bg-default min-h-screen">
    <div
      class="mx-auto flex min-h-screen max-w-6xl items-center justify-center px-6"
    >
      <div class="grid w-full gap-10 lg:grid-cols-[1.1fr_0.9fr]">
        <div class="hidden flex-col justify-center gap-6 lg:flex">
          <p class="text-sm tracking-[0.4em] text-orange-600 uppercase">
            Matterway
          </p>
          <h1 class="text-foreground text-4xl font-semibold">Administracija</h1>
          <p class="text-muted max-w-md text-sm">
            Bezbedan pristup administrativnim tokovima za katalog, korisnike i
            prodaju. Prijavite se nalogom zaposlenog da nastavite.
          </p>
        </div>
        <UCard class="!border-default !bg-elevated/75 !border !shadow-sm">
          <template #header>
            <div class="space-y-1">
              <p class="text-sm tracking-[0.3em] text-orange-600 uppercase">
                Pristup za zaposlene
              </p>
              <h2 class="text-foreground text-2xl font-semibold">Prijava</h2>
              <p class="text-muted text-sm">
                Koristite podatke za prijavu zaposlenog naloga.
              </p>
            </div>
          </template>

          <form class="space-y-4" @submit.prevent="submit">
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
              />
            </UFormField>

            <StatusMessages v-if="error" :error="error" />

            <UButton type="submit" color="primary" :loading="loading">
              {{ loading ? "Prijava" : "Prijavi se" }}
            </UButton>
          </form>
        </UCard>
      </div>
    </div>
  </div>
</template>
