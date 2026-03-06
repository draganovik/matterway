<script setup lang="ts">
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
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>
