<script lang="ts" setup>
import { formatMoney } from "@composables/formatMoney";
import type CardPaymentModel from "#models/CardPaymentModel";

const payment = defineModel<CardPaymentModel>({ required: true });

const props = withDefaults(
  defineProps<{
    totalItems: number;
    totalPrice: number;
    loading?: boolean;
  }>(),
  {
    loading: false,
  },
);

const emit = defineEmits<{
  (e: "submit"): void;
}>();

const handleSubmit = () => emit("submit");
</script>

<template>
  <form
    class="rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
    @submit.prevent="handleSubmit"
  >
    <div class="mb-6 flex items-center gap-4">
      <span
        class="flex h-12 w-12 items-center justify-center rounded-2xl bg-blue-50 text-blue-600 dark:bg-blue-500/10 dark:text-blue-200"
      >
        <svg
          class="h-6 w-6"
          fill="none"
          stroke="currentColor"
          stroke-width="1.8"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M2.25 8.25h19.5m-17.25 6h3.375m-3.375 3h3.375M6 5.25h12A2.25 2.25 0 0120.25 7.5v9a2.25 2.25 0 01-2.25 2.25H6A2.25 2.25 0 013.75 16.5v-9A2.25 2.25 0 016 5.25z"
          />
        </svg>
      </span>
      <div>
        <h2 class="text-lg font-semibold text-slate-900 dark:text-white">
          Plaćanje
        </h2>
        <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">
          Sigurna obrada podataka
        </p>
      </div>
    </div>

    <div class="space-y-5">
      <div>
        <label
          for="cardNumber"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >
          Broj kartice
        </label>
        <input
          type="text"
          id="cardNumber"
          v-model="payment.cardNumber"
          placeholder="0000 0000 0000 0000"
          required
          inputmode="numeric"
          class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
        />
      </div>
      <div class="grid gap-5 sm:grid-cols-3">
        <div>
          <label
            for="expMonth"
            class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >
            Mesec
          </label>
          <input
            type="text"
            id="expMonth"
            v-model.number="payment.expMonth"
            placeholder="MM"
            required
            inputmode="numeric"
            class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
          />
        </div>
        <div>
          <label
            for="expYear"
            class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >
            Godina
          </label>
          <input
            type="text"
            id="expYear"
            v-model.number="payment.expYear"
            placeholder="GGGG"
            required
            inputmode="numeric"
            class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
          />
        </div>
        <div>
          <label
            for="cvc"
            class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >
            CVC
          </label>
          <input
            type="text"
            id="cvc"
            maxlength="4"
            v-model="payment.cvc"
            pattern="[0-9]{3,4}"
            placeholder="123"
            required
            inputmode="numeric"
            class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
          />
        </div>
      </div>
    </div>

    <div
      class="mt-6 space-y-3 border-t border-slate-200 pt-6 dark:border-slate-700"
    >
      <div class="flex items-center justify-between text-sm">
        <span class="text-slate-500 dark:text-slate-400">Ukupno proizvoda</span>
        <span class="font-medium text-slate-900 dark:text-white">
          {{ totalItems }}
        </span>
      </div>
      <div class="flex items-center justify-between">
        <span class="text-base font-semibold text-slate-900 dark:text-white"
          >Za uplatu</span
        >
        <span class="text-2xl font-bold text-blue-600 dark:text-blue-400">
          {{ formatMoney(totalPrice) }}
        </span>
      </div>
    </div>

    <button
      type="submit"
      :disabled="loading"
      class="mt-6 inline-flex w-full items-center justify-center gap-2 rounded-full bg-blue-600 px-5 py-3 text-sm font-semibold text-white transition hover:bg-blue-700 focus:outline-hidden focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-70"
    >
      <span>{{ loading ? "Obrada plaćanja..." : "Potvrdi plaćanje" }}</span>
      <svg
        class="h-4 w-4"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          d="M9 12.75 11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
        />
      </svg>
    </button>
  </form>
</template>
