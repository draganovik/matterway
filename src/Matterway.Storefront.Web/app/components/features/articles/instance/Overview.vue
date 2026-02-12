<script setup lang="ts">
import { useCart } from "~/composables/useCart";
import type { CatalogArticle } from "~/types/catalog/articles";
import { formatDate, formatMoney } from "~/utils/formatters";

const props = defineProps<{
  article: CatalogArticle;
}>();

const cart = useCart();
const quantity = computed(() => cart.quantityFor(props.article.id));

const hasDiscount = computed(
  () =>
    (props.article.discount?.percentage ?? 0) > 0 &&
    (props.article.basePrice ?? 0) > (props.article.price ?? 0),
);
</script>

<template>
  <UCard class="border-default bg-default border">
    <div class="space-y-4">
      <div>
        <p class="text-muted text-xs">#{{ props.article.articleCode }}</p>
        <h1 class="text-2xl font-semibold">{{ props.article.title }}</h1>
      </div>

      <div class="flex items-center gap-2">
        <p class="text-2xl font-semibold">
          {{ formatMoney(props.article.price ?? props.article.basePrice ?? 0) }}
        </p>
        <UBadge v-if="hasDiscount" color="success" variant="soft">
          -{{ Math.round((props.article.discount?.percentage ?? 0) * 100) }}%
        </UBadge>
      </div>
      <p v-if="hasDiscount" class="text-muted text-sm line-through">
        {{ formatMoney(props.article.basePrice ?? 0) }}
      </p>

      <div class="flex items-center gap-2">
        <UBadge
          :color="props.article.isAvailable ? 'success' : 'error'"
          variant="soft"
        >
          {{ props.article.isAvailable ? "Na stanju" : "Nije na stanju" }}
        </UBadge>
        <UBadge v-if="quantity > 0" color="primary" variant="subtle">
          U korpi: {{ quantity }}
        </UBadge>
      </div>

      <div class="flex items-center gap-2">
        <UButton
          :disabled="!props.article.isAvailable"
          color="primary"
          icon="i-lucide-plus"
          @click="cart.add(props.article)"
        >
          Dodaj u korpu
        </UButton>
        <UButton
          v-if="quantity > 0"
          color="neutral"
          variant="soft"
          icon="i-lucide-minus"
          @click="cart.decrease(props.article.id)"
        >
          Ukloni jedan
        </UButton>
      </div>

      <p class="text-muted text-xs">
        Ažurirano: {{ formatDate(props.article.updatedAt) }}
      </p>
    </div>
  </UCard>
</template>
