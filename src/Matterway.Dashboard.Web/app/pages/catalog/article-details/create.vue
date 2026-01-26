<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useArticleDetails } from '~/composables/useArticleDetails'
import { useRequestState } from '~/composables/useRequestState'
import { normalizeList } from '~/utils/http'

definePageMeta({
  title: 'Article Details',
  service: 'catalog',
  level: 'operator',
  action: 'create'
})

const api = useApiClient()

const createForm = reactive({
  articleId: '',
  detailSlug: '',
  value: ''
})
const createState = useRequestState()

const detailOptions = ref<Array<{ slug: string, title: string }>>([])
const detailState = useRequestState()

const { articlePreview, previewState, loadArticleDetails } = useArticleDetails()

async function loadDetails() {
  detailState.error = ''
  detailState.loading = true
  try {
    const result = await api.request<unknown>('catalog', 'admin/details')
    if (!result.ok) {
      detailState.error = result.error || 'Failed to load detail definitions.'
      detailOptions.value = []
      return
    }
    detailOptions.value = normalizeList(result.data)
  } catch (err) {
    detailState.error = err instanceof Error ? err.message : 'Failed to load detail definitions.'
    detailOptions.value = []
  } finally {
    detailState.loading = false
  }
}

async function createDetail() {
  createState.error = ''
  createState.success = ''
  if (!createForm.articleId || !createForm.detailSlug) {
    createState.error = 'Article Id and detail slug are required.'
    return
  }
  createState.loading = true
  try {
    const result = await api.request(
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
    if (!result.ok) {
      createState.error = result.error || 'Failed to add detail.'
      return
    }
    createState.success = 'Detail added.'
    await loadArticleDetails(createForm.articleId)
  } catch (err) {
    createState.error = err instanceof Error ? err.message : 'Failed to add detail.'
  } finally {
    createState.loading = false
  }
}
</script>

<template>
  <FeatureShell>
    <div class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard
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
