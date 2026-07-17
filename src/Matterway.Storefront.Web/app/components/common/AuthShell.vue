<script setup lang="ts">
const props = defineProps<{
  heroTitle: string
  heroDescription: string
  cardTitle: string
  cardDescription: string
}>()
</script>

<template>
  <div
    class="mx-auto grid max-w-6xl items-center gap-6 lg:grid-cols-[1.1fr_0.9fr]"
  >
    <div class="hidden space-y-3 lg:block">
      <h1 class="text-4xl font-semibold">{{ props.heroTitle }}</h1>
      <p class="text-muted max-w-md text-sm">{{ props.heroDescription }}</p>
    </div>

    <UCard>
      <template #header>
        <div class="space-y-1">
          <h2 class="text-2xl font-semibold">{{ props.cardTitle }}</h2>
          <p class="text-muted text-sm">{{ props.cardDescription }}</p>
        </div>
      </template>

      <slot />
    </UCard>
  </div>
</template>
