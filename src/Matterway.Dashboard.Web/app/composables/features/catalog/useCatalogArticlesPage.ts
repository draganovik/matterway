import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { usePaginationState } from "~/composables/workflows/pagination/usePaginationState"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type {
  CreateArticleResponse,
  GetArticleByIdResponse,
  QueryArticleResponse,
} from "~/types/catalog"

export function useCatalogArticlesPage() {
  const auth = useAuthSessionStore()
  const api = useCatalogClient()

  const canEdit = computed(() =>
    auth.hasPermission("catalog", ["operator", "manager"]),
  )

  const listState = useRequestState({ empty: "No articles found." })
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
  } = usePaginationState({ pageSize: 20 })

  const selectedId = ref<string | null>(null)
  const selectedArticle = ref<GetArticleByIdResponse | null>(null)
  const articleState = useRequestState()

  const createModalOpen = ref(false)

  async function loadArticles() {
    listState.loading = true
    listState.error = ""

    const result = await api.queryArticles({
      filter: filter.value.trim() || undefined,
      page: pagination.page,
      pageSize: pagination.pageSize,
    })

    listState.loading = false

    if (!result.ok) {
      listState.error = result.error || "Unable to load articles."
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
      selectedId.value &&
      !articles.value.some((item) => item.id === selectedId.value)
    ) {
      selectedId.value = null
      selectedArticle.value = null
      articleState.error = ""
    }
  }

  async function loadArticle(id: string) {
    articleState.loading = true
    articleState.error = ""

    const result = await api.getArticleById(id)

    articleState.loading = false

    if (!result.ok) {
      articleState.error = result.error || "Unable to load article."
      selectedArticle.value = null
      return
    }

    selectedArticle.value = result.data || null
  }

  function selectArticle(id: string) {
    selectedId.value = id
    void loadArticle(id)
  }

  function updateSelectedArticle(article: GetArticleByIdResponse | null) {
    selectedArticle.value = article
    if (!article) return

    articles.value = articles.value.map((item) =>
      item.id === article.id
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

  function handleArticleCreated(created: CreateArticleResponse) {
    if (!articles.value.some((item) => item.id === created.id)) {
      articles.value = [
        {
          id: created.id,
          code: created.articleCode,
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

    selectedId.value = created.id
    void loadArticle(created.id)
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
    selectedId,
    selectedArticle,
    articleState,
    createModalOpen,
    selectArticle,
    updateSelectedArticle,
    handleArticleCreated,
    searchArticles,
  }
}
