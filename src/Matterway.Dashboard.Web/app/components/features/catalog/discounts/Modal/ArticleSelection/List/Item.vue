<script setup lang="ts">
import { formatMoney } from "~/utils/formatters"

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
      <p class="text-foreground truncate text-sm font-medium">
        {{ item.title || "Artikal bez naziva" }}
      </p>
      <p class="text-muted truncate text-xs">
        {{ item.code || "Bez šifre" }}
      </p>
    </div>

    <UBadge
      :color="selected ? 'primary' : 'neutral'"
      variant="subtle"
      size="sm"
      class="shrink-0"
    >
      {{ selected ? "Izabrano" : "Izaberi" }}
    </UBadge>
  </div>

  <div class="text-muted mt-1 flex items-center justify-between text-xs">
    <UBadge
      :color="item.isAvailable ? 'success' : 'neutral'"
      variant="soft"
      size="sm"
    >
      {{ item.isAvailable ? "Dostupan" : "Nije dostupan" }}
    </UBadge>
    <span>Cena: {{ formatMoney(item.price ?? item.basePrice ?? null) }}</span>
  </div>
</template>
