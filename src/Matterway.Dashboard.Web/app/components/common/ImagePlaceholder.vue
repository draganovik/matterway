<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    icon?: string
    iconClass?: string
  }>(),
  {
    icon: "i-lucide-image",
    iconClass: "h-[30%] w-[30%]",
  },
)
</script>

<template>
  <div
    class="bg-muted/60 text-muted/50 flex h-full w-full items-center justify-center"
  >
    <UIcon :name="props.icon" :class="props.iconClass" />
  </div>
</template>
