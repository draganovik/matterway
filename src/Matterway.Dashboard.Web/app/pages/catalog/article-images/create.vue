<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useArticleImages } from '~/composables/useArticleImages'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Article Images',
  service: 'catalog',
  level: 'operator',
  action: 'create'
})

const api = useApiClient()

const createForm = reactive({
  articleId: '',
  orderIndex: 0,
  file: null as File | null,
  imageAlt: ''
})
const createState = useRequestState()

const { imagePreview, previewState, loadImages } = useArticleImages()

async function createImage() {
  createState.error = ''
  createState.success = ''
  if (!createForm.articleId || !createForm.file) {
    createState.error = 'Article Id and image file are required.'
    return
  }
  createState.loading = true
  try {
    const body = new FormData()
    body.append('file', createForm.file)
    body.append('orderIndex', String(createForm.orderIndex))
    if (createForm.imageAlt) body.append('imageAlt', createForm.imageAlt)

    const result = await api.request(
      'catalog',
      `admin/articles/${createForm.articleId}/images`,
      { method: 'POST', body }
    )
    if (!result.ok) {
      createState.error = result.error || 'Failed to add image.'
      return
    }
    createState.success = 'Image added.'
    await loadImages(createForm.articleId)
  } catch (err) {
    createState.error = err instanceof Error ? err.message : 'Failed to add image.'
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
            Add image
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createImage"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="createForm.articleId"
              placeholder="GUID"
              @blur="loadImages(createForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Order Index"
            required
          >
            <UInput
              v-model.number="createForm.orderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UFormField
            label="Image File"
            required
          >
            <UInput
              type="file"
              @change="(e: any) => (createForm.file = e.target.files?.[0] || null)"
            />
          </UFormField>
          <UFormField label="Image Alt">
            <UInput v-model="createForm.imageAlt" />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="createState.loading"
          >
            Add Image
          </UButton>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
        </UForm>
      </UCard>

      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Current images
          </h3>
        </template>
        <FormStatus
          :loading="previewState.loading"
          :error="previewState.error"
        />
        <div
          v-if="imagePreview.length"
          class="space-y-3"
        >
          <div
            v-for="image in imagePreview"
            :key="image.id"
            class="flex items-center gap-3 rounded-lg border border-default p-3 text-sm"
          >
            <img
              v-if="image.imageUrl"
              :src="image.imageUrl"
              :alt="image.imageAlt || ''"
              class="h-12 w-12 rounded object-cover"
            >
            <div>
              <p class="font-semibold">
                Order {{ image.orderIndex }}
              </p>
              <p class="text-muted">
                {{ image.imageAlt || 'No alt text' }}
              </p>
            </div>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Load an article to view images.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
