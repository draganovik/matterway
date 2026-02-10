<script setup lang="ts">
definePageMeta({
  title: 'Details',
  service: 'catalog',
  level: 'observer'
})
</script>

<template>
  <UDashboardPanel
    id="catalog-details"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Details">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <CatalogDetailsWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
