<script lang="ts" setup>
import { computed, onMounted } from "vue";
import { useCatalogStore } from "@stores/catalog";
import { featuredCollections } from "@composables/collections";

const router = useRouter();
const catalogStore = useCatalogStore();

useHead({
  title: "Naslovna",
});

onMounted(() => {
  catalogStore.fetchCatalog(1, 8, "", 0, 0);
});

const isLoading = computed(() => catalogStore.catalog === null);
const featuredProducts = computed(() =>
  (catalogStore.catalog ?? []).slice(0, 4),
);
const heroStats = computed(() => {
  const items = catalogStore.catalog ?? [];
  const available = items.filter((product) => product.isAvailable).length;
  return [
    {
      label: "Proizvoda u ponudi",
      value: items.length.toLocaleString("sr-RS"),
    },
    { label: "Na stanju", value: available.toLocaleString("sr-RS") },
    { label: "Godina iskustva", value: "10+" },
  ];
});

const collections = featuredCollections;

const handleCategoryClick = (category: { query?: string }) => {
  router.push({
    path: "/products",
    query: {
      productName: category.query || undefined,
    },
  });
};

const goToProducts = () => router.push("/products");
</script>

<template>
  <main class="space-y-16 pb-16">
    <section
      class="relative overflow-hidden rounded-3xl bg-linear-to-br from-blue-600 via-blue-500 to-indigo-500 text-white"
    >
      <div
        class="absolute -right-24 -top-24 h-64 w-64 rounded-full bg-white/20 blur-3xl"
      ></div>
      <div
        class="absolute -bottom-32 -left-10 h-72 w-72 rounded-full bg-sky-400/30 blur-3xl"
      ></div>
      <div
        class="relative mx-auto flex max-w-6xl flex-col gap-10 px-4 py-16 md:flex-row md:items-center md:justify-between md:px-6"
      >
        <div class="max-w-xl space-y-6">
          <span
            class="inline-flex items-center rounded-full border border-white/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide"
          >
            Pametna rešenja za dom
          </span>
          <h1 class="text-4xl font-semibold leading-tight sm:text-5xl">
            Pretvorite svaki prostor u pametno i ugodno okruženje
          </h1>
          <p class="text-lg text-white/80">
            Matterway povezuje vaše uređaje u jedinstven sistem, omogućavajući
            vam da upravljate Vašim domom ili kancelarijom sa lakoćom i
            efikasnošću.
          </p>
          <div class="flex flex-wrap gap-3">
            <button
              type="button"
              class="inline-flex items-center justify-center gap-2 rounded-full bg-white px-5 py-2.5 text-sm font-semibold text-blue-600 shadow-sm transition hover:bg-blue-50"
              @click="goToProducts"
            >
              <span>Pregledaj ponudu</span>
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
                  d="M13.5 4.5L21 12m0 0-7.5 7.5M21 12H3"
                />
              </svg>
            </button>
          </div>
        </div>
        <dl
          class="mx-auto grid w-full max-w-md grid-cols-3 gap-4 rounded-2xl border border-white/20 bg-white/10 p-6 backdrop-blur-sm md:max-w-sm"
        >
          <div
            v-for="stat in heroStats"
            :key="stat.label"
            class="grid grid-rows-2 gap-1 text-center"
          >
            <dt
              class="text-xs font-medium uppercase tracking-wide text-white/70"
            >
              {{ stat.label }}
            </dt>
            <dd class="text-2xl font-semibold text-white">
              {{ stat.value }}
            </dd>
          </div>
        </dl>
      </div>
    </section>

    <section id="kolekcije" class="mx-auto max-w-6xl px-4 md:px-6">
      <div class="flex flex-wrap items-end justify-between gap-4">
        <div class="space-y-2">
          <h2 class="text-2xl font-semibold text-slate-900 dark:text-slate-100">
            Popularne kolekcije
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400">
            Pronađite rešenja za svaku prostoriju i budžet, od rasvete do
            sigurnosnih sistema.
          </p>
        </div>
      </div>
      <div class="mt-6 grid gap-5 md:grid-cols-3">
        <CollectionCard
          v-for="collection in collections"
          :key="collection.name"
          :title="collection.name"
          :description="collection.description"
          :accent="collection.accent"
          :icon="collection.icon"
          @click="handleCategoryClick(collection)"
        />
      </div>
    </section>

    <section id="najnovije" class="mx-auto max-w-6xl px-4 md:px-6">
      <div class="flex flex-wrap items-end justify-between gap-4">
        <div class="space-y-2">
          <h2 class="text-2xl font-semibold text-slate-900 dark:text-slate-100">
            Najnoviji proizvodi
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400">
            Najtraženiji artikli koje su naši korisnici dodali poslednjih dana.
          </p>
        </div>
        <NuxtLink
          to="/products"
          class="inline-flex items-center gap-2 text-sm font-medium text-blue-600 hover:underline dark:text-blue-300"
        >
          Pogledaj sve
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
              d="M13.5 4.5L21 12m0 0-7.5 7.5M21 12H3"
            />
          </svg>
        </NuxtLink>
      </div>
      <div class="mt-6">
        <div v-if="isLoading" class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <ProductCartSkeleton v-for="n in 4" :key="`product-skeleton-${n}`" />
        </div>
        <div
          v-else-if="featuredProducts.length === 0"
          class="rounded-2xl border border-slate-200 bg-white p-12 text-center text-slate-500 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-300"
        >
          Trenutno nema dostupnih proizvoda. Svratite uskoro ponovo.
        </div>
        <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <ProductCard
            v-for="product in featuredProducts"
            :key="product.id"
            :product="product"
          />
        </div>
      </div>
    </section>
  </main>
</template>
