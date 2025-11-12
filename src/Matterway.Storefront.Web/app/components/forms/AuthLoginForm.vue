<script lang="ts" setup>
import type LoginModel from "#models/LoginModel";

const formModel = defineModel<LoginModel>({ required: true });

const props = withDefaults(
  defineProps<{
    loading?: boolean;
  }>(),
  {
    loading: false,
  },
);

const emit = defineEmits<{
  (e: "submit"): void;
}>();

const handleSubmit = () => {
  emit("submit");
};
</script>

<template>
  <form
    @submit.prevent="handleSubmit"
    class="w-full flex flex-col py-4 max-w-md place-self-center self-center"
  >
    <Icon
      name="heroicons-outline:user-circle"
      class="self-center mb-6 text-8xl text-blue-500"
      aria-hidden="true"
    />
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
        v-model="formModel.email"
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
        v-model="formModel.password"
        class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="•••••••••"
        required
      />
    </div>
    <div class="flex flex-col justify-between gap-4 md:flex-row">
      <button
        type="submit"
        :disabled="loading"
        class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white transition hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 disabled:cursor-not-allowed disabled:opacity-70 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800 sm:w-auto"
      >
        {{ loading ? "Prijavljivanje..." : "Ulogujte se" }}
      </button>

      <NuxtLink
        to="/register"
        class="grid place-content-center p-2 font-medium text-blue-600 hover:underline dark:text-blue-500"
      >
        Registracija
      </NuxtLink>
    </div>
  </form>
</template>
