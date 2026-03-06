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
  if (!value) return "Open-ended"
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return value
  return parsed.toLocaleString()
}
</script>

<template>
  <div class="grid gap-2">
    <div class="flex items-start justify-between gap-2">
      <p class="text-foreground text-base font-medium">
        {{ item.code || "Missing Code" }}
      </p>
      <UBadge color="neutral" variant="subtle">
        {{ item.articleCodes.length }} articles
      </UBadge>
    </div>

    <div class="text-muted text-xs">
      {{ item.percentage }} | {{ formatDateTime(item.validFrom) }} -
      {{ formatDateTime(item.validTo) }}
    </div>
  </div>
</template>
