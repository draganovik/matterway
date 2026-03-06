<script setup lang="ts">
import { useCartPage } from "~/composables/features/cart/useCartPage"
import { formatMoney } from "~/utils/formatters"

definePageMeta({
  title: "Korpa",
  public: true,
})

const { cart, isEmpty, goToCheckout, clearCart } = useCartPage()
</script>

<template>
  <div class="space-y-6">
    <UCard class="border-default bg-elevated/60 border">
      <div class="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p class="text-primary text-xs tracking-[0.3em] uppercase">Korpa</p>
          <h1 class="text-2xl font-semibold">Vaša korpa</h1>
        </div>
        <div class="flex items-center gap-2">
          <UBadge color="primary" variant="soft"
            >{{ cart.totalItems.value }} stavki</UBadge
          >
          <UBadge color="neutral" variant="soft">{{
            formatMoney(cart.totalPrice.value)
          }}</UBadge>
        </div>
      </div>
    </UCard>

    <EmptyState
      v-if="isEmpty"
      title="Korpa je prazna"
      description="Dodajte artikle iz kataloga da nastavite."
      icon="i-lucide-shopping-cart"
    >
      <UButton to="/articles" color="primary">Pregledaj artikle</UButton>
    </EmptyState>

    <div v-else class="grid gap-6 lg:grid-cols-[1fr_20rem]">
      <CartListView :items="cart.items.value" @remove="cart.remove" />

      <CartActionPanel
        :total-items="cart.totalItems.value"
        :total-price="cart.totalPrice.value"
        @checkout="goToCheckout"
        @clear="clearCart"
      />
    </div>
  </div>
</template>
