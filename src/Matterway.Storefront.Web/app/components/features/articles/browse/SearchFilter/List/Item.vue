<script setup lang="ts">
import type {
  DetailFilterDefinition,
  DetailFilterState,
} from "~/lib/articles/filters"

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
  <div class="border-default bg-default rounded-lg border p-2">
    <div
      class="grid gap-2 lg:grid-cols-[minmax(0,1.4fr)_minmax(0,0.8fr)_minmax(0,0.8fr)_2.5rem] lg:items-center"
    >
      <USelectMenu
        v-model="selectedSlug"
        v-model:search-term="searchTerm"
        :items="definitionItems"
        value-key="id"
        label-key="displayLabel"
        :search-input="true"
        placeholder="Detalj"
        aria-label="Detalj"
        size="sm"
        class="w-full"
        :ui="{ content: 'border border-default bg-default shadow-sm' }"
      />

      <template v-if="isNumeric">
        <UInputNumber
          :model-value="detailFilter.min ?? null"
          orientation="vertical"
          :min="0"
          :step="0.01"
          variant="outline"
          placeholder="Od"
          aria-label="Minimalna vrednost"
          size="sm"
          class="w-full"
          :ui="{ root: 'w-full', base: 'w-full text-left' }"
          @update:model-value="emit('setMin', toNumberOrUndefined($event))"
        />
        <UInputNumber
          :model-value="detailFilter.max ?? null"
          orientation="vertical"
          :min="0"
          :step="0.01"
          variant="outline"
          placeholder="Do"
          aria-label="Maksimalna vrednost"
          size="sm"
          class="w-full"
          :ui="{ root: 'w-full', base: 'w-full text-left' }"
          @update:model-value="emit('setMax', toNumberOrUndefined($event))"
        />
      </template>
      <template v-else>
        <UInput
          :model-value="detailFilter.value"
          placeholder="Vrednost"
          aria-label="Vrednost detalja"
          size="sm"
          class="w-full lg:col-span-2"
          @update:model-value="emit('setValue', String($event ?? ''))"
        />
      </template>

      <div class="flex justify-end">
        <UButton
          type="button"
          color="error"
          variant="ghost"
          icon="i-lucide-trash"
          size="sm"
          square
          aria-label="Ukloni filter"
          title="Ukloni filter"
          @click="emit('remove')"
        />
      </div>
    </div>
  </div>
</template>
