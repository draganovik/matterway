<script setup lang="ts">
import type {
  DetailFilterDefinition,
  DetailFilterState,
} from "~/lib/articles/filters"

type FiltersState = {
  search: string
  minPrice?: number
  maxPrice?: number
  detailFilters: DetailFilterState[]
}

const props = defineProps<{
  filters: FiltersState
  detailDefinitions: DetailFilterDefinition[]
  detailDefinitionsLoading: boolean
}>()

const emit = defineEmits<{
  submit: []
  reset: []
  addDetailFilter: []
  removeDetailFilter: [index: number]
  setSearch: [value: string]
  setMinPrice: [value: number | undefined]
  setMaxPrice: [value: number | undefined]
  setDetailFilterSlug: [payload: { index: number; slug: string }]
  setDetailFilterValue: [payload: { index: number; value: string }]
  setDetailFilterMin: [payload: { index: number; value: number | undefined }]
  setDetailFilterMax: [payload: { index: number; value: number | undefined }]
}>()

function toNumberOrUndefined(value: string | number | null | undefined) {
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : undefined
}
</script>

<template>
  <div class="h-fit lg:flex lg:h-full lg:min-h-0 lg:flex-col">
    <form
      class="space-y-3 lg:flex lg:h-full lg:min-h-0 lg:flex-col lg:overflow-hidden"
      @submit.prevent="emit('submit')"
    >
      <div class="flex items-center gap-2">
        <UButton type="submit" color="primary" class="flex-1 justify-center">
          Primeni filtere
        </UButton>
        <UButton
          type="button"
          color="neutral"
          variant="soft"
          icon="i-lucide-x"
          aria-label="Poništi filtere"
          title="Poništi filtere"
          @click="emit('reset')"
        />
      </div>

      <UFormField label="Pretraga">
        <UInput
          :model-value="filters.search"
          placeholder="npr. Philips Hue"
          class="w-full"
          @update:model-value="emit('setSearch', String($event ?? ''))"
        />
      </UFormField>

      <div class="grid grid-cols-2 gap-3">
        <UFormField label="Minimalna cena">
          <UInputNumber
            :model-value="filters.minPrice ?? null"
            orientation="vertical"
            :min="0"
            :step="0.01"
            variant="outline"
            placeholder="0"
            class="w-full"
            :ui="{ base: 'w-full text-left' }"
            @update:model-value="
              emit('setMinPrice', toNumberOrUndefined($event))
            "
          />
        </UFormField>
        <UFormField label="Maksimalna cena">
          <UInputNumber
            :model-value="filters.maxPrice ?? null"
            orientation="vertical"
            :min="0"
            :step="0.01"
            variant="outline"
            placeholder="100000"
            class="w-full"
            :ui="{ base: 'w-full text-left' }"
            @update:model-value="
              emit('setMaxPrice', toNumberOrUndefined($event))
            "
          />
        </UFormField>
      </div>

      <div class="space-y-3 lg:flex lg:min-h-0 lg:flex-1 lg:flex-col">
        <div class="flex items-center justify-between">
          <p class="text-sm font-medium">Filtriraj po detalju</p>
          <UButton
            type="button"
            color="neutral"
            variant="outline"
            icon="i-lucide-plus"
            :disabled="
              props.detailDefinitionsLoading || !props.detailDefinitions.length
            "
            @click="emit('addDetailFilter')"
          >
            Dodaj filter
          </UButton>
        </div>

        <div
          v-if="filters.detailFilters.length"
          class="space-y-2 lg:min-h-0 lg:flex-1 lg:overflow-y-auto lg:p-1"
        >
          <ArticlesBrowseSearchFilterListItem
            v-for="(detailFilter, index) in filters.detailFilters"
            :key="`detail-filter-${index}-${detailFilter.slug}`"
            :detail-filter="detailFilter"
            :detail-definitions="props.detailDefinitions"
            @remove="emit('removeDetailFilter', index)"
            @set-slug="emit('setDetailFilterSlug', { index, slug: $event })"
            @set-value="
              emit('setDetailFilterValue', {
                index,
                value: String($event ?? ''),
              })
            "
            @set-min="emit('setDetailFilterMin', { index, value: $event })"
            @set-max="emit('setDetailFilterMax', { index, value: $event })"
          />
        </div>
      </div>
    </form>
  </div>
</template>
