<script setup lang="ts">
type PaymentForm = {
  cardNumber: string
  expMonth: string
  expYear: string
  cvc: string
}

const props = defineProps<{
  currentYear: number
}>()

const payment = defineModel<PaymentForm>({ required: true })
</script>

<template>
  <UCard class="border-default bg-default border">
    <template #header>
      <h2 class="text-lg font-semibold">Plaćanje karticom</h2>
    </template>

    <div
      class="border-default bg-elevated mb-4 flex items-center gap-2 rounded-lg border px-3 py-2 text-xs"
    >
      <UIcon name="i-lucide-shield-check" class="text-primary h-4 w-4" />
      <span class="text-muted">
        Bezbedna SSL naplata. Podaci o kartici se ne čuvaju.
      </span>
    </div>

    <div class="grid gap-4 sm:grid-cols-3">
      <UFormField label="Broj kartice" required class="sm:col-span-3">
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
          type="text"
          placeholder="MM"
          autocomplete="cc-exp-month"
          inputmode="numeric"
          maxlength="2"
          class="w-full"
          required
        />
      </UFormField>

      <UFormField label="Godina" required>
        <UInput
          v-model="payment.expYear"
          type="number"
          :min="props.currentYear"
          placeholder="GGGG"
          autocomplete="cc-exp-year"
          inputmode="numeric"
          class="w-full"
          required
        />
      </UFormField>

      <UFormField label="CVC" required>
        <UInput
          v-model="payment.cvc"
          placeholder="123"
          autocomplete="cc-csc"
          inputmode="numeric"
          maxlength="4"
          class="w-full"
          required
        />
      </UFormField>
    </div>
  </UCard>
</template>
