<script lang="ts" setup>
import { useCatalogStore } from "~/store/catalog";
import { initCarousels } from "flowbite";
import ProductModel from "~/utils/ProductModel";
import { ProductDetails } from "~/utils/ProductModel";

const catalogStore = useCatalogStore();
const route = useRoute();
const router = useRouter();
let product: Ref<ProductModel> | Ref<null> = ref(null);

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
  if (product.value == null) {
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
    loadProduct();
  }
};

const removeSpec = async (detailId: string) => {
  if (product.value == null) {
    return;
  }
  const response = await catalogStore.deleteProductSpec(detailId);
  if (response.ok) {
    loadProduct();
  }
};

const updateSpec = async (detail: ProductDetails) => {
  if (product.value == null) {
    return;
  }
  const response = await catalogStore.updateProductSpec(
    detail.id,
    product.value.id,
    detail.title,
    detail.value,
  );
  if (response.ok) {
    loadProduct();
  }
};

const addImage = async () => {
  if (product.value == null) {
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
    loadProduct();
  }
};

const deleteImage = async (imageId: number) => {
  if (product.value == null) {
    return;
  }
  const response = await catalogStore.deleteProductImage(
    product.value.id,
    imageId,
  );
  if (response.ok) {
    loadProduct();
  }
};

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

const loadProduct = async () => {
  product.value = await catalogStore.fetchProductById(
    route.params.slug.toString(),
  );
  //initCarousels();
};

useHead({
  title: "Proizvod",
});

onMounted(async () => {
  loadProduct();
});
watch(
  () => product.value,
  () => {
    //wait for dom to update
    setTimeout(() => {
      initCarousels();
    }, 1000);
  },
);
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

  <section
    v-if="product != null"
    class="flex flex-col gap-8 md:grid md:grid-cols-5"
  >
    <div
      id="default-carousel"
      class="relative col-span-2 aspect-video w-full md:aspect-[4/3]"
      data-carousel="static"
    >
      <!-- Carousel wrapper -->
      <div class="relative h-full w-full overflow-hidden rounded-lg">
        <div
          data-carousel-item="true"
          :key="index"
          v-for="(image, index) in product?.productImages"
          class="hidden h-full w-full duration-700 ease-in-out"
        >
          <img
            :src="image.imageUrl"
            class="absolute left-1/2 top-1/2 block h-full w-full -translate-x-1/2 -translate-y-1/2 object-cover"
            :alt="image.imageAlt"
          />
        </div>
      </div>
      <!-- Slider indicators -->
      <div
        class="indicators absolute bottom-5 left-1/2 z-30 flex -translate-x-1/2 space-x-3"
      >
        <button
          v-for="(_, index) in product?.productImages"
          type="button"
          class="pill h-3 w-3 rounded-full"
          :aria-current="index == 0 ? 'true' : 'false'"
          :aria-label="'Slide' + index.toString()"
          :data-carousel-slide-to="index"
        ></button>
      </div>
      <!-- Slider controls -->
      <button
        type="button"
        class="group absolute left-0 top-0 z-30 flex h-full cursor-pointer items-center justify-center px-4 focus:outline-none"
        data-carousel-prev
      >
        <span
          class="inline-flex h-8 w-8 items-center justify-center rounded-full bg-white/30 group-hover:bg-white/50 group-focus:outline-none group-focus:ring-4 group-focus:ring-white dark:bg-slate-800/30 dark:group-hover:bg-slate-800/60 dark:group-focus:ring-slate-800/70 sm:h-10 sm:w-10"
        >
          <svg
            aria-hidden="true"
            class="h-5 w-5 text-white dark:text-slate-800 sm:h-6 sm:w-6"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M15 19l-7-7 7-7"
            ></path>
          </svg>
          <span class="sr-only">Previous</span>
        </span>
      </button>
      <button
        type="button"
        class="group absolute right-0 top-0 z-30 flex h-full cursor-pointer items-center justify-center px-4 focus:outline-none"
        data-carousel-next
      >
        <span
          class="inline-flex h-8 w-8 items-center justify-center rounded-full bg-white/30 group-hover:bg-white/50 group-focus:outline-none group-focus:ring-4 group-focus:ring-white dark:bg-slate-800/30 dark:group-hover:bg-slate-800/60 dark:group-focus:ring-slate-800/70 sm:h-10 sm:w-10"
        >
          <svg
            aria-hidden="true"
            class="h-5 w-5 text-white dark:text-slate-800 sm:h-6 sm:w-6"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9 5l7 7-7 7"
            ></path>
          </svg>
          <span class="sr-only">Next</span>
        </span>
      </button>
    </div>

    <div class="col-span-2 col-start-1 overflow-x-auto">
      <table
        class="w-full overflow-hidden rounded text-left text-sm text-gray-500 dark:text-gray-400"
      >
        <tbody>
          <tr
            v-for="image in product.productImages"
            class="bg-white dark:bg-gray-800"
          >
            <th>
              <div class="grid place-items-center">{{ image.id }}</div>
            </th>
            <th
              scope="row"
              class="whitespace-nowrap px-6 py-4 font-medium text-gray-900 dark:text-white"
            >
              {{
                image.imageUrl.length > 30
                  ? `...${image.imageUrl
                      .substring(image.imageUrl.lastIndexOf("/") + 1)
                      .slice(-30)}`
                  : image.imageUrl
              }}
            </th>
            <td class="px-6 py-4 text-right">
              <button
                @click="deleteImage(image.id)"
                type="button"
                class="font-medium text-red-600 hover:underline dark:text-red-500"
              >
                Ukloni
              </button>
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr
            class="bg-slate-50 text-xs font-semibold uppercase text-slate-700 dark:bg-slate-700 dark:text-slate-400"
          >
            <td class="w-min">
              <input
                placeholder="ID"
                type="number"
                v-model="inputImage.imageId"
                id="small-input"
                class="m-1 w-16 rounded-lg border border-gray-300 bg-gray-50 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-xs"
              />
            </td>
            <td class="w-full">
              <input
                placeholder="Add image url..."
                type="text"
                v-model="inputImage.imageUrl"
                id="small-input"
                class="m-1 block w-full rounded-lg border border-gray-300 bg-gray-50 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-xs"
              />
            </td>
            <td>
              <div class="flex w-max p-2">
                <button
                  type="button"
                  @click="addImage()"
                  class="rounded-lg border border-gray-200 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700"
                >
                  Dodaj sliku
                </button>
              </div>
            </td>
          </tr>
        </tfoot>
      </table>
    </div>

    <div
      class="col-span-3 col-start-3 row-span-3 row-start-1 flex flex-col gap-4"
    >
      <div>
        <label
          for="large-input"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Product title</label
        >
        <input
          v-model="product.title"
          type="text"
          id="large-input"
          class="sm:text-md block w-full rounded-lg border border-gray-300 bg-gray-50 p-4 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>

      <div class="items-center">
        <label
          for="default-input"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Jedinstveni broj:</label
        >
        <input
          pattern="[A-Z0-9]{5,10}"
          v-model="product.productCode"
          type="text"
          id="default-input"
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>

      <div class="mb-6 items-center">
        <label
          for="default-input"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Cena proizvoda:</label
        >
        <input
          v-model="product.price"
          type="number"
          id="default-input"
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </div>

      <div>
        <label
          for="message"
          class="mb-2 block text-sm font-medium text-gray-900 dark:text-white"
          >Description</label
        >
        <textarea
          v-model="product.description"
          id="message"
          rows="4"
          class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
          placeholder="Write your thoughts here..."
        ></textarea>
      </div>

      <div class="flex flex-col justify-between gap-3">
        <h2 class="text-lg font-medium text-slate-900 dark:text-slate-200">
          Detalji
        </h2>

        <div class="relative overflow-x-auto shadow-md sm:rounded-lg">
          <div class="relative overflow-x-auto shadow-md sm:rounded-lg">
            <table
              class="w-full text-left text-sm text-slate-500 dark:text-slate-400"
            >
              <thead
                class="bg-slate-50 text-xs uppercase text-slate-700 dark:bg-slate-700 dark:text-slate-400"
              >
                <tr>
                  <th scope="col" class="px-6 py-3">Naziv</th>
                  <th scope="col" class="px-6 py-3">Vrednost</th>
                  <th scope="col" class="px-6 py-3">Operacije</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="detail in product?.productDetails"
                  class="border-b bg-white hover:bg-slate-50 dark:border-slate-700 dark:bg-slate-800 dark:hover:bg-slate-600"
                >
                  <th
                    scope="row"
                    class="whitespace-nowrap px-4 py-4 font-medium text-slate-900 dark:text-white"
                  >
                    <input
                      v-model="detail.title"
                      type="text"
                      id="small-input"
                      class="w-full rounded-lg border border-gray-300 bg-gray-50 p-2 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-xs"
                    />
                  </th>
                  <td class="px-6 py-4">
                    <textarea
                      v-model="detail.value"
                      id="message"
                      rows="4"
                      class="block w-full resize-none rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
                      placeholder="Vrednost opisa proizvoda"
                    ></textarea>
                  </td>
                  <td class="px-6 py-4">
                    <div class="flex gap-4">
                      <button
                        @click="updateSpec(detail)"
                        type="button"
                        class="w-full font-medium text-green-600 hover:underline dark:text-green-500"
                      >
                        Sačuvaj
                      </button>
                      <button
                        @click="removeSpec(detail.id)"
                        type="button"
                        class="w-full font-medium text-red-600 hover:underline dark:text-red-500"
                      >
                        Ukloni
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
              <tfoot>
                <tr
                  class="bg-slate-50 text-xs font-semibold uppercase text-slate-700 dark:bg-slate-700 dark:text-slate-400"
                >
                  <th
                    scope="row"
                    class="whitespace-nowrap px-4 py-4 font-medium text-slate-900 dark:text-white"
                  >
                    <input
                      type="text"
                      id="small-input"
                      class="w-full rounded-lg border border-gray-300 bg-gray-50 p-2 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-xs"
                      placeholder="Naziv opisa proizvoda"
                      v-model="inputSpecs.title"
                    />
                  </th>
                  <td class="px-6 py-4">
                    <textarea
                      id="message"
                      rows="4"
                      class="block w-full resize-none rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
                      placeholder="Vrednost opisa proizvoda"
                      v-model="inputSpecs.value"
                    ></textarea>
                  </td>
                  <td class="px-6 py-4">
                    <button
                      type="button"
                      @click="addSpec()"
                      class="w-full rounded-lg border border-gray-200 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700"
                    >
                      Dodaj opis
                    </button>
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>
        </div>
      </div>
    </div>
  </section>

  <aside
    v-if="product != null"
    class="sticky bottom-4 mt-8 h-min w-full rounded-lg border border-slate-200/90 bg-white p-4 backdrop-blur-md backdrop-filter dark:border-slate-700 dark:bg-slate-800/90"
  >
    <div class="flex justify-between gap-4">
      <button
        @click="deleteProduct()"
        type="button"
        class="rounded-lg bg-red-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-800 focus:outline-none focus:ring-4 focus:ring-red-300 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-900"
      >
        Izbriši proizvod
      </button>
      <div class="flex gap-4">
        <button
          @click="router.push('/products')"
          type="button"
          class="rounded-lg border border-gray-200 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700"
        >
          Otkaži izmene
        </button>
        <button
          @click="catalogStore.updateProduct(product)"
          type="button"
          class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
        >
          Sačuvaj izmene
        </button>
      </div>
    </div>
  </aside>
</template>

<style scoped></style>
