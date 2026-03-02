<script setup lang="ts">
import { formatMoney } from "~/utils/formatters"

const props = defineProps<{
  totalItems: number
  totalPrice: number
}>()

const emit = defineEmits<{
  checkout: []
  clear: []
}>()
</script>

<template>
  <UCard class="border-default bg-default h-fit border">
    <div class="space-y-3">
      <div class="flex items-center justify-between text-sm">
        <span class="text-muted">Stavke</span>
        <span>{{ props.totalItems }}</span>
      </div>
      <div class="flex items-center justify-between text-sm">
        <span class="text-muted">Međuzbir</span>
        <span>{{ formatMoney(props.totalPrice) }}</span>
      </div>
      <div class="flex items-center justify-between text-sm">
        <span class="text-muted">Dostava</span>
        <span>Besplatna</span>
      </div>
      <div class="border-default border-t pt-2">
        <div class="flex items-center justify-between font-semibold">
          <span>Ukupno</span>
          <span>{{ formatMoney(props.totalPrice) }}</span>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="space-y-2">
        <UButton color="primary" block @click="emit('checkout')">
          Nastavi na plaćanje
        </UButton>
        <UButton color="neutral" variant="soft" block @click="emit('clear')">
          Isprazni korpu
        </UButton>
      </div>
    </template>
  </UCard>
</template>
