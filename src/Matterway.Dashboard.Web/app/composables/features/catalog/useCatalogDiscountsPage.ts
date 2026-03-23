import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { normalizeCode } from "~/utils/normalization"
import { parseNumberOr } from "~/utils/numbers"
import type {
  QueryArticleResponse,
  QueryDiscountResponse,
} from "~/types/catalog"

type DiscountListItem = {
  key: string
  code: string
  percentage: number | string
  validFrom: string
  validTo?: string | null
  articleCodes: string[]
}

type DiscountForm = {
  code: string
  percentage: number | null
  validFrom: string
  validTo: string
}

export function useCatalogDiscountsPage() {
  const auth = useAuthSessionStore()
  const api = useCatalogClient()

  const canEdit = computed(() =>
    auth.hasPermission("catalog", ["operator", "manager"]),
  )

  const listState = useRequestState({ empty: "Nema popusta." })
  const submitState = useRequestState()
  const deleteState = useRequestState()
  const deleteConfirmOpen = ref(false)

  const discounts = ref<DiscountListItem[]>([])
  const discountFilter = ref("")
  const listPage = ref(1)
  const listPageSize = ref(DEFAULT_PAGINATION_PAGE_SIZE)

  const selectedKey = ref<string | null>(null)
  const selectedDiscount = ref<DiscountListItem | null>(null)
  const isCreateMode = computed(() => !selectedDiscount.value)

  const form = ref<DiscountForm>({
    code: "",
    percentage: null,
    validFrom: getDefaultDateTimeLocal(),
    validTo: "",
  })
  const selectedArticleCodes = ref<string[]>([])

  function getDefaultDateTimeLocal() {
    const now = new Date()
    const year = now.getFullYear()
    const month = String(now.getMonth() + 1).padStart(2, "0")
    const day = String(now.getDate()).padStart(2, "0")
    const hour = String(now.getHours()).padStart(2, "0")
    const minute = String(now.getMinutes()).padStart(2, "0")
    return `${year}-${month}-${day}T${hour}:${minute}`
  }

  function toIsoDateTime(value: string) {
    const parsed = new Date(value)
    if (Number.isNaN(parsed.getTime())) return null
    return parsed.toISOString()
  }

  function toLocalDateTimeInput(value?: string | null) {
    if (!value) return ""
    const parsed = new Date(value)
    if (Number.isNaN(parsed.getTime())) return ""
    const year = parsed.getFullYear()
    const month = String(parsed.getMonth() + 1).padStart(2, "0")
    const day = String(parsed.getDate()).padStart(2, "0")
    const hour = String(parsed.getHours()).padStart(2, "0")
    const minute = String(parsed.getMinutes()).padStart(2, "0")
    return `${year}-${month}-${day}T${hour}:${minute}`
  }

  function sortDiscounts(items: DiscountListItem[]) {
    return [...items].sort((a, b) => {
      const leftCode = (a.code || "").toLowerCase()
      const rightCode = (b.code || "").toLowerCase()
      if (leftCode !== rightCode) return leftCode.localeCompare(rightCode)
      return (b.validFrom || "").localeCompare(a.validFrom || "")
    })
  }

  function resetMessages() {
    submitState.error = ""
    submitState.success = ""
    deleteState.error = ""
    deleteState.success = ""
  }

  function beginCreate() {
    selectedKey.value = null
    selectedDiscount.value = null
    form.value = {
      code: "",
      percentage: null,
      validFrom: getDefaultDateTimeLocal(),
      validTo: "",
    }
    selectedArticleCodes.value = []
    resetMessages()
  }

  function requestRemoveDiscount() {
    deleteState.error = ""
    deleteState.success = ""

    if (!canEdit.value) return

    const code = normalizeCode(form.value.code)
    if (!code) {
      deleteState.error = "Unesite kod za brisanje."
      return
    }

    deleteConfirmOpen.value = true
  }

  function applyDiscountToEditor(discount: DiscountListItem) {
    selectedKey.value = discount.key
    selectedDiscount.value = discount
    form.value = {
      code: discount.code,
      percentage: Number(discount.percentage),
      validFrom:
        toLocalDateTimeInput(discount.validFrom) || getDefaultDateTimeLocal(),
      validTo: toLocalDateTimeInput(discount.validTo),
    }
    selectedArticleCodes.value = [...discount.articleCodes]
    resetMessages()
  }

  function selectDiscount(key: string) {
    const discount = discounts.value.find((item) => item.key === key)
    if (!discount) return
    applyDiscountToEditor(discount)
  }

  const filteredDiscounts = computed(() => {
    const search = String(discountFilter.value ?? "")
      .trim()
      .toLowerCase()
    if (!search) return discounts.value
    return discounts.value.filter(
      (item) =>
        item.code.toLowerCase().includes(search) ||
        item.validFrom.toLowerCase().includes(search) ||
        `${item.percentage}`.toLowerCase().includes(search),
    )
  })

  const filteredDiscountCount = computed(() => filteredDiscounts.value.length)
  const filteredDiscountPages = computed(() =>
    Math.max(1, Math.ceil(filteredDiscountCount.value / listPageSize.value)),
  )
  const visibleDiscounts = computed(() => {
    const start = (listPage.value - 1) * listPageSize.value
    return filteredDiscounts.value.slice(start, start + listPageSize.value)
  })

  watch(
    filteredDiscountPages,
    (totalPages) => {
      if (listPage.value > totalPages) listPage.value = totalPages
    },
    { immediate: true },
  )

  function updateDiscountFilter(value: string) {
    discountFilter.value = value
  }

  function searchDiscounts() {
    listPage.value = 1
  }

  function changeListPage(value: number) {
    listPage.value = Math.min(Math.max(1, value), filteredDiscountPages.value)
  }

  function changeListPageSize(value: number) {
    listPageSize.value = Math.max(
      1,
      Number(value) || DEFAULT_PAGINATION_PAGE_SIZE,
    )
    listPage.value = 1
  }

  async function loadDiscountsFromArticlesFallback() {
    const pageSize = 100
    let page = 1
    const articleRows: QueryArticleResponse[] = []

    while (true) {
      const result = await api.queryArticles({ page, pageSize })

      if (!result.ok) {
        listState.error = result.error || "Učitavanje popusta nije uspelo."
        return
      }

      if (result.status === 204 || !result.data) break

      articleRows.push(...(result.data.data || []))
      const totalPages = Math.max(
        1,
        parseNumberOr(result.data.meta?.totalPages, 1),
      )
      if (page >= totalPages) break
      page += 1
    }

    const grouped = new Map<string, DiscountListItem>()

    for (const article of articleRows) {
      if (!article.discount || !article.code) continue
      const code = normalizeCode((article.discount.code ?? "").toString())
      const percentage = article.discount.percentage
      const validFrom = article.discount.validFrom
      const validTo = article.discount.validTo ?? null
      const key = code || `${percentage}|${validFrom}|${validTo || ""}`

      const existing = grouped.get(key)
      if (!existing) {
        grouped.set(key, {
          key,
          code,
          percentage,
          validFrom,
          validTo,
          articleCodes: [article.code],
        })
        continue
      }
      if (!existing.articleCodes.includes(article.code)) {
        existing.articleCodes.push(article.code)
      }
    }

    discounts.value = sortDiscounts(Array.from(grouped.values()))
  }

  function buildDiscountList(rows: QueryDiscountResponse[]) {
    const grouped = new Map<string, DiscountListItem>()

    for (const row of rows) {
      const code = normalizeCode((row.code ?? "").toString())
      const percentage = row.percentage ?? ""
      const validFrom = row.validFrom ?? ""
      const validTo = row.validTo ?? null
      const key = code || `${percentage}|${validFrom}|${validTo || ""}`
      const articleCode = row.articleCode?.toString() ?? ""

      const existing = grouped.get(key)
      if (!existing) {
        grouped.set(key, {
          key,
          code,
          percentage,
          validFrom,
          validTo,
          articleCodes: articleCode ? [articleCode] : [],
        })
        continue
      }
      if (articleCode && !existing.articleCodes.includes(articleCode)) {
        existing.articleCodes.push(articleCode)
      }
    }

    return sortDiscounts(Array.from(grouped.values()))
  }

  async function loadDiscounts() {
    listState.loading = true
    listState.error = ""

    const result = await api.queryDiscounts()

    if (result.ok) {
      discounts.value = buildDiscountList(result.data || [])
      listState.loading = false
    } else if (result.status === 404 || result.status === 405) {
      await loadDiscountsFromArticlesFallback()
      listState.loading = false
    } else {
      listState.error = result.error || "Učitavanje popusta nije uspelo."
      discounts.value = []
      listState.loading = false
    }

    if (!selectedKey.value) return

    const selected =
      discounts.value.find((item) => item.key === selectedKey.value) || null
    if (!selected) {
      beginCreate()
      return
    }

    applyDiscountToEditor(selected)
  }

  function buildPayload() {
    const code = normalizeCode(form.value.code)
    const percentage = form.value.percentage
    const validFrom = toIsoDateTime(String(form.value.validFrom ?? ""))
    const validToInput = String(form.value.validTo ?? "").trim()
    const validTo = validToInput ? toIsoDateTime(validToInput) : null

    if (!code) {
      submitState.error = "Kod je obavezan."
      return null
    }
    if (percentage == null || percentage < 0.01 || percentage > 1) {
      submitState.error = "Procenat mora biti između 0,01 i 1."
      return null
    }
    if (!validFrom) {
      submitState.error = "Polje „Važi od“ mora imati ispravan datum i vreme."
      return null
    }
    if (validToInput && !validTo) {
      submitState.error = "Polje „Važi do“ mora imati ispravan datum i vreme."
      return null
    }
    if (validTo && new Date(validTo) < new Date(validFrom)) {
      submitState.error =
        "Polje „Važi do“ mora biti veće ili jednako polju „Važi od“."
      return null
    }

    const articleCodes = [...new Set(selectedArticleCodes.value)]
    if (!articleCodes.length) {
      submitState.error = "Izaberite bar jedan artikal."
      return null
    }

    return {
      code,
      payload: {
        percentage,
        validFrom,
        validTo,
        articleCodes,
      },
    }
  }

  async function saveDiscount() {
    resetMessages()
    if (!canEdit.value) return

    const built = buildPayload()
    if (!built) return

    submitState.loading = true
    const result = await api.updateDiscount(built.code, built.payload)
    submitState.loading = false

    if (!result.ok) {
      submitState.error = result.error || "Čuvanje popusta nije uspelo."
      return
    }

    const key = built.code
    const next: DiscountListItem = {
      key,
      code: built.code,
      percentage: built.payload.percentage,
      validFrom: built.payload.validFrom,
      validTo: built.payload.validTo,
      articleCodes: built.payload.articleCodes,
    }

    discounts.value = sortDiscounts([
      ...discounts.value.filter(
        (item) => item.key !== key && item.code !== built.code,
      ),
      next,
    ])

    applyDiscountToEditor(next)
    submitState.success = `Popust ${built.code} je uspešno sačuvan.`
  }

  async function removeDiscount() {
    deleteState.error = ""
    deleteState.success = ""
    if (!canEdit.value) return

    const code = normalizeCode(form.value.code)
    if (!code) {
      deleteState.error = "Unesite kod za brisanje."
      return
    }

    deleteState.loading = true
    const result = await api.deleteDiscount(code)
    deleteState.loading = false

    if (!result.ok) {
      deleteState.error = result.error || "Brisanje popusta nije uspelo."
      return
    }

    discounts.value = discounts.value.filter(
      (item) => item.code !== code && item.key !== code,
    )
    beginCreate()
    deleteConfirmOpen.value = false
    deleteState.success =
      result.data?.message || `Popust ${code} je uspešno uklonjen.`
  }

  onMounted(() => {
    beginCreate()
    void loadDiscounts()
  })

  return {
    canEdit,
    listState,
    submitState,
    deleteState,
    discounts,
    discountFilter,
    listPage,
    listPageSize,
    selectedKey,
    selectedDiscount,
    isCreateMode,
    form,
    selectedArticleCodes,
    filteredDiscountCount,
    filteredDiscountPages,
    visibleDiscounts,
    updateDiscountFilter,
    searchDiscounts,
    changeListPage,
    changeListPageSize,
    beginCreate,
    selectDiscount,
    saveDiscount,
    removeDiscount,
    deleteConfirmOpen,
    requestRemoveDiscount,
  }
}
