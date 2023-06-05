<script lang="ts" setup>
import { useCatalogStore } from "~/store/catalog";
import { initCarousels } from "flowbite";
import ProductModel from "~/utils/ProductModel";
import { useSessionStore } from "~/store/session";
import { useCartStore } from "~/store/cart";

const catalogStore = useCatalogStore();
const sessionStore = useSessionStore();
const userCartStore = useCartStore();
const route = useRoute();
const router = useRouter();
let product: Ref<ProductModel> | Ref<null> = ref(null);

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
  product.value = await catalogStore.fetchProductById(
    route.params.slug.toString(),
  );
  console.log(product.value);
});
watch(product, () => {
  initCarousels();
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

  <section
    v-if="product != null"
    class="flex flex-col gap-8 md:grid md:grid-cols-5"
  >
    <div
      id="indicators-carousel"
      class="relative col-span-2 aspect-video w-full md:aspect-[4/3]"
      data-carousel="static"
    >
      <!-- Carousel wrapper -->
      <div class="relative h-full w-full overflow-hidden rounded-lg">
        <div
          v-for="image in product?.productImages"
          class="duration-700 ease-in-out"
          data-carousel-item="active"
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
        class="absolute bottom-5 left-1/2 z-30 flex -translate-x-1/2 space-x-3"
      >
        <button
          v-for="(_, index) in product?.productImages"
          type="button"
          class="h-3 w-3 rounded-full"
          :aria-current="index == 0 ? true : false"
          :aria-label="'Slide' + index"
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

    <div class="col-span-3 flex flex-col gap-4">
      <div class="grid gap-2">
        <h1
          class="max-w-2xl text-2xl font-extrabold tracking-tight text-slate-900 dark:text-slate-200 sm:text-3xl md:text-4xl lg:text-5xl"
        >
          {{ product?.title }}
        </h1>
        <div class="flex gap-3 text-slate-500">
          <span
            :class="product.isAvailable ? 'text-green-500' : 'text-red-500'"
            >{{
              product.isAvailable
                ? "Proizvod je na stanju"
                : "Proivod nije dostupan"
            }}</span
          >
          <span>Jedinstveni broj: {{ product.productCode }}</span>
        </div>
      </div>
      <div>
        <h2 class="sr-only">Product price</h2>
        <p class="text-3xl text-slate-900 dark:text-slate-200">
          {{ formatMoney(product?.price || 0) }}
        </p>
      </div>
      <div
        class="flex gap-4"
        v-if="
          sessionStore.getTokenData?.role != 'Admin' &&
          sessionStore.getTokenData?.role != 'Manager'
        "
      >
        <button
          @click="userCartStore.addToCart(product)"
          type="button"
          class="inline-flex items-center rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
        >
          <svg
            aria-hidden="true"
            class="-ml-1 mr-2 h-5 w-5"
            fill="currentColor"
            viewBox="0 0 20 20"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              clip-rule="evenodd"
              fill-rule="evenodd"
              d="M6 5v1H4.667a1.75 1.75 0 00-1.743 1.598l-.826 9.5A1.75 1.75 0 003.84 19H16.16a1.75 1.75 0 001.743-1.902l-.826-9.5A1.75 1.75 0 0015.333 6H14V5a4 4 0 00-8 0zm4-2.5A2.5 2.5 0 007.5 5v1h5V5A2.5 2.5 0 0010 2.5zM7.5 10a2.5 2.5 0 005 0V8.75a.75.75 0 011.5 0V10a4 4 0 01-8 0V8.75a.75.75 0 011.5 0V10z"
            ></path>
          </svg>
          Dodaj u korpu
        </button>
        <p
          v-if="userCartStore.isProductInCart(product.id)"
          class="grid min-w-[3rem] place-items-center rounded bg-slate-500/20 px-3 text-xl font-semibold"
        >
          {{ userCartStore.countProductsInCart(product.id) }}
        </p>
        <button
          type="button"
          v-if="userCartStore.isProductInCart(product.id)"
          @click="userCartStore.removeFromCart(product)"
          class="rounded-lg bg-red-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-800 focus:outline-none focus:ring-4 focus:ring-red-300 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-900"
        >
          Ukloni iz korpe
        </button>
      </div>
      <div
        class="flex gap-4"
        v-if="
          sessionStore.getTokenData?.role == 'Admin' ||
          sessionStore.getTokenData?.role == 'Manager'
        "
      >
        <NuxtLink
          :to="route.path + '/edit'"
          class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
          >Izmeni proizvod</NuxtLink
        >
        <button
          type="button"
          @click="deleteProduct()"
          class="rounded-lg bg-red-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-800 focus:outline-none focus:ring-4 focus:ring-red-300 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-900"
        >
          Obriši proivod
        </button>
      </div>

      <div>
        <h2 class="sr-only">Description</h2>

        <div class="prose max-w-none text-slate-500">
          <p>{{ product?.description }}</p>
        </div>
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
                  <th
                    scope="col"
                    class="px-6 py-3"
                    v-if="sessionStore.getTokenData?.role === 'Admin'"
                  >
                    Action
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="detail in product?.productDetails"
                  class="border-b bg-white hover:bg-slate-50 dark:border-slate-700 dark:bg-slate-800 dark:hover:bg-slate-600"
                >
                  <th
                    scope="row"
                    class="whitespace-nowrap px-6 py-4 font-medium text-slate-900 dark:text-white"
                  >
                    {{ detail?.title }}
                  </th>
                  <td class="px-6 py-4">
                    {{ detail?.value + (detail?.unit ? detail?.unit : "") }}
                  </td>
                  <td
                    class="px-6 py-4"
                    v-if="sessionStore.getTokenData?.role === 'Admin'"
                  >
                    <a
                      href="#"
                      class="font-medium text-blue-600 hover:underline dark:text-blue-500"
                      >Edit</a
                    >
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped></style>
