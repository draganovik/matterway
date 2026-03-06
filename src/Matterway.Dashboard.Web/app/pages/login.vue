<script setup lang="ts">
import { useAuthLoginPage } from "~/composables/features/auth/useAuthLoginPage"

definePageMeta({
  layout: false,
  public: true,
  title: "Sign In",
})

const { model, error, loading, initialize, submit } = useAuthLoginPage()

onMounted(async () => {
  await initialize()
})
</script>

<template>
  <div
    class="min-h-screen bg-gradient-to-br from-stone-100 via-stone-50 to-stone-200 dark:from-stone-950 dark:via-stone-900 dark:to-stone-950"
  >
    <div
      class="mx-auto flex min-h-screen max-w-6xl items-center justify-center px-6"
    >
      <div class="grid w-full gap-10 lg:grid-cols-[1.1fr_0.9fr]">
        <div class="hidden flex-col justify-center gap-6 lg:flex">
          <p class="text-sm tracking-[0.4em] text-orange-600 uppercase">
            Matterway
          </p>
          <h1 class="text-foreground text-4xl font-semibold">
            Management Plane
          </h1>
          <p class="text-muted max-w-md text-sm">
            Secure operational access for Catalog workflows. Sign in with an
            Employee account to continue.
          </p>
        </div>
        <UCard class="!border-default !bg-elevated/75 !border !shadow-sm">
          <template #header>
            <div class="space-y-1">
              <p class="text-sm tracking-[0.3em] text-orange-600 uppercase">
                Operator Access
              </p>
              <h2 class="text-foreground text-2xl font-semibold">Sign in</h2>
              <p class="text-muted text-sm">Use your employee credentials.</p>
            </div>
          </template>

          <form class="space-y-4" @submit.prevent="submit">
            <UFormField label="Email" required>
              <UInput
                v-model="model.email"
                type="email"
                placeholder="name@matterway.local"
                autocomplete="email"
                class="w-full"
                required
              />
            </UFormField>

            <UFormField label="Password" required>
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
              Sign in
            </UButton>
          </form>
        </UCard>
      </div>
    </div>
  </div>
</template>
