import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { usePaginationState } from "~/composables/workflows/pagination/usePaginationState"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type {
  CreateArticleResponse,
  GetArticleResponse,
  QueryArticleResponse,
} from "~/types/catalog"

export function useCatalogArticlesPage() {
  const auth = useAuthSessionStore()
  const api = useCatalogClient()

  const canEdit = computed(() =>
    auth.hasPermission("catalog", ["operator", "manager"]),
  )

  const listState = useRequestState("Nema artikala.", true)
  const articles = ref<QueryArticleResponse[]>([])
  const filter = ref("")
  const {
    pagination,
    resetTotals,
    applyMeta,
    changePage,
    changePageSize,
    searchWithPageReset,
    watchPagination,
  } = usePaginationState()

  const selectedCode = ref<string | null>(null)
  const selectedArticle = ref<GetArticleResponse | null>(null)
  const articleState = useRequestState()
  const removeState = useRequestState()
  const deleteConfirmOpen = ref(false)

  const createModalOpen = ref(false)

  async function loadArticles() {
    listState.loading = true
    listState.error = ""

    const result = await api.queryArticles({
      filter: String(filter.value ?? "").trim() || undefined,
      page: pagination.page,
      pageSize: pagination.pageSize,
    })

    listState.loading = false

    if (!result.ok) {
      listState.error = result.error || "Učitavanje artikala nije uspelo."
      articles.value = []
      return
    }

    if (result.status === 204 || !result.data) {
      articles.value = []
      resetTotals()
      return
    }

    articles.value = result.data.data || []
    applyMeta(result.data.meta, articles.value.length)

    if (
      selectedCode.value &&
      !articles.value.some((item) => item.code === selectedCode.value)
    ) {
      selectedCode.value = null
      selectedArticle.value = null
      articleState.error = ""
    }
  }

  async function loadArticle(code: string) {
    articleState.loading = true
    articleState.error = ""

    const result = await api.getArticle(code)

    articleState.loading = false

    if (!result.ok) {
      articleState.error = result.error || "Učitavanje artikla nije uspelo."
      selectedArticle.value = null
      return
    }

    selectedArticle.value = result.data || null
  }

  function selectArticle(code: string) {
    selectedCode.value = code
    void loadArticle(code)
  }

  function updateSelectedArticle(article: GetArticleResponse | null) {
    selectedArticle.value = article
    if (!article) {
      selectedCode.value = null
      return
    }

    const previousCode = selectedCode.value || article.code
    selectedCode.value = article.code

    articles.value = articles.value.map((item) =>
      item.code === previousCode
        ? {
            ...item,
            title: article.title,
            code: article.code,
            basePrice: article.basePrice,
            price: article.price,
            description: article.description,
            isAvailable: article.isAvailable,
          }
        : item,
    )
  }

  function requestRemoveArticle() {
    removeState.error = ""
    removeState.success = ""
    if (!canEdit.value) return
    if (!selectedArticle.value?.code) {
      removeState.error = "Izaberite artikal za brisanje."
      return
    }
    deleteConfirmOpen.value = true
  }

  async function removeArticle() {
    removeState.error = ""
    removeState.success = ""

    if (!canEdit.value) return

    const code = selectedArticle.value?.code || selectedCode.value
    if (!code) {
      removeState.error = "Izaberite artikal za brisanje."
      return
    }

    removeState.loading = true
    const result = await api.deleteArticle(code)
    removeState.loading = false

    if (!result.ok) {
      removeState.error = result.error || "Brisanje artikla nije uspelo."
      return
    }

    articles.value = articles.value.filter((item) => item.code !== code)
    selectedCode.value = null
    selectedArticle.value = null
    articleState.error = ""
    deleteConfirmOpen.value = false

    if (articles.value.length === 0 && pagination.page > 1) {
      pagination.page -= 1
    } else {
      void loadArticles()
    }

    removeState.success = result.data?.message || "Artikal je uspešno obrisan."
  }

  function handleArticleCreated(created: CreateArticleResponse) {
    if (!articles.value.some((item) => item.code === created.code)) {
      articles.value = [
        {
          code: created.code,
          title: created.title,
          basePrice: created.basePrice,
          price: created.price,
          description: created.description,
          isAvailable: created.isAvailable,
        },
        ...articles.value,
      ]
      pagination.totalCount += 1
    }

    selectedCode.value = created.code
    void loadArticle(created.code)
  }

  function searchArticles() {
    searchWithPageReset(loadArticles)
  }

  watchPagination(loadArticles)

  onMounted(() => {
    void loadArticles()
  })

  return {
    canEdit,
    listState,
    articles,
    filter,
    pagination,
    changePage,
    changePageSize,
    selectedCode,
    selectedArticle,
    articleState,
    removeState,
    deleteConfirmOpen,
    createModalOpen,
    selectArticle,
    updateSelectedArticle,
    requestRemoveArticle,
    removeArticle,
    handleArticleCreated,
    searchArticles,
  }
}
