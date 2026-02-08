<script lang="ts" setup>
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { useCartStore } from "@stores/cart";
import { useSessionStore } from "@stores/session";
import logoUrl from "@assets/brand/matterway-logo-blue.svg?url";

const router = useRouter();
const route = useRoute();

const cart = useCartStore();
const session = useSessionStore();

const searchTerm = ref("");
const showMobileMenu = ref(false);
const showMobileSearch = ref(false);
const searchInput = ref<HTMLInputElement | null>(null);

const navLinks = [
  { label: "Proizvodi", to: "/articles" },
  { label: "Kolekcije", to: "/#kolekcije" },
  { label: "Najnovije", to: "/#najnovije" },
];

const cartItemCount = computed(() => cart.getTotalItemCount);
const isLoggedIn = computed(() => session.isLoggedIn);

const isActiveLink = (target: string) => {
  const [path, hash] = target.split("#");
  if (hash) {
    return route.path === (path || "/") && route.hash === `#${hash}`;
  }

  return route.path === target || route.path.startsWith(`${target}/`);
};

const submitSearch = () => {
  const value = (searchTerm.value ?? "").toString().trim();
  if (!value) return;

  router.push({
    path: "/articles",
    query: {
      articleName: value,
    },
  });

  searchTerm.value = "";
  showMobileSearch.value = false;
  showMobileMenu.value = false;
};

const toggleMobileMenu = () => {
  showMobileMenu.value = !showMobileMenu.value;
  if (!showMobileMenu.value) {
    showMobileSearch.value = false;
  }
};

const toggleMobileSearch = () => {
  showMobileSearch.value = !showMobileSearch.value;
  if (showMobileSearch.value) {
    showMobileMenu.value = true;
  }
};

const goToCart = () => {
  router.push("/cart");
};

const logout = () => {
  session.logout();
  router.push("/");
  showMobileMenu.value = false;
};

watch(showMobileSearch, (visible) => {
  if (visible) {
    nextTick(() => searchInput.value?.focus());
  }
});

watch(
  () => route.fullPath,
  () => {
    showMobileMenu.value = false;
    showMobileSearch.value = false;
  },
);

onMounted(() => {
  if (session.isLoggedIn) {
    cart.fetchCartItems();
  }
});
</script>

<template>
  <header
    class="fixed top-0 z-50 w-full border-b border-slate-200 bg-white/90 backdrop-blur-sm supports-backdrop-filter:bg-white/70 dark:border-slate-700 dark:bg-slate-900/70"
  >
    <div
      class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3 lg:px-6"
    >
      <NuxtLink
        to="/"
        class="flex items-center gap-3 transition hover:opacity-90"
      >
        <img :src="logoUrl" alt="Matterway logo" class="h-9 w-9" />
        <span class="text-xl font-semibold text-slate-900 dark:text-white"
          >Matterway</span
        >
      </NuxtLink>

      <nav
        class="hidden items-center gap-6 text-sm font-medium text-slate-600 lg:flex"
      >
        <NuxtLink
          v-for="link in navLinks"
          :key="link.to"
          :to="link.to"
          class="rounded-full px-3 py-2 transition hover:text-blue-600 dark:hover:text-blue-300"
          :class="
            isActiveLink(link.to)
              ? 'bg-blue-50 text-blue-700 dark:bg-blue-900/40 dark:text-blue-200'
              : ''
          "
        >
          {{ link.label }}
        </NuxtLink>
      </nav>

      <div class="flex items-center gap-2">
        <form class="hidden lg:block" @submit.prevent="submitSearch">
          <label class="group relative flex items-center">
            <span
              class="pointer-events-none mt-1 absolute left-3 text-slate-400 group-focus-within:text-blue-600"
            >
              <Icon
                name="heroicons-outline:magnifying-glass"
                class="text-base"
                aria-hidden="true"
              />
            </span>
            <input
              v-model="searchTerm"
              type="search"
              placeholder="Pretraži proizvode"
              class="w-56 rounded-full border border-transparent bg-slate-100 py-2 pl-9 pr-4 text-sm text-slate-700 transition focus:border-blue-200 focus:bg-white focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:bg-slate-800 dark:text-slate-200 dark:focus:bg-slate-900"
            />
          </label>
        </form>

        <button
          type="button"
          class="rounded-full py-1 px-3 text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-hidden focus:ring-2 focus:ring-blue-200 dark:text-slate-300 dark:hover:bg-slate-800 lg:hidden"
          @click="toggleMobileSearch"
        >
          <span class="sr-only">Pretraga</span>
          <Icon
            name="heroicons-outline:magnifying-glass"
            class="text-xl mt-1"
            aria-hidden="true"
          />
        </button>

        <button
          type="button"
          class="relative rounded-full py-1 px-3 text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-hidden focus:ring-2 focus:ring-blue-200 dark:text-slate-300 dark:hover:bg-slate-800"
          @click="goToCart"
        >
          <span class="sr-only">Korpa</span>
          <Icon
            name="heroicons-outline:shopping-bag"
            class="text-xl mt-1"
            aria-hidden="true"
          />
          <span
            v-if="cartItemCount"
            class="absolute size-1.5 top-1 right-2.5 inline-flex items-center justify-center rounded-full bg-blue-600 text-xs font-semibold text-white"
          />
        </button>

        <NuxtLink
          v-if="!isLoggedIn"
          to="/login"
          class="hidden rounded-full border border-blue-200 px-4 py-2 text-sm font-medium text-blue-700 transition hover:border-blue-300 hover:bg-blue-50 dark:border-blue-800 dark:text-blue-200 dark:hover:bg-blue-900/30 lg:inline-flex"
        >
          Prijava
        </NuxtLink>

        <div v-else class="hidden items-center gap-3 lg:flex">
          <NuxtLink
            to="/orders"
            class="text-sm font-medium text-slate-600 transition hover:text-blue-600 dark:text-slate-300 dark:hover:text-blue-300"
          >
            Porudžbine
          </NuxtLink>
          <button
            type="button"
            class="rounded-full border border-slate-200 px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-hidden focus:ring-2 focus:ring-blue-200 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
            @click="logout"
          >
            Odjava
          </button>
        </div>

        <button
          type="button"
          class="rounded-full py-0.5 px-2.5 text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-hidden focus:ring-2 focus:ring-blue-200 dark:text-slate-300 dark:hover:bg-slate-800 lg:hidden"
          @click="toggleMobileMenu"
        >
          <span class="sr-only">Navigacija</span>
          <Icon
            name="heroicons-outline:bars-3"
            class="text-2xl mt-1"
            aria-hidden="true"
          />
        </button>
      </div>
    </div>

    <transition name="fade">
      <div
        v-if="showMobileSearch"
        class="border-t border-slate-200 bg-white/95 px-4 py-3 dark:border-slate-700 dark:bg-slate-900 lg:hidden"
      >
        <form @submit.prevent="submitSearch" class="flex items-center gap-2">
          <label
            class="flex flex-1 items-center gap-2 rounded-full bg-slate-100 px-4 py-2 dark:bg-slate-800"
          >
            <Icon
              name="heroicons-outline:magnifying-glass"
              class="text-xl text-slate-500"
              aria-hidden="true"
            />
            <input
              ref="searchInput"
              v-model="searchTerm"
              type="search"
              placeholder="Pretraži proizvode"
              class="flex-1 border-0 bg-transparent text-sm text-slate-700 placeholder-slate-400 focus:outline-hidden dark:text-slate-200"
            />
          </label>
          <button
            type="submit"
            class="rounded-full bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700"
          >
            Traži
          </button>
        </form>
      </div>
    </transition>

    <transition name="slide-fade">
      <div
        v-if="showMobileMenu"
        class="border-t border-slate-200 bg-white/95 px-4 py-4 dark:border-slate-700 dark:bg-slate-900 lg:hidden"
      >
        <nav class="flex flex-col gap-2">
          <NuxtLink
            v-for="link in navLinks"
            :key="link.to"
            :to="link.to"
            class="rounded-lg px-3 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 dark:text-slate-300 dark:hover:bg-slate-800 dark:hover:text-blue-300"
            :class="
              isActiveLink(link.to)
                ? 'bg-blue-50 text-blue-700 dark:bg-blue-900/40 dark:text-blue-200'
                : ''
            "
            @click="showMobileMenu = false"
          >
            {{ link.label }}
          </NuxtLink>
        </nav>
        <div class="mt-4 flex flex-col gap-2">
          <NuxtLink
            to="/cart"
            class="flex items-center justify-between rounded-lg bg-slate-100 px-3 py-2 text-sm font-medium text-slate-600 dark:bg-slate-800 dark:text-slate-200"
            @click="showMobileMenu = false"
          >
            <span>Korpa</span>
            <span
              class="text-xs font-semibold text-blue-600 dark:text-blue-300"
            >
              {{ cartItemCount }}
            </span>
          </NuxtLink>
          <NuxtLink
            v-if="isLoggedIn"
            to="/orders"
            class="rounded-lg px-3 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100 dark:text-slate-200 dark:hover:bg-slate-800"
            @click="showMobileMenu = false"
          >
            Porudžbine
          </NuxtLink>
          <button
            v-if="isLoggedIn"
            type="button"
            class="rounded-lg bg-red-600 px-3 py-2 text-sm font-medium text-white hover:bg-red-700"
            @click="logout"
          >
            Odjava
          </button>
          <NuxtLink
            v-else
            to="/login"
            class="rounded-lg bg-blue-600 px-3 py-2 text-center text-sm font-medium text-white hover:bg-blue-700"
            @click="showMobileMenu = false"
          >
            Prijava
          </NuxtLink>
        </div>
      </div>
    </transition>
  </header>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
.slide-fade-enter-active,
.slide-fade-leave-active {
  transition: all 0.2s ease;
}
.slide-fade-enter-from,
.slide-fade-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
