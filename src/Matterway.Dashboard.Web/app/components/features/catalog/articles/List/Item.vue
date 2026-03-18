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
    <p class="text-foreground w-full text-base font-medium">
      {{ item.title || "Artikal bez naziva" }}
    </p>
    <p class="text-muted text-sm">
      {{ item.code || "Bez šifre" }}
    </p>
  </div>

  <div
    class="text-muted mt-2 flex place-items-center justify-between gap-2 text-sm"
  >
    <UBadge :color="item.isAvailable ? 'success' : 'neutral'" variant="subtle">
      {{ item.isAvailable ? "Dostupan" : "Nije dostupan" }}
    </UBadge>
    Cena: {{ formatMoney(item.price ?? item.basePrice ?? null) }}
  </div>
</template>
