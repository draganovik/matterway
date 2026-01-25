<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  layout: false,
  public: true,
  title: 'Sign In'
})

onMounted(() => {
  if (auth.isLoggedIn.value) {
    void navigateTo(getFirstRoute())
  }
})
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-emerald-50 via-white to-slate-50 dark:from-slate-950 dark:via-slate-950 dark:to-emerald-950">
    <div class="mx-auto flex min-h-screen max-w-6xl items-center justify-center px-6">
      <div class="grid w-full gap-10 lg:grid-cols-[1.1fr_0.9fr]">
        <div class="hidden flex-col justify-center gap-6 lg:flex">
          <p class="text-sm uppercase tracking-[0.4em] text-emerald-600">
            Matterway
          </p>
          <h1 class="text-4xl font-semibold text-foreground">
            Management Plane
          </h1>
          <p class="max-w-md text-sm text-muted">
            Secure operational access for Catalog, Customers, Sales, and Identity services.
            Sign in with an Employee account to continue.
          </p>
        </div>
        <UCard class="border border-default shadow-xl">
          <template #header>
            <div class="space-y-1">
              <p class="text-sm uppercase tracking-[0.3em] text-emerald-600">
                Operator Access
              </p>
              <h2 class="text-2xl font-semibold text-foreground">
                Sign in
              </h2>
              <p class="text-sm text-muted">
                Use your employee credentials.
              </p>
            </div>
          </template>

          <UForm
            class="space-y-4"
            @submit="submit"
          >
            <UFormField
              label="Email"
              required
            >
              <UInput
                v-model="form.email"
                type="email"
                placeholder="name@matterway.local"
              />
            </UFormField>
            <UFormField
              label="Password"
              required
            >
              <UInput
                v-model="form.password"
                type="password"
                placeholder="••••••••"
              />
            </UFormField>
            <UAlert
              v-if="error"
              color="error"
              variant="soft"
              icon="i-lucide-alert-triangle"
            >
              {{ error }}
            </UAlert>
            <UButton
              type="submit"
              color="primary"
              :loading="loading"
              class="w-full"
            >
              Sign in
            </UButton>
          </UForm>
        </UCard>
      </div>
    </div>
  </div>
</template>
