<script setup lang="ts">
import { serviceSections } from '~/data/serviceRegistry'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  layout: false,
  public: true,
  title: 'Sign In'
})

const auth = useAuthSession()
const fields = [
  {
    name: 'email',
    label: 'Email',
    type: 'email',
    placeholder: 'name@matterway.local',
    required: true,
    autocomplete: 'email'
  },
  {
    name: 'password',
    label: 'Password',
    type: 'password',
    placeholder: '••••••••',
    required: true,
    autocomplete: 'current-password'
  }
]
const submitConfig = {
  label: 'Sign in',
  color: 'primary'
} as const
const error = ref('')
const loading = ref(false)

onMounted(async () => {
  await auth.initialize()
  if (auth.isLoggedIn.value) {
    await navigateTo(getFirstRoute())
  }
})

function getFirstRoute() {
  for (const service of serviceSections) {
    if (!auth.hasPermission(service.service, service.minimum)) continue
    const feature = service.features.find((item) =>
      auth.hasPermission(item.service, item.minimum)
    )
    if (feature) return feature.route
  }
  return '/'
}

async function handleSubmit(event: {
  data: Record<'email' | 'password', string>
}) {
  error.value = ''
  loading.value = true
  try {
    await auth.login(event.data.email, event.data.password)
    await navigateTo(getFirstRoute())
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Login failed.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div
    class="min-h-screen bg-gradient-to-br from-orange-50 via-white to-stone-100 dark:from-stone-950 dark:via-stone-950 dark:to-orange-950"
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
        <UCard class="border-default border shadow-xl">
          <UAuthForm
            title="Sign in"
            description="Use your employee credentials."
            :fields="fields"
            :submit="submitConfig"
            :loading="loading"
            @submit="handleSubmit"
          >
            <template #header>
              <div class="space-y-1">
                <p class="text-sm tracking-[0.3em] text-orange-600 uppercase">
                  Operator Access
                </p>
                <h2 class="text-foreground text-2xl font-semibold">Sign in</h2>
                <p class="text-muted text-sm">Use your employee credentials.</p>
              </div>
            </template>
            <template #validation>
              <StatusMessages v-if="error" :error="error" />
            </template>
          </UAuthForm>
        </UCard>
      </div>
    </div>
  </div>
</template>
