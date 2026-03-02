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
    class="w-full rounded-xl border px-4 py-3 text-left transition"
    :class="
      props.selected
        ? 'border-primary/40 bg-primary/5'
        : 'bg-background hover:border-default hover:bg-muted/40 border-transparent'
    "
    @click="emit('click', $event)"
  >
    <slot />
  </button>
</template>
