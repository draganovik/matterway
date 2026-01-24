<script setup lang="ts">
import type { FeatureDefinition } from '~/data/adminFeatures'

defineProps<{
  title: string
  description: string
  features: FeatureDefinition[]
}>()
</script>

<template>
  <div>
    <UPageCard
      :title="title"
      :description="description"
      variant="naked"
      orientation="horizontal"
      class="mb-4"
    >
      <UButton
        v-if="features[0]"
        :to="features[0].route"
        label="Open first feature"
        color="neutral"
        class="w-fit lg:ms-auto"
      />
    </UPageCard>

    <UPageCard
      variant="subtle"
      :ui="{ container: 'p-0 sm:p-0 gap-y-0', wrapper: 'items-stretch', header: 'p-4 mb-0 border-b border-default' }"
    >
      <template #header>
        <div class="flex items-center justify-between">
          <p class="text-sm font-medium text-foreground">
            Available features
          </p>
          <span class="text-xs text-muted">{{ features.length }} total</span>
        </div>
      </template>

      <div class="divide-y divide-default">
        <NuxtLink
          v-for="feature in features"
          :key="feature.key"
          :to="feature.route"
          class="flex items-center justify-between px-4 py-3 text-sm transition hover:bg-elevated/50"
        >
          <div class="flex items-center gap-2">
            <span class="h-2 w-2 rounded-full bg-primary/70" />
            <span class="font-medium text-foreground">{{ feature.label }}</span>
          </div>
          <span class="text-xs text-muted">Open</span>
        </NuxtLink>
      </div>
    </UPageCard>
  </div>
</template>
