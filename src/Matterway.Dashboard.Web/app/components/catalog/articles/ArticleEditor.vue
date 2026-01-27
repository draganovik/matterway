<script setup lang="ts">
import { useCatalogApi, type GetArticleByIdResponse } from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'

const props = withDefaults(defineProps<{
  article?: GetArticleByIdResponse | null
  loading?: boolean
  error?: string
  canEdit?: boolean
}>(), {
  article: null,
  loading: false,
  error: '',
  canEdit: false
})

const emit = defineEmits<{
  (event: 'update:article', value: GetArticleByIdResponse | null): void
  (event: 'updated', value: GetArticleByIdResponse): void
}>()

const api = useCatalogApi()

const updateState = useRequestState()

type ArticleForm = {
  articleCode: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const form = ref<ArticleForm>({
  articleCode: '',
  title: '',
  basePrice: '',
  description: '',
  isAvailable: true
})

watch(
  () => props.article,
  (article) => {
    if (!article) return
    form.value = {
      articleCode: article.code || '',
      title: article.title || '',
      basePrice: article.basePrice ?? '',
      description: article.description || '',
      isAvailable: article.isAvailable
    }
    updateState.error = ''
    updateState.success = ''
  },
  { immediate: true }
)

function updateArticleData(patch: Partial<GetArticleByIdResponse>) {
  if (!props.article) return
  const next = { ...props.article, ...patch }
  emit('update:article', next)
  emit('updated', next)
}

async function saveArticle() {
  updateState.error = ''
  updateState.success = ''
  if (!props.article) return
  if (!props.canEdit) return
  updateState.loading = true
  const result = await api.updateArticle(props.article.id, {
    articleCode: form.value.articleCode || null,
    title: form.value.title || null,
    basePrice: form.value.basePrice || null,
    description: form.value.description || null,
    isAvailable: form.value.isAvailable
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Unable to update article.'
    return
  }
  updateArticleData({
    code: result.data?.articleCode ?? form.value.articleCode,
    title: result.data?.title ?? form.value.title,
    basePrice: result.data?.basePrice ?? form.value.basePrice,
    description: result.data?.description ?? form.value.description,
    isAvailable: result.data?.isAvailable ?? form.value.isAvailable,
    updatedAt: result.data?.updatedAt ?? props.article.updatedAt
  })
  updateState.success = 'Article updated.'
}

function updateDetails(details: GetArticleByIdResponse['details']) {
  updateArticleData({ details })
}

function updateSpecifications(specifications: GetArticleByIdResponse['specifications']) {
  updateArticleData({ specifications })
}

function updateImages(images: GetArticleByIdResponse['images']) {
  updateArticleData({ images })
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div
      v-if="error"
      class="rounded-lg border border-red-200/60 bg-red-50/60 px-4 py-3 text-sm text-red-600"
    >
      {{ error }}
    </div>

    <div
      v-else-if="loading"
      class="rounded-lg border border-default bg-background px-4 py-6 text-center text-sm text-muted"
    >
      Loading article.
    </div>

    <div
      v-else-if="!article"
      class="rounded-lg border border-default bg-background px-4 py-6 text-center text-sm text-muted"
    >
      Select an article to start editing.
    </div>

    <div
      v-else
      class="flex flex-col gap-6"
    >
      <div class="rounded-lg border border-default bg-background p-5">
        <div class="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h3 class="text-base font-semibold text-foreground">
              Article Fields
            </h3>
            <p class="text-sm text-muted">
              Update primary details and availability.
            </p>
          </div>
          <div class="text-sm text-muted">
            ID: {{ article.id }}
          </div>
        </div>

        <div class="mt-4">
          <ArticleFieldsForm
            v-model="form"
            :disabled="!canEdit"
          />
        </div>

        <div class="mt-4 flex flex-wrap items-center gap-3">
          <UButton
            size="md"
            color="primary"
            :loading="updateState.loading"
            :disabled="!canEdit"
            @click="saveArticle"
          >
            Update Article
          </UButton>
          <FormStatus
            :error="updateState.error"
            :success="updateState.success"
          />
        </div>
      </div>

      <div class="rounded-lg border border-default bg-background p-5">
        <ArticleImagesGroup
          :article-id="article.id"
          :model-value="article.images || []"
          :can-edit="canEdit"
          @update:model-value="updateImages"
        />
      </div>

      <div class="rounded-lg border border-default bg-background p-5">
        <ArticleDetailsSpecsGroup
          :article-id="article.id"
          :details="article.details || []"
          :specifications="article.specifications || []"
          :can-edit="canEdit"
          @update:details="updateDetails"
          @update:specifications="updateSpecifications"
        />
      </div>
    </div>
  </div>
</template>
