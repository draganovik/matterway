<script setup lang="ts">
import type { CatalogArticle } from "~/types/catalog"
import { useCartStore } from "~/composables/stores/useCartStore"
import { formatMoney } from "~/utils/formatters"

const props = defineProps<{
  article: CatalogArticle
}>()

const cart = useCartStore()

const hasDiscount = computed(
  () =>
    (props.article.discount?.percentage ?? 0) > 0 &&
    (props.article.basePrice ?? 0) > (props.article.price ?? 0),
)

const discountLabel = computed(() =>
  hasDiscount.value
    ? `${Math.round((props.article.discount?.percentage ?? 0) * 100)}%`
    : null,
)

const currentQty = computed(() => cart.quantityFor(props.article.code))
</script>

<template>
  <UCard
    class="border-default h-full overflow-hidden border"
    :ui="{
      root: 'h-full flex flex-col',
      header: 'p-0 sm:p-0',
      body: 'flex flex-1 flex-col px-3 pt-3 pb-2 sm:px-3 sm:pt-3 sm:pb-2',
    }"
  >
    <template #header>
      <NuxtLink :to="`/articles/${article.code}`" class="block">
        <div class="bg-elevated aspect-4/3 w-full overflow-hidden">
          <ImageWithFallback
            :src="article.thumbnailImage?.imageUrl || null"
            :alt="article.thumbnailImage?.imageAlt || article.title"
            img-class="h-full w-full object-cover"
            placeholder-class="h-full w-full"
          />
        </div>
      </NuxtLink>
    </template>

    <div class="flex flex-1 flex-col gap-3">
      <div class="space-y-1">
        <NuxtLink
          :to="`/articles/${article.code}`"
          class="line-clamp-2 text-base font-semibold hover:text-cyan-700"
        >
          {{ article.title }}
        </NuxtLink>
        <p class="text-muted text-xs">#{{ article.code }}</p>
      </div>

      <div class="flex flex-1 items-end justify-between">
        <div>
          <p class="text-lg font-semibold">
            {{ formatMoney(article.price ?? article.basePrice ?? 0) }}
          </p>
          <p v-if="hasDiscount" class="text-muted text-xs line-through">
            {{ formatMoney(article.basePrice ?? 0) }}
          </p>
        </div>
        <UBadge v-if="discountLabel" color="success" variant="soft">
          -{{ discountLabel }}
        </UBadge>
      </div>

      <div class="flex items-center justify-between gap-2">
        <UBadge
          :color="article.isAvailable ? 'success' : 'error'"
          variant="soft"
        >
          {{ article.isAvailable ? "Na stanju" : "Nije na stanju" }}
        </UBadge>

        <div class="flex items-center gap-2">
          <CartQuantityInput
            v-if="currentQty > 0"
            :article-code="article.code"
            :quantity="currentQty"
            :disabled="!article.isAvailable"
            :show-remove="true"
            @remove="cart.remove(article.code)"
          />
          <UButton
            v-else
            :disabled="!article.isAvailable"
            color="primary"
            variant="soft"
            icon="i-lucide-plus"
            square
            @click="cart.add(article)"
          />
        </div>
      </div>
    </div>
  </UCard>
</template>
