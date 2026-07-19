<script setup lang="ts">
defineOptions({
  inheritAttrs: false,
})

const props = withDefaults(
  defineProps<{
    selected?: boolean
    disabled?: boolean
    type?: "button" | "submit" | "reset"
  }>(),
  {
    selected: false,
    disabled: false,
    type: "button",
  },
)

const emit = defineEmits<{
  (event: "click", payload: MouseEvent): void
}>()
</script>

<template>
  <button
    v-bind="$attrs"
    :type="props.type"
    :disabled="props.disabled"
    class="border-muted w-full cursor-pointer border-b px-3 py-2 text-left transition-colors last:border-b-0 disabled:cursor-not-allowed disabled:opacity-60"
    :class="
      props.selected ? 'bg-primary/10' : 'hover:bg-muted/60 bg-transparent'
    "
    @click="emit('click', $event)"
  >
    <slot />
  </button>
</template>
