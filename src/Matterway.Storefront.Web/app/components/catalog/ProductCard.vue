<script lang="ts" setup>
import { computed } from "vue";
import type ProductModel from "#models/ProductModel";
import { useCartStore } from "@stores/cart";
import { useSessionStore } from "@stores/session";

const session = useSessionStore();

const userCartStore = useCartStore();
const props = defineProps<{
  product: ProductModel;
}>();

const heroImage = computed(() => {
  if (props.product.thumbnailImage?.imageUrl) {
    return props.product.thumbnailImage;
  }
  const images = props.product.productImages ?? [];
  if (!images.length) {
    return null;
  }
  const sortedImages = [...images].sort(
    (a, b) =>
      (a.orderIndex ?? Number.MAX_SAFE_INTEGER) -
      (b.orderIndex ?? Number.MAX_SAFE_INTEGER),
  );
  const candidate = sortedImages[0];
  if (!candidate?.imageUrl) {
    return null;
  }
  return {
    imageUrl: candidate.imageUrl,
    imageAlt: candidate.imageAlt ?? props.product.title,
  };
});

const availabilityLabel = computed(() =>
  props.product.isAvailable ? "Na stanju" : "Nije dostupno",
);

const availabilityClasses = computed(() =>
  props.product.isAvailable
    ? "border-green-200 bg-green-50 text-green-700 dark:border-green-700 dark:bg-green-900/30 dark:text-green-200"
    : "border-red-200 bg-red-50 text-red-700 dark:border-red-700 dark:bg-red-900/30 dark:text-red-200",
);

const canManage = computed(() => {
  const role = session.getTokenData?.role;
  return role === "Admin" || role === "Manager";
});

const productLink = computed(() => `/products/${props.product.id}`);

const hasDiscount = computed(
  () =>
    (props.product.discount?.percentage ?? 0) > 0 &&
    (props.product.basePrice ?? 0) > (props.product.price ?? 0),
);

const discountPercentLabel = computed(() => {
  if (!hasDiscount.value) return null;
  return Math.round((props.product.discount?.percentage ?? 0) * 100);
});
</script>
<template>
  <div
    class="flex h-full w-full flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xs transition hover:-translate-y-1 hover:shadow-lg dark:border-slate-700 dark:bg-slate-800"
  >
    <NuxtLink :to="productLink" class="relative h-56 w-full overflow-hidden">
      <span
        v-if="availabilityLabel == 'Nije dostupno'"
        class="absolute right-1 top-1 inline-flex items-center rounded-full border px-3 py-1 text-xs font-semibold uppercase tracking-wide"
        :class="availabilityClasses"
      >
        {{ availabilityLabel }}
      </span>
      <img
        v-if="heroImage"
        class="h-full w-full object-cover transition duration-300"
        :src="heroImage.imageUrl"
        :alt="heroImage.imageAlt || product.title"
      />
      <div
        v-else
        class="flex h-full w-full items-center justify-center bg-slate-100 p-[33%] text-slate-400 dark:bg-slate-700/60 dark:text-slate-500"
      >
        <Icon
          name="heroicons-outline:photo"
          class="text-2xl"
          aria-hidden="true"
        />
      </div>
    </NuxtLink>
    <div class="flex flex-1 flex-col gap-5 p-5">
      <div class="flex flex-col gap-2">
        <NuxtLink :to="productLink">
          <h5
            class="text-lg font-semibold tracking-tight text-slate-900 transition hover:text-blue-700 dark:text-white dark:hover:text-blue-300"
          >
            {{ product.title }}
          </h5>
        </NuxtLink>
        <p class="text-xs uppercase tracking-wide text-slate-400">
          #{{ product.productCode }}
        </p>
      </div>

      <div class="flex items-baseline justify-between gap-3">
        <div class="flex flex-col gap-1">
          <div class="flex items-center gap-2">
            <p
              class="text-2xl font-semibold text-slate-900 dark:text-slate-100"
            >
              {{ formatMoney(product.price ?? product.basePrice ?? 0) }}
            </p>
            <span
              v-if="hasDiscount"
              class="inline-flex items-center rounded-full bg-emerald-100 px-2.5 py-0.5 text-xs font-semibold uppercase tracking-wide text-emerald-700 dark:bg-emerald-900/40 dark:text-emerald-200"
            >
              -{{ discountPercentLabel }}%
            </span>
          </div>
          <p
            v-if="hasDiscount"
            class="text-sm text-slate-500 line-through dark:text-slate-400"
          >
            {{ formatMoney(product.basePrice ?? 0) }}
          </p>
        </div>
        <NuxtLink
          v-if="canManage"
          :to="productLink + '/edit'"
          class="text-sm font-medium text-blue-600 hover:underline dark:text-blue-400"
        >
          Uredi
        </NuxtLink>
      </div>

      <div class="flex flex-1 flex-col justify-end gap-4">
        <div
          v-if="!canManage"
          class="flex flex-wrap items-center justify-between gap-3"
        >
          <div
            v-if="userCartStore.isProductInCart(product.id)"
            class="inline-flex items-center gap-2 rounded-lg border border-blue-100 bg-blue-50 px-3 py-1.5 text-sm font-medium text-blue-700 dark:border-blue-800 dark:bg-blue-900/30 dark:text-blue-200"
          >
            U korpi:
            <span class="text-lg">{{
              userCartStore.countProductsInCart(product.id)
            }}</span>
          </div>
          <div class="flex flex-1 items-center justify-end gap-2">
            <button
              v-if="userCartStore.isProductInCart(product.id)"
              type="button"
              class="rounded-lg border border-red-200 px-3 py-2 text-sm font-medium text-red-600 hover:bg-red-50 focus:outline-hidden focus:ring-2 focus:ring-red-200 dark:border-red-700 dark:text-red-300 dark:hover:bg-red-900/30 dark:focus:ring-red-800"
              @click="userCartStore.removeFromCart(product)"
            >
              Ukloni
            </button>
            <button
              type="button"
              class="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
              @click="userCartStore.addToCart(product)"
            >
              Dodaj u korpu
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
