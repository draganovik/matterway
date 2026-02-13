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

const definitionItems = computed(() =>
  props.detailDefinitions.map((option) => ({
    label: option.unit ? `${option.label} (${option.unit})` : option.label,
    value: option.slug,
  })),
)

const selectedDefinition = computed(() =>
  props.detailDefinitions.find(
    (definition) => definition.slug === props.detailFilter.slug,
  ),
)

const isNumeric = computed(() => Boolean(selectedDefinition.value?.unit))

function onSlugChange(value: string | number | null | undefined) {
  emit("setSlug", String(value ?? ""))
}

function toNumberOrUndefined(value: string | number | null | undefined) {
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : undefined
}
</script>

<template>
  <UCard class="border-default bg-default border">
    <div class="space-y-3">
      <UFormField label="Detalj" required>
        <USelect
          :model-value="detailFilter.slug"
          :items="definitionItems"
          placeholder="Izaberi detalj"
          class="w-full"
          @update:model-value="onSlugChange"
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

      <div class="flex justify-end">
        <UButton
          type="button"
          color="error"
          variant="ghost"
          icon="i-lucide-trash"
          @click="emit('remove')"
        >
          Ukloni
        </UButton>
      </div>
    </div>
  </UCard>
</template>
