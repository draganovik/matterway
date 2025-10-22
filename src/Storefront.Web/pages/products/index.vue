<script lang="ts" setup>
import { useCatalogStore } from "~/store/catalog";
import { useSessionStore } from "~/store/session";

const route = useRoute();
const router = useRouter();
const catalogStore = useCatalogStore();
const session = useSessionStore();
const pageSize = 6;

const queryParams = ref({
  page: route.query.page || 1,
  pageSize: route.query.pageSize || pageSize,
  productName: route.query.productName || undefined,
  minPrice: route.query.minPrice || undefined,
  maxPrice: route.query.maxPrice || undefined,
});

const pageLoad = () => {
  queryParams.value.productName = (route.query.productName || "") as string;
  queryParams.value.minPrice = route.query.minPrice || undefined;
  queryParams.value.maxPrice = route.query.maxPrice || undefined;
  catalogStore.fetchCatalog(
    (route.query.page || 1) as number,
    (route.query.pageSize || pageSize) as number,
    (route.query.productName || "") as string,
    (route.query.minPrice || 0) as number,
    (route.query.maxPrice || 0) as number,
  );
};

const paginate = (page: number = 1) => {
  queryParams.value.page = page;
  router.push({
    query: {
      page: queryParams.value.page || 1,
      pageSize: queryParams.value.pageSize || pageSize,
      productName: queryParams.value.productName || undefined,
      minPrice: queryParams.value.minPrice || undefined,
      maxPrice: queryParams.value.maxPrice || undefined,
    },
  });
};

const search = () => {
  router.push({
    query: {
      page: 1,
      pageSize: pageSize,
      productName: queryParams.value.productName || undefined,
      minPrice: queryParams.value.minPrice || undefined,
      maxPrice: queryParams.value.maxPrice || undefined,
    },
  });
  console.log(catalogStore.getCatalogMeta?.totalPages);
};

useHead({
  title: "Proizvodi",
});

onMounted(() => {
  pageLoad();
});

watch(
  () => route.query,
  () => pageLoad(),
);
</script>

<template>
  <div class="flex flex-col gap-6 md:flex-row md:gap-4">
    <aside class="w-80">
      <form @submit.prevent="search()">
        <div class="mb-6 grid gap-6">
          <div>
            <label
              for="productName"
              class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >Product name</label
            >
            <input
              type="text"
              id="productName"
              v-model="queryParams.productName"
              class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
              placeholder="Search for a product"
            />
          </div>
          <div>
            <label
              for="minPrice"
              class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >Najniža cena</label
            >
            <input
              type="number"
              id="minPrice"
              :max="queryParams.maxPrice?.toString()"
              v-model="queryParams.minPrice"
              class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
              placeholder="1000"
            />
          </div>
          <div>
            <label
              for="maxPrice"
              class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
              >Najviša cena</label
            >
            <input
              type="number"
              id="maxPrice"
              :min="queryParams.minPrice?.toString()"
              v-model="queryParams.maxPrice"
              class="block w-full rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
              placeholder="1200"
            />
          </div>
        </div>
        <button
          type="submit"
          class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
        >
          Pretraži
        </button>
        <NuxtLink
          v-if="
            session.getTokenData?.role == 'Admin' ||
            session.getTokenData?.role == 'Manager'
          "
          to="/products/create"
          class="mt-4 grid w-full place-items-center rounded-lg border border-gray-300 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-white dark:hover:border-gray-600 dark:hover:bg-gray-700 dark:focus:ring-gray-700"
          >Dodaj novi proizvod</NuxtLink
        >
      </form>
    </aside>
    <section class="w-full">
      <div
        v-if="catalogStore.catalog?.length == 0"
        class="grid w-full place-items-center gap-4 text-center text-slate-500"
      >
        <svg
          class="w-40"
          fill="currentColor"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg"
          aria-hidden="true"
        >
          <path
            clip-rule="evenodd"
            fill-rule="evenodd"
            d="M5.965 4.904l9.131 9.131a6.5 6.5 0 00-9.131-9.131zm8.07 10.192L4.904 5.965a6.5 6.5 0 009.131 9.131zM4.343 4.343a8 8 0 1111.314 11.314A8 8 0 014.343 4.343z"
          ></path>
        </svg>
        <h1 class="text-2xl">Traženi proizvodi trenutno nisu dostupni</h1>
      </div>
      <div v-else class="grid w-full grid-cols-2 gap-4 lg:grid-cols-3">
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

      <nav
        v-if="catalogStore.getCatalogMeta?.totalPages"
        class="mt-8 grid place-items-center"
        aria-label="Catalog pagination"
      >
        <ul class="inline-flex items-center -space-x-px">
          <li>
            <button
              :disabled="queryParams.page == 1"
              type="button"
              @click="paginate(1)"
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
            </button>
          </li>
          <li
            v-for="page in [
              ...Array(catalogStore.getCatalogMeta?.totalPages || 1).keys(),
            ]"
          >
            <button
              type="button"
              @click="paginate(page + 1)"
              :current-page="
                (route.query.page?.valueOf() == null && page == 0) ||
                page + 1 == route.query.page?.valueOf()
                  ? true
                  : false
              "
              class="border border-slate-300 bg-white px-3 py-2 leading-tight text-slate-500 hover:bg-slate-100 hover:text-slate-700 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-400 dark:hover:bg-slate-700 dark:hover:text-white"
            >
              {{ page + 1 }}
            </button>
          </li>
          <li>
            <button
              :disabled="
                queryParams.page == catalogStore.getCatalogMeta?.totalPages
              "
              type="button"
              @click="paginate(catalogStore.getCatalogMeta?.totalPages)"
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
            </button>
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
button[current-page="true"] {
  @apply bg-slate-100 text-slate-700 dark:bg-slate-700 dark:text-white;
}
</style>
