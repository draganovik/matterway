<script setup lang="ts">
import {
  OVERVIEW_CATEGORY_FILTERS,
  useOverviewPage,
} from "~/composables/features/overview/useOverviewPage"

definePageMeta({
  title: "Početna",
  public: true,
})

const {
  cart,
  loading,
  error,
  totalCount,
  latest,
  lights,
  cameras,
  loadOverview,
} = useOverviewPage()

await loadOverview()

const sections = computed(() => [
  {
    key: "latest",
    title: "Najnoviji artikli",
    description: "Poslednji dodati artikli u Matterway katalogu.",
    to: "/articles",
    items: latest.value,
  },
  {
    key: "lights",
    title: "Pametna rasveta",
    description: "Rasveta koju možete prilagoditi svom prostoru i ritmu.",
    to: {
      path: "/articles",
      query: { filter: OVERVIEW_CATEGORY_FILTERS.light },
    },
    items: lights.value,
  },
  {
    key: "cameras",
    title: "Pametne kamere",
    description: "Kamere za jednostavniji nadzor doma i okruženja.",
    to: {
      path: "/articles",
      query: { filter: OVERVIEW_CATEGORY_FILTERS.camera },
    },
    items: cameras.value,
  },
])
</script>

<template>
  <div class="mx-auto max-w-[62.25rem] space-y-8">
    <OverviewHeroPanel
      :total-count="totalCount"
      :cart-items="cart.totalItems.value"
    />

    <StatusMessages v-if="error" :error="error" />

    <section v-for="section in sections" :key="section.key" class="space-y-3">
      <div
        class="border-default flex items-end justify-between gap-4 border-b pb-2"
      >
        <div>
          <h2 class="text-xl font-semibold">{{ section.title }}</h2>
          <p class="text-muted mt-0.5 text-sm">{{ section.description }}</p>
        </div>
        <UButton
          :to="section.to"
          color="neutral"
          variant="ghost"
          trailing-icon="i-lucide-arrow-right"
          class="shrink-0"
        >
          Vidi više
        </UButton>
      </div>

      <div v-if="loading" class="flex flex-wrap justify-center gap-3">
        <USkeleton
          v-for="n in 4"
          :key="`${section.key}-skeleton-${n}`"
          class="h-[22rem] w-full sm:w-60 sm:flex-none"
        />
      </div>

      <EmptyState
        v-else-if="!section.items.length"
        :title="`Nema artikala u odeljku ${section.title.toLocaleLowerCase()}`"
        description="Pogledajte ostatak kataloga ili pokušajte ponovo kasnije."
      />

      <ArticlesBrowseListView
        v-else
        :items="section.items"
        :is-refreshing="false"
      />
    </section>
  </div>
</template>
