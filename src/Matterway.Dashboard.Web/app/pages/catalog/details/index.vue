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
      <div class="rounded-lg border border-default bg-background p-5">
        <h2 class="text-base font-semibold text-foreground">
          Details
        </h2>
        <p class="mt-2 text-sm text-muted">
          Details will follow the Articles canonical pattern with a reusable two-pane layout and editor groups.
        </p>
      </div>
    </template>
  </UDashboardPanel>
</template>
