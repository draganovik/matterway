<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

const route = useRoute()
const auth = useAuthSession()

const sections = computed(() =>
  adminServices
    .filter(section => auth.hasPermission(section.service, section.minimum))
    .map(section => ({
      ...section,
      features: section.features.filter(feature =>
        auth.hasPermission(feature.service, feature.minimum)
      )
    }))
    .filter(section => section.features.length > 0)
)

const isActive = (path: string) => route.path === path
</script>

<template>
  <nav class="space-y-6">
    <div
      v-for="section in sections"
      :key="section.key"
      class="space-y-2"
    >
      <p class="text-xs font-semibold uppercase tracking-[0.2em] text-muted">
        {{ section.label }}
      </p>
      <div class="space-y-1">
        <NuxtLink
          v-for="feature in section.features"
          :key="feature.key"
          :to="feature.route"
          class="flex items-center gap-2 rounded-lg px-3 py-2 text-sm transition"
          :class="
            isActive(feature.route)
              ? 'bg-primary/10 text-primary'
              : 'text-muted hover:bg-muted/40 hover:text-foreground'
          "
        >
          <span class="h-2 w-2 rounded-full bg-current opacity-70" />
          {{ feature.label }}
        </NuxtLink>
      </div>
    </div>
  </nav>
</template>
