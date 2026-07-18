<script setup lang="ts">
import { formatMoney } from "~/utils/formatters"

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
    <p class="text-highlighted w-full text-sm font-medium">
      {{ item.title || "Artikal bez naziva" }}
    </p>
    <p class="text-muted text-xs">
      {{ item.code || "Bez šifre" }}
    </p>
  </div>

  <div
    class="text-muted mt-1 flex place-items-center justify-between gap-2 text-xs"
  >
    <UBadge
      :color="item.isAvailable ? 'success' : 'neutral'"
      variant="subtle"
      size="sm"
    >
      {{ item.isAvailable ? "Dostupan" : "Nije dostupan" }}
    </UBadge>
    Cena: {{ formatMoney(item.price ?? item.basePrice ?? null) }}
  </div>
</template>
