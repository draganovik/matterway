<script setup lang="ts">
const { item, selected = false } = defineProps<{
  item: {
    title?: string | null
    code?: string | null
    isAvailable?: boolean
    price?: number | string | null
    basePrice?: number | string | null
  }
  selected?: boolean
}>()
</script>

<template>
  <div class="flex items-start justify-between gap-2">
    <div class="min-w-0">
      <p class="truncate text-base font-medium text-foreground">
        {{ item.title || 'Untitled Article' }}
      </p>
      <p class="truncate text-sm text-muted">
        {{ item.code || 'No code' }}
      </p>
    </div>

    <UBadge
      :color="selected ? 'primary' : 'neutral'"
      variant="subtle"
      class="shrink-0"
    >
      {{ selected ? 'Selected' : 'Pick' }}
    </UBadge>
  </div>

  <div class="mt-2 flex items-center justify-between text-sm text-muted">
    <UBadge
      :color="item.isAvailable ? 'success' : 'neutral'"
      variant="soft"
    >
      {{ item.isAvailable ? 'Available' : 'Unavailable' }}
    </UBadge>
    <span>Price: {{ item.price ?? item.basePrice ?? 'N/A' }}</span>
  </div>
</template>
