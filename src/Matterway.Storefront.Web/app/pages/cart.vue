<script lang="ts" setup>
import { computed } from "vue";
import { useCartStore } from "@stores/cart";

const router = useRouter();
const cartStore = useCartStore();

useHead({
  title: "Korpa",
});

const cartItems = computed(() => cartStore.getCartItems);
const totalItems = computed(() => cartStore.getTotalItemCount);
const totalPrice = computed(() => cartStore.getTotalPrice);
const isEmpty = computed(() => totalItems.value === 0);
const itemsLabel = computed(() =>
  totalItems.value === 1 ? "proizvod" : "proizvoda",
);
const subtitle = computed(() =>
  isEmpty.value
    ? "Vaša korpa je prazna. Pregledajte preporučene kolekcije i započnite kupovinu."
    : "Pregledajte proizvode u korpi i nastavite prema blagajni kada budete spremni.",
);

const goToProducts = () => router.push("/products");
const goToCheckout = () => {
  if (!isEmpty.value) {
    router.push("/checkout");
  }
};
</script>

<template>
  <main class="mx-auto max-w-6xl space-y-12 px-4 pb-16 md:px-6">
    <section class="space-y-3">
      <span
        class="inline-flex items-center rounded-full border border-slate-200 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-slate-500 dark:border-slate-700 dark:text-slate-300"
      >
        Pregled kupovine
      </span>
      <div class="flex flex-wrap items-center justify-between gap-4">
        <div class="space-y-2">
          <h1 class="text-3xl font-semibold text-slate-900 dark:text-white">
            Vaša korpa
          </h1>
          <p class="max-w-2xl text-sm text-slate-500 dark:text-slate-400">
            {{ subtitle }}
          </p>
        </div>
        <span
          class="inline-flex items-center rounded-full border border-blue-100 bg-blue-50 px-4 py-2 text-sm font-medium text-blue-600 dark:border-blue-900/40 dark:bg-blue-900/20 dark:text-blue-200"
        >
          {{ totalItems }} {{ itemsLabel }}
        </span>
      </div>
    </section>

    <div class="grid gap-8 lg:grid-cols-[minmax(0,2fr)_minmax(0,1fr)]">
      <section
        class="rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
      >
        <div
          v-if="isEmpty"
          class="flex flex-col items-center justify-center gap-5 text-center"
        >
          <span
            class="flex h-20 w-20 items-center justify-center rounded-full bg-blue-50 text-blue-600 dark:bg-blue-900/30 dark:text-blue-300"
          >
            <svg
              class="h-8 w-8"
              fill="none"
              stroke="currentColor"
              stroke-width="1.8"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M3 3h2l.4 2M7 13h10l3.055-6.109A1 1 0 0 0 19.117 6H6.163M7 13l-1.6 5.6A1 1 0 0 0 6.362 20H19M7 13l-2-8M9 21a1 1 0 1 1-2 0 1 1 0 0 1 2 0Zm10 0a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"
              />
            </svg>
          </span>
          <div class="space-y-2">
            <h2 class="text-xl font-semibold text-slate-900 dark:text-white">
              Korpa je prazna
            </h2>
            <p class="text-sm text-slate-500 dark:text-slate-400">
              Dodajte proizvode iz naših kolekcija kako biste započeli kupovinu.
            </p>
          </div>
          <button
            type="button"
            class="inline-flex items-center gap-2 rounded-full bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-blue-700 focus:outline-hidden focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            @click="goToProducts"
          >
            <span>Pregledaj proizvode</span>
            <svg
              class="h-4 w-4"
              fill="none"
              stroke="currentColor"
              stroke-width="1.8"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="m13.5 4.5 7.5 7.5m0 0-7.5 7.5M21 12H3"
              />
            </svg>
          </button>
        </div>
        <div v-else class="space-y-6">
          <header class="flex items-center justify-between">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">
              Pregled artikala
            </h2>
            <span
              class="text-xs font-medium uppercase tracking-wide text-slate-400 dark:text-slate-500"
            >
              Ažurirano u realnom vremenu
            </span>
          </header>
          <CartItemsList :items="cartItems" />
        </div>
      </section>

      <CartSummary
        :items-count="totalItems"
        :total-price="totalPrice"
        :is-empty="isEmpty"
        @checkout="goToCheckout"
        @continue="goToProducts"
      />
    </div>
  </main>
</template>
