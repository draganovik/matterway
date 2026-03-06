<script setup lang="ts">
import { useOverviewPage } from "~/composables/features/overview/useOverviewPage"

definePageMeta({
  title: "Pregled",
  public: true,
})

const {
  cart,
  loading,
  error,
  totalCount,
  featured,
  featuredCount,
  initialize,
} = useOverviewPage()

await initialize()
</script>

<template>
  <div class="space-y-8">
    <OverviewHeroPanel
      :total-count="totalCount"
      :featured-count="featuredCount"
      :cart-items="cart.totalItems.value"
    />

    <StatusMessages v-if="error" :error="error" />

    <section class="space-y-4">
      <div class="flex items-center justify-between">
        <h2 class="text-xl font-semibold">Izdvojeni artikli</h2>
        <UButton
          to="/articles"
          color="neutral"
          variant="ghost"
          trailing-icon="i-lucide-arrow-right"
        >
          Prikaži sve
        </UButton>
      </div>

      <div v-if="loading" class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <USkeleton
          v-for="n in 4"
          :key="`overview-skeleton-${n}`"
          class="h-80"
        />
      </div>

      <EmptyState
        v-else-if="!featured.length"
        title="Nema dostupnih artikala"
        description="Katalog je trenutno prazan."
      />

      <OverviewListView v-else :items="featured" />
    </section>
  </div>
</template>
