<script lang="ts" setup>
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { useCartStore } from "~/stores/cart";
import { useSessionStore } from "~/stores/session";
import logoUrl from "~/assets/brand/logo.svg?url";

const router = useRouter();
const route = useRoute();

const cart = useCartStore();
const session = useSessionStore();

const searchTerm = ref("");
const showMobileMenu = ref(false);
const showMobileSearch = ref(false);
const searchInput = ref<HTMLInputElement | null>(null);

const navLinks = [
  { label: "Proizvodi", to: "/products" },
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
    path: "/products",
    query: {
      productName: value,
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
    class="fixed top-0 z-50 w-full border-b border-slate-200 bg-white/90 backdrop-blur supports-[backdrop-filter]:bg-white/70 dark:border-slate-700 dark:bg-slate-900/70"
  >
    <div
      class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3 md:px-6"
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
        class="hidden items-center gap-6 text-sm font-medium text-slate-600 md:flex"
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
        <form class="hidden md:block" @submit.prevent="submitSearch">
          <label class="group relative flex items-center">
            <span
              class="pointer-events-none absolute left-3 text-slate-400 group-focus-within:text-blue-600"
            >
              <svg
                class="h-4 w-4"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                viewBox="0 0 24 24"
              >
                <circle
                  cx="11"
                  cy="11"
                  r="7"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
                <line
                  x1="16.65"
                  y1="16.65"
                  x2="21"
                  y2="21"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
              </svg>
            </span>
            <input
              v-model="searchTerm"
              type="search"
              placeholder="Pretraži proizvode"
              class="w-56 rounded-full border border-transparent bg-slate-100 py-2 pl-9 pr-4 text-sm text-slate-700 transition focus:border-blue-200 focus:bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 dark:bg-slate-800 dark:text-slate-200 dark:focus:bg-slate-900"
            />
          </label>
        </form>

        <button
          type="button"
          class="rounded-full p-2 text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-200 dark:text-slate-300 dark:hover:bg-slate-800 md:hidden"
          @click="toggleMobileSearch"
        >
          <span class="sr-only">Pretraga</span>
          <svg
            class="h-5 w-5"
            fill="none"
            stroke="currentColor"
            stroke-width="1.8"
            viewBox="0 0 24 24"
          >
            <circle
              cx="11"
              cy="11"
              r="7"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <line
              x1="16.65"
              y1="16.65"
              x2="21"
              y2="21"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </button>

        <button
          type="button"
          class="relative rounded-full p-2 text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-200 dark:text-slate-300 dark:hover:bg-slate-800"
          @click="goToCart"
        >
          <span class="sr-only">Korpa</span>
          <svg
            class="h-5 w-5"
            fill="none"
            stroke="currentColor"
            stroke-width="1.6"
            viewBox="0 0 24 24"
          >
            <g fill="none" stroke="currentColor" stroke-width="2">
              <path
                stroke-linecap="round"
                d="M8 12V8a4 4 0 0 1 4-4v0a4 4 0 0 1 4 4v4"
              />
              <path
                d="M3.694 12.668c.145-1.741.218-2.611.792-3.14S5.934 9 7.681 9h8.639c1.746 0 2.62 0 3.194.528s.647 1.399.792 3.14l.514 6.166c.084 1.013.126 1.52-.17 1.843c-.298.323-.806.323-1.824.323H5.174c-1.017 0-1.526 0-1.823-.323s-.255-.83-.17-1.843z"
              />
            </g>
          </svg>
          <span
            v-if="cartItemCount"
            class="absolute bottom-0 right-0 inline-flex items-center justify-center rounded-full bg-blue-600 px-1.5 text-xs font-semibold text-white"
          >
            {{ cartItemCount }}
          </span>
        </button>

        <NuxtLink
          v-if="!isLoggedIn"
          to="/login"
          class="hidden rounded-full border border-blue-200 px-4 py-2 text-sm font-medium text-blue-700 transition hover:border-blue-300 hover:bg-blue-50 dark:border-blue-800 dark:text-blue-200 dark:hover:bg-blue-900/30 md:inline-flex"
        >
          Prijava
        </NuxtLink>

        <div v-else class="hidden items-center gap-3 md:flex">
          <NuxtLink
            to="/orders"
            class="text-sm font-medium text-slate-600 transition hover:text-blue-600 dark:text-slate-300 dark:hover:text-blue-300"
          >
            Porudžbine
          </NuxtLink>
          <button
            type="button"
            class="rounded-full border border-slate-200 px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-200 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
            @click="logout"
          >
            Odjava
          </button>
        </div>

        <button
          type="button"
          class="rounded-full p-2 text-slate-600 transition hover:bg-slate-100 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-200 dark:text-slate-300 dark:hover:bg-slate-800 md:hidden"
          @click="toggleMobileMenu"
        >
          <span class="sr-only">Navigacija</span>
          <svg
            class="h-6 w-6"
            fill="none"
            stroke="currentColor"
            stroke-width="1.6"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M4 6h16M4 12h16M4 18h16"
            />
          </svg>
        </button>
      </div>
    </div>

    <transition name="fade">
      <div
        v-if="showMobileSearch"
        class="border-t border-slate-200 bg-white/95 px-4 py-3 dark:border-slate-700 dark:bg-slate-900 md:hidden"
      >
        <form @submit.prevent="submitSearch" class="flex items-center gap-2">
          <label
            class="flex flex-1 items-center gap-2 rounded-full bg-slate-100 px-4 py-2 dark:bg-slate-800"
          >
            <svg
              class="h-5 w-5 text-slate-500"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              viewBox="0 0 24 24"
            >
              <circle
                cx="11"
                cy="11"
                r="7"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <line
                x1="16.65"
                y1="16.65"
                x2="21"
                y2="21"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
            <input
              ref="searchInput"
              v-model="searchTerm"
              type="search"
              placeholder="Pretraži proizvode"
              class="flex-1 border-0 bg-transparent text-sm text-slate-700 placeholder-slate-400 focus:outline-none dark:text-slate-200"
            />
          </label>
          <button
            type="submit"
            class="rounded-full bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow hover:bg-blue-700"
          >
            Traži
          </button>
        </form>
      </div>
    </transition>

    <transition name="slide-fade">
      <div
        v-if="showMobileMenu"
        class="border-t border-slate-200 bg-white/95 px-4 py-4 dark:border-slate-700 dark:bg-slate-900 md:hidden"
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
