<script setup lang="ts">
const { isNotificationsSlideoverOpen } = useDashboard()

const notifications = [
  {
    id: 'ops-1',
    title: 'Catalog sync complete',
    body: 'Article inventory updated successfully.',
    date: new Date().toISOString()
  },
  {
    id: 'ops-2',
    title: 'Sales queue healthy',
    body: 'No stuck orders in the last 24 hours.',
    date: new Date().toISOString()
  }
]
</script>

<template>
  <USlideover
    v-model:open="isNotificationsSlideoverOpen"
    title="Notifications"
  >
    <template #body>
      <div class="space-y-3">
        <div
          v-for="item in notifications"
          :key="item.id"
          class="rounded-md border border-default px-3 py-2 text-sm"
        >
          <p class="font-semibold text-highlighted">
            {{ item.title }}
          </p>
          <p class="text-muted">
            {{ item.body }}
          </p>
          <p class="text-xs text-dimmed">
            {{ new Date(item.date).toLocaleString() }}
          </p>
        </div>
      </div>
    </template>
  </USlideover>
</template>
