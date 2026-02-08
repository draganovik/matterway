<script setup lang="ts">
definePageMeta({
  title: 'Discounts',
  service: 'catalog',
  level: 'observer'
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
      <div class="rounded-lg border border-default bg-background p-5">
        <h2 class="text-base font-semibold text-foreground">
          Discounts
        </h2>
        <p class="mt-2 text-sm text-muted">
          Discounts will be built using the reusable entity list and editor structure introduced by Articles.
        </p>
      </div>
    </template>
  </UDashboardPanel>
</template>
