<script lang="ts" setup>
const formModel = defineModel<{
  firstName: string;
  lastName: string;
  birthDate: Date;
  email: string;
  password: string;
  confirmPassword: string;
}>({ required: true });

const props = withDefaults(
  defineProps<{
    loading?: boolean;
    errorMessage?: string | null;
  }>(),
  {
    loading: false,
    errorMessage: null,
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
      />
    </svg>
    <h1 class="mb-8 text-center text-3xl font-bold">
      Registrujte se na Matterway
    </h1>
    <div class="mb-6">
      <label
        for="firstName"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >Ime</label
      >
      <input
        type="text"
        id="firstName"
        autocomplete="given-name"
        class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="Vaše ime"
        v-model="formModel.firstName"
        required
      />
    </div>
    <div class="mb-6">
      <label
        for="lastName"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >Prezime</label
      >
      <input
        type="text"
        id="lastName"
        autocomplete="family-name"
        class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="Vaše prezime"
        v-model="formModel.lastName"
        required
      />
    </div>
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
    <div class="mb-6 grid gap-6 md:grid-cols-2">
      <div>
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
      <div>
        <label
          for="confirmPassword"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
          >Lozinka (potvrda)</label
        >
        <input
          type="password"
          id="confirmPassword"
          autocorrect="off"
          v-model="formModel.confirmPassword"
          class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
          placeholder="•••••••••"
          required
        />
      </div>
    </div>
    <p
      v-if="errorMessage"
      class="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-2 text-sm text-red-700 dark:border-red-900/60 dark:bg-red-500/10 dark:text-red-200"
    >
      {{ errorMessage }}
    </p>
    <div class="flex flex-col justify-between gap-4 md:flex-row">
      <button
        type="submit"
        :disabled="loading"
        class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white transition hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 disabled:cursor-not-allowed disabled:opacity-70 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800 sm:w-auto"
      >
        {{ loading ? "Registracija..." : "Registrujte se" }}
      </button>

      <NuxtLink
        to="/login"
        class="grid place-content-center p-2 font-medium text-blue-600 hover:underline dark:text-blue-500"
      >
        Prijava
      </NuxtLink>
    </div>
  </form>
</template>
