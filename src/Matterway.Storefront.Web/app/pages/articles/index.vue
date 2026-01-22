<script lang="ts" setup>
import { computed, onMounted, reactive, watch } from "vue";
import {
  buildArticlesRsqlFilter,
  createEmptyDetailFilter,
  createEmptySpecificationFilter,
  parseDetailFilters,
  parseSpecificationFilters,
  serializeDetailFilters,
  serializeSpecificationFilters,
  supportedDetailFilters,
  supportedSpecificationFilters,
  resolveDetailDefinition,
  resolveSpecificationDefinition,
  type DetailFilterState,
  type SpecificationFilterState,
} from "@composables/articleFilters";
import { useCatalogStore } from "@stores/catalog";
import { useSessionStore } from "@stores/session";

const route = useRoute();
const router = useRouter();
const catalogStore = useCatalogStore();
const sessionStore = useSessionStore();

const DEFAULT_PAGE_SIZE = 9;

type FiltersState = {
  search: string;
  minPrice?: number;
  maxPrice?: number;
  detailFilters: DetailFilterState[];
  specificationFilters: SpecificationFilterState[];
};

const parseRouteDetailFilters = () => {
  const detailFiltersParam = route.query.detailFilters;
  const serialized =
    typeof detailFiltersParam === "string"
      ? detailFiltersParam
      : Array.isArray(detailFiltersParam)
        ? detailFiltersParam[0]
        : undefined;

  return parseDetailFilters(serialized);
};

const parseRouteSpecificationFilters = () => {
  const specificationFiltersParam = route.query.specFilters;
  const serialized =
    typeof specificationFiltersParam === "string"
      ? specificationFiltersParam
      : Array.isArray(specificationFiltersParam)
        ? specificationFiltersParam[0]
        : undefined;

  return parseSpecificationFilters(serialized);
};

const filters = reactive<FiltersState>({
  search: (route.query.articleName as string) ?? "",
  minPrice: route.query.minPrice ? Number(route.query.minPrice) : undefined,
  maxPrice: route.query.maxPrice ? Number(route.query.maxPrice) : undefined,
  detailFilters: parseRouteDetailFilters(),
  specificationFilters: parseRouteSpecificationFilters(),
});

const pagination = reactive({
  page: Number(route.query.page ?? 1),
  pageSize: Number(route.query.pageSize ?? DEFAULT_PAGE_SIZE),
});

const catalogMeta = computed(() => catalogStore.getCatalogMeta);
const totalPages = computed(() => catalogMeta.value?.totalPages ?? 0);
const totalCount = computed(() => catalogMeta.value?.totalCount ?? 0);
const articles = computed(() => catalogStore.catalog ?? []);
const isLoading = computed(() => catalogStore.catalog === null);
const canManage = computed(() =>
  sessionStore.hasPermission("catalog", "operator"),
);
const pageOptions = [9, 12, 18];
const detailFilterOptions = supportedDetailFilters;
const specificationFilterOptions = supportedSpecificationFilters;

const pages = computed(() =>
  totalPages.value
    ? Array.from({ length: totalPages.value }, (_, index) => index + 1)
    : [],
);

const applyRouteState = () => {
  filters.search = (route.query.articleName as string) ?? "";
  filters.minPrice = route.query.minPrice
    ? Number(route.query.minPrice)
    : undefined;
  filters.maxPrice = route.query.maxPrice
    ? Number(route.query.maxPrice)
    : undefined;
  filters.detailFilters = parseRouteDetailFilters();
  filters.specificationFilters = parseRouteSpecificationFilters();
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

  filters.specificationFilters.forEach((spec) => {
    if (typeof spec.min === "number" && Number.isNaN(spec.min)) {
      spec.min = undefined;
    }
    if (typeof spec.max === "number" && Number.isNaN(spec.max)) {
      spec.max = undefined;
    }
  });
};

const buildFilterQuery = () =>
  buildArticlesRsqlFilter({
    search: filters.search,
    minPrice: filters.minPrice,
    maxPrice: filters.maxPrice,
    detailFilters: filters.detailFilters,
    specificationFilters: filters.specificationFilters,
  });

const fetchArticles = () => {
  sanitizeNumericFilters();
  catalogStore.fetchCatalog({
    page: pagination.page,
    pageSize: pagination.pageSize,
    filter: buildFilterQuery(),
  });
};

const updateRoute = () => {
  sanitizeNumericFilters();
  const serializedDetailFilters = serializeDetailFilters(filters.detailFilters);
  const serializedSpecificationFilters = serializeSpecificationFilters(
    filters.specificationFilters,
  );
  router.push({
    query: {
      page: pagination.page !== 1 ? pagination.page : undefined,
      pageSize:
        pagination.pageSize !== DEFAULT_PAGE_SIZE
          ? pagination.pageSize
          : undefined,
      articleName: filters.search || undefined,
      minPrice: filters.minPrice !== undefined ? filters.minPrice : undefined,
      maxPrice: filters.maxPrice !== undefined ? filters.maxPrice : undefined,
      detailFilters: serializedDetailFilters,
      specFilters: serializedSpecificationFilters,
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
  filters.detailFilters = [];
  filters.specificationFilters = [];
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

const addDetailFilter = () => {
  filters.detailFilters.push(createEmptyDetailFilter());
};

const removeDetailFilter = (index: number) => {
  filters.detailFilters.splice(index, 1);
};

const handleDetailFilterSlugChange = (
  filter: DetailFilterState,
  slug: string,
) => {
  const definition = resolveDetailDefinition(slug);
  const fallback = supportedDetailFilters[0];
  filter.slug = definition?.slug ?? fallback?.slug ?? "";
  filter.value = "";
};

const addSpecificationFilter = () => {
  filters.specificationFilters.push(createEmptySpecificationFilter());
};

const removeSpecificationFilter = (index: number) => {
  filters.specificationFilters.splice(index, 1);
};

const handleSpecificationFilterSlugChange = (
  filter: SpecificationFilterState,
  slug: string,
) => {
  const definition = resolveSpecificationDefinition(slug);
  const fallback = supportedSpecificationFilters[0];
  filter.slug = definition?.slug ?? fallback?.slug ?? "";
  filter.min = undefined;
  filter.max = undefined;
};

const getSpecificationUnit = (slug: string) =>
  resolveSpecificationDefinition(slug)?.unit ?? "";

useHead({
  title: "Proizvodi",
});

onMounted(() => {
  applyRouteState();
  fetchArticles();
});

watch(
  () => route.query,
  () => {
    applyRouteState();
    fetchArticles();
  },
);
</script>

<template>
  <section class="grid gap-8 lg:grid-cols-[320px_auto] w-full">
    <aside
      class="space-y-6 rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
    >
      <header class="space-y-1">
        <h2 class="text-lg font-semibold text-slate-900 dark:text-slate-100">
          Filteri
        </h2>
        <p class="text-sm text-slate-500 dark:text-slate-400">
          Kombinujte nazive i cene kako biste brže došli do željenog proizvoda.
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

        <div class="space-y-3">
          <div class="flex items-center justify-between gap-3">
            <label
              class="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400"
            >
              Detalji proizvoda
            </label>
            <button
              type="button"
              class="inline-flex items-center gap-2 rounded-full border border-slate-200 px-3 py-1.5 text-xs font-semibold text-slate-600 transition hover:border-blue-200 hover:text-blue-700 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:text-slate-200 dark:hover:border-blue-500 dark:hover:text-blue-200 dark:focus:ring-blue-500"
              @click="addDetailFilter"
            >
              <Icon
                name="heroicons-outline:plus"
                class="text-base"
                aria-hidden="true"
              />
              Dodaj detalj
            </button>
          </div>
          <p class="text-xs text-slate-500 dark:text-slate-400">
            Filtrirajte po brendu, modelu ili drugim opisnim detaljima. Prazna
            polja se preskaču.
          </p>

          <div v-if="filters.detailFilters.length" class="space-y-3">
            <div
              v-for="(detailFilter, index) in filters.detailFilters"
              :key="`detail-filter-${index}-${detailFilter.slug}`"
              class="space-y-3 rounded-xl border border-slate-200 bg-slate-50 p-3 dark:border-slate-700 dark:bg-slate-900/30"
            >
              <div class="grid gap-2">
                <label
                  class="text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
                >
                  Tip detalja
                </label>
                <select
                  v-model="detailFilter.slug"
                  class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:ring-blue-500"
                  @change="
                    handleDetailFilterSlugChange(
                      detailFilter,
                      ($event.target as HTMLSelectElement).value,
                    )
                  "
                >
                  <option
                    v-for="option in detailFilterOptions"
                    :key="option.slug"
                    :value="option.slug"
                  >
                    {{ option.label }}
                  </option>
                </select>
              </div>

              <div class="grid gap-2">
                <label
                  class="text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
                >
                  Vrednost
                </label>
                <input
                  v-model="detailFilter.value"
                  type="text"
                  placeholder="npr. Philips, bele boje"
                  class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:ring-blue-500"
                />
              </div>

              <div class="flex justify-end">
                <button
                  type="button"
                  class="inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-red-600 hover:underline dark:text-red-300"
                  @click="removeDetailFilter(index)"
                >
                  <Icon
                    name="heroicons-outline:trash"
                    class="text-base"
                    aria-hidden="true"
                  />
                  Ukloni
                </button>
              </div>
            </div>
          </div>

          <div
            v-else
            class="rounded-xl border border-dashed border-slate-300 px-4 py-3 text-xs text-slate-500 dark:border-slate-700 dark:text-slate-400"
          >
            Dodajte filter po specifikaciji kako biste suzili pretragu.
          </div>
        </div>

        <div class="space-y-3">
          <div class="flex items-center justify-between gap-3">
            <label
              class="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400"
            >
              Specifikacije (numerički)
            </label>
            <button
              type="button"
              class="inline-flex items-center gap-2 rounded-full border border-slate-200 px-3 py-1.5 text-xs font-semibold text-slate-600 transition hover:border-blue-200 hover:text-blue-700 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:text-slate-200 dark:hover:border-blue-500 dark:hover:text-blue-200 dark:focus:ring-blue-500"
              @click="addSpecificationFilter"
            >
              <Icon
                name="heroicons-outline:plus"
                class="text-base"
                aria-hidden="true"
              />
              Specifikacija
            </button>
          </div>
          <p class="text-xs text-slate-500 dark:text-slate-400">
            Postavite minimalne i maksimalne vrednosti za dimenzije, snagu ili
            kapacitete. Prazna polja se preskaču.
          </p>

          <div v-if="filters.specificationFilters.length" class="space-y-3">
            <div
              v-for="(specFilter, index) in filters.specificationFilters"
              :key="`spec-filter-${index}-${specFilter.slug}`"
              class="space-y-3 rounded-xl border border-slate-200 bg-slate-50 p-3 dark:border-slate-700 dark:bg-slate-900/30"
            >
              <div class="grid gap-2">
                <label
                  class="text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
                >
                  Tip specifikacije
                </label>
                <select
                  v-model="specFilter.slug"
                  class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:ring-blue-500"
                  @change="
                    handleSpecificationFilterSlugChange(
                      specFilter,
                      ($event.target as HTMLSelectElement).value,
                    )
                  "
                >
                  <option
                    v-for="option in specificationFilterOptions"
                    :key="option.slug"
                    :value="option.slug"
                  >
                    {{ option.label }}
                    <span v-if="option.unit"> ({{ option.unit }})</span>
                  </option>
                </select>
              </div>

              <div class="grid gap-3">
                <div class="grid gap-2">
                  <label
                    class="text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
                  >
                    Minimalna vrednost
                  </label>
                  <input
                    v-model.number="specFilter.min"
                    type="number"
                    min="0"
                    :placeholder="
                      getSpecificationUnit(specFilter.slug)
                        ? `0 ${getSpecificationUnit(specFilter.slug)}`
                        : '0'
                    "
                    class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:ring-blue-500"
                  />
                </div>
                <div class="grid gap-2">
                  <label
                    class="text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
                  >
                    Maksimalna vrednost
                  </label>
                  <input
                    v-model.number="specFilter.max"
                    type="number"
                    min="0"
                    :placeholder="
                      getSpecificationUnit(specFilter.slug)
                        ? `Max ${getSpecificationUnit(specFilter.slug)}`
                        : '100'
                    "
                    class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100 dark:focus:ring-blue-500"
                  />
                </div>
              </div>

              <div class="flex justify-end">
                <button
                  type="button"
                  class="inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-red-600 hover:underline dark:text-red-300"
                  @click="removeSpecificationFilter(index)"
                >
                  <Icon
                    name="heroicons-outline:trash"
                    class="text-base"
                    aria-hidden="true"
                  />
                  Ukloni
                </button>
              </div>
            </div>
          </div>

          <div
            v-else
            class="rounded-xl border border-dashed border-slate-300 px-4 py-3 text-xs text-slate-500 dark:border-slate-700 dark:text-slate-400"
          >
            Dodajte numerički filter kako biste suzili pretragu prema dimenziji
            ili snazi.
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
              class="hidden rounded-full border border-slate-200 bg-white pl-3 text-right pr-8 py-2 text-sm font-medium text-slate-600 focus:border-blue-300 focus:outline-hidden focus:ring-2 focus:ring-blue-100 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-200 dark:focus:ring-blue-500 sm:flex"
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
          to="/articles/create"
          class="inline-flex w-full items-center justify-center gap-2 rounded-full border border-dashed border-blue-300 bg-blue-50 px-5 py-2 text-sm font-semibold text-blue-600 transition hover:bg-blue-100 focus:outline-hidden focus:ring-4 focus:ring-blue-200 dark:border-blue-700 dark:bg-blue-900/30 dark:text-blue-200 dark:hover:bg-blue-900/40"
        >
          <Icon
            name="heroicons-outline:plus"
            class="text-base"
            aria-hidden="true"
          />
          Dodaj novi proizvod
        </NuxtLink>
      </form>
    </aside>

    <section class="space-y-6">
      <div v-if="isLoading" class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        <ArticleCartSkeleton
          v-for="n in pagination.pageSize"
          :key="`article-skeleton-${n}`"
        />
      </div>

      <div
        v-else-if="articles.length === 0"
        class="grid min-h-64 place-items-center rounded-3xl border border-slate-200 bg-white p-12 text-center dark:border-slate-700 dark:bg-slate-800"
      >
        <div class="space-y-4 text-slate-500 dark:text-slate-300">
          <Icon
            name="heroicons-outline:magnifying-glass"
            class="mx-auto text-5xl"
            aria-hidden="true"
          />
          <h2 class="text-lg font-semibold text-slate-700 dark:text-slate-100">
            Nismo pronašli proizvode za izabrane filtere
          </h2>
          <p class="text-sm">
            Probajte da proširite kriterijume pretrage ili resetujte filtere da
            biste videli kompletnu ponudu.
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
        <ArticleCard
          v-for="article in articles"
          :key="article.id"
          :article="article"
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
</template>
