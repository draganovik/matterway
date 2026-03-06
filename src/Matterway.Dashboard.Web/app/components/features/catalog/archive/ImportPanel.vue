<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    canOperate?: boolean
    loading?: boolean
    error?: string
    success?: string
    hasSelectedFile?: boolean
    selectedFileName?: string
  }>(),
  {
    canOperate: false,
    loading: false,
    error: "",
    success: "",
    hasSelectedFile: false,
    selectedFileName: "",
  },
)

const emit = defineEmits<{
  (event: "select-file", value: File | null): void
  (event: "upload" | "clear"): void
}>()

const fileInputRef = ref<HTMLInputElement | null>(null)
const isDragOver = ref(false)

const actionColor = computed(() =>
  props.hasSelectedFile ? "success" : "neutral",
)

const controlsDisabled = computed(() => !props.canOperate || props.loading)

function openPicker() {
  if (controlsDisabled.value) return
  fileInputRef.value?.click()
}

function onFileInputChange(event: Event) {
  const target = event.target as HTMLInputElement | null
  emit("select-file", target?.files?.[0] ?? null)
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
  emit("select-file", event.dataTransfer?.files?.[0] ?? null)
}

function clearSelection() {
  if (controlsDisabled.value) return
  if (fileInputRef.value) fileInputRef.value.value = ""
  isDragOver.value = false
  emit("clear")
}
</script>

<template>
  <UCard class="!border-default h-full !border !ring-0">
    <template #header>
      <div>
        <h3 class="text-foreground text-sm font-semibold">Import Archive</h3>
        <p class="text-muted text-xs">
          Upload a previously exported archive to replace current catalog data
          and images.
        </p>
      </div>
    </template>

    <div class="space-y-4">
      <UFormField label="Archive File (.zip)" required>
        <input
          ref="fileInputRef"
          class="hidden"
          type="file"
          accept=".zip,application/zip"
          :disabled="controlsDisabled"
          @change="onFileInputChange"
        />
        <button
          type="button"
          class="border-default bg-elevated/40 w-full rounded-lg border border-dashed p-6 text-center transition"
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
            <UIcon name="i-lucide-file-up" class="size-6" />
            <p class="text-sm font-medium">Drag and drop a .zip archive</p>
            <p class="text-muted text-xs">or click to browse</p>
          </div>
        </button>

        <p class="text-muted mt-2 text-xs">
          {{
            props.hasSelectedFile
              ? `Selected: ${props.selectedFileName}`
              : "No archive selected."
          }}
        </p>

        <div class="mt-3 flex flex-wrap items-center gap-2">
          <UButton
            :color="actionColor"
            icon="i-lucide-upload"
            :loading="props.loading"
            :disabled="!props.canOperate || !props.hasSelectedFile"
            @click="emit('upload')"
          >
            Upload And Import
          </UButton>

          <UButton
            color="neutral"
            variant="soft"
            icon="i-lucide-x"
            :disabled="
              !props.canOperate || !props.hasSelectedFile || props.loading
            "
            @click="clearSelection"
          >
            Clear
          </UButton>
        </div>
      </UFormField>

      <StatusMessages
        :loading="props.loading && 'Importing archive...'"
        :error="props.error"
        :success="props.success"
      />
    </div>
  </UCard>
</template>
