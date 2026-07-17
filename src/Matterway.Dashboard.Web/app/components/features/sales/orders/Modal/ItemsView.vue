<script setup lang="ts">
import { useOrderRevealModal } from "~/composables/features/sales/useOrderRevealModal"
import { formatMoney } from "~/utils/formatters"
import { lineTotalOf, quantitySumOf } from "~/utils/salesOrderMetrics"

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
    revealErrorMessage: "Učitavanje stavki porudžbine nije uspelo.",
  })

const itemCount = computed(() => order.value?.items?.length || 0)

const quantitySum = computed(() => quantitySumOf(order.value))
</script>

<template>
  <UModal
    v-model:open="isOpen"
    :ui="{ content: 'sm:max-w-4xl' }"
    @after:leave="resetModalState"
  >
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          Stavke porudžbine
        </h3>
        <p class="text-muted text-sm">
          Pregled stavki za porudžbinu {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Učitavanje stavki porudžbine.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Porudžbina nije pronađena"
          description="Izabranu porudžbinu nije moguće učitati."
        />

        <template v-else-if="order">
          <div class="grid gap-3 sm:grid-cols-2">
            <div class="border-default/70 rounded-md border px-3 py-2">
              <p class="text-muted text-xs">Broj stavki</p>
              <p class="text-foreground mt-1 text-sm font-semibold">
                {{ itemCount }}
              </p>
            </div>

            <div class="border-default/70 rounded-md border px-3 py-2">
              <p class="text-muted text-xs">Ukupna količina</p>
              <p class="text-foreground mt-1 text-sm font-semibold">
                {{ quantitySum }}
              </p>
            </div>
          </div>

          <div
            v-if="!order.items?.length"
            class="border-default/70 bg-background text-muted rounded-md border px-3 py-3 text-sm"
          >
            Ova porudžbina nema stavke.
          </div>

          <div v-else class="space-y-2">
            <div
              v-for="item in order.items"
              :key="item.id"
              class="border-default/70 rounded-md border px-3 py-2"
            >
              <dl class="grid gap-2 sm:grid-cols-2">
                <div>
                  <dt class="text-muted text-xs">ID</dt>
                  <dd class="text-foreground mt-1 font-mono text-sm break-all">
                    {{ item.id }}
                  </dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Šifra artikla</dt>
                  <dd class="text-foreground mt-1 font-mono text-sm break-all">
                    {{ item.articleCode }}
                  </dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Naziv artikla</dt>
                  <dd class="text-foreground mt-1 text-sm">
                    {{ item.articleTitle || "-" }}
                  </dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Količina</dt>
                  <dd class="text-foreground mt-1 text-sm">
                    {{ item.quantity }}
                  </dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Jedinična cena</dt>
                  <dd class="text-foreground mt-1 text-sm">
                    {{ formatMoney(item.unitPrice) }}
                  </dd>
                </div>

                <div>
                  <dt class="text-muted text-xs">Ukupno po stavci</dt>
                  <dd class="text-foreground mt-1 text-sm font-medium">
                    {{ formatMoney(lineTotalOf(item)) }}
                  </dd>
                </div>
              </dl>
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
