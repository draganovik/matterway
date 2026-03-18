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
      <p class="text-foreground truncate text-base font-medium">
        {{ item.title || "Artikal bez naziva" }}
      </p>
      <p class="text-muted truncate text-sm">
        {{ item.code || "Bez šifre" }}
      </p>
    </div>

    <UBadge
      :color="selected ? 'primary' : 'neutral'"
      variant="subtle"
      class="shrink-0"
    >
      {{ selected ? "Izabrano" : "Izaberi" }}
    </UBadge>
  </div>

  <div class="text-muted mt-2 flex items-center justify-between text-sm">
    <UBadge :color="item.isAvailable ? 'success' : 'neutral'" variant="soft">
      {{ item.isAvailable ? "Dostupan" : "Nije dostupan" }}
    </UBadge>
    <span>Cena: {{ formatMoney(item.price ?? item.basePrice ?? null) }}</span>
  </div>
</template>
