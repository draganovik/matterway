<script setup lang="ts">
import type { FeatureDefinition } from "~/data/serviceRegistry"

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
      :ui="{
        container: 'p-0 sm:p-0 gap-y-0',
        wrapper: 'items-stretch',
        header: 'p-4 mb-0 border-b border-default',
      }"
    >
      <template #header>
        <div class="flex items-center justify-between">
          <p class="text-foreground text-sm font-medium">Available features</p>
          <span class="text-muted text-xs">{{ features.length }} total</span>
        </div>
      </template>

      <div class="divide-default divide-y">
        <NuxtLink
          v-for="feature in features"
          :key="feature.key"
          :to="feature.route"
          class="hover:bg-elevated/50 flex items-center justify-between px-4 py-3 text-sm transition"
        >
          <div class="flex items-center gap-2">
            <span class="bg-primary/70 h-2 w-2 rounded-full" />
            <span class="text-foreground font-medium">{{ feature.label }}</span>
          </div>
          <span class="text-muted text-xs">Open</span>
        </NuxtLink>
      </div>
    </UPageCard>
  </div>
</template>
