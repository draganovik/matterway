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
    class="data-[state=open]:bg-elevated"
  >
    <span class="flex items-center gap-2">
      <AppLogo class="h-6 w-6" />
      <span
        v-if="!collapsed"
        class="text-sm font-semibold"
      >Matterway Ops</span>
    </span>
  </UButton>
</template>
