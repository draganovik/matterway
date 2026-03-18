<script setup lang="ts">
withDefaults(
  defineProps<{
    title?: string
    description?: string
  }>(),
  {
    title: "Ništa nije izabrano",
    description: "Izaberite stavku sa liste da biste započeli izmenu.",
  },
)
</script>

<template>
  <div
    class="border-default bg-background rounded-lg border px-4 py-6 text-center"
  >
    <p class="text-foreground text-sm font-medium">
      {{ title }}
    </p>
    <p class="text-muted mt-1 text-sm">
      {{ description }}
    </p>
  </div>
</template>
