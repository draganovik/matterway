<script lang="ts" setup>
import { useCartStore } from "~/store/cart";
const cart = useCartStore();
useHead({
  title: "Korpa",
});
</script>

<template>
  <section class="flex flex-col items-end gap-8">
    <div class="relative w-full overflow-x-auto">
      <table class="w-full text-left text-sm text-gray-500 dark:text-gray-400">
        <caption
          class="rounded-t-lg bg-white p-5 text-left text-lg font-semibold text-gray-900 dark:bg-gray-800 dark:text-white"
        >
          Vaša korpa
          <p class="mt-1 text-sm font-normal text-gray-500 dark:text-gray-400">
            Korisnička korpa je kolekcija vaših odabranih proizvoda koji su
            spremni za kupovinu
          </p>
        </caption>
        <thead
          class="bg-gray-100 text-xs uppercase text-gray-700 dark:bg-gray-700 dark:text-gray-400"
        >
          <tr>
            <th scope="col" class="px-6 py-3">Proizvod</th>
            <th scope="col" class="px-6 py-3">Količina</th>
            <th scope="col" class="px-6 py-3">Ukupna cena</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="item in cart.getCartItems"
            class="bg-white dark:bg-gray-800"
          >
            <th
              scope="row"
              class="whitespace-nowrap px-6 py-4 font-medium text-gray-900 dark:text-white"
            >
              {{ item.productName }}
            </th>
            <td class="px-6 py-4">
              {{ item.quantity }}
            </td>
            <td class="px-6 py-4">
              {{ formatMoney(item.quantity * (item.unitPrice || 0)) }}
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr class="font-semibold text-gray-900 dark:text-white">
            <th scope="row" class="px-6 py-3 text-base">Ukupna za naplatu</th>
            <td class="px-6 py-3">{{ cart.getTotalItemCount }}</td>
            <td class="px-6 py-3">{{ formatMoney(cart.getTotalPrice) }}</td>
          </tr>
        </tfoot>
      </table>
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
