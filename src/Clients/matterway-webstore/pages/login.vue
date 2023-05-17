<script lang="ts" setup>
import { randomInt } from "crypto";
import { ref } from "vue";
import { useSessionStore } from "~/store/session";

const loginModel = ref(new LoginModel());

const sessionStore = useSessionStore();

const isSubmitting = ref(false);

async function submitFormLogin() {
  if(sessionStore.session != null) {
    console.log(sessionStore.session)
    return
  }
  try {
    isSubmitting.value = true;
    if(loginModel.value.validate())
    {
      sessionStore.login(loginModel.value);
    }
  } catch (error) {
    // Handle any errors that occur during form submission
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<template>
  <form
    @submit.prevent="submitFormLogin"
    class="w-full max-w-md place-self-center self-center"
  >
    <div class="mb-6">
      <label
        for="email"
        class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
        >Email adresa</label
      >
      <input
        type="email"
        id="email"
        v-model="loginModel.email"
        class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="john.doe@company.com"
        required
      />
    </div>
    <div class="mb-6">
      <label
        for="password"
        class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
        >Lozinka</label
      >
      <input
        type="password"
        id="password"
        v-model="loginModel.password"
        class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="•••••••••"
        required
      />
    </div>
    <div class="flex justify-between">
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
