<script setup lang="ts">
defineProps<{
  collapsed?: boolean
}>()
</script>

<template>
  <UButton
    color="neutral"
    variant="ghost"
    block
    :square="collapsed"
    class="data-[state=open]:bg-elevated flex items-center justify-start text-left"
    to="/"
  >
    <span class="flex w-full min-w-0 items-center gap-2">
      <SidebarAppLogo class="h-6 w-6 shrink-0" />
      <span v-if="!collapsed" class="min-w-0 truncate text-sm font-semibold"
        >Matterway Dashboard</span
      >
    </span>
  </UButton>
</template>
