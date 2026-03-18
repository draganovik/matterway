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

const selectedFile = ref<File | null>(null)

const actionColor = computed(() =>
  props.hasSelectedFile ? "success" : "neutral",
)

const controlsDisabled = computed(() => !props.canOperate || props.loading)

function handleSelectedFileChange(file: File | null | undefined) {
  const nextFile = file ?? null

  if (!nextFile) {
    selectedFile.value = null
    emit("select-file", null)
    return
  }

  if (!nextFile.name.toLowerCase().endsWith(".zip")) {
    selectedFile.value = null
    emit("select-file", nextFile)
    return
  }

  selectedFile.value = nextFile
  emit("select-file", nextFile)
}

function clearSelection() {
  if (controlsDisabled.value) return
  selectedFile.value = null
  emit("clear")
}

watch(
  () => props.hasSelectedFile,
  (hasSelectedFile) => {
    if (hasSelectedFile) return
    selectedFile.value = null
  },
  { immediate: true },
)
</script>

<template>
  <UCard
    class="dashboard-panel-surface !border-default h-full rounded-2xl !border !shadow-sm !ring-0"
    :ui="{ header: 'p-5 sm:p-5', body: 'px-5 pb-5 pt-0 sm:px-5 sm:pb-5' }"
  >
    <template #header>
      <div>
        <h3 class="text-foreground text-sm font-semibold">Uvoz arhive</h3>
        <p class="text-muted text-xs">
          Otpremite prethodno izvezenu arhivu da zamenite trenutne podatke
          kataloga i slike.
        </p>
      </div>
    </template>

    <div class="space-y-4">
      <UFormField label="Arhiva (.zip)" required>
        <UFileUpload
          :model-value="selectedFile"
          accept=".zip,application/zip"
          :preview="false"
          :interactive="!controlsDisabled"
          :disabled="controlsDisabled"
          class="w-full"
          :ui="{
            base: `border-default bg-default/70 w-full rounded-2xl border border-dashed p-6 text-center transition ${controlsDisabled ? 'cursor-not-allowed' : 'cursor-pointer'} hover:border-primary/60 data-[dragging=true]:border-primary data-[dragging=true]:bg-primary/5`,
            wrapper: 'flex flex-col items-center gap-2',
            avatar: 'hidden',
            label: 'mt-0 text-sm font-medium',
            description: 'text-muted mt-0 text-xs',
            actions: 'hidden',
          }"
          label="Prevucite .zip arhivu ovde"
          description="ili kliknite za izbor"
          @update:model-value="handleSelectedFileChange"
        >
          <template #leading>
            <UIcon name="i-lucide-file-up" class="size-6" />
          </template>
        </UFileUpload>

        <p class="text-muted mt-2 text-xs">
          {{
            props.hasSelectedFile
              ? `Izabrano: ${props.selectedFileName}`
              : "Arhiva nije izabrana."
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
            {{ props.loading ? "Uvoz arhive" : "Otpremi i uvezi" }}
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
            Očisti
          </UButton>
        </div>
      </UFormField>

      <StatusMessages
        :loading="props.loading && 'Uvoz arhive u toku...'"
        :error="props.error"
        :success="props.success"
      />
    </div>
  </UCard>
</template>
