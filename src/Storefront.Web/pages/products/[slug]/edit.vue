<script lang="ts" setup>
import { useCatalogStore } from "~/stores/catalog";
import ProductModel, { type ProductDetails } from "~/models/ProductModel";

const catalogStore = useCatalogStore();
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

  product.value.productImages
    ?.filter((image) => Boolean(image?.imageUrl))
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

const inputSpecs = ref({
  title: "",
  value: "",
});

const inputImage = ref({
  imageId: 0,
  imageUrl: "",
  imageAlt: "product image",
});

const addSpec = async () => {
  if (!product.value || !inputSpecs.value.title || !inputSpecs.value.value) {
    return;
  }
  const response = await catalogStore.createProductSpec(
    product.value.id,
    inputSpecs.value.title,
    inputSpecs.value.value,
  );
  if (response.ok) {
    inputSpecs.value.title = "";
    inputSpecs.value.value = "";
    await loadProduct();
  }
};

const removeSpec = async (detailId: string) => {
  if (!product.value) {
    return;
  }
  const response = await catalogStore.deleteProductSpec(detailId);
  if (response.ok) {
    await loadProduct();
  }
};

const updateSpec = async (detail: ProductDetails) => {
  if (!product.value) {
    return;
  }
  const response = await catalogStore.updateProductSpec(
    detail.id!,
    product.value.id,
    detail.title ?? "",
    detail.value ?? "",
    detail.type ?? "Specification",
    detail.unit ?? null,
  );
  if (response.ok) {
    await loadProduct();
  }
};

const addImage = async () => {
  if (!product.value || !inputImage.value.imageUrl) {
    return;
  }
  const response = await catalogStore.createProductImage(
    product.value.id,
    inputImage.value.imageId,
    inputImage.value.imageUrl,
    inputImage.value.imageAlt,
  );
  if (response.ok) {
    inputImage.value.imageId = 0;
    inputImage.value.imageUrl = "";
    inputImage.value.imageAlt = "product image";
    await loadProduct();
  }
};

const deleteImage = async (imageId: number) => {
  if (!product.value) {
    return;
  }
  const response = await catalogStore.deleteProductImage(
    product.value.id,
    imageId,
  );
  if (response.ok) {
    await loadProduct();
  }
};

const deleteProduct = async () => {
  if (!product.value) {
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

const loadProduct = async () => {
  product.value = await catalogStore.fetchProductById(
    route.params.slug.toString(),
  );
};

useHead({
  title: "Proizvod",
});

onMounted(async () => {
  await loadProduct();
});
</script>

<template>
  <section
    v-if="product == null"
    role="status"
    class="flex animate-pulse flex-col gap-8 md:grid md:grid-cols-5"
  >
    <div
      class="relative col-span-2 flex aspect-video w-full items-center justify-center rounded bg-slate-300 dark:bg-slate-700 md:aspect-[4/3]"
    >
      <svg
        class="h-12 w-12 text-slate-200"
        xmlns="http://www.w3.org/2000/svg"
        aria-hidden="true"
        fill="currentColor"
        viewBox="0 0 640 512"
      >
        <path
          d="M480 80C480 35.82 515.8 0 560 0C604.2 0 640 35.82 640 80C640 124.2 604.2 160 560 160C515.8 160 480 124.2 480 80zM0 456.1C0 445.6 2.964 435.3 8.551 426.4L225.3 81.01C231.9 70.42 243.5 64 256 64C268.5 64 280.1 70.42 286.8 81.01L412.7 281.7L460.9 202.7C464.1 196.1 472.2 192 480 192C487.8 192 495 196.1 499.1 202.7L631.1 419.1C636.9 428.6 640 439.7 640 450.9C640 484.6 612.6 512 578.9 512H55.91C25.03 512 .0006 486.1 .0006 456.1L0 456.1z"
        />
      </svg>
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

  <section v-else class="grid gap-10 lg:grid-cols-[minmax(280px,36%)_1fr]">
    <div class="space-y-6">
      <ProductImageCarousel
        :images="galleryImages"
        :fallback-alt="product?.title"
      />

      <div
        class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
      >
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-slate-900 dark:text-slate-100">
            Fotografije
          </h2>
          <span class="text-xs uppercase text-slate-400">
            {{ product.productImages?.length || 0 }} postojeće
          </span>
        </div>

        <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-300">
          <li
            v-for="image in product.productImages"
            :key="image.id"
            class="flex items-center justify-between gap-3 rounded-xl border border-slate-100 bg-slate-50 px-4 py-3 dark:border-slate-700 dark:bg-slate-900/40"
          >
            <div class="flex items-center gap-3">
              <span class="text-xs font-semibold text-slate-400"
                >#{{ image.id }}</span
              >
              <span class="max-w-[16rem] truncate font-medium">
                {{ image.imageUrl }}
              </span>
            </div>
            <button
              type="button"
              class="text-sm font-medium text-red-600 hover:underline dark:text-red-400"
              @click="deleteImage(image.id!)"
            >
              Ukloni
            </button>
          </li>
        </ul>

        <div
          class="space-y-3 rounded-xl border border-dashed border-slate-300 p-4 dark:border-slate-600"
        >
          <h3 class="text-sm font-semibold text-slate-900 dark:text-slate-100">
            Dodaj novu fotografiju
          </h3>
          <div class="grid gap-3 sm:grid-cols-[90px_1fr_1fr]">
            <input
              v-model.number="inputImage.imageId"
              type="number"
              min="0"
              placeholder="ID"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
            <input
              v-model="inputImage.imageUrl"
              type="url"
              placeholder="https://..."
              class="rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
            <input
              v-model="inputImage.imageAlt"
              type="text"
              placeholder="Alt tekst"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
          </div>
          <button
            type="button"
            class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            @click="addImage()"
          >
            Dodaj sliku
          </button>
        </div>
      </div>
    </div>

    <div class="space-y-6">
      <div
        class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
      >
        <h2 class="text-lg font-semibold text-slate-900 dark:text-slate-100">
          Osnovne informacije
        </h2>
        <div class="grid gap-4 md:grid-cols-2">
          <label class="grid gap-2 text-sm">
            <span class="font-medium text-slate-700 dark:text-slate-200"
              >Naziv proizvoda</span
            >
            <input
              v-model="product.title"
              type="text"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
          </label>
          <label class="grid gap-2 text-sm">
            <span class="font-medium text-slate-700 dark:text-slate-200"
              >Jedinstveni broj</span
            >
            <input
              v-model="product.productCode"
              pattern="[A-Z0-9]{5,10}"
              type="text"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
          </label>
          <label class="grid gap-2 text-sm">
            <span class="font-medium text-slate-700 dark:text-slate-200"
              >Cena (RSD)</span
            >
            <input
              v-model.number="product.price"
              type="number"
              min="0"
              step="0.01"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
          </label>
          <label
            class="flex items-center gap-3 self-end text-sm font-medium text-slate-700 dark:text-slate-200"
          >
            <input
              v-model="product.isAvailable"
              type="checkbox"
              class="h-4 w-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700"
            />
            Dostupan za kupovinu
          </label>
        </div>
        <label class="grid gap-2 text-sm">
          <span class="font-medium text-slate-700 dark:text-slate-200"
            >Opis proizvoda</span
          >
          <textarea
            v-model="product.description"
            rows="4"
            class="rounded-lg border border-slate-300 bg-slate-50 p-3 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
          ></textarea>
        </label>
        <div
          class="grid gap-3 text-xs uppercase tracking-wide text-slate-400 dark:text-slate-500 sm:grid-cols-2"
        >
          <div>
            Kreiran:
            <time class="font-medium text-slate-900 dark:text-slate-100">
              {{ formatDate(product.createdAt) }}
            </time>
          </div>
          <div>
            Poslednja izmena:
            <time class="font-medium text-slate-900 dark:text-slate-100">
              {{ formatDate(product.updatedAt) }}
            </time>
          </div>
        </div>
      </div>

      <div
        class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
      >
        <div class="flex flex-wrap items-center justify-between gap-3">
          <h2 class="text-lg font-semibold text-slate-900 dark:text-slate-100">
            Specifikacije
          </h2>
          <span class="text-xs uppercase text-slate-400">
            {{ product.productDetails?.length || 0 }} unosa
          </span>
        </div>

        <div class="space-y-4">
          <div
            v-for="detail in product.productDetails"
            :key="detail.id"
            class="rounded-xl border border-slate-100 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-900/40"
          >
            <div class="grid gap-3 md:grid-cols-[1fr_1fr_auto]">
              <input
                v-model="detail.title"
                type="text"
                placeholder="Naziv"
                class="rounded-lg border border-slate-300 bg-white p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-800 dark:text-white"
              />
              <textarea
                v-model="detail.value"
                rows="2"
                placeholder="Vrednost"
                class="rounded-lg border border-slate-300 bg-white p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-800 dark:text-white"
              ></textarea>
              <div class="flex flex-col gap-2 md:items-end">
                <button
                  type="button"
                  class="rounded-lg bg-blue-700 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
                  @click="updateSpec(detail)"
                >
                  Sačuvaj
                </button>
                <button
                  type="button"
                  class="rounded-lg bg-red-600 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-white hover:bg-red-700 focus:outline-none focus:ring-4 focus:ring-red-300 dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800"
                  @click="removeSpec(detail.id!)"
                >
                  Ukloni
                </button>
              </div>
            </div>
          </div>
        </div>

        <div
          class="space-y-3 rounded-xl border border-dashed border-slate-300 p-4 dark:border-slate-600"
        >
          <h3 class="text-sm font-semibold text-slate-900 dark:text-slate-100">
            Dodaj novu specifikaciju
          </h3>
          <div class="grid gap-3 md:grid-cols-2">
            <input
              v-model="inputSpecs.title"
              type="text"
              placeholder="Naziv"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
            <input
              v-model="inputSpecs.value"
              type="text"
              placeholder="Vrednost"
              class="rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
            />
          </div>
          <button
            type="button"
            class="w-full rounded-lg border border-slate-300 bg-white px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-100 focus:outline-none focus:ring-4 focus:ring-slate-200 dark:border-slate-600 dark:bg-slate-800 dark:text-slate-200 dark:hover:bg-slate-700 dark:focus:ring-slate-700"
            @click="addSpec()"
          >
            Dodaj specifikaciju
          </button>
        </div>
      </div>

      <footer
        class="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800 sm:flex-row sm:items-center sm:justify-between"
      >
        <div class="space-y-1 text-sm text-slate-500 dark:text-slate-400">
          <p>Sačuvajte izmene kako bi bile dostupne u katalogu.</p>
          <p>Brisanje je trajna akcija.</p>
        </div>
        <div class="flex flex-col gap-3 sm:flex-row">
          <button
            type="button"
            class="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-100 focus:outline-none focus:ring-4 focus:ring-slate-200 dark:border-slate-600 dark:bg-slate-800 dark:text-slate-200 dark:hover:bg-slate-700 dark:focus:ring-slate-700"
            @click="router.push('/products')"
          >
            Otkaži
          </button>
          <button
            type="button"
            class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            @click="catalogStore.updateProduct(product!)"
          >
            Sačuvaj izmene
          </button>
          <button
            type="button"
            class="rounded-lg bg-red-600 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-4 focus:ring-red-300 dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800"
            @click="deleteProduct()"
          >
            Obriši proizvod
          </button>
        </div>
      </footer>
    </div>
  </section>
</template>
