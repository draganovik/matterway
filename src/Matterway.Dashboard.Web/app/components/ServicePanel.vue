<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'
import { getFeatureByRoute } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

const props = defineProps<{
  serviceKey: 'catalog' | 'customers' | 'sales' | 'identity'
  title: string
}>()
const route = useRoute()
const auth = useAuthSession()
const actionOrder = ['query', 'create', 'update', 'delete']
const actionItems = computed<NavigationMenuItem[][]>(() => {
  const feature = getFeatureByRoute(route.path)
  if (!feature || feature.service !== props.serviceKey) return []
  const items = feature.actions
    .filter(action => auth.hasPermission(feature.service, action.permission))
    .sort((a, b) => actionOrder.indexOf(a.key) - actionOrder.indexOf(b.key))
    .map(action => ({
      label: action.label,
      to: action.key === 'query' ? feature.route : `${feature.route}/${action.key}`,
      exact: action.key === 'query'
    }))
  return items.length ? [items] : []
})
</script>

<template>
  <UDashboardPanel
    :id="serviceKey"
    :ui="{ body: 'lg:py-12' }"
  >
    <template #header>
      <UDashboardNavbar :title="title">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>

      <UDashboardToolbar v-if="actionItems.length">
        <UNavigationMenu
          :items="actionItems"
          highlight
          class="-mx-1 flex-1"
        />
      </UDashboardToolbar>
    </template>

    <template #body>
      <div class="flex flex-col gap-4 sm:gap-6 lg:gap-12 w-full lg:max-w-5xl mx-auto">
        <NuxtPage />
      </div>
    </template>
  </UDashboardPanel>
</template>
