<script setup lang="ts">
import { computed } from "vue";
import { formatMoney } from "@composables/formatMoney";

const props = defineProps<{
  itemsCount: number;
  totalPrice: number;
  isEmpty: boolean;
}>();

const emit = defineEmits<{
  (e: "checkout"): void;
  (e: "continue"): void;
}>();

const itemsLabel = computed(() =>
  props.itemsCount === 1 ? "proizvod" : "proizvoda",
);
</script>

<template>
  <aside
    class="flex flex-col gap-6 rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
  >
    <header class="space-y-1.5">
      <h2 class="text-xl font-semibold text-slate-900 dark:text-white">
        Sažetak narudžbine
      </h2>
      <p class="text-sm text-slate-500 dark:text-slate-400">
        {{
          isEmpty
            ? "Dodajte proizvode u korpu kako biste nastavili sa kupovinom."
            : "Proverite iznose i nastavite na blagajnu kada budete spremni."
        }}
      </p>
    </header>
    <dl class="space-y-3 text-sm">
      <div class="flex items-center justify-between">
        <dt class="text-slate-500 dark:text-slate-400">Broj artikala</dt>
        <dd class="font-semibold text-slate-900 dark:text-white">
          {{ itemsCount }} {{ itemsLabel }}
        </dd>
      </div>
      <div class="flex items-center justify-between">
        <dt class="text-slate-500 dark:text-slate-400">Međuzbir</dt>
        <dd class="font-semibold text-slate-900 dark:text-white">
          {{ formatMoney(totalPrice) }}
        </dd>
      </div>
      <div class="flex items-center justify-between">
        <dt class="text-slate-500 dark:text-slate-400">Dostava</dt>
        <dd class="font-semibold text-green-600 dark:text-green-400">
          Besplatna
        </dd>
      </div>
    </dl>
    <div
      class="flex items-center justify-between border-t border-slate-200 pt-4 text-base font-semibold text-slate-900 dark:border-slate-700 dark:text-white"
    >
      <span>Ukupno</span>
      <span>{{ formatMoney(totalPrice) }}</span>
    </div>
    <div class="flex flex-col gap-3">
      <button
        type="button"
        class="inline-flex items-center justify-center rounded-full bg-blue-600 px-6 py-3 text-sm font-semibold text-white transition hover:bg-blue-700 focus:outline-hidden focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60"
        :disabled="isEmpty"
        @click="emit('checkout')"
      >
        Nastavi na plaćanje
      </button>
      <button
        type="button"
        class="inline-flex items-center justify-center rounded-full border border-slate-300 px-6 py-3 text-sm font-semibold text-slate-700 transition hover:border-slate-400 hover:text-slate-900 focus:outline-hidden focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 dark:border-slate-600 dark:text-slate-200 dark:hover:border-slate-500 dark:hover:text-white"
        @click="emit('continue')"
      >
        Nastavi kupovinu
      </button>
    </div>
  </aside>
</template>
