<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    heroEyebrow?: string
    heroTitle: string
    heroDescription: string
    cardEyebrow: string
    cardTitle: string
    cardDescription: string
  }>(),
  {
    heroEyebrow: "Matterway",
  },
)
</script>

<template>
  <div
    class="mx-auto grid max-w-6xl items-center gap-8 lg:grid-cols-[1.1fr_0.9fr]"
  >
    <div class="hidden space-y-4 lg:block">
      <p class="text-primary text-xs tracking-[0.4em] uppercase">
        {{ props.heroEyebrow }}
      </p>
      <h1 class="text-4xl font-semibold">{{ props.heroTitle }}</h1>
      <p class="text-muted max-w-md text-sm">{{ props.heroDescription }}</p>
    </div>

    <UCard class="border-default border !shadow-sm">
      <template #header>
        <div class="space-y-1">
          <p class="text-primary text-xs tracking-[0.3em] uppercase">
            {{ props.cardEyebrow }}
          </p>
          <h2 class="text-2xl font-semibold">{{ props.cardTitle }}</h2>
          <p class="text-muted text-sm">{{ props.cardDescription }}</p>
        </div>
      </template>

      <slot />
    </UCard>
  </div>
</template>
