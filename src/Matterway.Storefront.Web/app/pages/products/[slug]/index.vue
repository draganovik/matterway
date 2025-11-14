<script lang="ts" setup>
import { useCatalogStore } from "@stores/catalog";
import type ProductModel from "#models/ProductModel";
import { useSessionStore } from "@stores/session";
import { useCartStore } from "@stores/cart";

const catalogStore = useCatalogStore();
const sessionStore = useSessionStore();
const userCartStore = useCartStore();
const route = useRoute();
const router = useRouter();
const product = ref<ProductModel | null>(null);

const formatDate = (value?: string | Date | null) => {
  if (!value) return "-";
  const date = value instanceof Date ? value : new Date(value);
  return new Intl.DateTimeFormat("sr-RS", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  }).format(date);
};

const galleryImages = computed(() => {
  if (!product.value) {
    return [];
  }

  const images: {
    id?: number | string;
    imageUrl?: string;
    imageAlt?: string;
  }[] = [];

  if (product.value.thumbnailImage?.imageUrl) {
    images.push({
      id: `thumbnail-${product.value.id}`,
      imageUrl: product.value.thumbnailImage.imageUrl,
      imageAlt: product.value.thumbnailImage.imageAlt ?? product.value.title,
    });
  }

  const sortedImages = [...(product.value.productImages ?? [])].sort(
    (a, b) =>
      (a.orderIndex ?? Number.MAX_SAFE_INTEGER) -
      (b.orderIndex ?? Number.MAX_SAFE_INTEGER),
  );

  sortedImages
    .filter((image) => Boolean(image?.imageUrl))
    .forEach((image) => {
      images.push({
        id: image.id,
        imageUrl: image.imageUrl,
        imageAlt: image.imageAlt ?? product.value?.title,
      });
    });

  const uniqueByUrl = new Map<string, (typeof images)[number]>();
  images.forEach((image) => {
    if (image.imageUrl && !uniqueByUrl.has(image.imageUrl)) {
      uniqueByUrl.set(image.imageUrl, image);
    }
  });

  return Array.from(uniqueByUrl.values());
});

const deleteProduct = async () => {
  if (product.value == null) {
    return;
  }
  if (!confirm("Da li ste sigurni da želite da obrišete proizvod?")) {
    return;
  }
  const response = await catalogStore.deleteProduct(product.value);
  if (response.ok) {
    router.push("/products");
  }
};

useHead({
  title: "Proizvod",
});
onMounted(async () => {
  if (!route.params.slug) {
    return;
  }
  product.value = await catalogStore.fetchProductById(
    route.params.slug.toString(),
  );
});
</script>

<template>
  <section
    v-if="product == null"
    role="status"
    class="flex animate-pulse flex-col gap-8 md:grid md:grid-cols-5"
  >
    <div
      class="relative col-span-2 flex aspect-video w-full items-center justify-center rounded-sm bg-slate-300 dark:bg-slate-700 md:aspect-4/3"
    >
      <Icon
        name="heroicons-outline:photo"
        class="text-5xl text-slate-200"
        aria-hidden="true"
      />
    </div>
    <div class="col-span-3 flex w-full flex-col md:grid-cols-5">
      <div
        class="mt-4 h-3 w-full rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
      <div
        class="mt-2.5 h-3 w-full rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
      <br />
      <div
        class="mt-2.5 h-2.5 max-w-[480px] rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
      <div
        class="mt-2.5 h-2.5 rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
      <div
        class="mt-2.5 h-2.5 max-w-[440px] rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
      <div
        class="mt-2.5 h-2.5 max-w-[460px] rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
      <div
        class="mt-2.5 h-2.5 max-w-[360px] rounded-full bg-slate-200 dark:bg-slate-700"
      ></div>
    </div>
    <span class="sr-only">Loading...</span>
  </section>

  <section
    v-if="product != null"
    class="grid gap-6 lg:grid-cols-[2fr_3fr] lg:grid-rows-[auto_1fr]"
  >
    <ProductImageCarousel
      class=""
      :images="galleryImages"
      :fallback-alt="product?.title"
    />

    <section
      class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
    >
      <div class="flex flex-wrap items-start justify-between gap-4">
        <div class="space-y-2">
          <span
            class="inline-flex items-center rounded-full border px-3 py-1 text-xs font-semibold uppercase tracking-wide"
            :class="
              product.isAvailable
                ? 'border-green-200 bg-green-50 text-green-700 dark:border-green-700 dark:bg-green-900/30 dark:text-green-200'
                : 'border-red-200 bg-red-50 text-red-700 dark:border-red-700 dark:bg-red-900/30 dark:text-red-200'
            "
          >
            {{ product.isAvailable ? "Na stanju" : "Nije dostupno" }}
          </span>
          <h1
            class="text-2xl font-bold tracking-tight text-slate-900 dark:text-slate-100 sm:text-3xl md:text-4xl"
          >
            {{ product.title }}
          </h1>
          <p class="text-sm text-slate-500 dark:text-slate-400">
            Jedinstveni broj:
            <span class="font-mono">{{ product.productCode }}</span>
          </p>
        </div>
      </div>
      <div
        class="flex flex-wrap items-center gap-6 text-sm text-slate-500 dark:text-slate-400"
      >
        <div>
          Kreiran:
          <time class="font-medium text-slate-900 dark:text-slate-200">
            {{ formatDate(product.createdAt) }}
          </time>
        </div>
        <div>
          Poslednja izmena:
          <time class="font-medium text-slate-900 dark:text-slate-200">
            {{ formatDate(product.updatedAt) }}
          </time>
        </div>
      </div>
      <div class="flex flex-col gap-3 sm:flex-row sm:items-center">
        <template
          v-if="
            sessionStore.getTokenData?.role != 'Admin' &&
            sessionStore.getTokenData?.role != 'Manager'
          "
        >
          <button
            @click="userCartStore.addToCart(product)"
            type="button"
            class="inline-flex items-center justify-center rounded-lg bg-blue-700 px-6 py-2.5 text-sm font-semibold text-white transition hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
          >
            <Icon
              name="heroicons-solid:shopping-bag"
              aria-hidden="true"
              class="-ml-1 mr-2 text-xl"
            />
            Dodaj u korpu
          </button>
          <div
            v-if="userCartStore.isProductInCart(product.id)"
            class="flex items-center gap-2 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm font-semibold dark:border-slate-700 dark:bg-slate-800/60"
          >
            U korpi:
            <span class="text-lg">{{
              userCartStore.countProductsInCart(product.id)
            }}</span>
            <button
              type="button"
              class="text-xs font-medium text-red-600 hover:underline dark:text-red-400"
              @click="userCartStore.removeFromCart(product)"
            >
              Ukloni
            </button>
          </div>
        </template>
        <template v-else>
          <div class="flex gap-3">
            <NuxtLink
              :to="route.path + '/edit'"
              class="inline-flex items-center justify-center rounded-lg bg-blue-700 px-5 py-2 text-sm font-medium text-white hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            >
              Izmeni proizvod
            </NuxtLink>
            <button
              type="button"
              class="rounded-lg bg-red-600 px-5 py-2 text-sm font-medium text-white hover:bg-red-700 focus:outline-hidden focus:ring-4 focus:ring-red-300 dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800"
              @click="deleteProduct()"
            >
              Obriši proizvod
            </button>
          </div>
        </template>
      </div>
      <div class="border-y border-slate-200 py-4 dark:border-slate-700">
        <p class="text-3xl font-semibold text-slate-900 dark:text-slate-100">
          {{ formatMoney(product.price || 0) }}
        </p>
      </div>
    </section>

    <section
      class="rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800 lg:col-span-2"
    >
      <div class="mb-8">
        <h2
          class="mb-4 text-lg font-semibold text-slate-900 dark:text-slate-100"
        >
          Opis
        </h2>
        <pre
          class="text-base whitespace-pre-wrap font-[inherit] leading-relaxed text-slate-600 dark:text-slate-300"
          >{{ product.description }}</pre
        >
      </div>
      <h2
        v-if="product.productDetails && product.productDetails.length"
        class="mb-4 text-lg font-semibold text-slate-900 dark:text-slate-100"
      >
        Specifikacije
      </h2>
      <dl class="grid gap-4 md:grid-cols-2">
        <div
          v-for="detail in product.productDetails"
          :key="detail.typeSlug ?? detail.title"
          class="rounded-xl border border-slate-100 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-900/40"
        >
          <dt
            class="text-xs uppercase tracking-wide text-slate-500 dark:text-slate-400"
          >
            {{ detail.title ?? `Tip #${detail.typeSlug}` }}
          </dt>
          <dd class="text-base font-medium text-slate-900 dark:text-slate-100">
            {{ detail.value }}
            <span
              v-if="detail.unit"
              class="text-sm text-slate-500 dark:text-slate-400"
            >
              {{ detail.unit }}
            </span>
          </dd>
        </div>
      </dl>
    </section>
  </section>
</template>

<style scoped></style>
