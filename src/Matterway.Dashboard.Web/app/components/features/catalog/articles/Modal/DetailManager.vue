<script setup lang="ts">
import { useCatalogApi } from "~/composables/useCatalogApi"
import type {
  ArticleDetailProperty,
  QueryDetailResponse,
} from "~/types/catalog"
import { useRequestState } from "~/composables/useRequestState"

type DetailSubmitPayload = {
  detailSlug: string
  title?: string | null
  unit?: string | null
  textValue?: string | null
  numericValue?: number | null
}

const props = withDefaults(
  defineProps<{
    open?: boolean
    mode?: "add" | "edit"
    detail?: ArticleDetailProperty | null
    canEdit?: boolean
    loading?: boolean
    error?: string
  }>(),
  {
    open: false,
    mode: "add",
    detail: null,
    canEdit: false,
    loading: false,
    error: "",
  },
)

const emit = defineEmits<{
  (event: "update:open", value: boolean): void
  (event: "submit", payload: DetailSubmitPayload): void
}>()

const api = useCatalogApi()

const definitionState = useRequestState()
const searchTerm = ref("")
const detailOptions = ref<QueryDetailResponse[]>([])
const selectedKey = ref("")
const valueInput = ref("")
const validationError = ref("")

const isOpen = computed({
  get: () => props.open,
  set: (value: boolean) => emit("update:open", value),
})

function toDisplayLabel(title: string, unit?: string | null) {
  const normalizedUnit = unit?.trim()
  return normalizedUnit ? `${title} (${normalizedUnit})` : title
}

const optionItems = computed(() =>
  detailOptions.value
    .filter((option) => option.slug)
    .map((option) => {
      const title = (option.title ?? option.slug ?? "").toString()
      const unit = option.unit?.trim() ?? ""
      return {
        id: option.slug as string,
        slug: option.slug as string,
        title,
        displayLabel: toDisplayLabel(title, unit),
        unit,
      }
    }),
)

const selectedOption = computed(() => {
  const match = optionItems.value.find(
    (option) => option.id === selectedKey.value,
  )
  if (match) return match
  if (props.detail?.detailSlug) {
    const title = props.detail.title ?? props.detail.detailSlug
    const unit = props.detail.unit?.trim() ?? ""
    return {
      id: props.detail.detailSlug,
      slug: props.detail.detailSlug,
      title,
      displayLabel: toDisplayLabel(title, unit),
      unit,
    }
  }
  if (!selectedKey.value) return null
  return {
    id: selectedKey.value,
    slug: selectedKey.value,
    title: selectedKey.value,
    displayLabel: selectedKey.value,
    unit: "",
  }
})

const isNumeric = computed(() => {
  if (props.mode === "edit") {
    return Boolean(props.detail?.unit)
  }
  return Boolean(selectedOption.value?.unit)
})

watch(
  () => props.open,
  (open) => {
    if (!open) {
      searchTerm.value = ""
      detailOptions.value = []
      selectedKey.value = ""
      valueInput.value = ""
      validationError.value = ""
      definitionState.error = ""
      definitionState.success = ""
      return
    }
    if (props.mode === "edit" && props.detail?.detailSlug) {
      selectedKey.value = props.detail.detailSlug
      valueInput.value =
        props.detail.textValue ?? props.detail.numericValue?.toString() ?? ""
    } else {
      selectedKey.value = ""
      valueInput.value = ""
      void loadDefinitions()
    }
  },
)

async function loadDefinitions() {
  definitionState.error = ""
  definitionState.success = ""
  definitionState.loading = true
  const result = await api.queryDetails({ limit: 200 })
  definitionState.loading = false
  if (!result.ok) {
    definitionState.error = result.error || "Unable to load definitions."
    return
  }
  detailOptions.value = result.data || []
  if (!selectedKey.value && detailOptions.value[0]?.slug) {
    selectedKey.value = detailOptions.value[0].slug as string
  }
}

function submit() {
  validationError.value = ""
  const slug =
    props.mode === "edit" ? (props.detail?.detailSlug ?? "") : selectedKey.value
  if (!slug) {
    validationError.value = "Select a detail."
    return
  }
  const rawValue = valueInput.value.trim()
  if (!rawValue) {
    validationError.value = "Provide a value."
    return
  }
  if (isNumeric.value) {
    const numericValue = Number(rawValue)
    if (!Number.isFinite(numericValue)) {
      validationError.value = "Numeric value is invalid."
      return
    }
    emit("submit", {
      detailSlug: slug,
      title: selectedOption.value?.title ?? props.detail?.title ?? null,
      unit: selectedOption.value?.unit ?? props.detail?.unit ?? null,
      textValue: null,
      numericValue,
    })
    return
  }
  emit("submit", {
    detailSlug: slug,
    title: selectedOption.value?.title ?? props.detail?.title ?? null,
    unit: selectedOption.value?.unit ?? props.detail?.unit ?? null,
    textValue: rawValue,
    numericValue: null,
  })
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          {{ mode === "edit" ? "Edit Detail" : "Add Detail" }}
        </h3>
        <p class="text-muted text-sm">
          {{
            mode === "edit"
              ? "Update the selected detail value."
              : "Attach a new detail to the article."
          }}
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <div class="grid gap-3">
          <UFormField label="Detail" required class="w-full">
            <USelectMenu
              v-model="selectedKey"
              v-model:search-term="searchTerm"
              :items="optionItems"
              value-key="id"
              label-key="displayLabel"
              :search-input="true"
              :disabled="!canEdit || mode === 'edit'"
              size="md"
              placeholder="Select detail"
              class="w-full"
              :ui="{ base: 'w-full' }"
            />
          </UFormField>
        </div>

        <UFormField :label="isNumeric ? 'Numeric Value' : 'Value'" required class="w-full">
          <UInput
            v-model="valueInput"
            :type="isNumeric ? 'number' : 'text'"
            :placeholder="isNumeric ? 'e.g. 42' : 'Enter value'"
            :disabled="!canEdit"
            size="md"
            class="w-full"
            :ui="{ root: 'w-full' }"
          />
        </UFormField>

        <StatusMessages
          :error="validationError || definitionState.error || error"
        />
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
          {{ mode === "edit" ? "Save Changes" : "Add Detail" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
