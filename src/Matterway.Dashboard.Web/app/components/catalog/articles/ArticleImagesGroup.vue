<script setup lang="ts">
import { useCatalogApi, type ArticleImageProperty } from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'

const props = withDefaults(defineProps<{
  articleId?: string | null
  modelValue?: ArticleImageProperty[]
  canEdit?: boolean
}>(), {
  articleId: null,
  modelValue: () => [],
  canEdit: false
})

const emit = defineEmits<{
  (event: 'update:modelValue', value: ArticleImageProperty[]): void
}>()

const api = useCatalogApi()
const images = ref<ArticleImageProperty[]>([])
const addForm = reactive({
  orderIndex: 0,
  imageAlt: '',
  file: null as File | null
})
const addState = useRequestState()
const updateState = useRequestState()
const removeState = useRequestState()
const replaceState = useRequestState()

const editMap = reactive<Record<string, { orderIndex: number | string, imageAlt: string }>>({})

watch(
  () => props.modelValue,
  (value) => {
    images.value = Array.isArray(value) ? [...value] : []
    syncEdits()
  },
  { immediate: true }
)

watch(
  () => props.articleId,
  () => {
    addForm.orderIndex = 0
    addForm.imageAlt = ''
    addForm.file = null
    addState.error = ''
    addState.success = ''
  }
)

function syncEdits() {
  for (const image of images.value) {
    const key = String(image.id || image.orderIndex)
    if (!editMap[key]) {
      editMap[key] = {
        orderIndex: image.orderIndex ?? 0,
        imageAlt: image.imageAlt || ''
      }
    } else {
      editMap[key].orderIndex = image.orderIndex ?? 0
      editMap[key].imageAlt = image.imageAlt || ''
    }
  }
}

function updateImages(next: ArticleImageProperty[]) {
  images.value = [...next]
  emit('update:modelValue', images.value)
  syncEdits()
}

async function addImage() {
  addState.error = ''
  addState.success = ''
  if (!props.articleId) {
    addState.error = 'Create the article before adding images.'
    return
  }
  if (!addForm.file) {
    addState.error = 'Select an image file to upload.'
    return
  }
  addState.loading = true
  const result = await api.addArticleImage(props.articleId, {
    orderIndex: addForm.orderIndex,
    file: addForm.file,
    imageAlt: addForm.imageAlt
  })
  addState.loading = false
  if (!result.ok) {
    addState.error = result.error || 'Unable to add image.'
    return
  }
  const next = [...images.value]
  if (result.data && typeof result.data === 'object') {
    next.push(result.data as ArticleImageProperty)
  }
  updateImages(next.sort((a, b) => Number(a.orderIndex) - Number(b.orderIndex)))
  addForm.file = null
  addForm.imageAlt = ''
  addState.success = 'Image added.'
}

async function updateImage(image: ArticleImageProperty) {
  updateState.error = ''
  updateState.success = ''
  if (!props.articleId) return
  const key = String(image.id || image.orderIndex)
  const edit = editMap[key] || {
    orderIndex: image.orderIndex ?? 0,
    imageAlt: image.imageAlt || ''
  }
  updateState.loading = true
  const result = await api.updateArticleImage(props.articleId, image.orderIndex, {
    imageAlt: edit.imageAlt,
    orderIndex: edit.orderIndex
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Unable to update image.'
    return
  }
  const next = images.value.map((item) => {
    if (item.id === image.id) {
      return {
        ...item,
        orderIndex: edit.orderIndex,
        imageAlt: edit.imageAlt
      }
    }
    return item
  })
  updateImages(next.sort((a, b) => Number(a.orderIndex) - Number(b.orderIndex)))
  updateState.success = 'Image updated.'
}

async function removeImage(image: ArticleImageProperty) {
  removeState.error = ''
  removeState.success = ''
  if (!props.articleId) return
  removeState.loading = true
  const result = await api.removeArticleImage(props.articleId, image.orderIndex)
  removeState.loading = false
  if (!result.ok) {
    removeState.error = result.error || 'Unable to remove image.'
    return
  }
  updateImages(images.value.filter(item => item.id !== image.id))
  removeState.success = 'Image removed.'
}

async function replaceImage(image: ArticleImageProperty, file: File) {
  replaceState.error = ''
  replaceState.success = ''
  if (!props.articleId) return
  replaceState.loading = true
  const removeResult = await api.removeArticleImage(props.articleId, image.orderIndex)
  if (!removeResult.ok) {
    replaceState.loading = false
    replaceState.error = removeResult.error || 'Unable to replace image.'
    return
  }
  const key = String(image.id || image.orderIndex)
  const edit = editMap[key]
  const addResult = await api.addArticleImage(props.articleId, {
    orderIndex: image.orderIndex,
    file,
    imageAlt: edit?.imageAlt || image.imageAlt || ''
  })
  replaceState.loading = false
  if (!addResult.ok) {
    replaceState.error = addResult.error || 'Unable to replace image.'
    return
  }
  const without = images.value.filter(item => item.id !== image.id)
  if (addResult.data && typeof addResult.data === 'object') {
    without.push(addResult.data as ArticleImageProperty)
  }
  updateImages(without.sort((a, b) => Number(a.orderIndex) - Number(b.orderIndex)))
  replaceState.success = 'Image replaced.'
}
</script>

<template>
  <div class="flex flex-col gap-5">
    <div class="flex items-center justify-between">
      <div>
        <h3 class="text-base font-semibold text-foreground">
          Images
        </h3>
        <p class="text-sm text-muted">
          Manage article visuals without leaving the editor.
        </p>
      </div>
    </div>

    <div
      v-if="!articleId"
      class="rounded-lg border border-default bg-background px-4 py-4 text-sm text-muted"
    >
      Create the article first to attach images.
    </div>

    <div
      v-else
      class="grid gap-4"
    >
      <div class="rounded-lg border border-default bg-background p-4">
        <div class="grid gap-4 md:grid-cols-[minmax(0,1fr)_minmax(0,1fr)_minmax(0,1fr)]">
          <UFormField
            label="Order Index"
            required
          >
            <UInput
              v-model="addForm.orderIndex"
              type="number"
              min="0"
              size="md"
              :disabled="!canEdit"
            />
          </UFormField>
          <UFormField label="Image Alt">
            <UInput
              v-model="addForm.imageAlt"
              size="md"
              :disabled="!canEdit"
            />
          </UFormField>
          <UFormField
            label="File"
            required
          >
            <UFileUpload
              v-model="addForm.file"
              accept="image/*"
              variant="button"
              size="md"
              label="Choose image"
              :preview="false"
              :reset="true"
              class="w-full"
              :disabled="!canEdit"
            />
          </UFormField>
        </div>
        <div class="mt-4 flex flex-wrap items-center gap-3">
          <UButton
            color="primary"
            :loading="addState.loading"
            :disabled="!canEdit"
            @click="addImage"
          >
            Add Image
          </UButton>
          <FormStatus
            :error="addState.error"
            :success="addState.success"
          />
        </div>
      </div>

      <div class="grid gap-3">
        <div
          v-for="image in images"
          :key="image.id"
          class="rounded-lg border border-default bg-background p-4"
        >
          <div class="flex flex-wrap items-center justify-between gap-4">
            <div class="flex min-w-50 flex-1 gap-4">
              <div class="h-20 w-20 overflow-hidden rounded-md bg-muted/60">
                <img
                  v-if="image.imageUrl"
                  :src="image.imageUrl"
                  :alt="image.imageAlt || ''"
                  class="h-full w-full object-cover"
                >
                <div
                  v-else
                  class="flex h-full w-full items-center justify-center text-xs text-muted"
                >
                  No image
                </div>
              </div>
              <div class="min-w-0">
                <p class="text-sm font-medium text-foreground">
                  Order {{ image.orderIndex }}
                </p>
                <p class="text-sm text-muted">
                  {{ image.imageAlt || 'No alt text' }}
                </p>
              </div>
            </div>
            <div class="flex flex-wrap items-center gap-2">
              <UFileUpload
                :model-value="null"
                accept="image/*"
                variant="button"
                size="sm"
                label="Replace"
                :preview="false"
                :reset="true"
                class="min-w-[7.5rem]"
                :disabled="!canEdit || replaceState.loading"
                @update:model-value="(file) => file && replaceImage(image, file)"
              />
              <UButton
                color="error"
                variant="ghost"
                :disabled="!canEdit"
                @click="removeImage(image)"
              >
                Remove
              </UButton>
            </div>
          </div>

          <div class="mt-4 grid gap-4 md:grid-cols-[minmax(0,1fr)_minmax(0,1fr)_auto]">
            <UFormField label="Order Index">
              <UInput
                v-model="editMap[String(image.id || image.orderIndex)]!.orderIndex"
                type="number"
                min="0"
                size="md"
                :disabled="!canEdit"
              />
            </UFormField>
            <UFormField label="Image Alt">
              <UInput
                v-model="editMap[String(image.id || image.orderIndex)]!.imageAlt"
                size="md"
                :disabled="!canEdit"
              />
            </UFormField>
            <div class="flex items-end">
              <UButton
                variant="outline"
                :loading="updateState.loading"
                :disabled="!canEdit"
                @click="updateImage(image)"
              >
                Update
              </UButton>
            </div>
          </div>
        </div>
      </div>

      <div class="flex flex-wrap gap-4">
        <FormStatus :error="updateState.error || replaceState.error || removeState.error" />
        <FormStatus :success="updateState.success || replaceState.success || removeState.success" />
      </div>
    </div>
  </div>
</template>
