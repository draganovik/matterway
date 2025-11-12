<script lang="ts" setup>
import { computed, onMounted, reactive, watch } from "vue";
import { useCatalogStore } from "@stores/catalog";
import { useSessionStore } from "@stores/session";

const route = useRoute();
const router = useRouter();
const catalogStore = useCatalogStore();
const sessionStore = useSessionStore();

const DEFAULT_PAGE_SIZE = 9;

const filters = reactive({
  search: (route.query.productName as string) ?? "",
  minPrice: route.query.minPrice ? Number(route.query.minPrice) : undefined,
  maxPrice: route.query.maxPrice ? Number(route.query.maxPrice) : undefined,
});

const pagination = reactive({
  page: Number(route.query.page ?? 1),
  pageSize: Number(route.query.pageSize ?? DEFAULT_PAGE_SIZE),
});

const catalogMeta = computed(() => catalogStore.getCatalogMeta);
const totalPages = computed(() => catalogMeta.value?.totalPages ?? 0);
const totalCount = computed(() => catalogMeta.value?.totalCount ?? 0);
const products = computed(() => catalogStore.catalog ?? []);
const isLoading = computed(() => catalogStore.catalog === null);
const canManage = computed(() => {
  const role = sessionStore.getTokenData?.role;
  return role === "Admin" || role === "Manager";
});
const pageOptions = [9, 12, 18];

const pages = computed(() =>
  totalPages.value
    ? Array.from({ length: totalPages.value }, (_, index) => index + 1)
    : [],
);

const applyRouteState = () => {
  filters.search = (route.query.productName as string) ?? "";
  filters.minPrice = route.query.minPrice
    ? Number(route.query.minPrice)
    : undefined;
  filters.maxPrice = route.query.maxPrice
    ? Number(route.query.maxPrice)
    : undefined;
  pagination.page = Number(route.query.page ?? 1) || 1;
  pagination.pageSize =
    Number(route.query.pageSize ?? DEFAULT_PAGE_SIZE) || DEFAULT_PAGE_SIZE;
};

const sanitizeNumericFilters = () => {
  if (typeof filters.minPrice === "number" && Number.isNaN(filters.minPrice)) {
    filters.minPrice = undefined;
  }
  if (typeof filters.maxPrice === "number" && Number.isNaN(filters.maxPrice)) {
    filters.maxPrice = undefined;
  }
};

const fetchProducts = () => {
  sanitizeNumericFilters();
  catalogStore.fetchCatalog(
    pagination.page,
    pagination.pageSize,
    filters.search,
    filters.minPrice ?? 0,
    filters.maxPrice ?? 0,
  );
};

const updateRoute = () => {
  sanitizeNumericFilters();
  router.push({
    query: {
      page: pagination.page !== 1 ? pagination.page : undefined,
      pageSize:
        pagination.pageSize !== DEFAULT_PAGE_SIZE
          ? pagination.pageSize
          : undefined,
      productName: filters.search || undefined,
      minPrice: filters.minPrice !== undefined ? filters.minPrice : undefined,
      maxPrice: filters.maxPrice !== undefined ? filters.maxPrice : undefined,
    },
  });
};

const submitFilters = () => {
  sanitizeNumericFilters();
  pagination.page = 1;
  updateRoute();
};

const resetFilters = () => {
  filters.search = "";
  filters.minPrice = undefined;
  filters.maxPrice = undefined;
  pagination.page = 1;
  updateRoute();
};

const goToPage = (page: number) => {
  if (page === pagination.page) return;
  if (page < 1 || (totalPages.value && page > totalPages.value)) return;
  pagination.page = page;
  updateRoute();
};

const changePageSize = (size: number) => {
  if (size === pagination.pageSize) return;
  pagination.pageSize = size;
  pagination.page = 1;
  updateRoute();
};

useHead({
  title: "Proizvodi",
});

onMounted(() => {
  applyRouteState();
  fetchProducts();
});

watch(
  () => route.query,
  () => {
    applyRouteState();
    fetchProducts();
  },
);
</script>

<template>
  <div class="mx-auto space-y-12 lg:space-y-16">
    <section class="grid gap-8 lg:grid-cols-[320px_1fr]">
      <aside
        class="space-y-6 rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
      >
        <header class="space-y-1">
          <h2 class="text-lg font-semibold text-slate-900 dark:text-slate-100">
            Filteri
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400">
            Kombinujte nazive i cene kako biste brže došli do željenog
            proizvoda.
          </p>
        </header>

        <form class="space-y-5" @submit.prevent="submitFilters">
          <div class="grid gap-2">
            <label
              class="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400"
            >
              Naziv proizvoda
            </label>
            <input
              v-model="filters.search"
              type="text"
              placeholder="npr. Philips Hue"
              class="rounded-xl border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:bg-slate-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:text-slate-800 dark:focus:ring-blue-500"
            />
          </div>

          <div class="grid gap-4 sm:grid-cols-2">
            <div class="grid gap-2">
              <label
                class="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400"
              >
                Minimalna cena
              </label>
              <input
                v-model.number="filters.minPrice"
                type="number"
                min="0"
                placeholder="0"
                class="w-full rounded-xl border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:bg-slate-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:text-slate-800 dark:focus:ring-blue-500"
              />
            </div>
            <div class="grid gap-2">
              <label
                class="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400"
              >
                Maksimalna cena
              </label>
              <input
                v-model.number="filters.maxPrice"
                type="number"
                min="0"
                placeholder="10000"
                class="w-full rounded-xl border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:bg-slate-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:text-slate-800 dark:focus:ring-blue-500"
              />
            </div>
          </div>

          <div class="flex flex-col gap-6 text-sm">
            <div class="flex flex-col items-center gap-2 sm:flex-row">
              <label
                for="page-size"
                class="w-full text-center text-slate-500 dark:text-slate-400 sm:pl-4 sm:text-left"
              >
                Prikaži po stranici:
              </label>
              <select
                id="page-size"
                :value="pagination.pageSize"
                class="hidden rounded-full border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-600 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-200 dark:focus:ring-blue-500 sm:flex"
                @change="
                  changePageSize(
                    Number(($event.target as HTMLSelectElement).value),
                  )
                "
              >
                <option
                  v-for="option in pageOptions"
                  :key="`page-size-${option}`"
                  :value="option"
                >
                  {{ option }}
                </option>
              </select>
              <div class="flex items-center gap-2 sm:hidden">
                <button
                  v-for="option in pageOptions"
                  :key="`mobile-page-size-${option}`"
                  type="button"
                  class="rounded-full border px-3 py-1.5 font-medium transition focus:outline-hidden"
                  :class="
                    option === pagination.pageSize
                      ? 'border-blue-200 bg-blue-100 text-blue-600 dark:border-blue-700 dark:bg-blue-900/30 dark:text-blue-200'
                      : 'border-slate-200 text-slate-500 hover:border-blue-200 dark:border-slate-700 dark:text-slate-300'
                  "
                  @click="changePageSize(option)"
                >
                  {{ option }}
                </button>
              </div>
            </div>
            <div
              class="rounded-full bg-slate-100 px-4 py-2 font-medium text-slate-600 dark:bg-slate-700 dark:text-slate-200"
            >
              Ukupno rezultata: {{ totalCount }}
            </div>
          </div>

          <div class="flex flex-col gap-3">
            <button
              type="submit"
              class="inline-flex items-center justify-center rounded-full bg-blue-600 px-5 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-hidden focus:ring-4 focus:ring-blue-200 dark:focus:ring-blue-500/40"
            >
              Primeni filtere
            </button>
            <button
              type="button"
              class="inline-flex items-center justify-center rounded-full border border-slate-200 px-5 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 focus:outline-hidden focus:ring-4 focus:ring-slate-200 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800 dark:focus:ring-slate-600/60"
              @click="resetFilters"
            >
              Resetuj
            </button>
          </div>

          <NuxtLink
            v-if="canManage"
            to="/products/create"
            class="inline-flex w-full items-center justify-center gap-2 rounded-full border border-dashed border-blue-300 bg-blue-50 px-5 py-2 text-sm font-semibold text-blue-600 transition hover:bg-blue-100 focus:outline-hidden focus:ring-4 focus:ring-blue-200 dark:border-blue-700 dark:bg-blue-900/30 dark:text-blue-200 dark:hover:bg-blue-900/40"
          >
            <svg
              class="h-4 w-4"
              fill="none"
              stroke="currentColor"
              stroke-width="1.8"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M12 6v12m6-6H6"
              />
            </svg>
            Dodaj novi proizvod
          </NuxtLink>
        </form>
      </aside>

      <section class="space-y-6">
        <div v-if="isLoading" class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          <ProductCartSkeleton
            v-for="n in pagination.pageSize"
            :key="`product-skeleton-${n}`"
          />
        </div>

        <div
          v-else-if="products.length === 0"
          class="grid min-h-64 place-items-center rounded-3xl border border-slate-200 bg-white p-12 text-center dark:border-slate-700 dark:bg-slate-800"
        >
          <div class="space-y-4 text-slate-500 dark:text-slate-300">
            <svg
              class="mx-auto h-12 w-12"
              fill="none"
              stroke="currentColor"
              stroke-width="1.8"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M9.813 15.904A6.5 6.5 0 018.25 4.5 6.5 6.5 0 1115.5 11a6.46 6.46 0 01-.904 3.313L21 20.719 20.281 21l-4.407-4.407A6.46 6.46 0 0112 17.5a6.46 6.46 0 01-2.187-.407"
              />
            </svg>
            <h2
              class="text-lg font-semibold text-slate-700 dark:text-slate-100"
            >
              Nismo pronašli proizvode za izabrane filtere
            </h2>
            <p class="text-sm">
              Probajte da proširite kriterijume pretrage ili resetujte filtere
              da biste videli kompletnu ponudu.
            </p>
            <button
              type="button"
              class="inline-flex items-center gap-2 rounded-full border border-slate-200 px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100 focus:outline-hidden focus:ring-4 focus:ring-slate-200 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
              @click="resetFilters"
            >
              Resetuj filtere
            </button>
          </div>
        </div>

        <div v-else class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          <ProductCard
            v-for="product in products"
            :key="product.id"
            :product="product"
          />
        </div>

        <nav
          v-if="totalPages > 1"
          class="flex flex-wrap items-center justify-between gap-4"
        >
          <div class="text-sm text-slate-500 dark:text-slate-400">
            Strana {{ pagination.page }} od {{ totalPages }}
          </div>
          <div class="flex items-center gap-2">
            <button
              type="button"
              class="rounded-full border border-slate-200 px-3 py-1.5 text-sm font-medium text-slate-600 transition hover:border-blue-200 hover:text-blue-600 focus:outline-hidden focus:ring-2 focus:ring-blue-100 disabled:cursor-not-allowed disabled:border-slate-200 disabled:text-slate-300 dark:border-slate-700 dark:text-slate-300 dark:hover:border-blue-500 dark:hover:text-blue-200 dark:focus:ring-blue-500"
              :disabled="pagination.page === 1"
              @click="goToPage(pagination.page - 1)"
            >
              Prethodna
            </button>
            <button
              v-for="page in pages"
              :key="`page-${page}`"
              type="button"
              class="rounded-full border px-3 py-1.5 text-sm font-medium transition focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:focus:ring-blue-500"
              :class="
                page === pagination.page
                  ? 'border-blue-200 bg-blue-100 text-blue-700 dark:border-blue-500 dark:bg-blue-900/50 dark:text-blue-200'
                  : 'border-slate-200 text-slate-600 hover:border-blue-200 hover:text-blue-600 dark:border-slate-700 dark:text-slate-300 dark:hover:border-blue-500 dark:hover:text-blue-200'
              "
              @click="goToPage(page)"
            >
              {{ page }}
            </button>
            <button
              type="button"
              class="rounded-full border border-slate-200 px-3 py-1.5 text-sm font-medium text-slate-600 transition hover:border-blue-200 hover:text-blue-600 focus:outline-hidden focus:ring-2 focus:ring-blue-100 disabled:cursor-not-allowed disabled:border-slate-200 disabled:text-slate-300 dark:border-slate-700 dark:text-slate-300 dark:hover:border-blue-500 dark:hover:text-blue-200 dark:focus:ring-blue-500"
              :disabled="pagination.page === totalPages"
              @click="goToPage(pagination.page + 1)"
            >
              Sledeća
            </button>
          </div>
        </nav>
      </section>
    </section>
  </div>
</template>
