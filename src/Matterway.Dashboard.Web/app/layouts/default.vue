<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'
import { serviceSections } from '~/data/serviceRegistry'
import { useAuthSession } from '~/composables/useAuthSession'

const auth = useAuthSession()
const { isNotificationsSlideoverOpen } = useDashboard()
const open = ref(false)

const iconMap: Record<string, string> = {
  catalog: 'i-lucide-package',
  customers: 'i-lucide-users',
  sales: 'i-lucide-shopping-bag',
  identity: 'i-lucide-shield'
}

const navItems = computed<NavigationMenuItem[]>(() => {
  return serviceSections
    .filter(section => auth.hasPermission(section.service, section.minimum))
    .map(section => ({
      label: section.label,
      icon: iconMap[section.key] || 'i-lucide-folder',
      to: `/${section.key}`,
      exact: true,
      type: 'trigger',
      defaultOpen: true,
      onSelect: () => {
        open.value = false
      },
      children: section.features
        .filter(feature => auth.hasPermission(feature.service, feature.minimum))
        .map(feature => ({
          label: feature.label,
          to: feature.route,
          onSelect: () => {
            open.value = false
          }
        }))
    }))
    .filter(section => Array.isArray(section.children) && section.children.length > 0)
})

const secondaryItems = computed<NavigationMenuItem[]>(() => ([
  {
    label: 'Solution Repository',
    icon: 'i-lucide-github',
    to: 'https://github.com/draganovik/aspire-matterway',
    target: '_blank'
  }
]))

const searchGroups = computed(() => [{
  id: 'services',
  label: 'Navigation',
  items: navItems.value.flatMap(section =>
    (section.children || []).map((child: NavigationMenuItem) => ({
      id: `${section.label}-${child.label}`,
      label: child.label,
      icon: section.icon,
      to: child.to
    }))
  )
}, {
  id: 'actions',
  label: 'Actions',
  items: [{
    id: 'open-notifications',
    label: 'Open notifications',
    icon: 'i-lucide-bell',
    onSelect: () => {
      isNotificationsSlideoverOpen.value = true
    }
  }]
}])
</script>

<template>
  <UDashboardGroup unit="rem">
    <UDashboardSidebar
      id="default"
      v-model:open="open"
      collapsible
      resizable
      class="bg-elevated/25"
      :ui="{ footer: 'lg:border-t lg:border-default' }"
    >
      <template #header="{ collapsed }">
        <TeamsMenu :collapsed="collapsed" />
      </template>

      <template #default="{ collapsed }">
        <UDashboardSearchButton
          :collapsed="collapsed"
          class="bg-transparent ring-default"
        />

        <UNavigationMenu
          :collapsed="collapsed"
          :items="navItems"
          orientation="vertical"
          tooltip
          popover
        />

        <UNavigationMenu
          :collapsed="collapsed"
          :items="secondaryItems"
          orientation="vertical"
          tooltip
          class="mt-auto"
        />
      </template>

      <template #footer="{ collapsed }">
        <UserMenu :collapsed="collapsed" />
      </template>
    </UDashboardSidebar>

    <UDashboardSearch :groups="searchGroups" />

    <slot />

    <NotificationsSlideover />
  </UDashboardGroup>
</template>
