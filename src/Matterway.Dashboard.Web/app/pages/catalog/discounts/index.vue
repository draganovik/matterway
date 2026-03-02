<script setup lang="ts">
definePageMeta({
  title: "Discounts",
  service: "catalog",
  permissions: ["observer", "operator", "manager"],
})
</script>

<template>
  <UDashboardPanel
    id="catalog-discounts"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Discounts">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <CatalogDiscountsWorkspace />
      </div>
    </template>
  </UDashboardPanel>
</template>
