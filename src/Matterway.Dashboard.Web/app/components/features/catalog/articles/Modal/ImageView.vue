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
const orderIndex = ref<number | string>(0)
const imageAlt = ref("")
const validationError = ref("")

const controlsDisabled = computed(
  () => !props.canEdit || props.loading || props.mode !== "add",
)

const orderIndexValue = computed({
  get: () => {
    const parsed = Number(orderIndex.value)
    return Number.isFinite(parsed) ? parsed : null
  },
  set: (value: number | null | undefined) => {
    orderIndex.value = value == null ? "" : Math.trunc(value)
  },
})

watch(
  () => props.open,
  (open) => {
    if (!open) {
      file.value = null
      orderIndex.value = 0
      imageAlt.value = ""
      validationError.value = ""
      return
    }
    if (props.mode === "edit" && props.image) {
      orderIndex.value = props.image.orderIndex ?? 0
      imageAlt.value = props.image.imageAlt || ""
      file.value = null
    } else {
      orderIndex.value = 0
      imageAlt.value = ""
      file.value = null
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

function handleSelectedFileChange(nextFile: File | null | undefined) {
  setSelectedFile(nextFile ?? null)
}

function clearSelection() {
  if (controlsDisabled.value) return
  validationError.value = ""
  file.value = null
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
    imageAlt: String(imageAlt.value ?? "").trim(),
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
          <UFileUpload
            :model-value="file"
            accept="image/*"
            :preview="false"
            :interactive="!controlsDisabled"
            :disabled="controlsDisabled"
            class="w-full"
            :ui="{
              base: `border-default bg-elevated/40 w-full rounded-lg border border-dashed p-5 text-center transition ${controlsDisabled ? 'cursor-not-allowed' : 'cursor-pointer'} hover:border-primary/60 data-[dragging=true]:border-primary data-[dragging=true]:bg-primary/5`,
              wrapper: 'flex flex-col items-center gap-2',
              avatar: 'hidden',
              label: 'mt-0 text-sm font-medium',
              description: 'text-muted mt-0 text-xs',
              actions: 'hidden',
            }"
            label="Drag and drop an image"
            description="or click to browse"
            @update:model-value="handleSelectedFileChange"
          >
            <template #leading>
              <UIcon name="i-lucide-image-up" class="size-6" />
            </template>
          </UFileUpload>
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
          <UInputNumber
            v-model="orderIndexValue"
            orientation="vertical"
            :min="0"
            :step="1"
            step-snapping
            variant="outline"
            placeholder="0"
            :disabled="!canEdit"
            class="w-full"
            :ui="{ root: 'w-full', base: 'w-full text-left' }"
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
