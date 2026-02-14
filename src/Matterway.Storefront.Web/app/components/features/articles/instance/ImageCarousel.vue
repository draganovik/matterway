<script setup lang="ts">
interface CarouselImage {
  id: string
  url: string
  alt: string
}

const props = defineProps<{
  images: CarouselImage[]
}>()

const activeIndex = ref(0)

const hasImages = computed(() => props.images.length > 0)
const hasMultipleImages = computed(() => props.images.length > 1)

const currentImage = computed(() => {
  if (!hasImages.value) return null
  const index = Math.min(activeIndex.value, props.images.length - 1)
  return props.images[index]
})

watch(
  () => props.images.length,
  () => {
    if (activeIndex.value >= props.images.length) {
      activeIndex.value = 0
    }
  },
)

function selectImage(index: number) {
  activeIndex.value = index
}

function showNext() {
  if (!hasMultipleImages.value) return
  activeIndex.value = (activeIndex.value + 1) % props.images.length
}

function showPrevious() {
  if (!hasMultipleImages.value) return
  activeIndex.value =
    (activeIndex.value - 1 + props.images.length) % props.images.length
}
</script>

<template>
  <UCard class="border-default bg-default border">
    <div class="space-y-3">
      <div class="bg-elevated relative overflow-hidden rounded-xl">
        <div class="relative aspect-[4/3]">
          <ImageWithFallback
            :src="currentImage?.url || null"
            :alt="currentImage?.alt || 'Slika artikla'"
            img-class="h-full w-full object-cover"
            placeholder-class="h-full w-full"
          />

          <template v-if="hasMultipleImages">
            <UButton
              color="neutral"
              variant="soft"
              icon="i-lucide-chevron-left"
              square
              class="absolute top-1/2 left-3 -translate-y-1/2"
              @click="showPrevious"
            />
            <UButton
              color="neutral"
              variant="soft"
              icon="i-lucide-chevron-right"
              square
              class="absolute top-1/2 right-3 -translate-y-1/2"
              @click="showNext"
            />
            <div
              class="bg-default/80 absolute right-3 bottom-3 rounded-full px-2 py-0.5 text-xs"
            >
              {{ activeIndex + 1 }} / {{ images.length }}
            </div>
          </template>
        </div>
      </div>

      <div v-if="hasMultipleImages" class="flex gap-2 overflow-x-auto pb-1">
        <button
          v-for="(image, index) in images"
          :key="image.id"
          type="button"
          class="bg-elevated relative w-24 shrink-0 overflow-hidden rounded-lg border"
          :class="index === activeIndex ? 'border-primary' : 'border-default'"
          @click="selectImage(index)"
        >
          <ImageWithFallback
            :src="image.url"
            :alt="image.alt"
            img-class="aspect-[4/3] h-full w-full object-cover"
            placeholder-class="aspect-[4/3] h-full w-full"
          />
        </button>
      </div>
    </div>
  </UCard>
</template>
