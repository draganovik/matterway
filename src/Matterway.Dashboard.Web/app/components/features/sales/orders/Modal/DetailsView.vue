<script setup lang="ts">
import { useOrderRevealModal } from "~/composables/features/sales/useOrderRevealModal"
import { formatDateTime, formatMoney } from "~/utils/formatters"
import { formatOrderType } from "~/utils/labels"

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
    revealErrorMessage: "Učitavanje detalja porudžbine nije uspelo.",
  })
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
        <h3 class="text-highlighted text-base font-semibold">
          Detalji porudžbine
        </h3>
        <p class="text-muted text-sm">
          Osnovni podaci i dostava za porudžbinu {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="
            loadState.loading ? 'Učitavanje detalja porudžbine.' : false
          "
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Porudžbina nije pronađena"
          description="Izabranu porudžbinu nije moguće učitati."
        />

        <template v-else-if="order">
          <div class="space-y-2">
            <h4 class="text-highlighted text-sm font-semibold">Pregled</h4>
            <dl class="grid gap-3 sm:grid-cols-2">
              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">ID</dt>
                <dd
                  class="text-highlighted mt-1 font-mono text-sm font-medium break-all"
                >
                  {{ order.id }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">ID kupca</dt>
                <dd
                  class="text-highlighted mt-1 font-mono text-sm font-medium break-all"
                >
                  {{ order.customerId || "-" }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Tip</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ formatOrderType(order.type) }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Kreirano</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ formatDateTime(order.placedAt) }}
                </dd>
              </div>

              <div
                class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
              >
                <dt class="text-muted text-xs">Ukupan iznos</dt>
                <dd class="text-highlighted mt-1 text-sm font-semibold">
                  {{ formatMoney(order.totalAmount) }}
                </dd>
              </div>
            </dl>
          </div>

          <div class="space-y-2">
            <h4 class="text-highlighted text-sm font-semibold">
              Podaci za dostavu
            </h4>
            <dl v-if="order.deliveryInfo" class="grid gap-3 sm:grid-cols-2">
              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Država</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.country || "-" }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Grad</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.city || "-" }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Poštanski broj</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.zipCode || "-" }}
                </dd>
              </div>

              <div class="border-default/70 rounded-md border px-3 py-2">
                <dt class="text-muted text-xs">Kontakt telefon</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.contactPhone || "-" }}
                </dd>
              </div>

              <div
                class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
              >
                <dt class="text-muted text-xs">Adresa 1</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.addressLine1 || "-" }}
                </dd>
              </div>

              <div
                class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
              >
                <dt class="text-muted text-xs">Adresa 2</dt>
                <dd class="text-highlighted mt-1 text-sm font-medium">
                  {{ order.deliveryInfo.addressLine2 || "-" }}
                </dd>
              </div>
            </dl>

            <div
              v-else
              class="border-default/70 bg-default text-muted rounded-md border px-3 py-3 text-sm"
            >
              Podaci za dostavu nisu dostupni.
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
