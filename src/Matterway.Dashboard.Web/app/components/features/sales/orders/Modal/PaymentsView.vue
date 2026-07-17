<script setup lang="ts">
import { useOrderRevealModal } from "~/composables/features/sales/useOrderRevealModal"
import { formatDateTime, formatMoney } from "~/utils/formatters"
import { formatPaymentStatus } from "~/utils/labels"
import { paymentSumOf, paymentsBalanced } from "~/utils/salesOrderMetrics"

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

const { order, notFound, loadState, displayLabel, resetModalState } =
  useOrderRevealModal({
    isOpen,
    orderId: toRef(props, "orderId"),
    orderLabel: toRef(props, "orderLabel"),
    revealErrorMessage: "Učitavanje uplata nije uspelo.",
  })

const paymentSum = computed(() => paymentSumOf(order.value))

const isPaymentBalanced = computed(() => paymentsBalanced(order.value))
</script>

<template>
  <UModal
    v-model:open="isOpen"
    :ui="{
      content: 'sm:max-w-4xl',
    }"
    @after:leave="resetModalState"
  >
    <template #header>
      <div class="space-y-1">
        <h3 class="text-highlighted text-base font-semibold">Uplate</h3>
        <p class="text-muted text-sm">
          Pregled uplata za porudžbinu {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Učitavanje uplata.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Porudžbina nije pronađena"
          description="Izabranu porudžbinu nije moguće učitati."
        />

        <template v-else-if="order">
          <div class="space-y-2">
            <h4 class="text-highlighted text-sm font-semibold">Usklađenost</h4>

            <div class="border-default/70 rounded-md border px-3 py-2">
              <div class="flex flex-wrap items-center justify-between gap-2">
                <div class="text-sm">
                  <p class="text-muted text-xs">
                    Iznos porudžbine i zbir uplata
                  </p>
                  <p class="text-highlighted font-medium">
                    {{ formatMoney(order.totalAmount) }} naspram
                    {{ formatMoney(paymentSum) }}
                  </p>
                </div>
                <UBadge
                  :color="isPaymentBalanced ? 'success' : 'error'"
                  variant="subtle"
                >
                  {{
                    isPaymentBalanced
                      ? "Iznosi se poklapaju"
                      : "Iznosi se ne poklapaju"
                  }}
                </UBadge>
              </div>
            </div>
          </div>

          <div class="space-y-2">
            <div class="flex items-center justify-between gap-3">
              <h4 class="text-highlighted text-sm font-semibold">
                Registrovane uplate
              </h4>
              <UBadge color="neutral" variant="subtle" class="font-normal">
                {{ order.payments?.length || 0 }} uplata
              </UBadge>
            </div>

            <div
              v-if="!order.payments?.length"
              class="border-default/70 bg-default text-muted rounded-md border px-3 py-3 text-sm"
            >
              Za ovu porudžbinu nema registrovanih uplata.
            </div>

            <div v-else class="space-y-2">
              <div
                v-for="payment in order.payments"
                :key="payment.id"
                class="border-default/70 rounded-md border px-3 py-2"
              >
                <dl class="grid gap-2 sm:grid-cols-2">
                  <div>
                    <dt class="text-muted text-xs">ID</dt>
                    <dd
                      class="text-highlighted mt-1 font-mono text-sm break-all"
                    >
                      {{ payment.id }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Status</dt>
                    <dd class="text-highlighted mt-1 text-sm">
                      {{ formatPaymentStatus(payment.status) }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Provajder</dt>
                    <dd class="text-highlighted mt-1 text-sm">
                      {{ payment.provider || "-" }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Iznos</dt>
                    <dd class="text-highlighted mt-1 text-sm font-medium">
                      {{ formatMoney(payment.amount) }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Kreirano</dt>
                    <dd class="text-highlighted mt-1 text-sm">
                      {{ formatDateTime(payment.createdAt) }}
                    </dd>
                  </div>
                </dl>
              </div>
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
