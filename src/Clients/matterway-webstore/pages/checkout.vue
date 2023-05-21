<script lang="ts" setup>
import { useSessionStore } from "~/store/session";

const router = useRouter();
const session = useSessionStore();

const paymentData = ref({
  cardNumber: "4242424242424242",
  expMonth: 12,
  expYear: 2024,
  cvc: "123",
  amount: 100,
});

const pay = async () => {
  const val = useFetch("/api/payments", {
    method: "POST",
    body: JSON.stringify(paymentData.value),
  });
  console.log(await val);
};

useHead({
  title: "Kupovina",
});
definePageMeta({
  //middleware: "auth",
  //authOnlyRoles: [UserRole.Customer],
});
</script>

<template>
  <section class="grid place-items-center">
    <form @submit.prevent="pay()" class="w-80">
      <div class="mb-6">
        <label
          for="cardNumber"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Card Number:</label
        >
        <input
          type="text"
          id="cardNumber"
          v-model="paymentData.cardNumber"
          required
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="expMonth"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Expiration Month:</label
        >
        <input
          type="text"
          id="expMonth"
          v-model="paymentData.expMonth"
          required
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="expYear"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Expiration Year:</label
        >
        <input
          type="text"
          id="expYear"
          v-model="paymentData.expYear"
          required
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="cvc"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >CVC:</label
        >
        <input
          type="text"
          id="cvc"
          v-model="paymentData.cvc"
          required
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <div class="mb-6">
        <label
          for="amount"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Amount:</label
        >
        <input
          type="text"
          id="amount"
          v-model="paymentData.amount"
          required
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 shadow-sm focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:shadow-sm-light dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>
      <button
        type="submit"
        class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
      >
        Plati
      </button>
    </form>
  </section>
</template>

<style scoped></style>
