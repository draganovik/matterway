<script lang="ts" setup>
import { useCartStore } from "~/store/cart";
import { useSessionStore } from "~/store/session";
import AddressModel from "~/utils/AddressModel";
import CardPaymentModel from "~/utils/CardPaymentModel";

const cart = useCartStore();
const session = useSessionStore();

const paymentData: Ref<CardPaymentModel> = ref(
  new CardPaymentModel("4242424242424242", 12, 2024, "123", cart.getTotalPrice),
);

const addressData: Ref<AddressModel> = ref(
  new AddressModel("", "", "", "", "", ""),
);

const pay = async () => {
  const response = await fetch("/api/v1.0/payments", {
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

const tableSummary = {
  title: "Vaši proizvodi",
  description: "Proizvodi koje ste dodali za kupovinu",
  totalCount: cart.getTotalItemCount,
  totalPrice: cart.getTotalPrice,
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
  <section class="flex grid-cols-3 flex-col gap-4 md:grid">
    <section class="col-span-2">
      <ProductTable :products="cart.getCartItems" :summary="tableSummary" />
    </section>
    <form @submit.prevent="pay()" class="rounded-lg bg-slate-800 p-4">
      <svg
        class="mx-auto my-10 h-24 text-blue-500"
        fill="none"
        stroke="currentColor"
        stroke-width="1.5"
        viewBox="0 0 24 24"
        xmlns="http://www.w3.org/2000/svg"
        aria-hidden="true"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 002.25-2.25V6.75A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25v10.5A2.25 2.25 0 004.5 19.5z"
        ></path>
      </svg>
      <div class="mb-6">
        <label
          for="name"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >Ime primaoca</label
        >
        <input
          type="text"
          id="name"
          v-model="addressData.receiverName"
          placeholder="Ime i prezime"
          required
          class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="city"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >Grad</label
        >
        <input
          type="text"
          id="city"
          autocomplete="address-level2"
          v-model="addressData.city"
          placeholder="Naziv grada"
          required
          class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="postalCode"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >Poštanski broj</label
        >
        <input
          type="text"
          id="postalCode"
          v-model="addressData.zipCode"
          placeholder="00000"
          required
          class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="street"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >Ulica</label
        >
        <div class="flex gap-2">
          <input
            type="text"
            id="streetNumber"
            autocomplete="street-address"
            v-model="addressData.street"
            placeholder="Naziv ulice"
            required
            class="block w-3/5 rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
          <input
            type="text"
            id="streetNumber"
            v-model="addressData.residence"
            placeholder="Broj"
            required
            class="block w-2/5 rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
      </div>
      <div class="mb-6">
        <label
          for="cardNumber"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >Broj platne kartice:</label
        >
        <input
          type="text"
          id="cardNumber"
          v-model="paymentData.cardNumber"
          required
          class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="flex gap-4">
        <div class="mb-6">
          <label
            for="expMonth"
            class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
            >Mesec isteka kartice:</label
          >
          <input
            type="text"
            id="expMonth"
            v-model="paymentData.expMonth"
            required
            class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
        <div class="mb-6">
          <label
            for="expYear"
            class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
            >Godina isteka kartice:</label
          >
          <input
            type="text"
            id="expYear"
            v-model="paymentData.expYear"
            required
            class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
      </div>
      <div class="mb-6">
        <label
          for="cvc"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >CVC Kod:</label
        >
        <input
          type="text"
          id="cvc"
          maxlength="4"
          v-model="paymentData.cvc"
          pattern="[0-9]{3,4}"
          required
          class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <button
        type="submit"
        class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
      >
        Plati {{ formatMoney(cart.getTotalPrice) }}
      </button>
    </form>
  </section>
</template>

<style scoped></style>
