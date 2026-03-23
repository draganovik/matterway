import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { usePaginationState } from "~/composables/workflows/pagination/usePaginationState"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { normalizeSlug } from "~/utils/normalization"
import type { QueryDetailResponse } from "~/types/catalog"

type DetailForm = {
  slug: string
  title: string
  unit: string
}

export function useCatalogDetailsPage() {
  const auth = useAuthSessionStore()
  const api = useCatalogClient()

  const canEdit = computed(() =>
    auth.hasPermission("catalog", ["operator", "manager"]),
  )

  const listState = useRequestState({ empty: "Nema definicija detalja." })
  const saveState = useRequestState()
  const removeState = useRequestState()
  const deleteConfirmOpen = ref(false)

  const details = ref<QueryDetailResponse[]>([])
  const filter = ref("")
  const {
    pagination,
    resetTotals,
    applyMeta,
    changePage,
    changePageSize,
    searchWithPageReset,
    watchPagination,
  } = usePaginationState({ pageSize: DEFAULT_PAGINATION_PAGE_SIZE })

  const selectedSlug = ref<string | null>(null)
  const selectedDetail = ref<QueryDetailResponse | null>(null)
  const createModalOpen = ref(false)

  const form = ref<DetailForm>({
    slug: "",
    title: "",
    unit: "",
  })

  const canDelete = computed(() => Boolean(selectedSlug.value))

  function applyDetailToForm(detail: QueryDetailResponse | null) {
    if (!detail) {
      form.value = {
        slug: "",
        title: "",
        unit: "",
      }
      return
    }

    form.value = {
      slug: detail.slug || "",
      title: detail.title || "",
      unit: detail.unit || "",
    }
  }

  function clearSelectedDetail() {
    selectedSlug.value = null
    selectedDetail.value = null
    applyDetailToForm(null)
  }

  function resetDetailsList() {
    details.value = []
    resetTotals()
    clearSelectedDetail()
  }

  function resetMessages() {
    saveState.error = ""
    saveState.success = ""
    removeState.error = ""
    removeState.success = ""
  }

  async function loadDetails() {
    listState.loading = true
    listState.error = ""

    const result = await api.queryDetails({
      page: pagination.page,
      pageSize: pagination.pageSize,
      titleLike: String(filter.value ?? "").trim() || undefined,
    })

    listState.loading = false

    if (!result.ok) {
      listState.error =
        result.error || "Učitavanje definicija detalja nije uspelo."
      resetDetailsList()
      return
    }

    if (result.status === 204 || !result.data) {
      resetDetailsList()
      return
    }

    details.value = result.data.data || []
    applyMeta(result.data.meta, details.value.length)

    if (selectedSlug.value) {
      const match =
        details.value.find((item) => item.slug === selectedSlug.value) || null
      selectedDetail.value = match
      if (!match) {
        selectedSlug.value = null
        applyDetailToForm(null)
        return
      }
      applyDetailToForm(match)
    }
  }

  function beginCreate() {
    resetMessages()
    createModalOpen.value = true
  }

  function requestRemoveDetail() {
    resetMessages()

    if (!canEdit.value) return

    const slug = selectedSlug.value || normalizeSlug(form.value.slug)
    if (!slug) {
      removeState.error = "Izaberite detalj za brisanje."
      return
    }

    deleteConfirmOpen.value = true
  }

  function selectDetail(slug: string) {
    const match = details.value.find((item) => item.slug === slug)
    if (!match) return

    selectedSlug.value = slug
    selectedDetail.value = match
    applyDetailToForm(match)
    resetMessages()
  }

  function searchDetails() {
    searchWithPageReset(loadDetails)
  }

  async function saveDetail() {
    resetMessages()

    if (!canEdit.value) return

    const slug = normalizeSlug(selectedSlug.value || "")
    const title = String(form.value.title ?? "").trim()
    const unit = String(form.value.unit ?? "").trim()

    if (!slug) {
      saveState.error = "Izaberite detalj za ažuriranje."
      return
    }
    if (!title) {
      saveState.error = "Naziv je obavezan."
      return
    }

    saveState.loading = true

    const result = await api.putDetail(slug, {
      title,
      unit: unit || null,
    })

    saveState.loading = false

    if (!result.ok) {
      saveState.error = result.error || "Čuvanje detalja nije uspelo."
      return
    }

    const nextDetail: QueryDetailResponse = {
      slug: result.data?.slug ?? slug,
      title: result.data?.title ?? title,
      unit: result.data?.unit ?? (unit || null),
    }
    const targetSlug = nextDetail.slug || slug
    selectedSlug.value = targetSlug

    await loadDetails()

    const refreshedDetail =
      details.value.find((item) => item.slug === targetSlug) || nextDetail
    selectedSlug.value = targetSlug
    selectedDetail.value = refreshedDetail
    applyDetailToForm(refreshedDetail)

    saveState.success = "Detalj je uspešno ažuriran."
  }

  async function removeDetail() {
    resetMessages()

    if (!canEdit.value) return

    const slug = selectedSlug.value || normalizeSlug(form.value.slug)
    if (!slug) {
      removeState.error = "Izaberite detalj za brisanje."
      return
    }

    removeState.loading = true
    const result = await api.deleteDetail(slug)
    removeState.loading = false

    if (!result.ok) {
      removeState.error = result.error || "Brisanje detalja nije uspelo."
      return
    }

    const wasLastItemOnPage = details.value.length === 1
    clearSelectedDetail()
    deleteConfirmOpen.value = false

    if (wasLastItemOnPage && pagination.page > 1) {
      changePage(pagination.page - 1)
    } else {
      await loadDetails()
    }

    removeState.success = result.data?.message || "Detalj je uspešno uklonjen."
  }

  async function handleDetailCreated(detail: QueryDetailResponse) {
    const slug = detail.slug || ""
    selectedSlug.value = slug || null

    await loadDetails()

    if (!slug) return
    const createdDetail =
      details.value.find((item) => item.slug === slug) || detail
    selectedSlug.value = slug
    selectedDetail.value = createdDetail
    applyDetailToForm(createdDetail)
  }

  watchPagination(loadDetails)

  onMounted(() => {
    void loadDetails()
  })

  return {
    canEdit,
    listState,
    saveState,
    removeState,
    details,
    filter,
    pagination,
    changePage,
    changePageSize,
    selectedSlug,
    selectedDetail,
    createModalOpen,
    deleteConfirmOpen,
    form,
    canDelete,
    beginCreate,
    requestRemoveDetail,
    selectDetail,
    searchDetails,
    saveDetail,
    removeDetail,
    handleDetailCreated,
  }
}
