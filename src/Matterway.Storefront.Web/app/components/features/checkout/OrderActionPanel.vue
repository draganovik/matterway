<script setup lang="ts">
import type { CheckoutPaymentForm } from "~/types/checkout"
import { formatMoney } from "~/utils/formatters"

const props = defineProps<{
  currentYear: number
  totalItems: number
  totalPrice: number
  loading: boolean
}>()

const payment = defineModel<CheckoutPaymentForm>({ required: true })

const emit = defineEmits<{
  monthBlur: []
  submit: []
}>()

function parseOptionalNumber(value: string) {
  const trimmed = String(value ?? "").trim()
  if (!trimmed) return null

  const parsed = Number(trimmed)
  return Number.isFinite(parsed) ? parsed : null
}

const expMonthValue = computed({
  get: () => parseOptionalNumber(payment.value.expMonth),
  set: (value: number | null | undefined) => {
    if (value == null) {
      payment.value.expMonth = ""
      return
    }

    const normalized = Math.max(1, Math.min(12, Math.trunc(value)))
    payment.value.expMonth = String(normalized).padStart(2, "0")
  },
})

const expYearValue = computed({
  get: () => parseOptionalNumber(payment.value.expYear),
  set: (value: number | null | undefined) => {
    payment.value.expYear = value == null ? "" : String(Math.trunc(value))
  },
})
</script>

<template>
  <UCard class="h-full" :ui="{ root: 'flex h-full flex-col', body: 'flex-1' }">
    <template #header>
      <h2 class="text-base font-semibold">Plaćanje i potvrda porudžbine</h2>
    </template>

    <div class="space-y-4">
      <div class="grid gap-3 sm:grid-cols-2">
        <div class="border-default bg-default rounded-md border px-3 py-2.5">
          <p class="text-muted text-sm">Stavke</p>
          <p class="mt-1 text-lg leading-none font-semibold">
            {{ props.totalItems }}
          </p>
        </div>

        <div
          class="border-primary/20 bg-primary/5 rounded-md border px-3 py-2.5"
        >
          <p class="text-muted text-sm">Iznos</p>
          <p class="mt-1 text-lg leading-none font-semibold">
            {{ formatMoney(props.totalPrice) }}
          </p>
        </div>
      </div>

      <div class="border-default border-t pt-4">
        <div
          class="border-default bg-default mb-4 flex items-center gap-2 rounded-md border px-3 py-2 text-xs"
        >
          <UIcon name="i-lucide-shield-check" class="text-primary h-4 w-4" />
          <span class="text-muted">
            Bezbedno plaćanje putem SSL zaštite. Podaci sa kartice se ne čuvaju.
          </span>
        </div>

        <div class="grid gap-4 sm:grid-cols-2">
          <UFormField label="Broj kartice" required class="sm:col-span-2">
            <UInput
              v-model="payment.cardNumber"
              icon="i-lucide-credit-card"
              placeholder="1234 5678 9012 3456"
              autocomplete="cc-number"
              inputmode="numeric"
              maxlength="23"
              :ui="{ base: 'font-mono tracking-[0.08em]' }"
              class="w-full"
              required
            />
          </UFormField>

          <UFormField label="Mesec isteka" required>
            <UInputNumber
              v-model="expMonthValue"
              orientation="vertical"
              :min="1"
              :max="12"
              :step="1"
              step-snapping
              variant="outline"
              placeholder="MM"
              :format-options="{ useGrouping: false, minimumIntegerDigits: 2 }"
              autocomplete="cc-exp-month"
              class="w-full"
              :ui="{ base: 'w-full text-left' }"
              required
              @blur="emit('monthBlur')"
            />
          </UFormField>

          <UFormField label="Godina isteka" required>
            <UInputNumber
              v-model="expYearValue"
              orientation="vertical"
              :min="props.currentYear"
              :max="props.currentYear + 30"
              :step="1"
              step-snapping
              variant="outline"
              placeholder="GGGG"
              :format-options="{ useGrouping: false }"
              autocomplete="cc-exp-year"
              class="w-full"
              :ui="{ base: 'w-full text-left' }"
              required
            />
          </UFormField>

          <UFormField label="CVC" required class="sm:col-span-2">
            <UInput
              v-model="payment.cvc"
              type="text"
              placeholder="123"
              autocomplete="cc-csc"
              inputmode="numeric"
              pattern="[0-9]*"
              maxlength="4"
              class="w-full"
              required
            />
          </UFormField>
        </div>
      </div>
    </div>

    <template #footer>
      <UButton
        color="primary"
        block
        :loading="props.loading"
        @click="emit('submit')"
      >
        {{ props.loading ? "Obrada plaćanja" : "Plati i poruči" }}
      </UButton>
    </template>
  </UCard>
</template>
