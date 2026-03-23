import {
  buildArticlesRsqlFilter,
  createEmptyDetailFilter,
  normalizeDetailDefinitions,
  resolveDetailDefinition,
  type DetailFilterDefinition,
  type DetailFilterState,
} from "~/lib/articles/filters"
import {
  DEFAULT_PAGINATION_PAGE_SIZE,
  PAGINATION_PAGE_SIZE_OPTIONS,
} from "~/constants/pagination"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { CatalogArticle } from "~/types/catalog/articles"
import type { PaginationMeta } from "~/types/common/api"

type FiltersState = {
  search: string
  minPrice?: number
  maxPrice?: number
  detailFilters: DetailFilterState[]
}

export function useArticlesBrowsePage() {
  const route = useRoute()
  const router = useRouter()
  const catalogApi = useCatalogClient()
  const requestVersion = ref(0)
  const pageOptions = [...PAGINATION_PAGE_SIZE_OPTIONS]

  function getRouteQueryValue(value: unknown) {
    if (typeof value === "string") return value
    if (!Array.isArray(value)) return undefined
    const firstString = value.find((item) => typeof item === "string")
    return typeof firstString === "string" ? firstString : undefined
  }

  function normalizePageSize(value: unknown) {
    const parsed = Number(
      typeof value === "number"
        ? value
        : (getRouteQueryValue(value) ?? DEFAULT_PAGINATION_PAGE_SIZE),
    )

    return pageOptions.includes(parsed) ? parsed : DEFAULT_PAGINATION_PAGE_SIZE
  }

  function parseRouteFilterState(): FiltersState {
    const filterExpression = getRouteQueryValue(route.query.filter)
    if (!filterExpression) {
      return {
        search: "",
        minPrice: undefined,
        maxPrice: undefined,
        detailFilters: [],
      }
    }

    let decodedExpression = filterExpression
    try {
      decodedExpression = decodeURIComponent(filterExpression)
    } catch {
      decodedExpression = filterExpression
    }

    const nextState: FiltersState = {
      search: "",
      minPrice: undefined,
      maxPrice: undefined,
      detailFilters: [],
    }
    const detailMap = new Map<string, DetailFilterState>()

    function ensureDetail(slug: string) {
      const existing = detailMap.get(slug)
      if (existing) return existing
      const created: DetailFilterState = { slug }
      detailMap.set(slug, created)
      return created
    }

    for (const rawClause of decodedExpression.split(";")) {
      const clause = rawClause.trim()
      if (!clause) continue

      if (clause.startsWith("title==")) {
        nextState.search = clause.slice("title==".length)
        continue
      }

      if (clause.startsWith("price=ge=")) {
        const minPrice = Number(clause.slice("price=ge=".length))
        if (Number.isFinite(minPrice)) {
          nextState.minPrice = minPrice
        }
        continue
      }

      if (clause.startsWith("price=le=")) {
        const maxPrice = Number(clause.slice("price=le=".length))
        if (Number.isFinite(maxPrice)) {
          nextState.maxPrice = maxPrice
        }
        continue
      }

      const greaterEqualIndex = clause.indexOf("=ge=")
      if (greaterEqualIndex > 0) {
        const slug = clause.slice(0, greaterEqualIndex).trim()
        const minValue = Number(clause.slice(greaterEqualIndex + 4))
        if (!slug || !Number.isFinite(minValue)) continue
        const detail = ensureDetail(slug)
        detail.min = minValue
        detail.value = undefined
        continue
      }

      const lowerEqualIndex = clause.indexOf("=le=")
      if (lowerEqualIndex > 0) {
        const slug = clause.slice(0, lowerEqualIndex).trim()
        const maxValue = Number(clause.slice(lowerEqualIndex + 4))
        if (!slug || !Number.isFinite(maxValue)) continue
        const detail = ensureDetail(slug)
        detail.max = maxValue
        detail.value = undefined
        continue
      }

      const equalsIndex = clause.indexOf("==")
      if (equalsIndex > 0) {
        const slug = clause.slice(0, equalsIndex).trim()
        const value = clause.slice(equalsIndex + 2).trim()
        if (!slug || !value || slug === "title" || slug === "price") continue
        const detail = ensureDetail(slug)
        detail.value = value
        detail.min = undefined
        detail.max = undefined
      }
    }

    nextState.detailFilters = Array.from(detailMap.values())
    return nextState
  }

  const initialFilters = parseRouteFilterState()

  const filters = reactive<FiltersState>({
    search: initialFilters.search,
    minPrice: initialFilters.minPrice,
    maxPrice: initialFilters.maxPrice,
    detailFilters: initialFilters.detailFilters,
  })

  const pagination = reactive({
    page: Number(route.query.page ?? 1),
    pageSize: normalizePageSize(route.query.pageSize),
  })

  const items = ref<CatalogArticle[]>([])
  const meta = ref<PaginationMeta | null>(null)
  const loading = ref(false)
  const error = ref("")
  const hasLoadedOnce = ref(false)

  const detailDefinitions = ref<DetailFilterDefinition[]>([])
  const detailDefinitionsLoading = ref(false)

  const showInitialSkeleton = computed(
    () => loading.value && !hasLoadedOnce.value,
  )
  const isRefreshing = computed(() => loading.value && hasLoadedOnce.value)

  function withActiveFilterDefinitions(definitions: DetailFilterDefinition[]) {
    const map = new Map(
      definitions.map((definition) => [definition.slug, definition]),
    )
    for (const filter of filters.detailFilters) {
      const slug = filter.slug?.trim()
      if (!slug || map.has(slug)) continue
      map.set(slug, {
        slug,
        label: slug,
        unit: undefined,
      })
    }

    return Array.from(map.values()).sort((a, b) =>
      a.label.localeCompare(b.label),
    )
  }

  function mergeDetailDefinitions(
    next: DetailFilterDefinition[],
    current: DetailFilterDefinition[],
  ) {
    const map = new Map(
      current.map((definition) => [definition.slug, definition]),
    )

    for (const definition of next) {
      map.set(definition.slug, definition)
    }

    return Array.from(map.values()).sort((a, b) =>
      a.label.localeCompare(b.label),
    )
  }

  async function loadDetailDefinitions(forVersion: number) {
    detailDefinitionsLoading.value = true
    try {
      const articleCodes = items.value.map((item) => item.code).slice(0, 24)
      if (!articleCodes.length) {
        if (forVersion === requestVersion.value) {
          detailDefinitions.value = withActiveFilterDefinitions(
            detailDefinitions.value,
          )
        }
        return
      }

      const responses = await Promise.all(
        articleCodes.map((code) => catalogApi.getArticle(code)),
      )

      const rawDefinitions: Array<{
        slug?: string | null
        title?: string | null
        unit?: string | null
      }> = []

      for (const response of responses) {
        for (const detail of response.item?.articleDetails ?? []) {
          rawDefinitions.push({
            slug: detail.detailSlug,
            title: detail.title,
            unit: detail.unit,
          })
        }
      }

      if (forVersion !== requestVersion.value) return
      const normalizedDefinitions = normalizeDetailDefinitions(rawDefinitions)
      detailDefinitions.value = withActiveFilterDefinitions(
        mergeDetailDefinitions(normalizedDefinitions, detailDefinitions.value),
      )
    } finally {
      if (forVersion === requestVersion.value) {
        detailDefinitionsLoading.value = false
      }
    }
  }

  function applyRouteState() {
    const parsedFilters = parseRouteFilterState()
    filters.search = parsedFilters.search
    filters.minPrice = parsedFilters.minPrice
    filters.maxPrice = parsedFilters.maxPrice
    filters.detailFilters = parsedFilters.detailFilters
    pagination.page = Number(route.query.page ?? 1) || 1
    pagination.pageSize = normalizePageSize(route.query.pageSize)
    detailDefinitions.value = withActiveFilterDefinitions(
      detailDefinitions.value,
    )
  }

  function sanitizeNumericFilters() {
    if (
      typeof filters.minPrice === "number" &&
      Number.isNaN(filters.minPrice)
    ) {
      filters.minPrice = undefined
    }
    if (
      typeof filters.maxPrice === "number" &&
      Number.isNaN(filters.maxPrice)
    ) {
      filters.maxPrice = undefined
    }

    filters.detailFilters.forEach((detail) => {
      const definition = resolveDetailDefinition(
        detail.slug,
        detailDefinitions.value,
      )
      if (!definition?.unit) return

      if (typeof detail.min === "number" && Number.isNaN(detail.min)) {
        detail.min = undefined
      }
      if (typeof detail.max === "number" && Number.isNaN(detail.max)) {
        detail.max = undefined
      }
    })
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
    )
  }

  async function fetchArticles() {
    sanitizeNumericFilters()
    const forVersion = requestVersion.value + 1
    requestVersion.value = forVersion
    loading.value = true
    error.value = ""

    const result = await catalogApi.browseArticles({
      page: pagination.page,
      pageSize: pagination.pageSize,
      filter: buildFilterQuery(),
    })

    if (forVersion !== requestVersion.value) return
    items.value = result.items
    meta.value = result.meta
    if (result.error) {
      error.value = result.error
    }

    await loadDetailDefinitions(forVersion)
    if (forVersion !== requestVersion.value) return
    hasLoadedOnce.value = true
    loading.value = false
  }

  function updateRoute() {
    sanitizeNumericFilters()
    const filter = buildFilterQuery()
    router.push({
      query: {
        page: pagination.page !== 1 ? pagination.page : undefined,
        pageSize:
          pagination.pageSize !== DEFAULT_PAGINATION_PAGE_SIZE
            ? pagination.pageSize
            : undefined,
        filter: filter || undefined,
      },
    })
  }

  function submitFilters() {
    pagination.page = 1
    updateRoute()
  }

  function resetFilters() {
    filters.search = ""
    filters.minPrice = undefined
    filters.maxPrice = undefined
    filters.detailFilters = []
    pagination.page = 1
    updateRoute()
  }

  function addDetailFilter() {
    if (!detailDefinitions.value.length) return
    filters.detailFilters.push(createEmptyDetailFilter(detailDefinitions.value))
  }

  function removeDetailFilter(index: number) {
    filters.detailFilters.splice(index, 1)
  }

  function handleDetailFilterSlugChange(
    filter: DetailFilterState,
    slug: string,
  ) {
    filter.slug = slug
    filter.value = ""
    filter.min = undefined
    filter.max = undefined
  }

  function onDetailFilterSlugChange(payload: { index: number; slug: string }) {
    const filter = filters.detailFilters[payload.index]
    if (!filter) return
    handleDetailFilterSlugChange(filter, payload.slug)
  }

  function setSearch(value: string) {
    filters.search = value
  }

  function setMinPrice(value: number | undefined) {
    filters.minPrice = value
  }

  function setMaxPrice(value: number | undefined) {
    filters.maxPrice = value
  }

  function setDetailFilterValue(payload: { index: number; value: string }) {
    const filter = filters.detailFilters[payload.index]
    if (!filter) return
    filter.value = payload.value
  }

  function setDetailFilterMin(payload: {
    index: number
    value: number | undefined
  }) {
    const filter = filters.detailFilters[payload.index]
    if (!filter) return
    filter.min = payload.value
  }

  function setDetailFilterMax(payload: {
    index: number
    value: number | undefined
  }) {
    const filter = filters.detailFilters[payload.index]
    if (!filter) return
    filter.max = payload.value
  }

  function goToPage(page: number) {
    if (page === pagination.page) return
    if (
      page < 1 ||
      ((meta.value?.totalPages ?? 0) && page > (meta.value?.totalPages ?? 0))
    ) {
      return
    }
    pagination.page = page
    updateRoute()
  }

  function changePageSize(size: number) {
    const nextSize = normalizePageSize(size)
    if (nextSize === pagination.pageSize) return
    pagination.pageSize = nextSize
    pagination.page = 1
    updateRoute()
  }

  watch(
    () => route.query,
    () => {
      applyRouteState()
      void fetchArticles()
    },
    { immediate: true },
  )

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
  }
}
