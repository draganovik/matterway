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
    <p class="w-full text-base font-medium text-foreground">
      {{ item.title || 'Untitled Article' }}
    </p>
    <p class="text-sm text-muted">
      {{ item.code || 'No code' }}
    </p>
  </div>

  <div class="mt-2 flex place-items-center justify-between gap-2 text-sm text-muted">
    <UBadge
      :color="item.isAvailable ? 'success' : 'neutral'"
      variant="subtle"
    >
      {{ item.isAvailable ? 'Available' : 'Unavailable' }}
    </UBadge>
    Price: {{ item.price ?? item.basePrice ?? 'N/A' }}
  </div>
</template>
