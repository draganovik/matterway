<script lang="ts" setup>
import { useCatalogStore } from "@stores/catalog";
import ArticleModel from "#models/ArticleModel";

const catalogStore = useCatalogStore();
const router = useRouter();
const article = ref(new ArticleModel());
const isSubmitting = ref(false);

const createArticle = async () => {
  if (isSubmitting.value) {
    return;
  }
  isSubmitting.value = true;
  try {
    const response = await catalogStore.createArticle(article.value);
    if (response?.ok) {
      const data = await response.json();
      router.push(`${data.id}/edit`);
    }
  } finally {
    isSubmitting.value = false;
  }
};

useHead({
  title: "Novi proizvod",
});

definePageMeta({
  middleware: ["auth"],
  authOnlyPerms: ["catalog:operator", "catalog:administrator"],
});
</script>

<template>
  <section class="mx-auto flex max-w-4xl flex-col gap-6">
    <ArticleCreateForm v-model="article" />
  </section>

  <aside
    class="sticky bottom-4 mt-8 h-min w-full rounded-lg border border-slate-200/90 bg-white p-4 backdrop-blur-md backdrop-filter dark:border-slate-700 dark:bg-slate-800/90"
  >
    <div class="flex justify-end gap-4">
      <button
        type="button"
        class="rounded-lg border border-gray-200 bg-white px-5 py-2.5 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:outline-hidden focus:ring-4 focus:ring-gray-200 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700"
        @click="router.push('/articles')"
      >
        Otkaži
      </button>
      <button
        @click="createArticle"
        type="button"
        :disabled="isSubmitting"
        class="rounded-lg bg-blue-700 px-5 py-2.5 text-sm font-medium text-white transition hover:bg-blue-800 focus:outline-hidden focus:ring-4 focus:ring-blue-300 disabled:cursor-not-allowed disabled:opacity-70 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
      >
        {{ isSubmitting ? "Čuvanje..." : "Sačuvaj proizvod" }}
      </button>
    </div>
  </aside>
</template>
