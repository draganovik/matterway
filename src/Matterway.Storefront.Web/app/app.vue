<script setup lang="ts">
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"

const colorMode = useColorMode()
const color = computed(() =>
  colorMode.value === "dark" ? "#020617" : "#f1f5f9",
)
const auth = useAuthSessionStore()
const cart = useCartStore()
const route = useRoute()
const appTitle = "Matterway prodavnica"
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

let cartRefreshInFlight: Promise<void> | null = null
let lastCartRefreshAt = 0
const cartRefreshThrottleMs = 5000

async function refreshCartFromRemote() {
  if (!import.meta.client) return
  const now = Date.now()
  if (now - lastCartRefreshAt < cartRefreshThrottleMs) return
  if (cartRefreshInFlight) return cartRefreshInFlight

  cartRefreshInFlight = (async () => {
    lastCartRefreshAt = Date.now()
    await auth.initialize()
    await cart.refreshFromRemote()
  })()

  try {
    await cartRefreshInFlight
  } finally {
    cartRefreshInFlight = null
  }
}

function handleWindowFocus() {
  void refreshCartFromRemote()
}

function handleVisibilityChange() {
  if (document.visibilityState !== "visible") return
  void refreshCartFromRemote()
}

onMounted(() => {
  if (!import.meta.client) return
  window.addEventListener("focus", handleWindowFocus)
  document.addEventListener("visibilitychange", handleVisibilityChange)
  void refreshCartFromRemote()
})

onBeforeUnmount(() => {
  if (!import.meta.client) return
  window.removeEventListener("focus", handleWindowFocus)
  document.removeEventListener("visibilitychange", handleVisibilityChange)
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
