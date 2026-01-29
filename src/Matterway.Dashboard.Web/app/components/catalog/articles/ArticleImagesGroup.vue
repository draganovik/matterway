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
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-base font-semibold text-foreground">
        Images
      </h3>
    </div>

    <div
      v-if="!articleId"
      class="rounded-lg border border-default bg-background px-4 py-4 text-sm text-muted"
    >
      Create the article first to attach images.
    </div>

    <div
      v-else
      class="grid gap-4 @container"
    >
      <div class="grid grid-cols-4 gap-3">
        <UFormField
          label="File"
          required
        >
          <UFileUpload
            v-model="addForm.file"
            accept="image/*"
            variant="button"
            label="Choose image"
            :preview="false"
            :reset="true"
            class="px-auto px-2"
            :disabled="!canEdit"
          />
        </UFormField>
        <UFormField
          label="Order Index"
          required
        >
          <UInput
            v-model="addForm.orderIndex"
            type="number"
            min="0"
            :disabled="!canEdit"
            class="w-full"
          />
        </UFormField>
        <UFormField label="Image Alt">
          <UInput
            v-model="addForm.imageAlt"
            :disabled="!canEdit"
            class="w-full"
          />
        </UFormField>
        <UFormField>
          <br class="mt-1">
          <UButton
            color="primary"
            :loading="addState.loading"
            :disabled="!canEdit"
            @click="addImage"
          >
            Upload
          </UButton>
        </UFormField>
      </div>
      <div class="flex flex-wrap items-center gap-3">
        <FormStatus
          :error="addState.error"
          :success="addState.success"
        />
      </div>

      <div class="rounded-md border grid @sm:grid-cols-2 @md:grid-cols-3 @lg:grid-cols-4 border-default/40">
        <div
          v-for="image in images"
          :key="image.id"
          class="border-t border-default/40 px-3 py-3 first:border-t-0"
        >
          <div class="grid gap-3">
            <div class="aspect-4/3 overflow-hidden rounded-md bg-muted/60">
              <img
                v-if="image.imageUrl"
                :src="image.imageUrl"
                :alt="image.imageAlt || ''"
                class="w-full h-full object-cover"
              >
              <div
                v-else
                class="flex w-full h-full items-center justify-center text-xs text-muted"
              >
                No image
              </div>
            </div>
            <div class="grid gap-3">
              <UFormField
                label="Order Index"
                class="col-span-2"
              >
                <UInput
                  v-model="editMap[String(image.id || image.orderIndex)]!.orderIndex"
                  type="number"
                  min="0"
                  :disabled="!canEdit"
                  class="w-full"
                />
              </UFormField>
              <UFormField
                label="Image Alt"
                class="col-span-2"
              >
                <UInput
                  v-model="editMap[String(image.id || image.orderIndex)]!.imageAlt"
                  :disabled="!canEdit"
                  class="w-full"
                />
              </UFormField>
              <UButton
                variant="outline"
                :loading="updateState.loading"
                :disabled="!canEdit"
                class="justify-center"
                @click="updateImage(image)"
              >
                Update
              </UButton>
              <UButton
                color="error"
                variant="ghost"
                :disabled="!canEdit"
                class="justify-center"
                @click="removeImage(image)"
              >
                Remove
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
