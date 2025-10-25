<script lang="ts">
class Summary {
  title?: string;
  description?: string;
  totalCount?: number;
  totalPrice?: number;
}
</script>

<script setup lang="ts">
import CartItemModel from "~/models/CartItemModel";

const props = defineProps({
  products: {
    type: Array as PropType<CartItemModel[]>,
    required: true,
  },
  summary: {
    type: Summary,
    required: false,
  },
});
</script>
<template>
  <table
    class="w-full overflow-hidden rounded-lg text-left text-sm text-slate-500 dark:text-slate-400"
  >
    <caption
      v-if="summary"
      class="p-5 text-left text-lg font-semibold text-slate-900 dark:text-white"
    >
      {{
        summary.title
      }}
      <p class="mt-1 text-sm font-normal text-slate-500 dark:text-slate-400">
        {{ summary.description }}
      </p>
    </caption>
    <thead
      class="sticky top-0 bg-slate-100 text-xs uppercase text-slate-700 dark:bg-slate-700 dark:text-slate-400"
    >
      <tr>
        <th scope="col" class="px-6 py-3">Proizvod</th>
        <th scope="col" class="px-6 py-3">Količina</th>
        <th scope="col" class="px-6 py-3">Ukupna cena</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="item in products" class="bg-white dark:bg-slate-800">
        <th
          scope="row"
          class="whitespace-nowrap px-6 py-4 font-medium text-slate-900 dark:text-white"
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
    <tfoot v-if="summary">
      <tr class="font-semibold text-slate-900 dark:text-white">
        <th scope="row" class="px-6 py-3 text-xl font-bold">
          Ukupna za uplatu
        </th>
        <td class="px-6 py-3 text-xl font-bold">{{ summary.totalCount }}</td>
        <td class="px-6 py-3 text-2xl font-bold">
          {{ formatMoney(summary.totalPrice!) }}
        </td>
      </tr>
    </tfoot>
  </table>
</template>
