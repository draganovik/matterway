<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"
import { useCart } from "~/composables/useCart"
import { formatMoney } from "~/utils/formatters"

const auth = useAuthSession()
const cart = useCart()

const isEmpty = computed(() => cart.totalItems.value === 0)

function goToCheckout() {
  if (isEmpty.value) return
  if (!auth.isLoggedIn.value) {
    void navigateTo("/login?next=%2Fcheckout")
    return
  }
  void navigateTo("/checkout")
}

async function clearCart() {
  await cart.clear()
}
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
      <CartListPanel
        :items="cart.items.value"
        @increase="cart.increase"
        @decrease="cart.decrease"
        @remove="cart.remove"
      />

      <CartActionPanel
        :total-items="cart.totalItems.value"
        :total-price="cart.totalPrice.value"
        @checkout="goToCheckout"
        @clear="clearCart"
      />
    </div>
  </div>
</template>
