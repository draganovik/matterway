<script setup lang="ts">
const { item } = defineProps<{
  item: {
    code: string
    percentage: number | string
    validFrom: string
    validTo?: string | null
    articleCodes: string[]
  }
}>()

function formatDateTime(value?: string | null) {
  if (!value) return "Bez datuma završetka"
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return value
  return parsed.toLocaleString("sr-RS")
}
</script>

<template>
  <div class="grid gap-1">
    <div class="flex items-start justify-between gap-2">
      <p class="text-highlighted text-sm font-medium">
        {{ item.code || "Nedostaje kod" }}
      </p>
      <UBadge color="neutral" variant="subtle" size="sm">
        {{ item.articleCodes.length }} artikala
      </UBadge>
    </div>

    <div class="text-muted text-xs">
      {{ item.percentage }} | {{ formatDateTime(item.validFrom) }} -
      {{ formatDateTime(item.validTo) }}
    </div>
  </div>
</template>
