<script setup lang="ts">
import type {
  DetailFilterDefinition,
  DetailFilterState,
} from "~/composables/useArticleFilters"

const props = defineProps<{
  detailFilter: DetailFilterState
  detailDefinitions: DetailFilterDefinition[]
}>()

const emit = defineEmits<{
  remove: []
  setSlug: [slug: string]
  setValue: [value: string]
  setMin: [value: number | undefined]
  setMax: [value: number | undefined]
}>()

const searchTerm = ref("")

function toDisplayLabel(label: string, unit?: string | null) {
  const normalizedUnit = unit?.trim()
  return normalizedUnit ? `${label} (${normalizedUnit})` : label
}

const definitionItems = computed(() =>
  props.detailDefinitions.map((option) => {
    const title = option.label
    const unit = option.unit?.trim() ?? ""
    return {
      id: option.slug,
      slug: option.slug,
      title,
      displayLabel: toDisplayLabel(title, unit),
      unit,
    }
  }),
)

const selectedDefinition = computed(() =>
  props.detailDefinitions.find(
    (definition) => definition.slug === props.detailFilter.slug,
  ),
)

function normalizeSlug(value: unknown) {
  if (typeof value === "string" || typeof value === "number") {
    return String(value)
  }

  if (value && typeof value === "object") {
    const candidate = value as { id?: unknown; value?: unknown; slug?: unknown }
    if (typeof candidate.id === "string" || typeof candidate.id === "number") {
      return String(candidate.id)
    }
    if (
      typeof candidate.value === "string" ||
      typeof candidate.value === "number"
    ) {
      return String(candidate.value)
    }
    if (
      typeof candidate.slug === "string" ||
      typeof candidate.slug === "number"
    ) {
      return String(candidate.slug)
    }
  }

  return ""
}

const selectedSlug = computed({
  get: () => props.detailFilter.slug,
  set: (value: unknown) => emit("setSlug", normalizeSlug(value)),
})

const isNumeric = computed(() => Boolean(selectedDefinition.value?.unit))

function toNumberOrUndefined(value: string | number | null | undefined) {
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : undefined
}
</script>

<template>
  <UCard class="border-default bg-default border">
    <div class="space-y-3">
      <UFormField label="Detalj" required>
        <USelectMenu
          v-model="selectedSlug"
          v-model:search-term="searchTerm"
          :items="definitionItems"
          value-key="id"
          label-key="displayLabel"
          :search-input="true"
          placeholder="Izaberi detalj"
          class="w-full"
          :ui="{ content: 'border border-default bg-default shadow-sm' }"
        />
      </UFormField>

      <template v-if="isNumeric">
        <div class="grid grid-cols-2 gap-3">
          <UFormField label="Od">
            <UInput
              :model-value="detailFilter.min"
              type="number"
              min="0"
              placeholder="npr. 10"
              class="w-full"
              @update:model-value="emit('setMin', toNumberOrUndefined($event))"
            />
          </UFormField>
          <UFormField label="Do">
            <UInput
              :model-value="detailFilter.max"
              type="number"
              min="0"
              placeholder="npr. 100"
              class="w-full"
              @update:model-value="emit('setMax', toNumberOrUndefined($event))"
            />
          </UFormField>
        </div>
      </template>
      <template v-else>
        <UFormField label="Vrednost">
          <UInput
            :model-value="detailFilter.value"
            placeholder="Unesite vrednost"
            class="w-full"
            @update:model-value="emit('setValue', String($event ?? ''))"
          />
        </UFormField>
      </template>

      <div class="w-full">
        <UButton
          type="button"
          color="error"
          variant="ghost"
          icon="i-lucide-trash"
          block
          @click="emit('remove')"
        >
          Ukloni
        </UButton>
      </div>
    </div>
  </UCard>
</template>
