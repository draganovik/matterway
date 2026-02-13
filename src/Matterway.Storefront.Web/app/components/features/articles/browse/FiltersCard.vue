<script setup lang="ts">
import type {
  DetailFilterDefinition,
  DetailFilterState,
} from "~/composables/useArticleFilters"

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
  <UCard class="border-default bg-elevated/60 h-fit border">
    <template #header>
      <div class="space-y-1">
        <p class="text-primary text-xs tracking-[0.3em] uppercase">Pretraga</p>
        <h2 class="text-lg font-semibold">Pronađi artikle</h2>
      </div>
    </template>

    <form class="space-y-4" @submit.prevent="emit('submit')">
      <UFormField label="Naziv">
        <UInput
          :model-value="filters.search"
          placeholder="npr. Philips Hue"
          class="w-full"
          :ui="{ root: 'w-full' }"
          @update:model-value="emit('setSearch', String($event ?? ''))"
        />
      </UFormField>

      <div class="grid grid-cols-2 gap-3">
        <UFormField label="Min cena">
          <UInput
            :model-value="filters.minPrice"
            type="number"
            min="0"
            placeholder="0"
            class="w-full"
            @update:model-value="
              emit('setMinPrice', toNumberOrUndefined($event))
            "
          />
        </UFormField>
        <UFormField label="Max cena">
          <UInput
            :model-value="filters.maxPrice"
            type="number"
            min="0"
            placeholder="100000"
            class="w-full"
            @update:model-value="
              emit('setMaxPrice', toNumberOrUndefined($event))
            "
          />
        </UFormField>
      </div>

      <div class="space-y-3">
        <div class="flex items-center justify-between">
          <p class="text-sm font-medium">Filtriraj po detalju</p>
          <UButton
            type="button"
            color="neutral"
            variant="soft"
            icon="i-lucide-plus"
            :disabled="
              props.detailDefinitionsLoading || !props.detailDefinitions.length
            "
            @click="emit('addDetailFilter')"
          >
            Dodaj
          </UButton>
        </div>

        <p v-if="props.detailDefinitionsLoading" class="text-muted text-xs">
          Učitavanje tipova detalja...
        </p>

        <div v-if="filters.detailFilters.length" class="space-y-3">
          <ArticlesBrowseDetailFilterCard
            v-for="(detailFilter, index) in filters.detailFilters"
            :key="`detail-filter-${index}-${detailFilter.slug}`"
            :detail-filter="detailFilter"
            :detail-definitions="props.detailDefinitions"
            @remove="emit('removeDetailFilter', index)"
            @set-slug="
              emit('setDetailFilterSlug', { index, slug: String($event) })
            "
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
        <p v-else class="text-muted text-xs">Nema izabranih filtera detalja.</p>
      </div>

      <div class="space-y-2">
        <UButton type="submit" color="primary" block>Primeni filtere</UButton>
        <UButton
          type="button"
          color="neutral"
          variant="soft"
          block
          @click="emit('reset')"
        >
          Poništi
        </UButton>
      </div>
    </form>
  </UCard>
</template>
