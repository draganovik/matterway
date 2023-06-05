<script setup lang="ts">
const router = useRouter();
const registerModel = ref({
  firstName: "",
  lastName: "",
  birthDate: new Date(new Date().setFullYear(new Date().getFullYear() - 18)),
  email: "",
  password: "",
  confirmPassword: "",
});

const submitFormRegister = async () => {
  if (registerModel.value.password !== registerModel.value.confirmPassword) {
    alert("Lozinke se ne poklapaju");
    return;
  }
  const response = await createUser(
    registerModel.value.firstName,
    registerModel.value.lastName,
    registerModel.value.birthDate,
    registerModel.value.email,
    registerModel.value.password,
  );
  if (response.ok) {
    router.push("/");
  }
  console.log(registerModel);
};

useHead({
  title: "Registracija",
});
definePageMeta({
  middleware: "auth",
  authNoSession: true,
});
</script>

<template>
  <form
    @submit.prevent="submitFormRegister"
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
        v-model="registerModel.firstName"
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
        v-model="registerModel.lastName"
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
        v-model="registerModel.email"
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
        v-model="registerModel.password"
        class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        placeholder="•••••••••"
        required
      />
    </div>
    <div class="mb-6">
      <label
        for="confirmPassword"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >Lozinka (potvrda)</label
      >
      <input
        type="password"
        id="confirmPassword"
        autocorrect="off"
        v-model="registerModel.confirmPassword"
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
        Registrujte se se
      </button>

      <NuxtLink
        to="/login"
        class="grid place-content-center p-2 font-medium text-blue-600 hover:underline dark:text-blue-500"
        >Prijava</NuxtLink
      >
    </div>
  </form>
</template>

<style scoped></style>
