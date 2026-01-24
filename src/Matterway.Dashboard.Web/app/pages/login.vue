<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  layout: false,
  public: true,
  title: 'Sign In'
})

const auth = useAuthSession()
const form = reactive({
  email: '',
  password: ''
})
const error = ref('')
const loading = ref(false)

onMounted(() => {
  if (auth.isLoggedIn.value) {
    void navigateTo(getFirstRoute())
  }
})

function getFirstRoute() {
  for (const service of adminServices) {
    if (!auth.hasPermission(service.service, service.minimum)) continue
    const feature = service.features.find((item) =>
      auth.hasPermission(item.service, item.minimum)
    )
    if (feature) return feature.route
  }
  return '/'
}

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(form.email, form.password)
    await navigateTo(getFirstRoute())
  } catch (err: any) {
    error.value = err?.message || 'Login failed.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-emerald-50 via-white to-slate-50">
    <div class="mx-auto flex min-h-screen max-w-6xl items-center justify-center px-6">
      <div class="grid w-full gap-10 lg:grid-cols-[1.1fr_0.9fr]">
        <div class="hidden flex-col justify-center gap-6 lg:flex">
          <p class="text-sm uppercase tracking-[0.4em] text-emerald-600">Matterway</p>
          <h1 class="text-4xl font-semibold text-slate-900">
            Management Plane
          </h1>
          <p class="max-w-md text-sm text-slate-500">
            Secure operational access for Catalog, Customers, Sales, and Identity services.
            Sign in with an Employee account to continue.
          </p>
        </div>
        <UCard class="border border-slate-200/60 shadow-xl">
          <template #header>
            <div class="space-y-1">
              <p class="text-sm uppercase tracking-[0.3em] text-emerald-600">Operator Access</p>
              <h2 class="text-2xl font-semibold text-slate-900">Sign in</h2>
              <p class="text-sm text-muted">Use your employee credentials.</p>
            </div>
          </template>

          <UForm @submit="submit" class="space-y-4">
            <UFormField label="Email" required>
              <UInput v-model="form.email" type="email" placeholder="name@matterway.local" />
            </UFormField>
            <UFormField label="Password" required>
              <UInput v-model="form.password" type="password" placeholder="••••••••" />
            </UFormField>
            <UAlert v-if="error" color="red" variant="soft" icon="i-lucide-alert-triangle">
              {{ error }}
            </UAlert>
            <UButton type="submit" color="primary" :loading="loading" class="w-full">
              Sign in
            </UButton>
          </UForm>
        </UCard>
      </div>
    </div>
  </div>
</template>
