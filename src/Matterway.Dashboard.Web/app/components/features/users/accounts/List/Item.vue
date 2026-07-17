<script setup lang="ts">
import { formatDateTime } from "~/utils/formatters"
import { formatIdentityRole } from "~/utils/labels"

const { item } = defineProps<{
  item: {
    id?: string | null
    email?: string | null
    created?: string | null
    role?: string | null
  }
}>()

const displayEmail = computed(() => item.email?.trim() || "Bez imejla")
</script>

<template>
  <div class="flex items-start justify-between gap-3">
    <div class="min-w-0 space-y-0.5">
      <p class="text-highlighted truncate text-sm font-medium">
        {{ displayEmail }}
      </p>
      <p class="text-muted truncate font-mono text-xs">
        {{ item.id || "Bez ID-ja" }}
      </p>
      <p class="text-muted text-xs">
        Kreiran: {{ formatDateTime(item.created) }}
      </p>
    </div>

    <UBadge
      color="neutral"
      variant="subtle"
      size="sm"
      class="shrink-0 font-normal whitespace-nowrap"
    >
      {{ formatIdentityRole(item.role) }}
    </UBadge>
  </div>
</template>
