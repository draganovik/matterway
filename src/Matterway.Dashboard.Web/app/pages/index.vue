<script setup lang="ts">
import { serviceSections } from '~/data/serviceRegistry'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Overview'
})

const auth = useAuthSession()
const sections = computed(() => {
  return serviceSections
    .filter(section => auth.hasPermission(section.service, section.minimum))
    .map(section => ({
      ...section,
      features: section.features.filter(feature =>
        auth.hasPermission(feature.service, feature.minimum)
      )
    }))
    .filter(section => section.features.length)
})
</script>

<template>
  <UDashboardPanel id="home">
    <template #header>
      <UDashboardNavbar
        title="Overview"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #right />
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="space-y-8">
        <UCard class="border border-default bg-elevated/20">
          <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
            <div>
              <p class="text-xs uppercase tracking-[0.3em] text-orange-600 dark:text-orange-400">
                Dashboard
              </p>
              <h1 class="text-3xl font-semibold text-foreground">
                Matterway Operations
              </h1>
              <p class="text-sm text-muted">
                Manage Catalog, Customers, Sales, and Identity with permission-aware tooling.
              </p>
            </div>
            <div class="flex flex-wrap gap-2">
              <UBadge
                color="primary"
                variant="soft"
              >
                Employee
              </UBadge>
              <UBadge
                color="neutral"
                variant="soft"
              >
                JWT Access
              </UBadge>
              <UBadge
                color="neutral"
                variant="soft"
              >
                Permission Scoped
              </UBadge>
            </div>
          </div>
        </UCard>

        <div class="grid gap-6 lg:grid-cols-2">
          <UCard
            v-for="section in sections"
            :key="section.key"
            class="border border-default bg-elevated/10"
          >
            <template #header>
              <div>
                <p class="text-xs uppercase tracking-[0.3em] text-orange-600 dark:text-orange-400">
                  {{ section.label }}
                </p>
                <h2 class="text-xl font-semibold text-foreground">
                  Service features
                </h2>
              </div>
            </template>
            <div class="space-y-3">
              <NuxtLink
                v-for="feature in section.features"
                :key="feature.key"
                :to="feature.route"
                class="flex items-center justify-between rounded-lg border border-default px-4 py-3 text-sm transition hover:border-primary/30 hover:bg-primary/10"
              >
                <span>{{ feature.label }}</span>
                <span class="text-xs text-muted">Open</span>
              </NuxtLink>
            </div>
          </UCard>
        </div>
      </div>
    </template>
  </UDashboardPanel>
</template>
