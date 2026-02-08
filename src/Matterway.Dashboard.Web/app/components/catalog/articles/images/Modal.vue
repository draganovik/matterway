<script setup lang="ts">
import type { ArticleImageProperty } from '~/types/catalog'

type ImageSubmitPayload = {
  orderIndex: number
  imageAlt: string
  file?: File | null
}

const props = withDefaults(defineProps<{
  open?: boolean
  mode?: 'add' | 'edit'
  image?: ArticleImageProperty | null
  canEdit?: boolean
  loading?: boolean
  error?: string
}>(), {
  open: false,
  mode: 'add',
  image: null,
  canEdit: false,
  loading: false,
  error: ''
})

const emit = defineEmits<{
  (event: 'update:open', value: boolean): void
  (event: 'submit', payload: ImageSubmitPayload): void
}>()

const isOpen = computed({
  get: () => props.open,
  set: (value: boolean) => emit('update:open', value)
})

const file = ref<File | null>(null)
const orderIndex = ref<number | string>(0)
const imageAlt = ref('')
const validationError = ref('')

watch(
  () => props.open,
  (open) => {
    if (!open) {
      file.value = null
      orderIndex.value = 0
      imageAlt.value = ''
      validationError.value = ''
      return
    }
    if (props.mode === 'edit' && props.image) {
      orderIndex.value = props.image.orderIndex ?? 0
      imageAlt.value = props.image.imageAlt || ''
      file.value = null
    } else {
      orderIndex.value = 0
      imageAlt.value = ''
      file.value = null
    }
  }
)

function submit() {
  validationError.value = ''
  const parsedOrder = Number(orderIndex.value)
  if (!Number.isFinite(parsedOrder) || parsedOrder < 0) {
    validationError.value = 'Order index must be a valid number.'
    return
  }
  if (props.mode === 'add' && !file.value) {
    validationError.value = 'Select an image file to upload.'
    return
  }
  emit('submit', {
    orderIndex: parsedOrder,
    imageAlt: imageAlt.value.trim(),
    file: file.value
  })
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-base font-semibold text-foreground">
          {{ mode === 'edit' ? 'Edit Image' : 'Add Image' }}
        </h3>
        <p class="text-sm text-muted">
          {{ mode === 'edit' ? 'Update image metadata and order.' : 'Upload a new image for the article.' }}
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField
          v-if="mode === 'add'"
          label="File"
          required
        >
          <UFileUpload
            v-model="file"
            accept="image/*"
            variant="button"
            label="Choose image"
            :preview="false"
            :reset="true"
            :disabled="!canEdit"
          />
        </UFormField>

        <UFormField
          label="Order Index"
          required
        >
          <UInput
            v-model="orderIndex"
            type="number"
            min="0"
            :disabled="!canEdit"
          />
        </UFormField>

        <UFormField label="Image Alt">
          <UInput
            v-model="imageAlt"
            :disabled="!canEdit"
          />
        </UFormField>

        <FormStatus :error="validationError || error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="loading"
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="loading"
          :disabled="!canEdit"
          @click="submit"
        >
          {{ mode === 'edit' ? 'Save Changes' : 'Upload Image' }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
