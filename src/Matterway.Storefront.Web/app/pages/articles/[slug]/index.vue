<script setup lang="ts">
import { useArticlesInstancePage } from "~/composables/features/articles/useArticlesInstancePage"

definePageMeta({
  public: true,
})

const { article, error, loading, gallery, initialize } =
  useArticlesInstancePage()

await initialize()
</script>

<template>
  <div class="space-y-6">
    <StatusMessages v-if="error" :error="error" />

    <div v-if="loading" class="flex flex-col gap-6 lg:flex-row">
      <div class="flex flex-col gap-6 lg:w-2/5 lg:flex-none">
        <USkeleton class="aspect-4/3" />
        <USkeleton class="h-72" />
      </div>
      <div class="flex flex-col gap-6 lg:w-3/5 lg:flex-none">
        <USkeleton class="h-56" />
        <USkeleton class="h-72" />
      </div>
    </div>

    <template v-else-if="article">
      <div class="flex flex-col gap-6 lg:flex-row">
        <div class="flex flex-col gap-6 lg:w-2/5 lg:flex-none">
          <ArticlesInstanceGalleryPanel :images="gallery" />
          <ArticlesInstanceDescriptionPanel
            :description="article.description"
          />
        </div>

        <div class="flex flex-col gap-6 lg:w-3/5 lg:flex-none">
          <ArticlesInstanceOverviewPanel :article="article" />
          <ArticlesInstanceDetailsPanel :details="article.articleDetails" />
        </div>
      </div>
    </template>
  </div>
</template>
