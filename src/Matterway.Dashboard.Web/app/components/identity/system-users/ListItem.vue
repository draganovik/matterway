<script setup lang="ts">
import { formatDateTime } from '~/utils/formatters'

const { item } = defineProps<{
  item: {
    id?: string | null
    email?: string | null
    created?: string | null
    role?: string | null
  }
}>()

const displayEmail = computed(() => item.email?.trim() || 'No email')
</script>

<template>
  <div class="flex items-start justify-between gap-3">
    <div class="min-w-0 space-y-1">
      <p class="text-foreground truncate text-base font-medium">
        {{ displayEmail }}
      </p>
      <p class="text-muted truncate font-mono text-xs">
        {{ item.id || 'No ID' }}
      </p>
      <p class="text-muted text-xs">
        Created: {{ formatDateTime(item.created) }}
      </p>
    </div>

    <UBadge
      color="neutral"
      variant="subtle"
      class="shrink-0 font-normal whitespace-nowrap"
    >
      {{ item.role || 'Unknown role' }}
    </UBadge>
  </div>
</template>
