<script setup lang="ts">
definePageMeta({
  title: "System Users",
  service: "identity",
  level: "operator",
})
</script>

<template>
  <UDashboardPanel
    id="identity-system-users"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="System Users">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <IdentitySystemUsersWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
