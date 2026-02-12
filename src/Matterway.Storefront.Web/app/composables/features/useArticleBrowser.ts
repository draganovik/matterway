import {
  buildArticlesRsqlFilter,
  createEmptyDetailFilter,
  normalizeDetailDefinitions,
  parseDetailFilters,
  resolveDetailDefinition,
  serializeDetailFilters,
  type DetailFilterDefinition,
  type DetailFilterState,
} from "~/composables/useArticleFilters";
import { useCatalogApi } from "~/composables/useCatalogApi";
import type { CatalogArticle } from "~/types/catalog/articles";
import type { PaginationMeta } from "~/types/common/api";

const DEFAULT_PAGE_SIZE = 9;

type FiltersState = {
  search: string;
  minPrice?: number;
  maxPrice?: number;
  detailFilters: DetailFilterState[];
};

export function useArticleBrowser() {
  const route = useRoute();
  const router = useRouter();
  const catalogApi = useCatalogApi();
  const requestVersion = ref(0);

  function parseRouteDetailFilters() {
    const detailFiltersParam = route.query.detailFilters;
    const serialized =
      typeof detailFiltersParam === "string"
        ? detailFiltersParam
        : Array.isArray(detailFiltersParam)
          ? detailFiltersParam[0]
          : undefined;

    return parseDetailFilters(serialized);
  }

  const filters = reactive<FiltersState>({
    search: (route.query.articleName as string) ?? "",
    minPrice: route.query.minPrice ? Number(route.query.minPrice) : undefined,
    maxPrice: route.query.maxPrice ? Number(route.query.maxPrice) : undefined,
    detailFilters: parseRouteDetailFilters(),
  });

  const pagination = reactive({
    page: Number(route.query.page ?? 1),
    pageSize: Number(route.query.pageSize ?? DEFAULT_PAGE_SIZE),
  });

  const items = ref<CatalogArticle[]>([]);
  const meta = ref<PaginationMeta | null>(null);
  const loading = ref(false);
  const error = ref("");
  const hasLoadedOnce = ref(false);

  const detailDefinitions = ref<DetailFilterDefinition[]>([]);
  const detailDefinitionsLoading = ref(false);

  const pageOptions = [9, 12, 18];
  const showInitialSkeleton = computed(
    () => loading.value && !hasLoadedOnce.value,
  );
  const isRefreshing = computed(() => loading.value && hasLoadedOnce.value);
  const pages = computed(() => {
    const total = meta.value?.totalPages ?? 0;
    return total ? Array.from({ length: total }, (_, index) => index + 1) : [];
  });

  function withActiveFilterDefinitions(definitions: DetailFilterDefinition[]) {
    const map = new Map(
      definitions.map((definition) => [definition.slug, definition]),
    );
    for (const filter of filters.detailFilters) {
      const slug = filter.slug?.trim();
      if (!slug || map.has(slug)) continue;
      map.set(slug, {
        slug,
        label: slug,
        unit: undefined,
      });
    }

    return Array.from(map.values()).sort((a, b) =>
      a.label.localeCompare(b.label),
    );
  }

  async function loadDetailDefinitions(forVersion: number) {
    detailDefinitionsLoading.value = true;
    try {
      const articleIds = items.value.map((item) => item.id).slice(0, 24);
      if (!articleIds.length) {
        if (forVersion === requestVersion.value) {
          detailDefinitions.value = withActiveFilterDefinitions([]);
        }
        return;
      }

      const responses = await Promise.all(
        articleIds.map((id) => catalogApi.getArticle(id)),
      );

      const rawDefinitions: Array<{
        slug?: string | null;
        title?: string | null;
        unit?: string | null;
      }> = [];

      for (const response of responses) {
        for (const detail of response.item?.articleDetails ?? []) {
          rawDefinitions.push({
            slug: detail.detailSlug,
            title: detail.title,
            unit: detail.unit,
          });
        }
      }

      if (forVersion !== requestVersion.value) return;
      detailDefinitions.value = withActiveFilterDefinitions(
        normalizeDetailDefinitions(rawDefinitions),
      );
    } finally {
      if (forVersion === requestVersion.value) {
        detailDefinitionsLoading.value = false;
      }
    }
  }

  function applyRouteState() {
    filters.search = (route.query.articleName as string) ?? "";
    filters.minPrice = route.query.minPrice
      ? Number(route.query.minPrice)
      : undefined;
    filters.maxPrice = route.query.maxPrice
      ? Number(route.query.maxPrice)
      : undefined;
    filters.detailFilters = parseRouteDetailFilters();
    pagination.page = Number(route.query.page ?? 1) || 1;
    pagination.pageSize =
      Number(route.query.pageSize ?? DEFAULT_PAGE_SIZE) || DEFAULT_PAGE_SIZE;
    detailDefinitions.value = withActiveFilterDefinitions(
      detailDefinitions.value,
    );
  }

  function sanitizeNumericFilters() {
    if (
      typeof filters.minPrice === "number" &&
      Number.isNaN(filters.minPrice)
    ) {
      filters.minPrice = undefined;
    }
    if (
      typeof filters.maxPrice === "number" &&
      Number.isNaN(filters.maxPrice)
    ) {
      filters.maxPrice = undefined;
    }

    filters.detailFilters.forEach((detail) => {
      const definition = resolveDetailDefinition(
        detail.slug,
        detailDefinitions.value,
      );
      if (!definition?.unit) return;

      if (typeof detail.min === "number" && Number.isNaN(detail.min)) {
        detail.min = undefined;
      }
      if (typeof detail.max === "number" && Number.isNaN(detail.max)) {
        detail.max = undefined;
      }
    });
  }

  function buildFilterQuery() {
    return buildArticlesRsqlFilter(
      {
        search: filters.search,
        minPrice: filters.minPrice,
        maxPrice: filters.maxPrice,
        detailFilters: filters.detailFilters,
      },
      detailDefinitions.value,
    );
  }

  async function fetchArticles() {
    sanitizeNumericFilters();
    const forVersion = requestVersion.value + 1;
    requestVersion.value = forVersion;
    loading.value = true;
    error.value = "";

    const result = await catalogApi.browseArticles({
      page: pagination.page,
      pageSize: pagination.pageSize,
      filter: buildFilterQuery(),
    });

    if (forVersion !== requestVersion.value) return;
    items.value = result.items;
    meta.value = result.meta;
    if (result.error) {
      error.value = result.error;
    }

    await loadDetailDefinitions(forVersion);
    if (forVersion !== requestVersion.value) return;
    hasLoadedOnce.value = true;
    loading.value = false;
  }

  function updateRoute() {
    sanitizeNumericFilters();
    const serializedDetailFilters = serializeDetailFilters(
      filters.detailFilters,
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
      },
    });
  }

  function submitFilters() {
    pagination.page = 1;
    updateRoute();
  }

  function resetFilters() {
    filters.search = "";
    filters.minPrice = undefined;
    filters.maxPrice = undefined;
    filters.detailFilters = [];
    pagination.page = 1;
    updateRoute();
  }

  function addDetailFilter() {
    if (!detailDefinitions.value.length) return;
    filters.detailFilters.push(
      createEmptyDetailFilter(detailDefinitions.value),
    );
  }

  function removeDetailFilter(index: number) {
    filters.detailFilters.splice(index, 1);
  }

  function handleDetailFilterSlugChange(
    filter: DetailFilterState,
    slug: string,
  ) {
    filter.slug = slug;
    filter.value = "";
    filter.min = undefined;
    filter.max = undefined;
  }

  function onDetailFilterSlugChange(payload: { index: number; slug: string }) {
    const filter = filters.detailFilters[payload.index];
    if (!filter) return;
    handleDetailFilterSlugChange(filter, payload.slug);
  }

  function setSearch(value: string) {
    filters.search = value;
  }

  function setMinPrice(value: number | undefined) {
    filters.minPrice = value;
  }

  function setMaxPrice(value: number | undefined) {
    filters.maxPrice = value;
  }

  function setDetailFilterValue(payload: { index: number; value: string }) {
    const filter = filters.detailFilters[payload.index];
    if (!filter) return;
    filter.value = payload.value;
  }

  function setDetailFilterMin(payload: {
    index: number;
    value: number | undefined;
  }) {
    const filter = filters.detailFilters[payload.index];
    if (!filter) return;
    filter.min = payload.value;
  }

  function setDetailFilterMax(payload: {
    index: number;
    value: number | undefined;
  }) {
    const filter = filters.detailFilters[payload.index];
    if (!filter) return;
    filter.max = payload.value;
  }

  function goToPage(page: number) {
    if (page === pagination.page) return;
    if (
      page < 1 ||
      ((meta.value?.totalPages ?? 0) && page > (meta.value?.totalPages ?? 0))
    ) {
      return;
    }
    pagination.page = page;
    updateRoute();
  }

  function changePageSize(size: number) {
    if (size === pagination.pageSize) return;
    pagination.pageSize = size;
    pagination.page = 1;
    updateRoute();
  }

  watch(
    () => route.query,
    () => {
      applyRouteState();
      void fetchArticles();
    },
    { immediate: true },
  );

  return {
    filters,
    pagination,
    items,
    meta,
    error,
    showInitialSkeleton,
    isRefreshing,
    detailDefinitions,
    detailDefinitionsLoading,
    pageOptions,
    pages,
    submitFilters,
    resetFilters,
    addDetailFilter,
    removeDetailFilter,
    onDetailFilterSlugChange,
    setSearch,
    setMinPrice,
    setMaxPrice,
    setDetailFilterValue,
    setDetailFilterMin,
    setDetailFilterMax,
    goToPage,
    changePageSize,
  };
}
