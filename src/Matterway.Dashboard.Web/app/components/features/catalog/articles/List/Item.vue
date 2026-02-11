<script setup lang="ts">
const { item } = defineProps<{
  item: {
    title?: string | null
    code?: string | null
    isAvailable?: boolean
    price?: number | string | null
    basePrice?: number | string | null
  }
}>()
</script>

<template>
  <div class="flex flex-wrap items-center justify-between gap-1">
    <p class="text-foreground w-full text-base font-medium">
      {{ item.title || 'Untitled Article' }}
    </p>
    <p class="text-muted text-sm">
      {{ item.code || 'No code' }}
    </p>
  </div>

  <div
    class="text-muted mt-2 flex place-items-center justify-between gap-2 text-sm"
  >
    <UBadge :color="item.isAvailable ? 'success' : 'neutral'" variant="subtle">
      {{ item.isAvailable ? 'Available' : 'Unavailable' }}
    </UBadge>
    Price: {{ item.price ?? item.basePrice ?? 'N/A' }}
  </div>
</template>
