<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    src?: string | null
    alt?: string
    imgClass?: string
    placeholderClass?: string
  }>(),
  {
    src: null,
    alt: "",
    imgClass: "",
    placeholderClass: "",
  },
)

const loadFailed = ref(false)

const resolvedSrc = computed(() => {
  if (typeof props.src !== "string") return null
  const value = props.src.trim()
  return value.length ? value : null
})

const shouldRenderImage = computed(
  () => Boolean(resolvedSrc.value) && !loadFailed.value,
)

watch(
  () => props.src,
  () => {
    loadFailed.value = false
  },
)

function onImageError() {
  loadFailed.value = true
}
</script>

<template>
  <img
    v-if="shouldRenderImage"
    :src="resolvedSrc || undefined"
    :alt="props.alt"
    :class="props.imgClass"
    @error="onImageError"
  />
  <ImagePlaceholder v-else :class="props.placeholderClass" />
</template>
