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
  detailSlug: '',
  value: ''
})
const createState = reactive({ loading: false, error: '', success: '' })

const updateForm = reactive({
  articleId: '',
  detailSlug: '',
  value: ''
})
const updateState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  articleId: '',
  detailSlug: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

const detailOptions = ref<any[]>([])
const detailState = reactive({ loading: false, error: '' })

const articlePreview = ref<any | null>(null)
const previewState = reactive({ loading: false, error: '' })

async function loadDetails() {
  detailState.loading = true
  detailState.error = ''
  detailOptions.value = []
  const result = await api.request<any>('catalog', 'admin/details?limit=50', {
    method: 'GET'
  })
  detailState.loading = false
  if (!result.ok) {
    detailState.error = result.error || 'Failed to load details.'
    return
  }
  detailOptions.value = result.data || []
}

async function loadArticleDetails(articleId: string) {
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

async function createDetail() {
  createState.loading = true
  createState.error = ''
  createState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${createForm.articleId}/details`,
    {
      method: 'POST',
      body: JSON.stringify({
        detailSlug: createForm.detailSlug,
        value: createForm.value
      })
    }
  )
  createState.loading = false
  if (!result.ok) {
    createState.error = result.error || 'Failed to add detail.'
    return
  }
  createState.success = 'Detail added.'
  await loadArticleDetails(createForm.articleId)
}

async function updateDetail() {
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${updateForm.articleId}/details/${updateForm.detailSlug}`,
    {
      method: 'PATCH',
      body: JSON.stringify({
        value: updateForm.value
      })
    }
  )
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update detail.'
    return
  }
  updateState.success = 'Detail updated.'
  await loadArticleDetails(updateForm.articleId)
}

async function deleteDetail() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${deleteForm.articleId}/details/${deleteForm.detailSlug}`,
    {
      method: 'DELETE'
    }
  )
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete detail.'
    return
  }
  deleteState.success = 'Detail removed.'
  await loadArticleDetails(deleteForm.articleId)
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
            View article details
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="() => loadArticleDetails(queryForm.articleId)"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="queryForm.articleId"
              placeholder="GUID"
              @blur="loadArticleDetails(queryForm.articleId)"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="previewState.loading"
          >
            Load Details
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
            Add detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createDetail"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="createForm.articleId"
              placeholder="GUID"
              @blur="loadArticleDetails(createForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Detail Slug"
            required
          >
            <USelectMenu
              v-model="createForm.detailSlug"
              :items="detailOptions"
              value-attribute="slug"
              option-attribute="title"
              placeholder="Select detail"
            />
          </UFormField>
          <UFormField
            label="Value"
            required
          >
            <UInput v-model="createForm.value" />
          </UFormField>
          <div class="flex flex-wrap gap-3">
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Add Detail
            </UButton>
            <UButton
              color="neutral"
              variant="outline"
              @click="loadDetails"
            >
              Load Detail Definitions
            </UButton>
          </div>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
          <FormStatus
            :loading="detailState.loading"
            :error="detailState.error"
          />
        </UForm>
      </UCard>

      <UCard
        v-else-if="active === 'update'"
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            Update detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateDetail"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="updateForm.articleId"
              placeholder="GUID"
              @blur="loadArticleDetails(updateForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Detail Slug"
            required
          >
            <UInput
              v-model="updateForm.detailSlug"
              placeholder="detail-slug"
            />
          </UFormField>
          <UFormField
            label="Value"
            required
          >
            <UInput v-model="updateForm.value" />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update Detail
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
            Remove detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteDetail"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="deleteForm.articleId"
              placeholder="GUID"
              @blur="loadArticleDetails(deleteForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Detail Slug"
            required
          >
            <UInput
              v-model="deleteForm.detailSlug"
              placeholder="detail-slug"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Detail
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
            Article details
          </h3>
        </template>
        <FormStatus
          :loading="previewState.loading"
          :error="previewState.error"
        />
        <div
          v-if="articlePreview?.details?.length"
          class="space-y-2 text-sm"
        >
          <div
            v-for="detail in articlePreview.details"
            :key="detail.detailSlug"
            class="rounded-lg border border-default px-3 py-2"
          >
            <p class="font-semibold">
              {{ detail.title || detail.detailSlug }}
            </p>
            <p class="text-muted">
              {{ detail.value }}
            </p>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Load an article to see details.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
