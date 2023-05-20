<script lang="ts" setup>
import { useCatalogStore } from "~/store/catalog";

const route = useRoute();
const catalogStore = useCatalogStore();
watch(
  () => route.query,
  () => refresh(),
);
const refresh = () => {
  catalogStore.fetchCatalog(
    (route.query.page || 1) as number,
    (route.query.pageSize || 3) as number,
  );
};
refresh();
useHead({
  title: "Proizvodi",
});
</script>

<template>
  <div class="flex flex-col gap-6 md:flex-row md:gap-4">
    <aside>
      <form>
        <div class="mb-6 grid gap-6">
          <div>
            <label
              for="company"
              class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >Company</label
            >
            <input
              type="text"
              id="company"
              class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
              placeholder="Flowbite"
              required
            />
          </div>
          <div>
            <label
              for="phone"
              class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >Phone number</label
            >
            <input
              type="tel"
              id="phone"
              class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
              placeholder="123-45-678"
              pattern="[0-9]{3}-[0-9]{2}-[0-9]{3}"
              required
            />
          </div>
          <div>
            <label
              for="visitors"
              class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >Unique visitors (per month)</label
            >
            <input
              type="number"
              id="visitors"
              class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
              placeholder=""
              required
            />
          </div>
        </div>
        <div class="mb-6 flex items-start">
          <div class="flex h-5 items-center">
            <input
              id="remember"
              type="checkbox"
              value=""
              class="focus:ring-3 h-4 w-4 rounded border border-slate-300 bg-slate-50 focus:ring-blue-300 dark:border-slate-600 dark:bg-slate-700 dark:ring-offset-slate-800 dark:focus:ring-blue-600"
              required
            />
          </div>
          <label
            for="remember"
            class="ml-2 text-sm font-medium text-slate-900 dark:text-slate-300"
            >I agree with the
            <a href="#" class="text-blue-600 hover:underline dark:text-blue-500"
              >terms and conditions</a
            >.</label
          >
        </div>
        <button
          type="submit"
          class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800 sm:w-auto"
        >
          Submit
        </button>
      </form>
    </aside>
    <section class="grid w-full place-items-center gap-6">
      <div class="grid w-full grid-cols-2 gap-4 lg:grid-cols-3">
        <ProductCartSkeleton
          v-if="catalogStore.catalog == null"
          v-for="product in [...Array(3).keys()]"
          :key="product"
        />
        <ProductCard
          v-for="product in catalogStore.catalog"
          :key="product.id"
          :product="product"
        />
      </div>

      <nav aria-label="Catalog pagination">
        <ul class="inline-flex items-center -space-x-px">
          <li>
            <NuxtLink
              :to="{
                path: '/products',
                query: {
                  page: 1,
                  pageSize: catalogStore.getCatalogMeta?.pageSize || 3,
                },
              }"
              class="ml-0 block rounded-l-lg border border-slate-300 bg-white px-3 py-2 leading-tight text-slate-500 hover:bg-slate-100 hover:text-slate-700 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-400 dark:hover:bg-slate-700 dark:hover:text-white"
            >
              <span class="sr-only">Previous</span>
              <svg
                aria-hidden="true"
                class="h-5 w-5"
                fill="currentColor"
                viewBox="0 0 20 20"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  fill-rule="evenodd"
                  d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z"
                  clip-rule="evenodd"
                ></path>
              </svg>
            </NuxtLink>
          </li>
          <li
            v-for="page in [
              ...Array(catalogStore.getCatalogMeta?.totalPages || 1).keys(),
            ]"
          >
            <NuxtLink
              :to="{
                path: '/products',
                query: {
                  page: page + 1,
                  pageSize: catalogStore.getCatalogMeta?.pageSize || 3,
                },
              }"
              :current-page="page + 1 == route.query.page?.valueOf() ? true : false"
              class="border border-slate-300 bg-white px-3 py-2 leading-tight text-slate-500 hover:bg-slate-100 hover:text-slate-700 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-400 dark:hover:bg-slate-700 dark:hover:text-white"
              >{{ page + 1 }}</NuxtLink
            >
          </li>
          <li>
            <NuxtLink
              :to="{
                path: '/products',
                query: {
                  page: catalogStore.getCatalogMeta?.totalPages,
                  pageSize: catalogStore.getCatalogMeta?.pageSize || 3,
                },
              }"
              class="block rounded-r-lg border border-slate-300 bg-white px-3 py-2 leading-tight text-slate-500 hover:bg-slate-100 hover:text-slate-700 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-400 dark:hover:bg-slate-700 dark:hover:text-white"
            >
              <span class="sr-only">Next</span>
              <svg
                aria-hidden="true"
                class="h-5 w-5"
                fill="currentColor"
                viewBox="0 0 20 20"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  fill-rule="evenodd"
                  d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z"
                  clip-rule="evenodd"
                ></path>
              </svg>
            </NuxtLink>
          </li>
        </ul>
      </nav>
    </section>
  </div>
</template>

<style scoped>
[aria-label="Catalog pagination"] ul li {
  display: flex;
}
a[current-page="true"] {
  @apply bg-slate-100 text-slate-700 dark:bg-slate-700 dark:text-white;
}
</style>
