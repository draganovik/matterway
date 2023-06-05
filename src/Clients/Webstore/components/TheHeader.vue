<script lang="ts" setup>
import { initFlowbite } from "flowbite";
import { useCartStore } from "~/store/cart";
import { useSessionStore } from "~/store/session";

const router = useRouter();

const cart = useCartStore();
const session = useSessionStore();

const logout = () => {
  session.logout();
  router.push("/");
};

const search = () => {
  if (searchTerm.value == undefined || searchTerm.value == "") {
    return;
  }
  router.push({
    path: "/products",
    query: {
      productName: searchTerm.value,
    },
  });
  searchTerm.value = undefined;
};

let searchTerm = ref(undefined);

// initialize components based on data attribute selectors
onMounted(() => {
  initFlowbite();
  if (session.isLoggedIn) {
    cart.fetchCartItems();
  }
});
</script>

<template>
  <nav
    class="fixed z-40 m-4 w-[calc(100%-2rem)] rounded-lg border border-slate-200/90 bg-white backdrop-blur-md backdrop-filter dark:border-slate-700 dark:bg-slate-800/90"
  >
    <div
      class="mx-auto flex max-w-screen-xl flex-wrap items-center justify-between p-4"
    >
      <NuxtLink to="/" class="flex items-center">
        <img
          src="../assets/brand/logo.svg"
          class="mr-3 h-9"
          alt="Matterway Logo"
        />
        <span
          class="self-center whitespace-nowrap text-2xl font-semibold dark:text-white"
          >Matterway</span
        >
      </NuxtLink>
      <div class="flex md:order-1">
        <button
          type="button"
          data-collapse-toggle="navbar-search"
          aria-controls="navbar-search"
          aria-expanded="false"
          class="mr-1 rounded-lg p-2.5 text-sm text-slate-500 hover:bg-slate-100 focus:outline-none focus:ring-4 focus:ring-slate-200 dark:text-slate-400 dark:hover:bg-slate-700 dark:focus:ring-slate-700 md:hidden"
        >
          <svg
            class="h-5 w-5"
            aria-hidden="true"
            fill="currentColor"
            viewBox="0 0 20 20"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              fill-rule="evenodd"
              d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z"
              clip-rule="evenodd"
            ></path>
          </svg>
          <span class="sr-only">Search</span>
        </button>
        <div class="w-58 relative hidden md:block lg:w-64">
          <div
            class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3"
          >
            <svg
              class="h-5 w-5 text-slate-500"
              aria-hidden="true"
              fill="currentColor"
              viewBox="0 0 20 20"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                fill-rule="evenodd"
                d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z"
                clip-rule="evenodd"
              ></path>
            </svg>
            <span class="sr-only">Search icon</span>
          </div>
          <input
            v-model="searchTerm"
            v-on:keyup.enter="search()"
            type="text"
            id="search-navbar"
            class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2 pl-10 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
            placeholder="Search..."
          />
        </div>
        <button
          data-collapse-toggle="navbar-search"
          type="button"
          class="inline-flex items-center rounded-lg p-2 text-sm text-slate-500 hover:bg-slate-100 focus:outline-none focus:ring-2 focus:ring-slate-200 dark:text-slate-400 dark:hover:bg-slate-700 dark:focus:ring-slate-600 md:hidden"
          aria-controls="navbar-search"
          aria-expanded="false"
        >
          <span class="sr-only">Open menu</span>
          <svg
            class="h-6 w-6"
            aria-hidden="true"
            fill="currentColor"
            viewBox="0 0 20 20"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              fill-rule="evenodd"
              d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
              clip-rule="evenodd"
            ></path>
          </svg>
        </button>
      </div>
      <div
        class="hidden w-full items-center justify-between md:order-2 md:flex md:w-auto"
        id="navbar-search"
      >
        <div class="relative mt-3 md:hidden">
          <div
            class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3"
          >
            <svg
              class="h-5 w-5 text-slate-500"
              aria-hidden="true"
              fill="currentColor"
              viewBox="0 0 20 20"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                fill-rule="evenodd"
                d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z"
                clip-rule="evenodd"
              ></path>
            </svg>
          </div>
          <input
            v-model="searchTerm"
            v-on:keyup.enter="search()"
            type="text"
            id="search-navbar-mini"
            class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2 pl-10 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
            placeholder="Search..."
          />
        </div>
        <ul
          class="mt-4 flex flex-col rounded-lg border border-slate-100 bg-slate-50 p-4 font-medium dark:border-slate-700 dark:bg-slate-800 md:mt-0 md:flex-row md:items-center md:gap-5 md:border-0 md:bg-transparent md:p-0 lg:gap-8"
        >
          <li>
            <NuxtLink
              to="/"
              class="block rounded py-2 pl-3 pr-4 text-slate-900 hover:bg-slate-100 dark:border-slate-700 dark:text-white dark:hover:bg-slate-700 dark:hover:text-white md:p-0 md:hover:bg-transparent md:hover:text-blue-700 md:dark:hover:bg-transparent md:dark:hover:text-blue-500"
              >Naslovna</NuxtLink
            >
          </li>
          <li>
            <NuxtLink
              to="/products"
              class="block rounded py-2 pl-3 pr-4 text-slate-900 hover:bg-slate-100 dark:border-slate-700 dark:text-white dark:hover:bg-slate-700 dark:hover:text-white md:p-0 md:hover:bg-transparent md:hover:text-blue-700 md:dark:hover:bg-transparent md:dark:hover:text-blue-500"
              >Proizvodi</NuxtLink
            >
          </li>
          <li v-if="!session.isLoggedIn">
            <NuxtLink
              to="/login"
              class="block rounded py-2 pl-3 pr-4 text-slate-900 hover:bg-slate-100 dark:border-slate-700 dark:text-white dark:hover:bg-slate-700 dark:hover:text-white md:p-0 md:hover:bg-transparent md:hover:text-blue-700 md:dark:hover:bg-transparent md:dark:hover:text-blue-500"
              >Prijava</NuxtLink
            >
          </li>
          <div class="flex flex-col md:flex-row">
            <li v-show="session.isLoggedIn">
              <button
                type="button"
                data-dropdown-toggle="dropdownNavbar"
                aria-expanded="false"
                class="relative mr-1 flex w-full flex-row gap-2 rounded p-2.5 text-slate-500 hover:bg-slate-100 focus:outline-none focus:ring-4 focus:ring-slate-200 dark:text-slate-400 dark:hover:bg-slate-700 dark:focus:ring-slate-700 md:w-auto"
              >
                <svg
                  class="h-5 w-5"
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
                    d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z"
                  ></path>
                </svg>
                <span class="text-slate-900 dark:text-white md:sr-only"
                  >Nalog</span
                >
              </button>
              <!-- Dropdown menu -->
              <div
                id="dropdownNavbar"
                class="z-10 hidden w-44 divide-y divide-slate-100 overflow-hidden rounded-lg bg-white font-normal shadow dark:divide-slate-600 dark:bg-slate-700"
              >
                <ul
                  class="py-2 text-sm text-slate-700 dark:text-slate-400"
                  aria-labelledby="dropdownLargeButton"
                >
                  <li>
                    <NuxtLink
                      to="/profile"
                      class="block px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-600 dark:hover:text-white"
                      >Profil</NuxtLink
                    >
                  </li>
                  <li>
                    <NuxtLink
                      to="/orders"
                      class="block px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-600 dark:hover:text-white"
                      >Istorija kupovine</NuxtLink
                    >
                  </li>
                </ul>
                <div class="py-1">
                  <button
                    type="button"
                    aria-controls="navbar-search"
                    aria-expanded="true"
                    @click="logout()"
                    class="block w-full px-4 py-2 text-left text-sm text-slate-700 hover:bg-slate-100 dark:text-slate-400 dark:hover:bg-slate-600 dark:hover:text-white"
                  >
                    Odjava
                  </button>
                </div>
              </div>
            </li>
            <li>
              <NuxtLink
                v-if="
                  session.getTokenData?.role != 'Admin' &&
                  session.getTokenData?.role != 'Manager'
                "
                to="/cart"
                data-collapse-toggle="navbar-search"
                aria-controls="navbar-search"
                aria-expanded="false"
                class="relative mr-1 flex w-full flex-row gap-2 rounded p-2.5 text-slate-500 hover:bg-slate-100 focus:outline-none focus:ring-4 focus:ring-slate-200 dark:text-slate-400 dark:hover:bg-slate-700 dark:focus:ring-slate-700 md:w-auto"
              >
                <svg
                  class="h-5 w-5"
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
                    d="M15.75 10.5V6a3.75 3.75 0 10-7.5 0v4.5m11.356-1.993l1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 01-1.12-1.243l1.264-12A1.125 1.125 0 015.513 7.5h12.974c.576 0 1.059.435 1.119 1.007zM8.625 10.5a.375.375 0 11-.75 0 .375.375 0 01.75 0zm7.5 0a.375.375 0 11-.75 0 .375.375 0 01.75 0z"
                  ></path>
                </svg>
                <span
                  class="text-base text-slate-900 dark:text-white md:sr-only"
                  >Korpa</span
                >
                <div
                  v-if="cart.getCartItems.length > 0"
                  class="absolute right-2 inline-flex h-5 w-5 items-center justify-center rounded-md border border-blue-400/40 border-white bg-blue-600/70 text-[8pt] font-bold text-white dark:border-blue-700/50 md:bottom-0 md:right-0"
                >
                  {{ cart.getCartItems.length }}
                </div>
              </NuxtLink>
            </li>
          </div>
        </ul>
      </div>
    </div>
  </nav>
</template>

<style scoped>
.router-link-exact-active:not([href="/"]):not([href="/cart"]) {
  @apply bg-blue-700 md:bg-transparent md:text-blue-700 md:dark:text-blue-500;
}
.router-link-exact-active[href="/cart"] {
  @apply bg-blue-700 md:bg-transparent;
}
</style>
