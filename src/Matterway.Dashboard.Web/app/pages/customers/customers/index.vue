<script setup lang="ts">
definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer'
})
</script>

<template>
  <UDashboardPanel
    id="customers-customers"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Customers">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <CustomersCustomersWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
