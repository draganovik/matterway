<script setup lang="ts">
const colorMode = useColorMode()
const color = computed(() =>
  colorMode.value === "dark" ? "#020617" : "#f1f5f9",
)
const route = useRoute()
const appTitle = "Matterway Prodavnica"
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
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator color="var(--ui-primary)" />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>
