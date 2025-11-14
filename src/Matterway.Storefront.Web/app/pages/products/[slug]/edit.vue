<script lang="ts" setup>
import { useCatalogStore, type ProductDetailTypeOption } from "@stores/catalog";
import ProductModel, {
  type ProductDetails,
  type ProductImages,
} from "#models/ProductModel";

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

const inputSpecs = ref({
  typeSearch: "",
  value: "",
});

const selectedDetailType = ref<ProductDetailTypeOption | null>(null);
const detailTypeOptions = ref<ProductDetailTypeOption[]>([]);
const isDetailTypeDropdownOpen = ref(false);
const isSearchingDetailTypes = ref(false);

let detailTypeSearchHandle: ReturnType<typeof setTimeout> | null = null;

const canSubmitDetail = computed(() => {
  return (
    Boolean(selectedDetailType.value?.slug) &&
    inputSpecs.value.value.trim().length > 0
  );
});

const clearDetailTypeSelection = () => {
  selectedDetailType.value = null;
  detailTypeOptions.value = [];
  inputSpecs.value.typeSearch = "";
  isDetailTypeDropdownOpen.value = false;
};

const resetSpecForm = () => {
  inputSpecs.value.value = "";
  clearDetailTypeSelection();
};

const handleDetailTypeSelect = (option: ProductDetailTypeOption) => {
  selectedDetailType.value = option;
  inputSpecs.value.typeSearch = option.title ?? "";
  isDetailTypeDropdownOpen.value = false;
};

const handleDetailTypeFocus = () => {
  if (detailTypeOptions.value.length > 0) {
    isDetailTypeDropdownOpen.value = true;
  }
};

const handleDetailTypeBlur = () => {
  setTimeout(() => {
    isDetailTypeDropdownOpen.value = false;
  }, 120);
};

const fetchDetailTypeOptions = (search: string) => {
  if (detailTypeSearchHandle) {
    clearTimeout(detailTypeSearchHandle);
  }

  if (!search || search.trim().length < 2) {
    detailTypeOptions.value = [];
    isDetailTypeDropdownOpen.value = false;
    return;
  }

  isDetailTypeDropdownOpen.value = true;

  detailTypeSearchHandle = setTimeout(async () => {
    isSearchingDetailTypes.value = true;
    try {
      detailTypeOptions.value = await catalogStore.queryProductDetailTypes(
        search.trim(),
        8,
      );
      isDetailTypeDropdownOpen.value = true;
    } catch (error) {
      console.error("Failed to fetch detail types", error);
      detailTypeOptions.value = [];
      isDetailTypeDropdownOpen.value = false;
    } finally {
      isSearchingDetailTypes.value = false;
    }
  }, 250);
};

watch(
  () => inputSpecs.value.typeSearch,
  (newValue) => {
    if (selectedDetailType.value?.title === newValue) {
      return;
    }
    selectedDetailType.value = null;
    fetchDetailTypeOptions(newValue);
  },
);

const inputImage = ref({
  orderIndex: 0,
  imageAlt: "product image",
  file: null as File | null,
});

const imageFileInput = ref<HTMLInputElement | null>(null);
const imageOrderInputs = ref<Record<string, number>>({});

const syncImageOrderInputs = () => {
  if (!product.value?.productImages) {
    imageOrderInputs.value = {};
    return;
  }
  const entries: Record<string, number> = {};
  product.value.productImages.forEach((image) => {
    if (image.id) {
      entries[image.id] = image.orderIndex ?? 0;
    }
  });
  imageOrderInputs.value = entries;
};

const resetImageForm = () => {
  inputImage.value.orderIndex = product.value?.productImages?.length ?? 0;
  inputImage.value.imageAlt = "product image";
  inputImage.value.file = null;
  if (imageFileInput.value) {
    imageFileInput.value.value = "";
  }
};

const handleImageFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement | null;
  inputImage.value.file = target?.files?.[0] ?? null;
};

const addSpec = async () => {
  const detailValue = inputSpecs.value.value.trim();
  if (!product.value || !selectedDetailType.value?.slug || !detailValue) {
    return;
  }
  const response = await catalogStore.createProductSpec(
    product.value.id,
    selectedDetailType.value.slug,
    detailValue,
  );
  if (response.ok) {
    resetSpecForm();
    await loadProduct();
  }
};

const removeSpec = async (typeSlug?: string) => {
  if (!product.value || !typeSlug) {
    return;
  }
  const response = await catalogStore.deleteProductSpec(
    product.value.id,
    typeSlug,
  );
  if (response.ok) {
    await loadProduct();
  }
};

const updateSpec = async (detail: ProductDetails) => {
  const detailValue = detail.value?.trim();
  if (!product.value || !detail.typeSlug || !detailValue) {
    return;
  }
  const response = await catalogStore.updateProductSpec(
    product.value.id,
    detail.typeSlug,
    detailValue,
  );
  if (response.ok) {
    await loadProduct();
  }
};

const addImage = async () => {
  if (!product.value || !inputImage.value.file) {
    return;
  }
  const response = await catalogStore.createProductImage(
    product.value.id,
    inputImage.value.orderIndex,
    inputImage.value.file,
    inputImage.value.imageAlt,
  );
  if (response.ok) {
    await loadProduct();
  }
};

const deleteImage = async (orderIndex?: number) => {
  if (!product.value) {
    return;
  }
  if (typeof orderIndex !== "number") {
    return;
  }
  const response = await catalogStore.deleteProductImage(
    product.value.id,
    orderIndex,
  );
  if (response.ok) {
    await loadProduct();
  }
};

const updateImageOrder = async (image: ProductImages) => {
  if (
    !product.value ||
    !image.id ||
    typeof image.orderIndex !== "number" ||
    !(image.id in imageOrderInputs.value)
  ) {
    return;
  }

  const targetIndex = imageOrderInputs.value[image.id];

  if (targetIndex === image.orderIndex) {
    return;
  }

  const response = await catalogStore.updateProductImage(
    product.value.id,
    image.orderIndex,
    { orderIndex: targetIndex },
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
  const slug = route.params.slug;
  if (!slug) {
    // If no slug is available, ensure state is reset and avoid calling toString() on undefined.
    product.value = null;
    imageOrderInputs.value = {};
    resetSpecForm();
    return;
  }

  product.value = await catalogStore.fetchProductById(slug.toString());
  if (product.value) {
    syncImageOrderInputs();
    resetImageForm();
  } else {
    imageOrderInputs.value = {};
  }
  resetSpecForm();
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

  <section v-else class="grid gap-10 lg:grid-cols-[minmax(280px,36%)_1fr]">
    <div class="space-y-6">
      <ProductImageCarousel
        :images="galleryImages"
        :fallback-alt="product?.title"
      />

      <div
        class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
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
            :key="image.id ?? image.orderIndex"
            class="flex flex-col gap-3 rounded-xl border border-slate-100 bg-slate-50 px-4 py-3 dark:border-slate-700 dark:bg-slate-900/40 sm:items-center sm:justify-between"
          >
            <div class="min-w-0 w-full flex-1 space-y-1">
              <span class="text-xs font-semibold uppercase text-slate-400"
                >Redosled #{{ image.orderIndex }}</span
              >
              <p class="w-full overflow-x-auto font-medium">
                {{ image.imageUrl }}
              </p>
              <p v-if="image.imageAlt" class="text-xs text-slate-400">
                Alt: {{ image.imageAlt }}
              </p>
            </div>
            <div
              v-if="image.id"
              class="flex flex-col w-full justify-between items-stretch gap-2 sm:flex-row sm:items-end"
            >
              <label class="flex flex-col items-center gap-2 text-xs uppercase">
                Pozicija
                <input
                  v-model.number="imageOrderInputs[image.id]"
                  type="number"
                  min="0"
                  class="w-20 rounded-lg border border-slate-300 bg-white px-2 py-1 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-800 dark:text-white"
                />
              </label>
              <div class="flex gap-2">
                <button
                  type="button"
                  class="rounded-lg border border-blue-200 px-3 py-1.5 text-xs font-semibold text-blue-700 hover:bg-blue-50 focus:outline-hidden focus:ring-2 focus:ring-blue-200 dark:border-blue-800 dark:text-blue-200 dark:hover:bg-blue-900/30"
                  @click="updateImageOrder(image)"
                >
                  Sačuvaj
                </button>
                <button
                  type="button"
                  class="rounded-lg border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-600 hover:bg-red-50 focus:outline-hidden focus:ring-2 focus:ring-red-200 dark:border-red-700 dark:text-red-300 dark:hover:bg-red-900/30"
                  @click="deleteImage(image.orderIndex)"
                >
                  Ukloni
                </button>
              </div>
            </div>
          </li>
        </ul>

        <div
          class="space-y-3 rounded-xl border border-dashed border-slate-300 p-4 dark:border-slate-600"
        >
          <h3 class="text-sm font-semibold text-slate-900 dark:text-slate-100">
            Dodaj novu fotografiju
          </h3>
          <div class="grid gap-3 sm:grid-cols-[120px_1fr]">
            <label class="text-xs font-semibold uppercase text-slate-500">
              Redosled
              <input
                v-model.number="inputImage.orderIndex"
                type="number"
                min="0"
                class="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
              />
            </label>
            <label class="text-xs font-semibold uppercase text-slate-500">
              Alt tekst
              <input
                v-model="inputImage.imageAlt"
                type="text"
                placeholder="Alt tekst"
                class="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
              />
            </label>
          </div>
          <label class="text-xs font-semibold uppercase text-slate-500">
            Datoteka
            <input
              ref="imageFileInput"
              type="file"
              accept="image/*"
              class="mt-1 block w-full cursor-pointer rounded-lg border border-slate-300 bg-white text-sm file:mr-4 file:rounded-md file:border-0 file:bg-blue-600 file:px-4 file:py-2 file:text-sm file:font-semibold file:text-white hover:file:bg-blue-700 dark:border-slate-600 dark:bg-slate-800 dark:text-slate-200 dark:file:bg-blue-500"
              @change="handleImageFileChange"
            />
          </label>
          <p v-if="inputImage.file" class="text-xs text-slate-400">
            Selektovano: {{ inputImage.file?.name }}
          </p>
          <button
            type="button"
            class="w-full rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 disabled:cursor-not-allowed disabled:bg-blue-400 disabled:opacity-70 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            :disabled="!inputImage.file"
            @click="addImage()"
          >
            Dodaj sliku
          </button>
        </div>
      </div>
    </div>

    <div class="space-y-6">
      <div
        class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
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
              class="h-4 w-4 rounded-sm border-slate-300 text-blue-600 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700"
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
        class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
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
            :key="detail.typeSlug ?? detail.title"
            class="rounded-xl border border-slate-100 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-900/40"
          >
            <div class="grid gap-3 md:grid-cols-[1fr_auto]">
              <div class="space-y-2">
                <p
                  class="text-xs font-semibold uppercase text-slate-500 dark:text-slate-400"
                >
                  {{ detail.title ?? `Tip #${detail.typeSlug}` }}
                  <span
                    v-if="detail.unit"
                    class="ml-1 text-xs font-normal text-slate-400 dark:text-slate-500"
                  >
                    ({{ detail.unit }})
                  </span>
                </p>
                <textarea
                  v-model="detail.value"
                  rows="2"
                  placeholder="Vrednost"
                  class="w-full rounded-lg border border-slate-300 bg-white p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-800 dark:text-white"
                ></textarea>
              </div>
              <div class="flex flex-col gap-2 md:items-end">
                <button
                  type="button"
                  class="rounded-lg bg-blue-700 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-white hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
                  @click="updateSpec(detail)"
                >
                  Sačuvaj
                </button>
                <button
                  type="button"
                  class="rounded-lg bg-red-600 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-white hover:bg-red-700 focus:outline-hidden focus:ring-4 focus:ring-red-300 dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800"
                  @click="removeSpec(detail.typeSlug ?? undefined)"
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
            <div class="space-y-2 flex flex-col">
              <label
                class="text-xs font-semibold uppercase text-slate-500 dark:text-slate-400"
              >
                Tip specifikacije
              </label>
              <div class="relative">
                <input
                  v-model="inputSpecs.typeSearch"
                  type="text"
                  placeholder="Počnite da kucate naziv..."
                  class="w-full rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
                  @focus="handleDetailTypeFocus"
                  @blur="handleDetailTypeBlur"
                />
                <button
                  v-if="selectedDetailType"
                  type="button"
                  class="absolute inset-y-0 right-2 flex items-center text-slate-400 hover:text-slate-600 dark:text-slate-500 dark:hover:text-slate-300"
                  @mousedown.prevent
                  @click="clearDetailTypeSelection"
                >
                  &times;
                </button>
                <div
                  v-if="isDetailTypeDropdownOpen"
                  class="absolute z-20 mt-1 w-full rounded-xl border border-slate-200 bg-white shadow-xl dark:border-slate-700 dark:bg-slate-900"
                >
                  <p
                    v-if="isSearchingDetailTypes"
                    class="px-4 py-3 text-sm text-slate-500 dark:text-slate-400"
                  >
                    Pretraga u toku...
                  </p>
                  <template v-else>
                    <ul
                      v-if="detailTypeOptions.length > 0"
                      class="max-h-56 divide-y divide-slate-100 overflow-y-auto dark:divide-slate-700"
                    >
                      <li
                        v-for="option in detailTypeOptions"
                        :key="option.slug"
                        class="cursor-pointer px-4 py-2 hover:bg-slate-50 dark:hover:bg-slate-800/60"
                        @mousedown.prevent="handleDetailTypeSelect(option)"
                      >
                        <p
                          class="text-sm font-medium text-slate-900 dark:text-slate-100"
                        >
                          {{ option.title }}
                        </p>
                        <p
                          v-if="option.unit"
                          class="text-xs text-slate-500 dark:text-slate-400"
                        >
                          {{ option.unit }}
                        </p>
                      </li>
                    </ul>
                    <p
                      v-else
                      class="px-4 py-3 text-sm text-slate-500 dark:text-slate-400"
                    >
                      {{
                        inputSpecs.typeSearch.trim().length < 2
                          ? "Unesite najmanje 2 karaktera"
                          : "Nismo pronašli rezultate"
                      }}
                    </p>
                  </template>
                </div>
              </div>
              <p
                v-if="selectedDetailType?.unit"
                class="text-xs text-slate-500 dark:text-slate-400"
              >
                Merna jedinica: {{ selectedDetailType.unit }}
              </p>
            </div>
            <div class="space-y-2 flex flex-col">
              <label
                class="text-xs font-semibold uppercase text-slate-500 dark:text-slate-400"
              >
                Vrednost
              </label>
              <input
                v-model="inputSpecs.value"
                type="text"
                placeholder="npr. 120"
                class="rounded-lg border border-slate-300 bg-slate-50 p-2 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
              />
            </div>
          </div>
          <button
            type="button"
            class="w-full rounded-lg border border-slate-300 bg-white px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-100 focus:outline-hidden focus:ring-4 focus:ring-slate-200 disabled:cursor-not-allowed disabled:opacity-60 dark:border-slate-600 dark:bg-slate-800 dark:text-slate-200 dark:hover:bg-slate-700 dark:focus:ring-slate-700"
            :disabled="!canSubmitDetail"
            @click="addSpec()"
          >
            Dodaj specifikaciju
          </button>
        </div>
      </div>

      <footer
        class="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800 sm:flex-row sm:items-center sm:justify-between"
      >
        <div class="space-y-1 text-sm text-slate-500 dark:text-slate-400">
          <p>Sačuvajte izmene kako bi bile dostupne u katalogu.</p>
          <p>Brisanje je trajna akcija.</p>
        </div>
        <div class="flex flex-col gap-3 sm:flex-row">
          <button
            type="button"
            class="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-100 focus:outline-hidden focus:ring-4 focus:ring-slate-200 dark:border-slate-600 dark:bg-slate-800 dark:text-slate-200 dark:hover:bg-slate-700 dark:focus:ring-slate-700"
            @click="router.push('/products')"
          >
            Otkaži
          </button>
          <button
            type="button"
            class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            @click="catalogStore.updateProduct(product!)"
          >
            Sačuvaj izmene
          </button>
          <button
            type="button"
            class="rounded-lg bg-red-600 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-700 focus:outline-hidden focus:ring-4 focus:ring-red-300 dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800"
            @click="deleteProduct()"
          >
            Obriši proizvod
          </button>
        </div>
      </footer>
    </div>
  </section>
</template>
