<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"

const colorMode = useColorMode()
const color = computed(() =>
  colorMode.value === "dark" ? "#020617" : "#f1f5f9",
)
const auth = useAuthSession()
const route = useRoute()
const appTitle = "Matterway prodavnica"
const isBooting = computed(
  () => !auth.isInitialized.value && !route.meta?.public,
)
const pageTitle = computed(() => {
  const title = route.meta?.title
  return typeof title === "string" && title.trim().length ? title : undefined
})

useHead({
  title: pageTitle,
  titleTemplate: (titleChunk) =>
    titleChunk ? `${titleChunk} - ${appTitle}` : appTitle,
  meta: [
    { charset: "utf-8" },
    { name: "viewport", content: "width=device-width, initial-scale=1" },
    { key: "theme-color", name: "theme-color", content: color },
  ],
  link: [{ rel: "icon", href: "/favicon.svg" }],
  htmlAttrs: {
    lang: "sr",
  },
})

onMounted(() => {
  if (!import.meta.client) return
  void auth.initialize()
})
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator color="var(--ui-primary)" />
    <div
      v-if="isBooting"
      class="min-h-screen bg-linear-to-br from-slate-200 via-slate-100 to-slate-50 dark:from-slate-950 dark:via-slate-950 dark:to-slate-950"
    >
      <div
        class="mx-auto flex min-h-screen max-w-3xl items-center justify-center px-6"
      >
        <div class="flex flex-col items-center gap-4 text-center">
          <div
            class="h-10 w-10 animate-spin rounded-full border border-slate-500 border-t-transparent dark:border-slate-400"
          />
        </div>
      </div>
    </div>
    <NuxtLayout v-else>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>
