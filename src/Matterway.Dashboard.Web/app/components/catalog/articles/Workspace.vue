<script setup lang="ts">
import { useCatalogApi, type CreateArticleResponse, type GetArticleByIdResponse, type QueryArticleResponse } from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'
import { useAuthSession } from '~/composables/useAuthSession'

const auth = useAuthSession()
const api = useCatalogApi()

const canEdit = computed(() => auth.hasPermission('catalog', 'operator'))

const listState = useRequestState({ empty: 'No articles found.' })
const articles = ref<QueryArticleResponse[]>([])
const filter = ref('')
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1
})

const selectedId = ref<string | null>(null)
const selectedArticle = ref<GetArticleByIdResponse | null>(null)
const articleState = useRequestState()

const createModalOpen = ref(false)

function parseNumber(value: number | string | undefined, fallback: number) {
  if (value === undefined || value === null || value === '') return fallback
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : fallback
}

async function loadArticles() {
  listState.loading = true
  listState.error = ''

  const result = await api.queryArticles({
    filter: filter.value.trim() || undefined,
    page: pagination.page,
    pageSize: pagination.pageSize
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || 'Unable to load articles.'
    articles.value = []
    return
  }

  if (result.status === 204 || !result.data) {
    articles.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    return
  }

  articles.value = result.data.data || []
  pagination.totalCount = parseNumber(result.data.meta?.totalCount, articles.value.length)
  pagination.totalPages = Math.max(1, parseNumber(result.data.meta?.totalPages, 1))
  pagination.page = Math.max(1, parseNumber(result.data.meta?.currentPage, pagination.page))
  pagination.pageSize = Math.max(1, parseNumber(result.data.meta?.pageSize, pagination.pageSize))

  if (selectedId.value && !articles.value.some(item => item.id === selectedId.value)) {
    selectedId.value = null
    selectedArticle.value = null
    articleState.error = ''
  }
}

async function loadArticle(id: string) {
  articleState.loading = true
  articleState.error = ''

  const result = await api.getArticleById(id)

  articleState.loading = false

  if (!result.ok) {
    articleState.error = result.error || 'Unable to load article.'
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

  articles.value = articles.value.map(item =>
    item.id === article.id
      ? {
          ...item,
          title: article.title,
          code: article.code,
          basePrice: article.basePrice,
          price: article.price,
          description: article.description,
          isAvailable: article.isAvailable
        }
      : item
  )
}

function handleArticleCreated(created: CreateArticleResponse) {
  if (!articles.value.some(item => item.id === created.id)) {
    articles.value = [
      {
        id: created.id,
        code: created.articleCode,
        title: created.title,
        basePrice: created.basePrice,
        price: created.price,
        description: created.description,
        isAvailable: created.isAvailable
      },
      ...articles.value
    ]
    pagination.totalCount += 1
  }

  selectedId.value = created.id
  void loadArticle(created.id)
}

function searchArticles() {
  if (pagination.page !== 1) {
    pagination.page = 1
    return
  }
  void loadArticles()
}

function changePage(page: number) {
  pagination.page = page
}

function changePageSize(pageSize: number) {
  pagination.pageSize = pageSize
  pagination.page = 1
}

watch([() => pagination.page, () => pagination.pageSize], () => {
  void loadArticles()
})

onMounted(() => {
  void loadArticles()
})
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="shrink-0 flex flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-base font-semibold text-foreground">
          Browse Articles
        </h2>
        <p class="text-sm text-muted">
          Search existing articles and enrich a selected article with images and details.
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
        <CatalogArticlesPanel
          :article="selectedArticle"
          :loading="articleState.loading"
          :error="articleState.error"
          :can-edit="canEdit"
          @update:article="updateSelectedArticle"
        />
      </template>
    </EntitiesSplitView>
  </div>

  <CatalogArticlesCreateModal
    v-model:open="createModalOpen"
    :can-edit="canEdit"
    @created="handleArticleCreated"
  />
</template>
