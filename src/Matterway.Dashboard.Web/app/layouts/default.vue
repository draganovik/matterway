<script setup lang="ts">
import { useAuthSession } from '~/composables/useAuthSession'

const auth = useAuthSession()
const sidebarOpen = ref(false)

const displayRole = computed(() => auth.role.value ?? 'Unknown')
const displayPerms = computed(() => auth.permissions.value.join(', '))

async function handleLogout() {
  await auth.logout()
  await navigateTo('/login')
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-white to-emerald-50">
    <div class="flex min-h-screen">
      <aside
        class="fixed inset-y-0 left-0 z-40 w-72 -translate-x-full border-r border-slate-200/70 bg-white/80 p-6 shadow-xl backdrop-blur transition duration-200 lg:static lg:translate-x-0"
        :class="sidebarOpen ? 'translate-x-0' : ''"
      >
        <div class="flex items-center justify-between">
          <NuxtLink to="/" class="text-lg font-semibold tracking-tight">
            Matterway Ops
          </NuxtLink>
          <UButton
            icon="i-lucide-x"
            color="neutral"
            variant="ghost"
            class="lg:hidden"
            @click="sidebarOpen = false"
          />
        </div>
        <div class="mt-10">
          <SidebarNav />
        </div>
      </aside>

      <div class="flex flex-1 flex-col">
        <header class="sticky top-0 z-30 border-b border-slate-200/70 bg-white/70 backdrop-blur">
          <div class="flex items-center justify-between px-6 py-4">
            <div class="flex items-center gap-3">
              <UButton
                icon="i-lucide-panel-left"
                color="neutral"
                variant="ghost"
                class="lg:hidden"
                @click="sidebarOpen = true"
              />
              <div>
                <p class="text-sm text-muted">Matterway Management Plane</p>
                <p class="text-xl font-semibold text-slate-900">
                  {{ $route.meta?.title || 'Dashboard' }}
                </p>
              </div>
            </div>
            <div class="flex items-center gap-4">
              <div class="hidden text-right text-xs text-muted sm:block">
                <p>Role: {{ displayRole }}</p>
                <p class="max-w-[360px] truncate">Perms: {{ displayPerms || 'none' }}</p>
              </div>
              <UButton
                icon="i-lucide-log-out"
                color="neutral"
                variant="outline"
                @click="handleLogout"
              >
                Logout
              </UButton>
            </div>
          </div>
        </header>

        <main class="flex-1 px-6 py-8">
          <slot />
        </main>
      </div>
    </div>
  </div>
</template>
