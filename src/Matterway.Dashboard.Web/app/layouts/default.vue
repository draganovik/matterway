<script setup lang="ts">
import type { NavigationMenuItem } from "@nuxt/ui"
import { useAuthorizedSections } from "~/composables/workflows/useAuthorizedSections"

const open = ref(false)
const authorizedSections = useAuthorizedSections()

const iconMap: Record<string, string> = {
  catalog: "i-lucide-package",
  users: "i-lucide-users",
  sales: "i-lucide-receipt-text",
}

const navItems = computed<NavigationMenuItem[]>(() => {
  return authorizedSections.value
    .map((section) => ({
      label: section.label,
      icon: iconMap[section.key] || "i-lucide-folder",
      to: `/${section.key}`,
      exact: true,
      type: "trigger" as const,
      defaultOpen: true,
      onSelect: () => {
        open.value = false
      },
      children: section.features.map((feature) => ({
        label: feature.label,
        to: feature.route,
        exact: false,
        onSelect: () => {
          open.value = false
        },
      })),
    }))
    .filter(
      (section) =>
        Array.isArray(section.children) && section.children.length > 0,
    )
})

const secondaryItems = computed<NavigationMenuItem[]>(() => [
  {
    label: "Solution Repository",
    icon: "i-lucide-github",
    to: "https://github.com/draganovik/aspire-matterway",
    target: "_blank",
  },
])

const searchGroups = computed(() => [
  {
    id: "services",
    label: "Navigation",
    items: navItems.value.flatMap((section) =>
      (section.children || []).map((child: NavigationMenuItem) => ({
        id: `${section.label}-${child.label}`,
        label: child.label,
        icon: section.icon,
        to: child.to,
      })),
    ),
  },
])
</script>

<template>
  <UDashboardGroup unit="rem">
    <UDashboardSidebar
      id="default"
      v-model:open="open"
      collapsible
      resizable
      class="bg-default"
      :ui="{ footer: 'lg:border-t lg:border-default' }"
    >
      <template #header="{ collapsed }">
        <SidebarBrandButton :collapsed="collapsed" />
      </template>

      <template #default="{ collapsed }">
        <UDashboardSearchButton
          :collapsed="collapsed"
          class="ring-default bg-transparent"
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
        <SidebarUserDropdown :collapsed="collapsed" />
      </template>
    </UDashboardSidebar>

    <UDashboardSearch :groups="searchGroups" :color-mode="false" />

    <slot />
  </UDashboardGroup>
</template>
