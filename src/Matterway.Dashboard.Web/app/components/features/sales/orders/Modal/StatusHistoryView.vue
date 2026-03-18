<script setup lang="ts">
import { useOrderRevealModal } from "~/composables/features/sales/useOrderRevealModal"
import { formatDateTime } from "~/utils/formatters"
import { formatOrderStatus } from "~/utils/labels"

const props = withDefaults(
  defineProps<{
    orderId?: string | null
    orderLabel?: string
  }>(),
  {
    orderId: null,
    orderLabel: "",
  },
)

const isOpen = defineModel<boolean>("open", { required: true })

const { order, notFound, loadState, displayLabel } = useOrderRevealModal({
  isOpen,
  orderId: toRef(props, "orderId"),
  orderLabel: toRef(props, "orderLabel"),
  revealErrorMessage: "Učitavanje istorije statusa nije uspelo.",
})
</script>

<template>
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-3xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          Istorija statusa
        </h3>
        <p class="text-muted text-sm">
          Pregled promena statusa za porudžbinu {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Učitavanje istorije statusa.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Porudžbina nije pronađena"
          description="Izabranu porudžbinu nije moguće učitati."
        />

        <template v-else-if="order">
          <div class="flex items-center justify-between gap-3">
            <h4 class="text-foreground text-sm font-semibold">
              Promene statusa
            </h4>
            <UBadge color="neutral" variant="subtle" class="font-normal">
              {{ order.statusHistory?.length || 0 }} unosa
            </UBadge>
          </div>

          <EntitiesEmptyState
            v-if="!order.statusHistory?.length"
            title="Nema istorije statusa"
            description="Za ovu porudžbinu nisu zabeležene promene statusa."
          />

          <div v-else class="space-y-2">
            <div
              v-for="(entry, index) in order.statusHistory"
              :key="`${entry.changedAt}:${entry.status}:${index}`"
              class="border-default/70 rounded-md border px-3 py-2"
            >
              <div class="flex items-start justify-between gap-2">
                <p class="text-foreground text-sm font-medium">
                  {{ formatOrderStatus(entry.status) }}
                </p>
                <p class="text-muted text-xs">Unos {{ index + 1 }}</p>
              </div>

              <p class="text-muted mt-1 text-xs">
                Izmenjeno: {{ formatDateTime(entry.changedAt) }}
              </p>
              <p class="text-muted mt-1 text-xs">
                Napomena: {{ entry.note || "-" }}
              </p>
            </div>
          </div>
        </template>
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-end">
        <UButton variant="ghost" @click="isOpen = false">Zatvori</UButton>
      </div>
    </template>
  </UModal>
</template>
