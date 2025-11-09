<script lang="ts" setup>
import { useCatalogStore } from "~/stores/catalog";
import ProductModel from "~/models/ProductModel";

const catalogStore = useCatalogStore();
const router = useRouter();
const product = ref(new ProductModel());

const createProduct = async () => {
  const response = await catalogStore.createProduct(product.value);
  if (response) {
    const data = await response.json();
    await new Promise((resolve) => setTimeout(resolve, 1000));
    router.push(`${data.id}/edit`);
  }
};

useHead({
  title: "Novi proizvod",
});

definePageMeta({
  middleware: ["auth"],
  authOnlyRoles: ["Admin", "Manager"],
});
</script>

<template>
  <section class="mx-auto flex max-w-4xl flex-col gap-6">
    <div
      class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800"
    >
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
          rows="6"
          class="rounded-lg border border-slate-300 bg-slate-50 p-3 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
        ></textarea>
      </label>

      <p class="text-sm text-slate-500 dark:text-slate-400">
        Fotografije i dodatni detalji možete dodati nakon kreiranja proizvoda.
      </p>
    </div>
  </section>

  <aside
    class="sticky bottom-4 mt-8 h-min w-full rounded-lg border border-slate-200/90 bg-white p-4 backdrop-blur-md backdrop-filter dark:border-slate-700 dark:bg-slate-800/90"
  >
    <div class="flex justify-end gap-4">
      <button
        type="button"
        class="rounded-lg border border-gray-200 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700"
        @click="router.push('/products')"
      >
        Otkaži
      </button>
      <button
        @click="createProduct()"
        type="button"
        class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
      >
        Sačuvaj proizvod
      </button>
    </div>
  </aside>
</template>
