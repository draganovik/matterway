<script setup lang="ts">
import type { OrderResponse, OrderStatusResponse } from "~/types/sales"
import { formatDateTime, formatMoney } from "~/utils/formatters"
import { formatOrderStatus, formatOrderType } from "~/utils/labels"
import {
  paymentSumOf,
  paymentsBalanced,
  quantitySumOf,
} from "~/utils/salesOrderMetrics"

const props = withDefaults(
  defineProps<{
    order?: OrderResponse | null
    error?: string
    canManageStatuses?: boolean
  }>(),
  {
    order: null,
    error: "",
    canManageStatuses: false,
  },
)

const emit = defineEmits<{
  (
    event:
      | "revealDetails"
      | "revealStatusHistory"
      | "createStatus"
      | "revealPayments"
      | "revealItems",
  ): void
}>()

function latestStatusOf(order: OrderResponse | null | undefined) {
  const entries = order?.statusHistory || []
  return entries.length > 0 ? (entries[entries.length - 1] ?? null) : null
}

const latestStatus = computed<OrderStatusResponse | null>(() =>
  latestStatusOf(props.order),
)

const paymentSum = computed(() => paymentSumOf(props.order))
const paymentBalanced = computed(() => paymentsBalanced(props.order))
const itemCount = computed(() => props.order?.items?.length || 0)
const quantitySum = computed(() => quantitySumOf(props.order))
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-foreground text-base font-semibold">
        {{ order ? "Pregled porudžbine" : "Porudžbine" }}
      </h3>
      <p class="text-muted text-sm">
        Svako otvaranje učitava najnovije podatke iz Sales API-ja za izabrani
        prikaz.
      </p>
    </div>

    <StatusMessages v-if="error" :error="error" />

    <EntitiesEmptyState
      v-else-if="!order"
      title="Ništa nije izabrano"
      description="Izaberite porudžbinu sa liste da biste pregledali detalje."
    />

    <div v-else class="space-y-4">
      <div class="text-muted font-mono text-sm break-all">
        ID porudžbine: {{ order.id }}
      </div>

      <div class="grid gap-3 sm:grid-cols-3">
        <div class="border-default/70 rounded-lg border px-3 py-2">
          <p class="text-muted text-xs">Tip</p>
          <p class="text-foreground mt-1 text-sm font-medium">
            {{ formatOrderType(order.type) }}
          </p>
        </div>

        <div class="border-default/70 rounded-lg border px-3 py-2">
          <p class="text-muted text-xs">Kreirano</p>
          <p class="text-foreground mt-1 text-sm font-medium">
            {{ formatDateTime(order.placedAt) }}
          </p>
        </div>

        <div class="border-default/70 rounded-lg border px-3 py-2">
          <p class="text-muted text-xs">Ukupan iznos</p>
          <p class="text-foreground mt-1 text-sm font-semibold">
            {{ formatMoney(order.totalAmount) }}
          </p>
        </div>
      </div>

      <div
        class="border-default/70 flex flex-wrap items-start justify-between gap-3 rounded-lg border p-3"
      >
        <div class="space-y-1">
          <h4 class="text-foreground text-sm font-semibold">
            Detalji porudžbine
          </h4>
          <p class="text-muted text-sm">
            Pogledajte osnovne podatke i adresu za dostavu.
          </p>
        </div>

        <UButton variant="outline" @click="emit('revealDetails')">
          Otvori detalje
        </UButton>
      </div>

      <div class="border-default/70 space-y-2 rounded-lg border p-3">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <h4 class="text-foreground text-sm font-semibold">
            Istorija statusa
          </h4>

          <div class="flex items-center gap-2">
            <UBadge color="neutral" variant="subtle" class="font-normal">
              {{ order.statusHistory?.length || 0 }} unosa
            </UBadge>
            <UButton
              size="xs"
              variant="ghost"
              @click="emit('revealStatusHistory')"
            >
              Otvori istoriju statusa
            </UButton>
            <UButton
              color="primary"
              variant="outline"
              :disabled="!canManageStatuses"
              @click="emit('createStatus')"
            >
              Novi status
            </UButton>
          </div>
        </div>

        <div
          v-if="latestStatus"
          class="bg-background border-default/60 rounded-md border px-3 py-2"
        >
          <p class="text-foreground text-sm font-medium">
            {{ formatOrderStatus(latestStatus.status) }}
          </p>
          <p class="text-muted mt-1 text-xs">
            Izmenjeno: {{ formatDateTime(latestStatus.changedAt) }}
          </p>
          <p class="text-muted mt-1 text-xs">
            Napomena: {{ latestStatus.note || "-" }}
          </p>
        </div>

        <p v-else class="text-muted text-sm">Istorija statusa nije dostupna.</p>
      </div>

      <div class="border-default/70 space-y-2 rounded-lg border p-3">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <h4 class="text-foreground text-sm font-semibold">Uplate</h4>

          <div class="flex items-center gap-2">
            <UBadge
              :color="paymentBalanced ? 'success' : 'error'"
              variant="subtle"
              class="font-normal"
            >
              {{
                paymentBalanced
                  ? "Iznosi se poklapaju"
                  : "Iznosi se ne poklapaju"
              }}
            </UBadge>
            <UButton size="xs" variant="ghost" @click="emit('revealPayments')">
              Otvori uplate
            </UButton>
          </div>
        </div>

        <p class="text-muted text-sm">
          Ukupno: {{ formatMoney(order.totalAmount) }}
        </p>
        <p class="text-muted text-sm">
          Zbir uplata: {{ formatMoney(paymentSum) }}
        </p>
      </div>

      <div class="border-default/70 space-y-2 rounded-lg border p-3">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <h4 class="text-foreground text-sm font-semibold">Stavke</h4>

          <UButton size="xs" variant="ghost" @click="emit('revealItems')">
            Otvori stavke
          </UButton>
        </div>

        <p class="text-muted text-sm">Broj stavki: {{ itemCount }}</p>
        <p class="text-muted text-sm">Ukupna količina: {{ quantitySum }}</p>
      </div>
    </div>
  </div>
</template>
