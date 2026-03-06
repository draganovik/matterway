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
</script>

<template>
  <UCard
    class="border-default h-full border"
    :ui="{ root: 'flex h-full flex-col', body: 'flex-1' }"
  >
    <template #header>
      <h2 class="text-base font-semibold">Plaćanje i pregled porudžbine</h2>
    </template>

    <div class="space-y-6">
      <div class="grid gap-3 sm:grid-cols-2">
        <div
          class="border-default bg-elevated/50 rounded-lg border px-3 py-2.5"
        >
          <p class="text-muted text-[11px] tracking-[0.12em] uppercase">
            Stavke
          </p>
          <p class="mt-1 text-lg leading-none font-semibold">
            {{ props.totalItems }}
          </p>
        </div>

        <div
          class="border-primary/20 bg-primary/5 rounded-lg border px-3 py-2.5"
        >
          <p class="text-muted text-[11px] tracking-[0.12em] uppercase">
            Iznos
          </p>
          <p class="mt-1 text-lg leading-none font-semibold">
            {{ formatMoney(props.totalPrice) }}
          </p>
        </div>
      </div>

      <div class="border-default border-t pt-5">
        <div
          class="border-default bg-elevated mb-4 flex items-center gap-2 rounded-lg border px-3 py-2 text-xs"
        >
          <UIcon name="i-lucide-shield-check" class="text-primary h-4 w-4" />
          <span class="text-muted">
            Bezbedna SSL naplata. Podaci o kartici se ne čuvaju.
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

          <UFormField label="Mesec" required>
            <UInput
              v-model="payment.expMonth"
              type="number"
              min="1"
              max="12"
              step="1"
              placeholder="MM"
              autocomplete="cc-exp-month"
              inputmode="numeric"
              class="w-full"
              required
              @blur="emit('monthBlur')"
            />
          </UFormField>

          <UFormField label="Godina" required>
            <UInput
              v-model="payment.expYear"
              type="number"
              :min="props.currentYear"
              :max="props.currentYear + 30"
              step="1"
              placeholder="GGGG"
              autocomplete="cc-exp-year"
              inputmode="numeric"
              class="w-full"
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
        {{ props.loading ? "Obrada..." : "Plati i poruči" }}
      </UButton>
    </template>
  </UCard>
</template>
