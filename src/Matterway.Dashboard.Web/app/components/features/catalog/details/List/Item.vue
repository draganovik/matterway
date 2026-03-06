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
  <div class="flex items-start justify-between gap-2">
    <div class="min-w-0">
      <p class="text-foreground truncate leading-tight font-medium">
        {{ item.title || item.slug || "Untitled detail" }}
      </p>
      <p class="text-muted truncate text-sm leading-tight">
        {{ item.slug || "No slug" }}
      </p>
    </div>

    <UBadge
      v-if="item.unit"
      color="neutral"
      variant="subtle"
      class="shrink-0 text-xs"
    >
      {{ item.unit }}
    </UBadge>
  </div>
</template>
