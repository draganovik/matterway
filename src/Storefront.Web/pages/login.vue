<script lang="ts" setup>
import { ref } from "vue";
import { useSessionStore } from "~/stores/session";
import LoginModel from "~/models/LoginModel";

useHead({
  title: "Prijava",
});

definePageMeta({
  middleware: "auth",
  authNoSession: true,
});

const loginModel = ref(new LoginModel());

const sessionStore = useSessionStore();

const isSubmitting = ref(false);

async function submitFormLogin() {
  if (sessionStore.session != null) {
    console.log(sessionStore.session);
    return;
  }
  try {
    isSubmitting.value = true;
    if (loginModel.value.validate()) {
      await sessionStore.login(loginModel.value);
    }
  } catch (error) {
    // Handle any errors that occur during form submission
  } finally {
    isSubmitting.value = false;
    if (sessionStore.isLoggedIn) {
      navigateTo("/");
    }
  }
}
</script>

<template>
  <form
    @submit.prevent="submitFormLogin"
    class="w-full max-w-md place-self-center self-center"
  >
    <svg
      class="mx-auto mb-6 h-20 w-20 text-blue-500"
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
        d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z"
      ></path>
    </svg>
    <h1 class="mb-8 text-center text-3xl font-bold">
      Prijavite se na Matterway
    </h1>
    <div class="mb-6">
      <label
        for="email"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >Email adresa</label
      >
      <input
        type="email"
        id="email"
        v-model="loginModel.email"
        class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="john.doe@company.com"
        required
      />
    </div>
    <div class="mb-6">
      <label
        for="password"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >Lozinka</label
      >
      <input
        type="password"
        id="password"
        v-model="loginModel.password"
        class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="•••••••••"
        required
      />
    </div>
    <div class="flex flex-col justify-between gap-4 md:flex-row">
      <button
        type="submit"
        class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800 sm:w-auto"
      >
        Ulogujte se
      </button>

      <NuxtLink
        to="/register"
        class="grid place-content-center p-2 font-medium text-blue-600 hover:underline dark:text-blue-500"
        >Registracija</NuxtLink
      >
    </div>
  </form>
</template>

<style scoped></style>
