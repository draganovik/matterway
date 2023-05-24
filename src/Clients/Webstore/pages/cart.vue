<script lang="ts" setup>
import { useCartStore } from "~/store/cart";
const cart = useCartStore();
const tableSummary = {
  title: "Vaša korpa",
  description:
    "Korisnička korpa je kolekcija vaših odabranih proizvoda koji su spremni za kupovinu",
  totalCount: cart.getTotalItemCount,
  totalPrice: cart.getTotalPrice,
};
useHead({
  title: "Korpa",
});
</script>

<template>
  <section class="flex flex-col items-end gap-8">
    <div class="relative w-full overflow-x-auto">
      <ProductTable :products="cart.getCartItems" :summary="tableSummary" />
    </div>
    <NuxtLink
      v-if="cart.getTotalItemCount > 0"
      to="/checkout"
      class="rounded-lg bg-blue-700 px-8 py-3 text-center text-base font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
      >Kupi proizvode</NuxtLink
    >
  </section>
</template>

<style scoped></style>
