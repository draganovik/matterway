<script setup lang="ts">
definePageMeta({
  title: 'Articles',
  service: 'catalog',
  level: 'observer'
})
</script>

<template>
  <UDashboardPanel
    id="catalog-articles"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Articles">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <CatalogArticlesWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
