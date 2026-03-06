<script setup lang="ts">
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"

const auth = useAuthSessionStore()
const appTitle = "Matterway Dashboard"
const route = useRoute()
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
    {
      key: "theme-color-light",
      name: "theme-color",
      content: "#f5f5f4",
      media: "(prefers-color-scheme: light)",
    },
    {
      key: "theme-color-dark",
      name: "theme-color",
      content: "#0c0a09",
      media: "(prefers-color-scheme: dark)",
    },
  ],
  link: [{ rel: "icon", href: "/favicon.svg" }],
  htmlAttrs: {
    lang: "en",
  },
})

onMounted(() => {
  if (!import.meta.client) return
  void auth.initialize()
})
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator />
    <NuxtLayout>
      <NuxtPage v-slot="{ Component, route: pageRoute }">
        <Transition name="app-content-fade" mode="out-in" appear>
          <component :is="Component" :key="pageRoute.fullPath" />
        </Transition>
      </NuxtPage>
    </NuxtLayout>
  </UApp>
</template>

<style>
.app-content-fade-enter-active,
.app-content-fade-leave-active {
  transition: opacity 0.18s ease;
}

.app-content-fade-enter-from,
.app-content-fade-leave-to {
  opacity: 0;
}
</style>
