<script setup lang="ts">
import type { SalesOrder } from "~/types/sales/orders"
import { formatMoney } from "~/utils/formatters"

const props = withDefaults(
  defineProps<{
    order?: SalesOrder | null
  }>(),
  {
    order: null,
  },
)

const isOpen = defineModel<boolean>("open", { required: true })

const items = computed(() => props.order?.items ?? [])

const totalQuantity = computed(() =>
  items.value.reduce((sum, item) => sum + (item.quantity ?? 0), 0),
)

function lineTotalOf(quantity?: number, unitPrice?: number) {
  const resolvedQuantity = quantity ?? 0
  const resolvedUnitPrice = unitPrice ?? 0
  return (
    Math.round((resolvedQuantity * resolvedUnitPrice + Number.EPSILON) * 100) /
    100
  )
}
</script>

<template>
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-4xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-base font-semibold">Stavke porudžbine</h3>
        <p class="text-muted text-sm">Porudžbina: {{ order?.id || "-" }}</p>
      </div>
    </template>

    <template #body>
      <div v-if="order" class="space-y-4">
        <div class="grid gap-3 sm:grid-cols-2">
          <div class="border-default/70 rounded-md border px-3 py-2">
            <p class="text-muted text-xs">Broj stavki</p>
            <p class="mt-1 text-sm font-semibold">{{ items.length }}</p>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <p class="text-muted text-xs">Ukupna količina</p>
            <p class="mt-1 text-sm font-semibold">{{ totalQuantity }}</p>
          </div>
        </div>

        <div
          v-if="!items.length"
          class="border-default/70 text-muted rounded-md border px-3 py-4 text-sm"
        >
          Ova porudžbina nema stavke.
        </div>

        <div v-else class="space-y-2">
          <div
            v-for="(item, index) in items"
            :key="`${order.id}-${item.articleTitle || 'item'}-${index}`"
            class="border-default/70 rounded-md border px-3 py-2"
          >
            <dl class="grid gap-2 sm:grid-cols-2">
              <div>
                <dt class="text-muted text-xs">Naziv artikla</dt>
                <dd class="mt-1 text-sm">{{ item.articleTitle || "-" }}</dd>
              </div>

              <div>
                <dt class="text-muted text-xs">Količina</dt>
                <dd class="mt-1 text-sm">{{ item.quantity ?? 0 }}</dd>
              </div>

              <div>
                <dt class="text-muted text-xs">Cena po komadu</dt>
                <dd class="mt-1 text-sm">
                  {{ formatMoney(item.unitPrice ?? 0) }}
                </dd>
              </div>

              <div>
                <dt class="text-muted text-xs">Ukupno</dt>
                <dd class="mt-1 text-sm font-medium">
                  {{ formatMoney(lineTotalOf(item.quantity, item.unitPrice)) }}
                </dd>
              </div>
            </dl>
          </div>
        </div>
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-end">
        <UButton variant="ghost" @click="isOpen = false">Zatvori</UButton>
      </div>
    </template>
  </UModal>
</template>
