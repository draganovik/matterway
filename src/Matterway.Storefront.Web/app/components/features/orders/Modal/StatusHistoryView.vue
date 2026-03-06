<script setup lang="ts">
import type { SalesOrder } from "~/types/sales/orders"
import { formatDateTime } from "~/utils/formatters"

const props = withDefaults(
  defineProps<{
    order?: SalesOrder | null
  }>(),
  {
    order: null,
  },
)

const isOpen = defineModel<boolean>("open", { required: true })

const cachedOrder = ref<SalesOrder | null>(props.order)

watch(
  () => props.order,
  (order) => {
    if (order) {
      cachedOrder.value = order
    }
  },
  { immediate: true },
)

const displayOrder = computed(() =>
  isOpen.value ? props.order : props.order ?? cachedOrder.value,
)

function toTimestamp(value?: string | null) {
  if (!value) return 0
  const timestamp = new Date(value).getTime()
  return Number.isNaN(timestamp) ? 0 : timestamp
}

const statusHistory = computed(() => {
  const entries = [...(displayOrder.value?.statusHistory ?? [])]
  entries.sort(
    (left, right) => toTimestamp(right.changedAt) - toTimestamp(left.changedAt),
  )
  return entries
})
</script>

<template>
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-3xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-base font-semibold">Istorija statusa porudžbine</h3>
        <p class="text-muted text-sm">
          Porudžbina: {{ displayOrder?.id || "-" }}
        </p>
      </div>
    </template>

    <template #body>
      <div v-if="displayOrder" class="space-y-4">
        <div class="flex items-center justify-between gap-3">
          <h4 class="text-sm font-semibold">Vremenska linija</h4>
          <UBadge color="neutral" variant="subtle" class="font-normal">
            {{ statusHistory.length }} unosa
          </UBadge>
        </div>

        <div
          v-if="!statusHistory.length"
          class="border-default/70 text-muted rounded-md border px-3 py-4 text-sm"
        >
          Nema zabeleženih promena statusa za ovu porudžbinu.
        </div>

        <div v-else class="space-y-2">
          <div
            v-for="(entry, index) in statusHistory"
            :key="`${entry.changedAt}:${entry.status}:${index}`"
            class="border-default/70 rounded-md border px-3 py-2"
          >
            <div class="flex items-start justify-between gap-2">
              <p class="text-sm font-medium">{{ entry.status || "-" }}</p>
              <p class="text-muted text-xs">
                #{{ statusHistory.length - index }}
              </p>
            </div>

            <p class="text-muted mt-1 text-xs">
              Promenjeno: {{ formatDateTime(entry.changedAt) }}
            </p>
            <p class="text-muted mt-1 text-xs">
              Napomena: {{ entry.note || "-" }}
            </p>
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
