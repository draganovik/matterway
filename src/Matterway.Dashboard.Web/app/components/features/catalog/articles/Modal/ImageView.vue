<script setup lang="ts">
import type { ArticleImageProperty } from "~/types/catalog"

type ImageSubmitPayload = {
  orderIndex: number
  imageAlt: string
  file?: File | null
}

const props = withDefaults(
  defineProps<{
    open?: boolean
    mode?: "add" | "edit"
    image?: ArticleImageProperty | null
    canEdit?: boolean
    loading?: boolean
    error?: string
  }>(),
  {
    open: false,
    mode: "add",
    image: null,
    canEdit: false,
    loading: false,
    error: "",
  },
)

const emit = defineEmits<{
  (event: "update:open", value: boolean): void
  (event: "submit", payload: ImageSubmitPayload): void
}>()

const isOpen = computed({
  get: () => props.open,
  set: (value: boolean) => emit("update:open", value),
})

const file = ref<File | null>(null)
const fileInputRef = ref<HTMLInputElement | null>(null)
const isDragOver = ref(false)
const orderIndex = ref<number | string>(0)
const imageAlt = ref("")
const validationError = ref("")

const controlsDisabled = computed(
  () => !props.canEdit || props.loading || props.mode !== "add",
)

watch(
  () => props.open,
  (open) => {
    if (!open) {
      file.value = null
      if (fileInputRef.value) fileInputRef.value.value = ""
      isDragOver.value = false
      orderIndex.value = 0
      imageAlt.value = ""
      validationError.value = ""
      return
    }
    if (props.mode === "edit" && props.image) {
      orderIndex.value = props.image.orderIndex ?? 0
      imageAlt.value = props.image.imageAlt || ""
      file.value = null
      if (fileInputRef.value) fileInputRef.value.value = ""
      isDragOver.value = false
    } else {
      orderIndex.value = 0
      imageAlt.value = ""
      file.value = null
      if (fileInputRef.value) fileInputRef.value.value = ""
      isDragOver.value = false
    }
  },
)

function setSelectedFile(next: File | null) {
  validationError.value = ""
  if (!next) {
    file.value = null
    return
  }

  const isImage = !next.type || next.type.startsWith("image/")
  if (!isImage) {
    file.value = null
    validationError.value = "Only image files are supported."
    return
  }

  file.value = next
}

function openPicker() {
  if (controlsDisabled.value) return
  fileInputRef.value?.click()
}

function onFileInputChange(event: Event) {
  const target = event.target as HTMLInputElement | null
  setSelectedFile(target?.files?.[0] ?? null)
}

function onDragEnter() {
  if (controlsDisabled.value) return
  isDragOver.value = true
}

function onDragOver() {
  if (controlsDisabled.value) return
  isDragOver.value = true
}

function onDragLeave() {
  isDragOver.value = false
}

function onDrop(event: DragEvent) {
  if (controlsDisabled.value) return
  isDragOver.value = false
  setSelectedFile(event.dataTransfer?.files?.[0] ?? null)
}

function clearSelection() {
  if (controlsDisabled.value) return
  validationError.value = ""
  file.value = null
  isDragOver.value = false
  if (fileInputRef.value) fileInputRef.value.value = ""
}

function submit() {
  validationError.value = ""
  const parsedOrder = Number(orderIndex.value)
  if (!Number.isFinite(parsedOrder) || parsedOrder < 0) {
    validationError.value = "Order index must be a valid number."
    return
  }
  if (props.mode === "add" && !file.value) {
    validationError.value = "Select an image file to upload."
    return
  }
  emit("submit", {
    orderIndex: parsedOrder,
    imageAlt: imageAlt.value.trim(),
    file: file.value,
  })
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          {{ mode === "edit" ? "Edit Image" : "Add Image" }}
        </h3>
        <p class="text-muted text-sm">
          {{
            mode === "edit"
              ? "Update image metadata and order."
              : "Upload a new image for the article."
          }}
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField v-if="mode === 'add'" label="File" required>
          <input
            ref="fileInputRef"
            class="hidden"
            type="file"
            accept="image/*"
            :disabled="controlsDisabled"
            @change="onFileInputChange"
          />
          <button
            type="button"
            class="border-default bg-elevated/40 w-full rounded-lg border border-dashed p-5 text-center transition"
            :class="[
              isDragOver
                ? 'border-primary bg-primary/5'
                : 'hover:border-primary/60',
              controlsDisabled
                ? 'cursor-not-allowed opacity-60'
                : 'cursor-pointer',
            ]"
            :disabled="controlsDisabled"
            @click="openPicker"
            @dragenter.prevent="onDragEnter"
            @dragover.prevent="onDragOver"
            @dragleave.prevent="onDragLeave"
            @drop.prevent="onDrop"
          >
            <div class="flex flex-col items-center gap-2">
              <UIcon name="i-lucide-image-up" class="size-6" />
              <p class="text-sm font-medium">Drag and drop an image</p>
              <p class="text-muted text-xs">or click to browse</p>
            </div>
          </button>
          <p class="text-muted mt-2 text-xs">
            {{ file ? `Selected: ${file.name}` : "No image selected." }}
          </p>
          <div class="mt-3 flex flex-wrap items-center gap-2">
            <UButton
              color="neutral"
              variant="soft"
              icon="i-lucide-x"
              :disabled="controlsDisabled || !file"
              @click="clearSelection"
            >
              Clear
            </UButton>
          </div>
        </UFormField>

        <UFormField label="Order Index" required>
          <UInput
            v-model="orderIndex"
            type="number"
            min="0"
            placeholder="0"
            :disabled="!canEdit"
          />
        </UFormField>

        <UFormField label="Image Alt">
          <UInput
            v-model="imageAlt"
            placeholder="Front view of article"
            :disabled="!canEdit"
          />
        </UFormField>

        <StatusMessages :error="validationError || error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton variant="ghost" :disabled="loading" @click="isOpen = false">
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="loading"
          :disabled="!canEdit"
          @click="submit"
        >
          {{ mode === "edit" ? "Save Changes" : "Upload Image" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
