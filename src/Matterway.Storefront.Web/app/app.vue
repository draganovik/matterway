<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"
import { useCart } from "~/composables/useCart"

const colorMode = useColorMode()
const color = computed(() =>
  colorMode.value === "dark" ? "#020617" : "#f1f5f9",
)
const auth = useAuthSession()
const cart = useCart()
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
  void auth.initialize()
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
