<script setup lang="ts">
import { useCatalogApi } from '~/composables/useCatalogApi'
import type { ArticleImageProperty } from '~/types/catalog'
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
const addState = useRequestState()
const updateState = useRequestState()
const removeState = useRequestState()
const imageModalOpen = ref(false)
const imageModalMode = ref<'add' | 'edit'>('add')
const activeImage = ref<ArticleImageProperty | null>(null)

watch(
  () => props.modelValue,
  (value) => {
    images.value = Array.isArray(value) ? [...value] : []
  },
  { immediate: true }
)

watch(
  () => props.articleId,
  () => {
    addState.error = ''
    addState.success = ''
  }
)

function updateImages(next: ArticleImageProperty[]) {
  images.value = [...next]
  emit('update:modelValue', images.value)
}

function openAddImagesModal() {
  addState.error = ''
  addState.success = ''
  if (!props.articleId) {
    addState.error = 'Create the article before adding images.'
    return
  }
  imageModalMode.value = 'add'
  activeImage.value = null
  imageModalOpen.value = true
}

function openEditImagesModal(image: ArticleImageProperty) {
  updateState.error = ''
  updateState.success = ''
  imageModalMode.value = 'edit'
  activeImage.value = image
  imageModalOpen.value = true
}

async function addImage(payload: { orderIndex: number, imageAlt: string, file?: File | null }) {
  addState.error = ''
  addState.success = ''
  if (!props.articleId) {
    addState.error = 'Create the article before adding images.'
    return
  }
  if (!payload.file) {
    addState.error = 'Select an image file to upload.'
    return
  }
  addState.loading = true
  const result = await api.addArticleImage(props.articleId, {
    orderIndex: payload.orderIndex,
    file: payload.file,
    imageAlt: payload.imageAlt
  })
  addState.loading = false
  if (!result.ok) {
    addState.error = result.error || 'Unable to add image.'
    return
  }
  const next = [...images.value]
  if (result.data) next.push(result.data)
  updateImages(next.sort((a, b) => Number(a.orderIndex) - Number(b.orderIndex)))
  addState.success = 'Image added.'
  imageModalOpen.value = false
}

async function updateImage(payload: { orderIndex: number, imageAlt: string }) {
  updateState.error = ''
  updateState.success = ''
  if (!props.articleId) return
  if (!activeImage.value) {
    updateState.error = 'Select an image to edit.'
    return
  }
  const targetOrderIndex = activeImage.value.orderIndex
  if (targetOrderIndex === undefined || targetOrderIndex === null) {
    updateState.error = 'Selected image is missing an order index.'
    return
  }
  updateState.loading = true
  const result = await api.updateArticleImage(props.articleId, targetOrderIndex, {
    imageAlt: payload.imageAlt,
    orderIndex: payload.orderIndex
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Unable to update image.'
    return
  }
  const next = images.value.map((item) => {
    if (item.id === activeImage.value?.id) {
      return {
        ...item,
        orderIndex: payload.orderIndex,
        imageAlt: payload.imageAlt
      }
    }
    return item
  })
  updateImages(next.sort((a, b) => Number(a.orderIndex) - Number(b.orderIndex)))
  updateState.success = 'Image updated.'
  imageModalOpen.value = false
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

async function handleImageSubmit(payload: { orderIndex: number, imageAlt: string, file?: File | null }) {
  if (imageModalMode.value === 'add') {
    await addImage(payload)
    return
  }
  await updateImage(payload)
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-base font-semibold text-foreground">
        Images
      </h3>
      <UButton
        color="primary"
        variant="outline"
        :disabled="!canEdit || !articleId"
        @click="openAddImagesModal"
      >
        Add Image
      </UButton>
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
              <UButton
                variant="outline"
                :disabled="!canEdit"
                class="justify-center"
                @click="openEditImagesModal(image)"
              >
                Edit
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
        <FormStatus :error="addState.error || updateState.error || removeState.error" />
        <FormStatus :success="addState.success || updateState.success || removeState.success" />
      </div>
    </div>
  </div>

  <CatalogArticlesImagesModal
    v-model:open="imageModalOpen"
    :mode="imageModalMode"
    :image="activeImage"
    :can-edit="canEdit"
    :loading="imageModalMode === 'add' ? addState.loading : updateState.loading"
    :error="imageModalMode === 'add' ? addState.error : updateState.error"
    @submit="handleImageSubmit"
  />
</template>
