<script setup lang="ts">
defineProps<{
  title: string
  description: string
  icon?: string
}>()
</script>

<template>
  <UCard>
    <div class="flex flex-col items-center gap-3 py-6 text-center">
      <UIcon :name="icon || 'i-lucide-inbox'" class="text-muted h-8 w-8" />
      <h3 class="text-base font-semibold">{{ title }}</h3>
      <p class="text-muted max-w-xl text-sm">{{ description }}</p>
      <slot />
    </div>
  </UCard>
</template>
