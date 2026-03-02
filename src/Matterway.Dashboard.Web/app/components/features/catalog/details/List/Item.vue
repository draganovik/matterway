<script setup lang="ts">
const { item } = defineProps<{
  item: {
    slug?: string | null
    title?: string | null
    unit?: string | null
  }
}>()
</script>

<template>
  <div class="flex items-start justify-between gap-3">
    <div class="min-w-0">
      <p class="text-foreground truncate text-base font-medium">
        {{ item.title || item.slug || "Untitled detail" }}
      </p>
      <p class="text-muted truncate text-sm">
        {{ item.slug || "No slug" }}
      </p>
    </div>

    <UBadge v-if="item.unit" color="neutral" variant="subtle" class="shrink-0">
      {{ item.unit }}
    </UBadge>
  </div>
</template>
