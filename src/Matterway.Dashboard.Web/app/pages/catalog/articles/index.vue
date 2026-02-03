<script setup lang="ts">
import { useCatalogApi, type GetArticleByIdResponse, type QueryArticleResponse } from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'
import { useAuthSession } from '~/composables/useAuthSession'

const auth = useAuthSession()
const api = useCatalogApi()

definePageMeta({
  title: 'Articles',
  service: 'catalog',
  level: 'observer',
  tabs: [
    { label: 'Browse', to: '/catalog/articles', exact: true, exactQuery: true },
    { label: 'Create New', to: { path: '/catalog/articles', query: { view: 'create' } }, exact: true, exactQuery: true }
  ]
})

type ArticleForm = {
  articleCode: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const canEdit = computed(() => auth.hasPermission('catalog', 'operator'))

const route = useRoute()
const activeTab = computed(() => (route.query.view === 'create' ? 'create' : 'browse'))

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

const createForm = ref<ArticleForm>({
  articleCode: '',
  title: '',
  basePrice: '',
  description: '',
  isAvailable: true
})
const createState = useRequestState()
const createdArticle = ref<GetArticleByIdResponse | null>(null)

const createStep = computed(() => (createdArticle.value ? 2 : 1))

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

async function createArticle() {
  createState.error = ''
  createState.success = ''
  if (!canEdit.value) return
  const payload = createForm.value
  const articleCode = payload.articleCode.trim()
  const title = payload.title.trim()
  const description = payload.description.trim()
  if (!articleCode || !title || !payload.basePrice || !description) {
    createState.error = 'Fill in all required fields before creating the article.'
    return
  }
  createState.loading = true
  const result = await api.createArticle({
    articleCode,
    title,
    basePrice: payload.basePrice,
    description,
    isAvailable: payload.isAvailable
  })
  createState.loading = false
  if (!result.ok || !result.data) {
    createState.error = result.error || 'Unable to create article.'
    return
  }
  const data = result.data
  createState.success = 'Article created.'
  await loadCreatedArticle(data.id)
  if (!articles.value.some(item => item.id === data.id)) {
    articles.value = [
      {
        id: data.id,
        code: data.articleCode,
        title: data.title,
        basePrice: data.basePrice,
        price: data.price,
        description: data.description,
        isAvailable: data.isAvailable
      },
      ...articles.value
    ]
  }
}

async function loadCreatedArticle(id: string) {
  const result = await api.getArticleById(id)
  if (result.ok) {
    createdArticle.value = result.data || null
  }
}

function resetCreate() {
  createdArticle.value = null
  createForm.value = {
    articleCode: '',
    title: '',
    basePrice: '',
    description: '',
    isAvailable: true
  }
  createState.error = ''
  createState.success = ''
}

function updateCreatedArticle(patch: Partial<GetArticleByIdResponse>) {
  if (!createdArticle.value) return
  createdArticle.value = { ...createdArticle.value, ...patch }
}

watch([() => pagination.page, () => pagination.pageSize], () => {
  void loadArticles()
})

onMounted(() => {
  void loadArticles()
})
</script>

<template>
  <FeatureShell>
    <div
      v-if="activeTab === 'browse'"
      class="space-y-6"
    >
      <EntitySplitView>
        <template #list>
          <EntityListPanel
            title="Browse Articles"
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
            @search="() => { pagination.page = 1; loadArticles() }"
            @update:page="(value) => (pagination.page = value)"
            @update:page-size="(value) => { pagination.pageSize = value; pagination.page = 1 }"
            @select="selectArticle"
          >
            <template #item="{ item }">
              <div class="flex flex-wrap items-center justify-between gap-1">
                <p class="text-base w-full font-medium text-foreground">
                  {{ item.title || 'Untitled Article' }}
                </p>
                <p class="text-sm text-muted">
                  {{ item.code || 'No code' }}
                </p>
              </div>

              <div class="mt-2 text-sm flex gap-2 place-items-center justify-between text-muted">
                <UBadge
                  :color="item.isAvailable ? 'success' : 'neutral'"
                  variant="subtle"
                >
                  {{ item.isAvailable ? 'Available' : 'Unavailable' }}
                </UBadge> Price: {{ item.price ?? item.basePrice ?? 'N/A' }}
              </div>
            </template>
          </EntityListPanel>
        </template>

        <template #detail>
          <ArticleEditor
            :article="selectedArticle"
            :loading="articleState.loading"
            :error="articleState.error"
            :can-edit="canEdit"
            @update:article="updateSelectedArticle"
          />
        </template>
      </EntitySplitView>
    </div>

    <div
      v-else
      class="space-y-6"
    >
      <div class="rounded-lg border border-default bg-background p-5">
        <div class="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h2 class="text-base font-semibold text-foreground">
              Create Article (Step {{ createStep }} of 2)
            </h2>
            <p class="text-sm text-muted">
              Start with the core article, then attach images and details.
            </p>
          </div>
          <UButton
            v-if="createdArticle"
            variant="outline"
            @click="resetCreate"
          >
            Create Another
          </UButton>
        </div>

        <div class="mt-4">
          <ArticleFieldsForm
            v-model="createForm"
            :disabled="!canEdit || Boolean(createdArticle)"
          />
        </div>

        <div class="mt-4 flex flex-wrap items-center gap-3">
          <UButton
            size="md"
            color="primary"
            :loading="createState.loading"
            :disabled="!canEdit || Boolean(createdArticle)"
            @click="createArticle"
          >
            Create Article
          </UButton>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
        </div>
      </div>

      <div class="rounded-lg border border-default bg-background p-5">
        <ArticleImagesGroup
          :article-id="createdArticle?.id || null"
          :model-value="createdArticle?.images || []"
          :can-edit="canEdit"
          @update:model-value="(images) => updateCreatedArticle({ images })"
        />
      </div>

      <div class="rounded-lg border border-default bg-background p-5">
        <ArticleDetailsSpecsGroup
          :article-id="createdArticle?.id || null"
          :details="createdArticle?.details || []"
          :can-edit="canEdit"
          @update:details="(details) => updateCreatedArticle({ details })"
        />
      </div>
    </div>
  </FeatureShell>
</template>
