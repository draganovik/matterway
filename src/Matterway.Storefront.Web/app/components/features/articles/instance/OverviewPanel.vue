<script setup lang="ts">
import { useCartStore } from "~/composables/stores/useCartStore"
import type { CatalogArticle } from "~/types/catalog/articles"
import { formatDate, formatMoney } from "~/utils/formatters"

const props = defineProps<{
  article: CatalogArticle
}>()

const cart = useCartStore()
const quantity = computed(() => cart.quantityFor(props.article.code))

const hasDiscount = computed(
  () =>
    (props.article.discount?.percentage ?? 0) > 0 &&
    (props.article.basePrice ?? 0) > (props.article.price ?? 0),
)
</script>

<template>
  <UCard class="border-default border">
    <div class="space-y-4">
      <div>
        <p class="text-muted text-xs">#{{ props.article.code }}</p>
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
      </div>

      <div class="flex items-center gap-2">
        <UButton
          v-if="quantity === 0"
          :disabled="!props.article.isAvailable"
          size="lg"
          color="primary"
          icon="i-lucide-plus"
          @click="cart.add(props.article)"
        >
          Dodaj u korpu
        </UButton>
        <template v-else>
          <span class="text-muted text-sm font-medium">U korpi:</span>
          <CartQuantityInput
            :article-code="props.article.code"
            :quantity="quantity"
            size="lg"
            :show-remove="true"
            @remove="cart.remove(props.article.code)"
          />
        </template>
      </div>

      <p class="text-muted text-xs">
        Ažurirano: {{ formatDate(props.article.updatedAt) }}
      </p>
    </div>
  </UCard>
</template>
