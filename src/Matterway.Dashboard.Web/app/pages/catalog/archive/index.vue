<script setup lang="ts">
definePageMeta({
  title: "Catalog Data",
  service: "catalog",
  permissions: ["operator", "manager"],
})
</script>

<template>
  <UDashboardPanel
    id="catalog-archive"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Catalog Data">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <CatalogArchiveWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
