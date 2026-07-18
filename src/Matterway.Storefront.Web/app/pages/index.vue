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
  <div class="mx-auto max-w-[66.25rem] space-y-8">
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

      <div
        v-if="loading"
        class="-mx-4 flex w-[calc(100%+2rem)] snap-x snap-mandatory scroll-px-4 gap-3 overflow-x-auto overscroll-x-contain px-4 pb-2 sm:-mx-5 sm:w-[calc(100%+2.5rem)] sm:scroll-px-5 sm:px-5 lg:-mx-6 lg:w-[calc(100%+3rem)] lg:scroll-px-6 lg:px-6 min-[75rem]:-mx-[2.875rem] min-[75rem]:w-[calc(100%+5.75rem)] min-[75rem]:scroll-px-[2.875rem] min-[75rem]:px-[2.875rem]"
        :aria-label="`Učitavanje odeljka ${section.title}`"
      >
        <USkeleton
          v-for="n in 4"
          :key="`${section.key}-skeleton-${n}`"
          class="h-[22rem] w-[min(16rem,calc(100vw-2rem))] flex-none snap-start"
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
        :aria-label="section.title"
        class="-mx-4 w-[calc(100%+2rem)] sm:-mx-5 sm:w-[calc(100%+2.5rem)] lg:-mx-6 lg:w-[calc(100%+3rem)] min-[75rem]:-mx-[2.875rem] min-[75rem]:w-[calc(100%+5.75rem)]"
        carousel
      />
    </section>
  </div>
</template>
