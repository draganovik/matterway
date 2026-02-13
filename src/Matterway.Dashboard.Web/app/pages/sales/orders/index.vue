<script setup lang="ts">
definePageMeta({
  title: "Orders",
  service: "sales",
  level: "observer",
})
</script>

<template>
  <UDashboardPanel
    id="sales-orders"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Orders">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <SalesOrdersWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
