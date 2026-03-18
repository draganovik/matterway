import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
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

  const details = ref<QueryDetailResponse[]>([])
  const filter = ref("")
  const limit = ref(20)

  const selectedSlug = ref<string | null>(null)
  const selectedDetail = ref<QueryDetailResponse | null>(null)
  const createModalOpen = ref(false)

  const form = ref<DetailForm>({
    slug: "",
    title: "",
    unit: "",
  })

  const canDelete = computed(() => Boolean(selectedSlug.value))

  function sortDetails(items: QueryDetailResponse[]) {
    return [...items].sort((a, b) => {
      const left = (a.title || a.slug || "").toLowerCase()
      const right = (b.title || b.slug || "").toLowerCase()
      return left.localeCompare(right)
    })
  }

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
      limit: Math.min(50, Math.max(1, limit.value)),
      titleLike: String(filter.value ?? "").trim() || undefined,
    })

    listState.loading = false

    if (!result.ok) {
      listState.error =
        result.error || "Učitavanje definicija detalja nije uspelo."
      details.value = []
      return
    }

    details.value = sortDetails(result.data || [])

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

  function selectDetail(slug: string) {
    const match = details.value.find((item) => item.slug === slug)
    if (!match) return

    selectedSlug.value = slug
    selectedDetail.value = match
    applyDetailToForm(match)
    resetMessages()
  }

  function searchDetails() {
    void loadDetails()
  }

  function updateLimit(value: number) {
    const next = Math.min(50, Math.max(1, Number(value) || 20))
    if (next === limit.value) return
    limit.value = next
    void loadDetails()
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

    const withoutCurrent = details.value.filter(
      (item) => item.slug !== nextDetail.slug,
    )
    details.value = sortDetails([...withoutCurrent, nextDetail])

    selectedSlug.value = nextDetail.slug || slug
    selectedDetail.value = nextDetail
    applyDetailToForm(nextDetail)

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

    details.value = details.value.filter((item) => item.slug !== slug)
    selectedSlug.value = null
    selectedDetail.value = null
    applyDetailToForm(null)

    removeState.success = result.data?.message || "Detalj je uspešno uklonjen."
  }

  function handleDetailCreated(detail: QueryDetailResponse) {
    const next = details.value.filter((item) => item.slug !== detail.slug)
    details.value = sortDetails([...next, detail])

    const slug = detail.slug || ""
    if (!slug) return

    selectedSlug.value = slug
    selectedDetail.value = detail
    applyDetailToForm(detail)
  }

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
    limit,
    selectedSlug,
    selectedDetail,
    createModalOpen,
    form,
    canDelete,
    beginCreate,
    selectDetail,
    searchDetails,
    updateLimit,
    saveDetail,
    removeDetail,
    handleDetailCreated,
  }
}
