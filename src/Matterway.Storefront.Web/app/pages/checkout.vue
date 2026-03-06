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
    <UCard class="border-default bg-elevated/60 border">
      <div class="flex items-center justify-between gap-3">
        <div>
          <p class="text-primary text-xs tracking-[0.3em] uppercase">
            Plaćanje
          </p>
          <h1 class="text-2xl font-semibold">Završi porudžbinu</h1>
        </div>
        <UBadge color="primary" variant="soft">
          {{ totalItems }} stavki
        </UBadge>
      </div>
    </UCard>

    <StatusMessages v-if="error" :error="error" />

    <div class="grid gap-6 lg:grid-cols-[1fr_22rem]">
      <div class="space-y-6">
        <CheckoutDeliveryPanel v-model="address" />
        <CheckoutPaymentPanel
          v-model="payment"
          :current-year="currentYear"
          @month-blur="normalizeExpMonthOnBlur"
        />
      </div>

      <CheckoutOrderActionPanel
        :total-items="totalItems"
        :total-price="totalPrice"
        :loading="loading"
        @submit="submitCheckout"
      />
    </div>
  </div>
</template>
