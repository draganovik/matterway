<script setup lang="ts">
import { useCartPage } from "~/composables/features/cart/useCartPage"
import { formatMoney } from "~/utils/formatters"

definePageMeta({
  title: "Korpa",
  public: true,
})

const { cart, isEmpty, initialize, goToCheckout } = useCartPage()

await initialize()
</script>

<template>
  <div class="space-y-4">
    <header
      class="border-default flex flex-wrap items-center justify-between gap-3 border-b pb-3"
    >
      <h1 class="text-xl font-semibold">Vaša korpa</h1>
      <div class="flex items-center gap-2">
        <UBadge color="primary" variant="soft">
          {{ cart.totalItems.value }} stavki
        </UBadge>
        <UBadge color="neutral" variant="soft">
          {{ formatMoney(cart.totalPrice.value) }}
        </UBadge>
      </div>
    </header>

    <EmptyState
      v-if="isEmpty"
      title="Korpa je prazna"
      description="Dodajte artikle iz kataloga da biste nastavili kupovinu."
      icon="i-lucide-shopping-cart"
    >
      <UButton to="/articles" color="primary">Pogledaj artikle</UButton>
    </EmptyState>

    <div v-else class="grid items-start gap-4 lg:grid-cols-[1fr_20rem]">
      <CartListView :items="cart.items.value" @remove="cart.remove" />

      <CartActionPanel
        class="lg:sticky lg:top-20"
        :total-items="cart.totalItems.value"
        :total-price="cart.totalPrice.value"
        @checkout="goToCheckout"
        @clear="cart.clear"
      />
    </div>
  </div>
</template>
