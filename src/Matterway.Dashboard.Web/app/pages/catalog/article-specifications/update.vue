<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useArticleSpecifications } from '~/composables/useArticleSpecifications'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Article Specifications',
  service: 'catalog',
  level: 'operator',
  action: 'update'
})

const api = useApiClient()

const updateForm = reactive({
  articleId: '',
  specificationSlug: '',
  value: 0
})
const updateState = useRequestState()

const { articlePreview, previewState, loadArticleSpecs } = useArticleSpecifications()

async function updateSpec() {
  updateState.error = ''
  updateState.success = ''
  if (!updateForm.articleId || !updateForm.specificationSlug) {
    updateState.error = 'Article Id and specification are required.'
    return
  }
  updateState.loading = true
  try {
    const result = await api.request(
      'catalog',
      `admin/articles/${updateForm.articleId}/specifications/${updateForm.specificationSlug}`,
      {
        method: 'PATCH',
        body: JSON.stringify({ value: updateForm.value })
      }
    )
    if (!result.ok) {
      updateState.error = result.error || 'Failed to update specification.'
      return
    }
    updateState.success = 'Specification updated.'
    await loadArticleSpecs(updateForm.articleId)
  } catch (err) {
    updateState.error = err instanceof Error ? err.message : 'Failed to update specification.'
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
