<script setup lang="ts">
import { useCheckoutPage } from "~/composables/features/checkout/useCheckoutPage"

definePageMeta({
  title: "Plaćanje",
})

const {
  address,
  payment,
  currentYear,
  totalItems,
  totalPrice,
  error,
  loading,
  initialize,
  submitCheckout,
  normalizeExpMonthOnBlur,
} = useCheckoutPage()

await initialize()
</script>

<template>
  <div class="space-y-6">
    <UCard class="border-default border">
      <div class="flex items-center justify-between gap-3">
        <div>
          <h1 class="text-2xl font-semibold">Završi porudžbinu</h1>
        </div>
        <UBadge color="primary" variant="soft">
          {{ totalItems }} stavki
        </UBadge>
      </div>
    </UCard>

    <div
      class="grid items-start gap-6 lg:grid-cols-[1fr_26rem] lg:items-stretch"
    >
      <CheckoutDeliveryPanel v-model="address" />

      <div
        class="grid gap-4 lg:h-full lg:min-h-0"
        :class="
          error
            ? 'lg:grid-rows-[minmax(0,1fr)_auto]'
            : 'lg:grid-rows-[minmax(0,1fr)]'
        "
      >
        <CheckoutOrderActionPanel
          v-model="payment"
          class="lg:sticky lg:top-24 lg:h-full"
          :current-year="currentYear"
          :total-items="totalItems"
          :total-price="totalPrice"
          :loading="loading"
          @month-blur="normalizeExpMonthOnBlur"
          @submit="submitCheckout"
        />

        <StatusMessages v-if="error" :error="error" />
      </div>
    </div>
  </div>
</template>
