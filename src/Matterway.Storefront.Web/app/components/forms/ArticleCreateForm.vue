<script lang="ts" setup>
import type ArticleModel from "#models/ArticleModel";

const article = defineModel<ArticleModel>({ required: true });
</script>

<template>
  <div
    class="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
  >
    <div class="grid gap-4 md:grid-cols-2">
      <label class="grid gap-2 text-sm">
        <span class="font-medium text-slate-700 dark:text-slate-200"
          >Naziv proizvoda</span
        >
        <input
          v-model="article.title"
          type="text"
          class="rounded-lg border border-slate-300 bg-slate-50 p-2.5 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
        />
      </label>
      <label class="grid gap-2 text-sm">
        <span class="font-medium text-slate-700 dark:text-slate-200"
          >Jedinstveni broj</span
        >
        <input
          v-model="article.articleCode"
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
          v-model.number="article.price"
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
          v-model="article.isAvailable"
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
        v-model="article.description"
        rows="6"
        class="rounded-lg border border-slate-300 bg-slate-50 p-3 text-sm text-slate-900 focus:border-blue-500 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-700 dark:text-white"
      ></textarea>
    </label>

    <p class="text-sm text-slate-500 dark:text-slate-400">
      Fotografije i dodatni detalji možete dodati nakon kreiranja proizvoda.
    </p>
  </div>
</template>
