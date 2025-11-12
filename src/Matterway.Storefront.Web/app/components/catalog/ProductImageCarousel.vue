<script lang="ts" setup>
import { computed, ref, watch } from "vue";

interface CarouselImage {
  id?: number | string;
  imageUrl?: string;
  imageAlt?: string;
}

const props = defineProps<{
  images?: CarouselImage[];
  fallbackAlt?: string;
}>();

const activeIndex = ref(0);

const slides = computed(() =>
  (props.images ?? []).filter(
    (image): image is CarouselImage & { imageUrl: string } =>
      Boolean(image?.imageUrl),
  ),
);

watch(
  slides,
  (currentSlides) => {
    if (currentSlides.length === 0) {
      activeIndex.value = 0;
      return;
    }

    if (activeIndex.value >= currentSlides.length) {
      activeIndex.value = currentSlides.length - 1;
    }
  },
  { immediate: true },
);

const goTo = (index: number) => {
  if (index < 0 || index >= slides.value.length) {
    return;
  }
  activeIndex.value = index;
};

const next = () => {
  if (slides.value.length <= 1) {
    return;
  }
  activeIndex.value = (activeIndex.value + 1) % slides.value.length;
};

const previous = () => {
  if (slides.value.length <= 1) {
    return;
  }
  activeIndex.value =
    (activeIndex.value - 1 + slides.value.length) % slides.value.length;
};
</script>

<template>
  <div class="relative w-full">
    <div
      class="relative aspect-video w-full overflow-hidden rounded-2xl border border-slate-200 bg-slate-100 dark:border-slate-700 dark:bg-slate-700 md:aspect-4/3"
    >
      <template v-if="slides.length > 0">
        <Transition name="slide" mode="out-in">
          <img
            :key="activeIndex"
            :src="slides[activeIndex].imageUrl"
            class="absolute left-1/2 top-1/2 h-full w-full -translate-x-1/2 -translate-y-1/2 object-cover"
            :alt="
              slides[activeIndex].imageAlt || fallbackAlt || 'Product image'
            "
          />
        </Transition>
      </template>
      <div
        v-else
        class="flex h-full w-full items-center justify-center p-[33%] text-slate-400"
      >
        <Icon
          name="heroicons-outline:photo"
          class="text-2xl"
          aria-hidden="true"
        />
      </div>
    </div>

    <div
      v-if="slides.length > 1"
      class="pointer-events-none absolute inset-0 flex items-center justify-between px-2"
    >
      <button
        type="button"
        class="pointer-events-auto inline-flex items-center justify-center rounded-full bg-white/80 p-2 text-slate-600 shadow transition hover:bg-white focus:outline-hidden focus:ring-2 focus:ring-blue-500 dark:bg-slate-800/80 dark:text-slate-100 dark:hover:bg-slate-800"
        aria-label="Previous image"
        @click="previous"
      >
        <Icon
          name="heroicons-outline:chevron-left"
          aria-hidden="true"
          class="text-xl"
        />
      </button>
      <button
        type="button"
        class="pointer-events-auto inline-flex items-center justify-center rounded-full bg-white/80 p-2 text-slate-600 shadow transition hover:bg-white focus:outline-hidden focus:ring-2 focus:ring-blue-500 dark:bg-slate-800/80 dark:text-slate-100 dark:hover:bg-slate-800"
        aria-label="Next image"
        @click="next"
      >
        <Icon
          name="heroicons-outline:chevron-right"
          aria-hidden="true"
          class="text-xl"
        />
      </button>
    </div>

    <div
      v-if="slides.length > 1"
      class="mt-4 flex items-center justify-center gap-2"
    >
      <button
        v-for="(slide, index) in slides"
        :key="slide.id ?? slide.imageUrl ?? index"
        type="button"
        class="h-3 w-3 rounded-full border border-transparent transition"
        :class="
          index === activeIndex
            ? 'bg-blue-600 dark:bg-blue-400'
            : 'bg-slate-300 hover:bg-slate-400 dark:bg-slate-600 dark:hover:bg-slate-500'
        "
        :aria-label="`Show image ${index + 1}`"
        @click="goTo(index)"
      />
    </div>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
