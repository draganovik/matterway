<script setup lang="ts">
import { useAuthSession } from '~/composables/useAuthSession'

const colorMode = useColorMode()
const color = computed(() =>
  colorMode.value === 'dark' ? '#0c0a09' : '#f5f5f4'
)
const auth = useAuthSession()
const route = useRoute()
const appTitle = 'Matterway Dashboard'
const isBooting = computed(
  () => !auth.isInitialized.value && !route.meta?.public
)
const pageTitle = computed(() => {
  const title = route.meta?.title
  return typeof title === 'string' && title.trim().length ? title : undefined
})

useHead({
  title: pageTitle,
  titleTemplate: (titleChunk) =>
    titleChunk ? `${titleChunk} - ${appTitle}` : appTitle,
  meta: [
    { charset: 'utf-8' },
    { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { key: 'theme-color', name: 'theme-color', content: color }
  ],
  link: [{ rel: 'icon', href: '/favicon.svg' }],
  htmlAttrs: {
    lang: 'en'
  }
})

onMounted(() => {
  if (!import.meta.client) return
  // Dashboard follows system theme automatically (no manual override).
  colorMode.preference = 'system'
  void auth.initialize()
})
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator />
    <div
      v-if="isBooting"
      class="min-h-screen bg-linear-to-br from-stone-100 via-stone-50 to-stone-200 dark:from-stone-950 dark:via-stone-900 dark:to-stone-950"
    >
      <div
        class="mx-auto flex min-h-screen max-w-3xl items-center justify-center px-6"
      >
        <div class="flex flex-col items-center gap-4 text-center">
          <div
            class="h-10 w-10 animate-spin rounded-full border-2 border-orange-500 border-t-transparent"
          />
        </div>
      </div>
    </div>
    <NuxtLayout v-else>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>
