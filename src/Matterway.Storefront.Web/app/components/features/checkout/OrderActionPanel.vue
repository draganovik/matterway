<script setup lang="ts">
import { formatMoney } from "~/utils/formatters"

const props = defineProps<{
  totalItems: number
  totalPrice: number
  loading: boolean
}>()

const emit = defineEmits<{
  submit: []
}>()
</script>

<template>
  <UCard class="border-default bg-default h-fit border">
    <template #header>
      <h2 class="text-base font-semibold">Pregled porudžbine</h2>
    </template>

    <div class="space-y-3 text-sm">
      <div class="flex items-center justify-between">
        <span class="text-muted">Stavke</span>
        <span>{{ props.totalItems }}</span>
      </div>
      <div class="flex items-center justify-between">
        <span class="text-muted">Iznos</span>
        <span>{{ formatMoney(props.totalPrice) }}</span>
      </div>
    </div>

    <template #footer>
      <UButton
        color="primary"
        block
        :loading="props.loading"
        @click="emit('submit')"
      >
        {{ props.loading ? "Obrada..." : "Plati odmah" }}
      </UButton>
    </template>
  </UCard>
</template>
