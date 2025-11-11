<script setup lang="ts">
import type CartItemModel from "~/models/CartItemModel";
import { formatMoney } from "~/composables/formatMoney";

defineProps<{
  items: CartItemModel[];
}>();
</script>

<template>
  <ul class="space-y-4">
    <li
      v-for="item in items"
      :key="item.productId"
      class="flex flex-col gap-4 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-blue-200 hover:shadow-md dark:border-slate-700 dark:bg-slate-800"
    >
      <div class="flex flex-wrap items-start justify-between gap-3">
        <div class="space-y-1">
          <p class="text-base font-semibold text-slate-900 dark:text-white">
            {{ item.productName }}
          </p>
          <p class="text-sm text-slate-500 dark:text-slate-400">
            Jedinična cena: {{ formatMoney(item.unitPrice || 0) }}
          </p>
        </div>
        <div class="text-right">
          <p class="text-xs uppercase text-slate-500 dark:text-slate-400">
            Ukupno
          </p>
          <p class="text-lg font-semibold text-blue-600 dark:text-blue-300">
            {{ formatMoney(item.quantity * (item.unitPrice || 0)) }}
          </p>
        </div>
      </div>
      <div class="flex items-center justify-between text-sm">
        <span
          class="inline-flex items-center gap-1 rounded-full border border-slate-200 px-3 py-1 text-xs font-medium uppercase tracking-wide text-slate-500 dark:border-slate-600 dark:text-slate-300"
        >
          Količina
          <span class="text-sm font-semibold text-slate-900 dark:text-white">
            {{ item.quantity }}
          </span>
        </span>
        <NuxtLink
          v-if="item.productId"
          :to="`/products/${item.productId}`"
          class="text-sm font-medium text-blue-600 hover:underline dark:text-blue-300"
        >
          Detalji proizvoda
        </NuxtLink>
      </div>
    </li>
  </ul>
</template>
