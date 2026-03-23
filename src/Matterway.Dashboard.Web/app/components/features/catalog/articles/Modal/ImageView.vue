<script setup lang="ts">
import { useModalCloseReset } from "~/composables/workflows/modal/useModalCloseReset"
import type { ArticleImageProperty } from "~/types/catalog"
import {
  catalogImageUploadAccept,
  getSupportedCatalogImageFormatsLabel,
  isSupportedCatalogImageFile,
} from "~/utils/catalogImages"

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
const orderIndex = ref<number | null>(0)
const imageAlt = ref("")
const validationError = ref("")
const supportedFormatsLabel = getSupportedCatalogImageFormatsLabel()

const controlsDisabled = computed(
  () => !props.canEdit || props.loading || props.mode !== "add",
)

function resetModalState() {
  file.value = null
  orderIndex.value = 0
  imageAlt.value = ""
  validationError.value = ""
}

function initializeModal() {
  resetModalState()

  if (props.mode === "edit" && props.image) {
    orderIndex.value = Number(props.image.orderIndex ?? 0)
    imageAlt.value = props.image.imageAlt || ""
  }
}

function setSelectedFile(next: File | null) {
  validationError.value = ""
  if (!next) {
    file.value = null
    return
  }

  if (!isSupportedCatalogImageFile(next)) {
    file.value = null
    validationError.value = `Podržani formati su samo: ${supportedFormatsLabel}.`
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
  const parsedOrder = orderIndex.value
  if (parsedOrder == null || parsedOrder < 0) {
    validationError.value = "Redosled mora biti ispravan broj."
    return
  }
  if (props.mode === "add" && !file.value) {
    validationError.value = "Izaberite sliku za otpremanje."
    return
  }
  emit("submit", {
    orderIndex: parsedOrder,
    imageAlt: String(imageAlt.value ?? "").trim(),
    file: file.value,
  })
}

useModalCloseReset({
  isOpen,
  onCloseReset: resetModalState,
  onOpen: initializeModal,
  delayMs: 300,
})
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          {{ mode === "edit" ? "Izmena slike" : "Dodavanje slike" }}
        </h3>
        <p class="text-muted text-sm">
          {{
            mode === "edit"
              ? "Ažurirajte metapodatke slike i redosled prikaza."
              : "Otpremite novu sliku za artikal."
          }}
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField v-if="mode === 'add'" label="Datoteka" required>
          <UFileUpload
            :model-value="file"
            :accept="catalogImageUploadAccept"
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
            label="Prevucite sliku ovde"
            :description="`ili kliknite za izbor (${supportedFormatsLabel})`"
            @update:model-value="handleSelectedFileChange"
          >
            <template #leading>
              <UIcon name="i-lucide-image-up" class="size-6" />
            </template>
          </UFileUpload>
          <p class="text-muted mt-2 text-xs">
            {{ file ? `Izabrano: ${file.name}` : "Slika nije izabrana." }}
          </p>
          <div class="mt-3 flex flex-wrap items-center gap-2">
            <UButton
              color="neutral"
              variant="soft"
              icon="i-lucide-x"
              :disabled="controlsDisabled || !file"
              @click="clearSelection"
            >
              Ukloni izbor
            </UButton>
          </div>
        </UFormField>

        <UFormField label="Redosled" required>
          <UInputNumber
            v-model="orderIndex"
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

        <UFormField label="Alt tekst slike">
          <UInput
            v-model="imageAlt"
            placeholder="Prednji prikaz artikla"
            :disabled="!canEdit"
            class="w-full"
          />
        </UFormField>

        <StatusMessages :error="validationError || error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton variant="ghost" :disabled="loading" @click="isOpen = false">
          Otkaži
        </UButton>
        <UButton
          color="primary"
          :loading="loading"
          :disabled="!canEdit"
          @click="submit"
        >
          {{
            loading
              ? mode === "edit"
                ? "Čuvanje izmena"
                : "Otpremanje slike"
              : mode === "edit"
                ? "Sačuvaj izmene"
                : "Otpremi sliku"
          }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
