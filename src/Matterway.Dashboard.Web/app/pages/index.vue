<script setup lang="ts">
import { useAuthorizedSections } from '~/composables/useAuthorizedSections'

definePageMeta({
  title: 'Overview'
})

const sections = useAuthorizedSections()
</script>

<template>
  <UDashboardPanel id="home">
    <template #header>
      <UDashboardNavbar title="Overview" :ui="{ right: 'gap-3' }">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #right />
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="space-y-8">
        <UCard class="!border-default !bg-elevated/75 !border !shadow-sm">
          <div
            class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between"
          >
            <div>
              <p
                class="text-xs tracking-[0.3em] text-orange-600 uppercase dark:text-orange-400"
              >
                Dashboard
              </p>
              <h1 class="text-foreground text-3xl font-semibold">
                Matterway Operations
              </h1>
              <p class="text-muted text-sm">
                Manage service domains with permission-aware tooling.
              </p>
            </div>
            <div class="flex flex-wrap gap-2">
              <UBadge color="primary" variant="soft"> Employee </UBadge>
              <UBadge color="neutral" variant="soft"> Catalog </UBadge>
              <UBadge color="neutral" variant="soft"> Customers </UBadge>
              <UBadge color="neutral" variant="soft">
                Permission Scoped
              </UBadge>
            </div>
          </div>
        </UCard>

        <div class="grid gap-6 lg:grid-cols-2">
          <UCard
            v-for="section in sections"
            :key="section.key"
            class="!border-default !bg-elevated/75 !border !shadow-sm"
          >
            <template #header>
              <div>
                <p
                  class="text-xs tracking-[0.3em] text-orange-600 uppercase dark:text-orange-400"
                >
                  {{ section.label }}
                </p>
                <h2 class="text-foreground text-xl font-semibold">
                  Service features
                </h2>
              </div>
            </template>
            <div class="space-y-3">
              <NuxtLink
                v-for="feature in section.features"
                :key="feature.key"
                :to="feature.route"
                class="border-default hover:border-primary/30 hover:bg-primary/10 flex items-center justify-between rounded-lg border px-4 py-3 text-sm transition"
              >
                <span>{{ feature.label }}</span>
                <span class="text-muted text-xs">Open</span>
              </NuxtLink>
            </div>
          </UCard>
        </div>
      </div>
    </template>
  </UDashboardPanel>
</template>
