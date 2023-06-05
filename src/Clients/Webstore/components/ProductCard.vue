<script lang="ts" setup>
import { useCartStore } from "~/store/cart";
import { useSessionStore } from "~/store/session";

const session = useSessionStore();

const userCartStore = useCartStore();
defineProps({
  product: {
    type: ProductModel,
    required: true,
  },
});
</script>
<template>
  <div
    class="grid w-full max-w-sm grid-rows-[min-content] flex-col place-items-stretch rounded-lg border border-slate-200 bg-white shadow dark:border-slate-700 dark:bg-slate-800"
  >
    <NuxtLink
      :to="'/products/' + product.id"
      class="h-60 w-full"
      v-if="product.productImages.length > 0"
    >
      <img
        class="h-full w-full rounded-t-lg object-cover"
        :src="
          product.productImages.sort((a, b) => +!!a.isMain - +!!b.isMain)[0]
            .imageUrl
        "
        :alt="
          product.productImages.sort((a, b) => +!!a.isMain - +!!b.isMain)[0]
            .imageAlt
        "
      />
    </NuxtLink>
    <div class="flex w-full flex-col justify-between gap-8 p-5 pt-3">
      <NuxtLink :to="'/products/' + product.id">
        <h5
          class="text-xl font-semibold tracking-tight text-slate-900 dark:text-white"
        >
          {{ product.title }}
        </h5>
      </NuxtLink>
      <div class="mb-5 mt-2.5 flex items-center" v-if="false">
        <svg
          aria-hidden="true"
          class="h-5 w-5 text-yellow-300"
          fill="currentColor"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg"
        >
          <title>First star</title>
          <path
            d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
          ></path>
        </svg>
        <svg
          aria-hidden="true"
          class="h-5 w-5 text-yellow-300"
          fill="currentColor"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg"
        >
          <title>Second star</title>
          <path
            d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
          ></path>
        </svg>
        <svg
          aria-hidden="true"
          class="h-5 w-5 text-yellow-300"
          fill="currentColor"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg"
        >
          <title>Third star</title>
          <path
            d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
          ></path>
        </svg>
        <svg
          aria-hidden="true"
          class="h-5 w-5 text-yellow-300"
          fill="currentColor"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg"
        >
          <title>Fourth star</title>
          <path
            d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
          ></path>
        </svg>
        <svg
          aria-hidden="true"
          class="h-5 w-5 text-yellow-300"
          fill="currentColor"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg"
        >
          <title>Fifth star</title>
          <path
            d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
          ></path>
        </svg>
        <span
          class="ml-3 mr-2 rounded bg-blue-100 px-2.5 py-0.5 text-xs font-semibold text-blue-800 dark:bg-blue-200 dark:text-blue-800"
          >5.0</span
        >
      </div>
      <div class="flex flex-col items-end justify-between gap-4">
        <span
          class="text-right text-3xl font-bold text-slate-900 dark:text-white"
          >{{ formatMoney(product.price) }}</span
        >
        <div
          class="grid w-full grid-cols-5 gap-4"
          v-if="
            session.getTokenData?.role != 'Admin' &&
            session.getTokenData?.role != 'Manager'
          "
        >
          <p
            v-if="userCartStore.isProductInCart(product.id)"
            class="grid place-items-center rounded bg-slate-500/20 text-xl font-semibold"
          >
            {{ userCartStore.countProductsInCart(product.id) }}
          </p>
          <button
            type="button"
            v-if="userCartStore.isProductInCart(product.id)"
            @click="userCartStore.removeFromCart(product)"
            :class="
              userCartStore.isProductInCart(product.id)
                ? 'col-span-4'
                : 'col-span-5'
            "
            class="w-full rounded-lg bg-red-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-800 focus:outline-none focus:ring-4 focus:ring-red-300 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-900"
          >
            Ukloni iz korpe
          </button>
          <button
            type="button"
            @click="userCartStore.addToCart(product)"
            class="col-span-5 w-full rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
          >
            Dodaj u korpu
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<style></style>
