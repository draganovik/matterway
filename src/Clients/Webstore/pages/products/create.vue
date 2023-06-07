<script lang="ts" setup>
import { useCatalogStore } from "~/store/catalog";
import ProductModel from "~/utils/ProductModel";
import { useSessionStore } from "~/store/session";

const catalogStore = useCatalogStore();
const sessionStore = useSessionStore();
const router = useRouter();
let product: Ref<ProductModel> = ref(new ProductModel());

const createProduct = async () => {
  const response = await catalogStore.createProduct(product.value);
  if (response) {
    const data = await response.json();
    await new Promise((resolve) => setTimeout(resolve, 1000));
    router.push(`${data.id}/edit`);
  }
};

useHead({
  title: "Proizvod",
});

definePageMeta({
  middleware: ["auth"],
  authOnlyRoles: ["Admin", "Manager"],
});
</script>

<template>
  <section class="flex flex-col gap-8 md:grid md:grid-cols-5">
    <div class="col-span-2 col-start-1 overflow-x-auto">
      <table
        class="w-full overflow-hidden rounded text-left text-sm text-gray-500 dark:text-gray-400"
      >
        <tbody>
          <tr
            v-for="image in product.productImages"
            class="bg-white dark:bg-gray-800"
          >
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
            <td colspan="3">
              <div
                class="grid grid-flow-col-dense place-items-center gap-2 p-2"
              >
                <input
                  placeholder="Add image url..."
                  type="text"
                  id="small-input"
                  class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-xs"
                />
                <button
                  type="button"
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
                  <td
                    class="px-6 py-4"
                    v-if="sessionStore.getTokenData?.role === 'Admin'"
                  >
                    <button
                      type="button"
                      class="w-full font-medium text-red-600 hover:underline dark:text-red-500"
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
                  <th
                    scope="row"
                    class="whitespace-nowrap px-4 py-4 font-medium text-slate-900 dark:text-white"
                  >
                    <input
                      type="text"
                      id="small-input"
                      class="w-full rounded-lg border border-gray-300 bg-gray-50 p-2 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-xs"
                      placeholder="Naziv opisa proizvoda"
                    />
                  </th>
                  <td class="px-6 py-4">
                    <textarea
                      id="message"
                      rows="4"
                      class="block w-full resize-none rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
                      placeholder="Vrednost opisa proizvoda"
                    ></textarea>
                  </td>
                  <td class="px-6 py-4">
                    <button
                      type="button"
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
      <div class="flex gap-4">
        <button
          type="button"
          class="rounded-lg border border-gray-200 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700"
        >
          Otkaži izmene
        </button>
        <button
          @click="createProduct()"
          type="button"
          class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
        >
          Sačuvaj proizvod
        </button>
      </div>
    </div>
  </aside>
</template>

<style scoped></style>
