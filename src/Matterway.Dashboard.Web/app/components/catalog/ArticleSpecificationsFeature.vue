<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'

const api = useApiClient()
const { active } = useFeatureTabs()

const queryForm = reactive({
  articleId: ''
})

const createForm = reactive({
  articleId: '',
  specificationSlug: '',
  value: null as number | null
})
const createState = reactive({ loading: false, error: '', success: '' })

const updateForm = reactive({
  articleId: '',
  specificationSlug: '',
  value: null as number | null
})
const updateState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  articleId: '',
  specificationSlug: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

const specOptions = ref<any[]>([])
const specState = reactive({ loading: false, error: '' })

const articlePreview = ref<any | null>(null)
const previewState = reactive({ loading: false, error: '' })

async function loadSpecs() {
  specState.loading = true
  specState.error = ''
  specOptions.value = []
  const result = await api.request<any>('catalog', 'admin/specifications?limit=50', {
    method: 'GET'
  })
  specState.loading = false
  if (!result.ok) {
    specState.error = result.error || 'Failed to load specifications.'
    return
  }
  specOptions.value = result.data || []
}

async function loadArticleSpecs(articleId: string) {
  if (!articleId) return
  previewState.loading = true
  previewState.error = ''
  const result = await api.request<any>('catalog', `public/articles/${articleId}`, {}, true)
  previewState.loading = false
  if (!result.ok || !result.data) {
    previewState.error = result.error || 'Article not found.'
    return
  }
  articlePreview.value = result.data
}

async function createSpec() {
  createState.loading = true
  createState.error = ''
  createState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${createForm.articleId}/specifications`,
    {
      method: 'POST',
      body: JSON.stringify({
        specificationSlug: createForm.specificationSlug,
        value: createForm.value
      })
    }
  )
  createState.loading = false
  if (!result.ok) {
    createState.error = result.error || 'Failed to add specification.'
    return
  }
  createState.success = 'Specification added.'
  await loadArticleSpecs(createForm.articleId)
}

async function updateSpec() {
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${updateForm.articleId}/specifications/${updateForm.specificationSlug}`,
    {
      method: 'PATCH',
      body: JSON.stringify({
        value: updateForm.value
      })
    }
  )
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update specification.'
    return
  }
  updateState.success = 'Specification updated.'
  await loadArticleSpecs(updateForm.articleId)
}

async function deleteSpec() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${deleteForm.articleId}/specifications/${deleteForm.specificationSlug}`,
    {
      method: 'DELETE'
    }
  )
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete specification.'
    return
  }
  deleteState.success = 'Specification removed.'
  await loadArticleSpecs(deleteForm.articleId)
}
</script>

<template>
  <FeatureShell>
    <div class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard
        v-if="active === 'query'"
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            View article specifications
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="() => loadArticleSpecs(queryForm.articleId)"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="queryForm.articleId"
              placeholder="GUID"
              @blur="loadArticleSpecs(queryForm.articleId)"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="previewState.loading"
          >
            Load Specifications
          </UButton>
          <FormStatus :error="previewState.error" />
        </UForm>
      </UCard>

      <UCard
        v-else-if="active === 'create'"
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            Add specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createSpec"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="createForm.articleId"
              placeholder="GUID"
              @blur="loadArticleSpecs(createForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Specification"
            required
          >
            <USelectMenu
              v-model="createForm.specificationSlug"
              :items="specOptions"
              value-attribute="slug"
              option-attribute="title"
              placeholder="Select specification"
            />
          </UFormField>
          <UFormField
            label="Value"
            required
          >
            <UInput
              v-model.number="createForm.value"
              type="number"
              step="0.01"
            />
          </UFormField>
          <div class="flex flex-wrap gap-3">
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Add Specification
            </UButton>
            <UButton
              color="neutral"
              variant="outline"
              @click="loadSpecs"
            >
              Load Specifications
            </UButton>
          </div>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
          <FormStatus
            :loading="specState.loading"
            :error="specState.error"
          />
        </UForm>
      </UCard>

      <UCard
        v-else-if="active === 'update'"
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            Update specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateSpec"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="updateForm.articleId"
              placeholder="GUID"
              @blur="loadArticleSpecs(updateForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Specification Slug"
            required
          >
            <UInput
              v-model="updateForm.specificationSlug"
              placeholder="spec-slug"
            />
          </UFormField>
          <UFormField
            label="Value"
            required
          >
            <UInput
              v-model.number="updateForm.value"
              type="number"
              step="0.01"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update Specification
          </UButton>
          <FormStatus
            :error="updateState.error"
            :success="updateState.success"
          />
        </UForm>
      </UCard>

      <UCard
        v-else-if="active === 'delete'"
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            Remove specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteSpec"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="deleteForm.articleId"
              placeholder="GUID"
              @blur="loadArticleSpecs(deleteForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Specification Slug"
            required
          >
            <UInput
              v-model="deleteForm.specificationSlug"
              placeholder="spec-slug"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Specification
          </UButton>
          <FormStatus
            :error="deleteState.error"
            :success="deleteState.success"
          />
        </UForm>
      </UCard>

      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Article specifications
          </h3>
        </template>
        <FormStatus
          :loading="previewState.loading"
          :error="previewState.error"
        />
        <div
          v-if="articlePreview?.specifications?.length"
          class="space-y-2 text-sm"
        >
          <div
            v-for="spec in articlePreview.specifications"
            :key="spec.specificationSlug"
            class="rounded-lg border border-default px-3 py-2"
          >
            <p class="font-semibold">
              {{ spec.title || spec.specificationSlug }}
            </p>
            <p class="text-muted">
              {{ spec.value }} {{ spec.unit || '' }}
            </p>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Load an article to see specifications.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
