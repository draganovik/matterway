<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession";
import { useCart } from "~/composables/useCart";

const auth = useAuthSession();
const cart = useCart();
const route = useRoute();

const model = reactive({
  email: "",
  password: "",
});
const loading = ref(false);
const error = ref("");

function resolveNextRoute() {
  const next = route.query.next;
  if (typeof next === "string" && next.startsWith("/")) {
    return next;
  }
  return "/";
}

onMounted(async () => {
  await auth.initialize();
  if (auth.isLoggedIn.value && auth.isCustomer.value) {
    await navigateTo(resolveNextRoute());
  }
});

async function submit() {
  error.value = "";
  loading.value = true;
  try {
    await auth.login({
      email: model.email,
      password: model.password,
    });

    await cart.clear();
    await navigateTo(resolveNextRoute());
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Prijava nije uspela.";
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <div
    class="mx-auto grid max-w-6xl items-center gap-8 lg:grid-cols-[1.1fr_0.9fr]"
  >
    <div class="hidden space-y-4 lg:block">
      <p class="text-primary text-xs tracking-[0.4em] uppercase">Matterway</p>
      <h1 class="text-4xl font-semibold">Pristup za kupce</h1>
      <p class="text-muted max-w-md text-sm">
        Prijavite se kupčevim nalogom da biste kreirali porudžbine i pratili
        istoriju kupovine.
      </p>
    </div>

    <UCard class="border-default border shadow-lg">
      <template #header>
        <div class="space-y-1">
          <p class="text-primary text-xs tracking-[0.3em] uppercase">Prijava</p>
          <h2 class="text-2xl font-semibold">Dobro došli nazad</h2>
          <p class="text-muted text-sm">Unesite podatke kupčevog naloga.</p>
        </div>
      </template>

      <form class="space-y-4" @submit.prevent="submit">
        <UInput
          v-model="model.email"
          type="email"
          label="Imejl"
          autocomplete="email"
          required
        />
        <UInput
          v-model="model.password"
          type="password"
          label="Lozinka"
          autocomplete="current-password"
          required
        />
        <StatusMessages v-if="error" :error="error" />
        <div
          class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between"
        >
          <UButton type="submit" color="primary" :loading="loading"
            >Prijavi se</UButton
          >
          <NuxtLink to="/register" class="text-primary text-sm hover:underline"
            >Kreiraj nalog</NuxtLink
          >
        </div>
      </form>
    </UCard>
  </div>
</template>
