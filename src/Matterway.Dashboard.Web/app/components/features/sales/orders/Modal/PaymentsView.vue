<script setup lang="ts">
import { useOrderReveal } from "~/composables/useOrderReveal"
import { formatDateTime, formatMoney } from "~/utils/formatters"
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

const { order, notFound, loadState, displayLabel } = useOrderReveal({
  isOpen,
  orderId: toRef(props, "orderId"),
  orderLabel: toRef(props, "orderLabel"),
  revealErrorMessage: "Unable to reveal payments.",
})

const paymentSum = computed(() => paymentSumOf(order.value))

const isPaymentBalanced = computed(() => paymentsBalanced(order.value))
</script>

<template>
  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-4xl' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Payments</h3>
        <p class="text-muted text-sm">
          Revealed payments for {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing payments.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Order not found"
          description="The selected order could not be loaded."
        />

        <template v-else-if="order">
          <div class="space-y-2">
            <h4 class="text-foreground text-sm font-semibold">
              Reconciliation
            </h4>

            <div class="border-default/70 rounded-md border px-3 py-2">
              <div class="flex flex-wrap items-center justify-between gap-2">
                <div class="text-sm">
                  <p class="text-muted text-xs">Order Amount vs Payments Sum</p>
                  <p class="text-foreground font-medium">
                    {{ formatMoney(order.totalAmount) }} vs
                    {{ formatMoney(paymentSum) }}
                  </p>
                </div>
                <UBadge
                  :color="isPaymentBalanced ? 'success' : 'error'"
                  variant="subtle"
                >
                  {{
                    isPaymentBalanced ? "Amounts Match" : "Amounts Do Not Match"
                  }}
                </UBadge>
              </div>
            </div>
          </div>

          <div class="space-y-2">
            <div class="flex items-center justify-between gap-3">
              <h4 class="text-foreground text-sm font-semibold">
                Registered Payments
              </h4>
              <UBadge color="neutral" variant="subtle" class="font-normal">
                {{ order.payments?.length || 0 }} payments
              </UBadge>
            </div>

            <div
              v-if="!order.payments?.length"
              class="border-default/70 bg-background text-muted rounded-md border px-3 py-3 text-sm"
            >
              No payments registered for this order.
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
                      class="text-foreground mt-1 font-mono text-sm break-all"
                    >
                      {{ payment.id }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Status</dt>
                    <dd class="text-foreground mt-1 text-sm">
                      {{ payment.status }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Provider</dt>
                    <dd class="text-foreground mt-1 text-sm">
                      {{ payment.provider || "-" }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Reference ID</dt>
                    <dd
                      class="text-foreground mt-1 font-mono text-sm break-all"
                    >
                      {{ payment.referenceId || "-" }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Amount</dt>
                    <dd class="text-foreground mt-1 text-sm font-medium">
                      {{ formatMoney(payment.amount) }}
                    </dd>
                  </div>

                  <div>
                    <dt class="text-muted text-xs">Created At</dt>
                    <dd class="text-foreground mt-1 text-sm">
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
        <UButton variant="ghost" @click="isOpen = false">Close</UButton>
      </div>
    </template>
  </UModal>
</template>
