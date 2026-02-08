<script setup lang="ts">
import { useCatalogApi, type CreateArticleResponse } from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'

type ArticleBaseForm = {
  articleCode: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const { canEdit = false } = defineProps<{
  canEdit?: boolean
}>()

const emit = defineEmits<{
  created: [article: CreateArticleResponse]
}>()

const isOpen = defineModel<boolean>('open', { required: true })

const api = useCatalogApi()
const createState = useRequestState()

const form = ref<ArticleBaseForm>({
  articleCode: '',
  title: '',
  basePrice: '',
  description: '',
  isAvailable: true
})

function resetForm() {
  form.value = {
    articleCode: '',
    title: '',
    basePrice: '',
    description: '',
    isAvailable: true
  }
  createState.error = ''
}

watch(isOpen, (open) => {
  if (open) resetForm()
})

async function createArticle() {
  createState.error = ''
  if (!canEdit) return

  const payload = form.value
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

  emit('created', result.data)
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-base font-semibold text-foreground">
          Create New Article
        </h3>
        <p class="text-sm text-muted">
          Save core article data, then continue editing it in the browse view.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <CatalogArticlesBaseForm
          v-model="form"
          :disabled="!canEdit || createState.loading"
        />
        <FormStatus :error="createState.error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="createState.loading"
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canEdit"
          @click="createArticle"
        >
          Create Article
        </UButton>
      </div>
    </template>
  </UModal>
</template>
