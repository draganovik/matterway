<script setup lang="ts">
import { useCatalogApi } from '~/composables/useCatalogApi'
import type { GetArticleByIdResponse } from '~/types/catalog'
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

function updateImages(images: GetArticleByIdResponse['images']) {
  updateArticleData({ images })
}
</script>

<template>
  <div class="flex flex-col gap-5">
    <StatusMessages
      v-if="error"
      :error="error"
    />

    <StatusMessages
      v-else-if="loading"
      loading="Loading article."
    />

    <div
      v-else-if="!article"
      class="space-y-4"
    >
      <div class="space-y-1">
        <h3 class="text-base font-semibold text-foreground">
          Article Editor
        </h3>
        <p class="text-sm text-muted">
          {{ canEdit ? 'Operator permission is required for create, update, and delete.' : 'Read-only mode: operator permission required for changes.' }}
        </p>
      </div>

      <EntitiesEmptyState
        title="Nothing selected"
        description="Select an item from the list to start editing."
      />
    </div>

    <div
      v-else
      class="grid gap-5"
    >
      <section>
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
            ID: {{ article?.id }}
          </div>
        </div>

        <div class="mt-4">
          <CatalogArticlesBaseForm
            v-model="form"
            :disabled="!canEdit"
          />
        </div>

        <div
          class="mt-4 flex flex-wrap items-center gap-3"
        >
          <UButton
            size="lg"
            color="primary"
            :loading="updateState.loading"
            :disabled="!canEdit"
            @click="saveArticle"
          >
            Save Changes
          </UButton>
          <StatusMessages
            :error="updateState.error"
            :success="updateState.success"
          />
        </div>
      </section>

      <section class="space-y-3">
        <CatalogArticlesImagesView
          :article-id="article?.id ?? null"
          :model-value="article?.images || []"
          :can-edit="canEdit"
          @update:model-value="updateImages"
        />
      </section>

      <section class="space-y-3">
        <CatalogArticlesDetailsView
          :article-id="article?.id ?? null"
          :details="article?.details || []"
          :can-edit="canEdit"
          @update:details="updateDetails"
        />
      </section>
    </div>
  </div>
</template>
