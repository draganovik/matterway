<script setup lang="ts">
import { useCatalogApi } from "~/composables/useCatalogApi"
import type {
  CreateArticleResponse,
  GetArticleByIdResponse,
  QueryArticleResponse,
} from "~/types/catalog"
import { useRequestState } from "~/composables/useRequestState"
import { useWorkspacePagination } from "~/composables/useWorkspacePagination"
import { useAuthSession } from "~/composables/useAuthSession"

const auth = useAuthSession()
const api = useCatalogApi()

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
} = useWorkspacePagination({ pageSize: 20 })

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
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="flex shrink-0 flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-foreground text-base font-semibold">Browse Articles</h2>
        <p class="text-muted text-sm">
          Search existing articles and enrich a selected article with images and
          details.
        </p>
      </div>

      <UButton
        color="primary"
        :disabled="!canEdit"
        @click="createModalOpen = true"
      >
        Create New
      </UButton>
    </div>

    <EntitiesSplitView
      class="min-h-0 flex-1"
      list-class="overflow-y-auto"
      detail-class="overflow-y-auto"
      :detail-loading="articleState.loading"
    >
      <template #list>
        <EntitiesListPanel
          title="Articles"
          description="Use RSQL filters to locate articles by title, code, or attributes."
          :items="articles"
          item-key="id"
          item-title-key="title"
          item-subtitle-key="code"
          :selected-id="selectedId"
          :filter="filter"
          :loading="listState.loading"
          :error="listState.error"
          :empty-message="listState.empty"
          :page="pagination.page"
          :page-size="pagination.pageSize"
          :total-count="pagination.totalCount"
          :total-pages="pagination.totalPages"
          @update:filter="(value) => (filter = value)"
          @search="searchArticles"
          @update:page="changePage"
          @update:page-size="changePageSize"
          @select="selectArticle"
        >
          <template #item="{ item }">
            <CatalogArticlesListItem :item="item" />
          </template>
        </EntitiesListPanel>
      </template>

      <template #detail>
        <CatalogArticlesPanelInformationView
          :article="selectedArticle"
          :error="articleState.error"
          :can-edit="canEdit"
          @update:article="updateSelectedArticle"
        />
      </template>
    </EntitiesSplitView>
  </div>

  <CatalogArticlesModalInformationManager
    v-model:open="createModalOpen"
    :can-edit="canEdit"
    @created="handleArticleCreated"
  />
</template>
