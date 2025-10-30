<script lang="ts" setup>
import { computed } from "vue";
import { useCartStore } from "~/stores/cart";
import { useSessionStore } from "~/stores/session";
import { formatMoney } from "~/composables/formatMoney";
import AddressModel from "~/models/AddressModel";
import CardPaymentModel from "~/models/CardPaymentModel";

const cart = useCartStore();
const session = useSessionStore();

const paymentData: Ref<CardPaymentModel> = ref(
  new CardPaymentModel("4242424242424242", 12, 2029, "123", cart.getTotalPrice),
);

const addressData: Ref<AddressModel> = ref({
  receiverName: "",
  residence: "",
  street: "",
  city: "",
  zipCode: "",
  note: "",
  validate: () => true,
} as AddressModel);

const cartItems = computed(() => cart.getCartItems);
const totalItems = computed(() => cart.getTotalItemCount);
const totalPrice = computed(() => cart.getTotalPrice);

const pay = async () => {
  const response = await fetch("/api/payments", {
    method: "POST",
    body: JSON.stringify({
      ...paymentData.value,
      ...addressData.value,
      items: cart.getCartItems,
      userId: session.getTokenData?.nameid,
    }),
  });
  if (response.ok) {
    const order = await response.json();
    console.log(order);
    cart.clearCart();
    navigateTo("/orders");
  }
};

useHead({
  title: "Kupovina",
});

definePageMeta({
  middleware: [
    function (to, from) {
      const cartCheck = useCartStore();
      if (!cartCheck.getTotalItemCount) {
        return navigateTo("/cart");
      }
    },
    "auth",
  ],
});
</script>

<template>
  <main class="mx-auto max-w-6xl space-y-12 px-4 pb-16 md:px-6">
    <section class="space-y-3">
      <span
        class="inline-flex items-center rounded-full border border-slate-200 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-slate-500 dark:border-slate-700 dark:text-slate-300"
      >
        Završetak kupovine
      </span>
      <div class="flex flex-wrap items-center justify-between gap-4">
        <div class="space-y-2">
          <h1 class="text-3xl font-semibold text-slate-900 dark:text-white">
            Kupovina
          </h1>
          <p class="max-w-2xl text-sm text-slate-500 dark:text-slate-400">
            Unesite podatke o dostavi i plaćanju kako biste završili kupovinu.
          </p>
        </div>
        <span
          class="inline-flex items-center rounded-full border border-blue-100 bg-blue-50 px-4 py-2 text-sm font-medium text-blue-600 dark:border-blue-900/40 dark:bg-blue-900/20 dark:text-blue-200"
        >
          {{ totalItems }} {{ totalItems === 1 ? "proizvod" : "proizvoda" }}
        </span>
      </div>
    </section>

    <div class="grid gap-8 lg:grid-cols-[minmax(0,2fr),minmax(0,1fr)]">
      <div class="space-y-8">
        <!-- Order Items -->
        <section
          class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
        >
          <header class="mb-6 flex items-center justify-between">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">
              Vaša narudžbina
            </h2>
            <span
              class="text-xs font-medium uppercase tracking-wide text-slate-400 dark:text-slate-500"
            >
              {{ totalItems }} {{ totalItems === 1 ? "artikal" : "artikala" }}
            </span>
          </header>
          <CartItemsList :items="cartItems" />
        </section>

        <!-- Delivery Address -->
        <section
          class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
        >
          <header class="mb-6">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">
              Podaci za dostavu
            </h2>
            <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">
              Unesite adresu na koju želite da dostavimo vašu narudžbinu.
            </p>
          </header>
          <div class="space-y-5">
            <div>
              <label
                for="name"
                class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >
                Ime primaoca
              </label>
              <input
                type="text"
                id="name"
                v-model="addressData.receiverName"
                placeholder="Ime i prezime"
                required
                class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
              />
            </div>
            <div class="grid gap-5 sm:grid-cols-2">
              <div>
                <label
                  for="city"
                  class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
                >
                  Grad
                </label>
                <input
                  type="text"
                  id="city"
                  autocomplete="address-level2"
                  v-model="addressData.city"
                  placeholder="Naziv grada"
                  required
                  class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
                />
              </div>
              <div>
                <label
                  for="postalCode"
                  class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
                >
                  Poštanski broj
                </label>
                <input
                  type="text"
                  id="postalCode"
                  v-model="addressData.zipCode"
                  placeholder="00000"
                  required
                  class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
                />
              </div>
            </div>
            <div class="grid gap-5 sm:grid-cols-[2fr,1fr]">
              <div>
                <label
                  for="street"
                  class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
                >
                  Ulica
                </label>
                <input
                  type="text"
                  id="street"
                  autocomplete="street-address"
                  v-model="addressData.street"
                  placeholder="Naziv ulice"
                  required
                  class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
                />
              </div>
              <div>
                <label
                  for="streetNumber"
                  class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
                >
                  Broj
                </label>
                <input
                  type="text"
                  id="streetNumber"
                  v-model="addressData.residence"
                  placeholder="Broj"
                  required
                  class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
                />
              </div>
            </div>
          </div>
        </section>
      </div>

      <!-- Payment Form -->
      <form
        @submit.prevent="pay()"
        class="h-fit rounded-3xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
      >
        <div class="mb-6 flex flex-col items-center gap-3 text-center">
          <span
            class="flex h-16 w-16 items-center justify-center rounded-full bg-blue-50 text-blue-600 dark:bg-blue-900/30 dark:text-blue-300"
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
                d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 002.25-2.25V6.75A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25v10.5A2.25 2.25 0 004.5 19.5z"
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
              v-model="paymentData.cardNumber"
              placeholder="0000 0000 0000 0000"
              required
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
                v-model="paymentData.expMonth"
                placeholder="MM"
                required
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
                v-model="paymentData.expYear"
                placeholder="GGGG"
                required
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
                v-model="paymentData.cvc"
                pattern="[0-9]{3,4}"
                placeholder="123"
                required
                class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
              />
            </div>
          </div>
        </div>

        <div
          class="mt-6 space-y-3 border-t border-slate-200 pt-6 dark:border-slate-700"
        >
          <div class="flex items-center justify-between text-sm">
            <span class="text-slate-500 dark:text-slate-400"
              >Ukupno proizvoda</span
            >
            <span class="font-medium text-slate-900 dark:text-white">{{
              totalItems
            }}</span>
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
          class="mt-6 inline-flex w-full items-center justify-center gap-2 rounded-full bg-blue-600 px-5 py-3 text-sm font-semibold text-white transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          <span>Potvrdi plaćanje</span>
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
              d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </button>
      </form>
    </div>
  </main>
</template>
