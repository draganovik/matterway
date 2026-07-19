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
    class="hover:border-accented transition-colors"
    :ui="{
      root: 'flex flex-col',
      header: 'p-0 sm:p-0',
      body: 'flex flex-1 flex-col p-3 sm:p-3',
    }"
  >
    <template #header>
      <NuxtLink :to="`/articles/${article.code}`" class="block">
        <div class="bg-elevated relative aspect-4/3 w-full overflow-hidden">
          <ImageWithFallback
            :src="article.thumbnailImage?.imageUrl || null"
            :alt="article.thumbnailImage?.imageAlt || article.title"
            img-class="h-full w-full object-cover"
            placeholder-class="h-full w-full"
          />
          <UBadge
            v-if="!article.isAvailable"
            color="error"
            variant="soft"
            class="absolute top-2 right-2"
          >
            Nije na stanju
          </UBadge>
        </div>
      </NuxtLink>
    </template>

    <div class="flex flex-1 flex-col gap-2.5">
      <div class="space-y-1">
        <NuxtLink
          :to="`/articles/${article.code}`"
          class="hover:text-primary line-clamp-2 text-base font-semibold"
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

      <div class="flex min-h-8 items-start justify-end">
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
          :aria-label="`Dodaj ${article.title} u korpu`"
          color="primary"
          variant="soft"
          icon="i-lucide-plus"
          square
          @click="cart.add(article)"
        />
      </div>
    </div>
  </UCard>
</template>
