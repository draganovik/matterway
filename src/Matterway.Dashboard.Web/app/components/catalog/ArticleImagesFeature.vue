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
  orderIndex: 0,
  file: null as File | null,
  imageAlt: ''
})
const createState = reactive({ loading: false, error: '', success: '' })

const updateForm = reactive({
  articleId: '',
  orderIndex: 0,
  newOrderIndex: null as number | null,
  imageAlt: ''
})
const updateState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  articleId: '',
  orderIndex: 0
})
const deleteState = reactive({ loading: false, error: '', success: '' })

const imagePreview = ref<any[]>([])
const previewState = reactive({ loading: false, error: '' })

async function loadImages(articleId: string) {
  if (!articleId) return
  previewState.loading = true
  previewState.error = ''
  imagePreview.value = []
  const result = await api.request<any>('catalog', `public/articles/${articleId}`, {}, true)
  previewState.loading = false
  if (!result.ok || !result.data) {
    previewState.error = result.error || 'Failed to load article.'
    return
  }
  imagePreview.value = result.data.images || []
}

async function createImage() {
  if (!createForm.articleId || !createForm.file) {
    createState.error = 'Article Id and image file are required.'
    return
  }
  createState.loading = true
  createState.error = ''
  createState.success = ''
  const formData = new FormData()
  formData.append('orderIndex', String(createForm.orderIndex))
  formData.append('file', createForm.file)
  if (createForm.imageAlt) formData.append('imageAlt', createForm.imageAlt)

  const result = await api.request<any>('catalog', `admin/articles/${createForm.articleId}/images`, {
    method: 'POST',
    body: formData
  })
  createState.loading = false
  if (!result.ok) {
    createState.error = result.error || 'Failed to add image.'
    return
  }
  createState.success = 'Image added.'
  await loadImages(createForm.articleId)
}

async function updateImage() {
  if (!updateForm.articleId) {
    updateState.error = 'Article Id is required.'
    return
  }
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const payload: Record<string, any> = {}
  if (updateForm.imageAlt) payload.imageAlt = updateForm.imageAlt
  if (updateForm.newOrderIndex !== null) payload.orderIndex = updateForm.newOrderIndex

  const result = await api.request<any>(
    'catalog',
    `admin/articles/${updateForm.articleId}/images/${updateForm.orderIndex}`,
    {
      method: 'PATCH',
      body: JSON.stringify(payload)
    }
  )
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update image.'
    return
  }
  updateState.success = 'Image updated.'
  await loadImages(updateForm.articleId)
}

async function deleteImage() {
  if (!deleteForm.articleId) {
    deleteState.error = 'Article Id is required.'
    return
  }
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/articles/${deleteForm.articleId}/images/${deleteForm.orderIndex}`,
    { method: 'DELETE' }
  )
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to remove image.'
    return
  }
  deleteState.success = 'Image removed.'
  await loadImages(deleteForm.articleId)
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
            Find images
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="() => loadImages(queryForm.articleId)"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="queryForm.articleId"
              placeholder="GUID"
              @blur="loadImages(queryForm.articleId)"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="previewState.loading"
          >
            Load Images
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

      <UCard
        v-else-if="active === 'update'"
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            Update image
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateImage"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="updateForm.articleId"
              placeholder="GUID"
              @blur="loadImages(updateForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Current Order Index"
            required
          >
            <UInput
              v-model.number="updateForm.orderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UFormField label="New Order Index">
            <UInput
              v-model.number="updateForm.newOrderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UFormField label="Image Alt">
            <UInput v-model="updateForm.imageAlt" />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update Image
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
            Remove image
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteImage"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="deleteForm.articleId"
              placeholder="GUID"
              @blur="loadImages(deleteForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Order Index"
            required
          >
            <UInput
              v-model.number="deleteForm.orderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Image
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
