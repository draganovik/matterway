<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useArticleDetails } from '~/composables/useArticleDetails'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Article Details',
  service: 'catalog',
  level: 'operator',
  action: 'update'
})

const api = useApiClient()

const updateForm = reactive({
  articleId: '',
  detailSlug: '',
  value: ''
})
const updateState = useRequestState()

const { articlePreview, previewState, loadArticleDetails } = useArticleDetails()

async function updateDetail() {
  updateState.error = ''
  updateState.success = ''
  if (!updateForm.articleId || !updateForm.detailSlug) {
    updateState.error = 'Article Id and detail slug are required.'
    return
  }
  updateState.loading = true
  try {
    const result = await api.request(
      'catalog',
      `admin/articles/${updateForm.articleId}/details/${updateForm.detailSlug}`,
      {
        method: 'PATCH',
        body: JSON.stringify({ value: updateForm.value })
      }
    )
    if (!result.ok) {
      updateState.error = result.error || 'Failed to update detail.'
      return
    }
    updateState.success = 'Detail updated.'
    await loadArticleDetails(updateForm.articleId)
  } catch (err) {
    updateState.error = err instanceof Error ? err.message : 'Failed to update detail.'
  } finally {
    updateState.loading = false
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
