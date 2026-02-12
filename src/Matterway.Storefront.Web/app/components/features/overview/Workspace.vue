<script setup lang="ts">
import { useCatalogApi } from "~/composables/useCatalogApi";
import { useCart } from "~/composables/useCart";

const catalogApi = useCatalogApi();
const cart = useCart();

const loading = ref(true);
const error = ref("");
const articles = ref(
  [] as Awaited<ReturnType<typeof catalogApi.browseArticles>>["items"],
);
const totalCount = ref(0);

async function loadOverview() {
  loading.value = true;
  error.value = "";

  const result = await catalogApi.browseArticles({
    page: 1,
    pageSize: 8,
  });

  if (result.error) error.value = result.error;
  articles.value = result.items;
  totalCount.value = result.meta?.totalCount ?? result.items.length;
  loading.value = false;
}

onMounted(() => {
  void loadOverview();
});

const featured = computed(() => articles.value.slice(0, 4));
const availableCount = computed(
  () => articles.value.filter((article) => article.isAvailable).length,
);
</script>

<template>
  <div class="space-y-8">
    <UCard class="border-default bg-elevated/60 border">
      <div class="grid gap-6 lg:grid-cols-[1.2fr_1fr]">
        <div class="space-y-4">
          <p class="text-primary text-xs tracking-[0.3em] uppercase">
            Pregled
          </p>
          <h1 class="text-3xl font-semibold">Prodavnica za kupce</h1>
          <p class="text-muted max-w-2xl text-sm">
            Pregledajte proizvode za pametan dom, koristite korpu kao gost ili
            kao prijavljen korisnik i završite kupovinu u jednom toku.
          </p>
          <div class="flex flex-wrap gap-2">
            <UButton to="/articles" color="primary" icon="i-lucide-arrow-right">
              Pregledaj artikle
            </UButton>
            <UButton
              to="/cart"
              color="neutral"
              variant="soft"
              icon="i-lucide-shopping-bag"
            >
              Otvori korpu
            </UButton>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <UCard class="border-default bg-default border">
            <p class="text-muted text-xs">Stavke u katalogu</p>
            <p class="text-2xl font-semibold">{{ totalCount }}</p>
          </UCard>
          <UCard class="border-default bg-default border">
            <p class="text-muted text-xs">Trenutno dostupno</p>
            <p class="text-2xl font-semibold">{{ availableCount }}</p>
          </UCard>
          <UCard class="border-default bg-default border col-span-2">
            <p class="text-muted text-xs">Stavki u vašoj korpi</p>
            <p class="text-2xl font-semibold">{{ cart.totalItems.value }}</p>
          </UCard>
        </div>
      </div>
    </UCard>

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

      <CommonEmptyState
        v-else-if="!featured.length"
        title="Nema dostupnih artikala"
        description="Katalog je trenutno prazan."
      />

      <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <FeaturesArticlesBrowseListItem
          v-for="article in featured"
          :key="article.id"
          :article="article"
        />
      </div>
    </section>
  </div>
</template>
