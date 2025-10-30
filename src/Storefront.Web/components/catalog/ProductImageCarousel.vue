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
      class="relative aspect-video w-full overflow-hidden rounded-2xl border border-slate-200 bg-slate-100 dark:border-slate-700 dark:bg-slate-700 md:aspect-[4/3]"
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
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          stroke-width="1.5"
          stroke="currentColor"
          class="size-6"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="m2.25 15.75 5.159-5.159a2.25 2.25 0 0 1 3.182 0l5.159 5.159m-1.5-1.5 1.409-1.409a2.25 2.25 0 0 1 3.182 0l2.909 2.909m-18 3.75h16.5a1.5 1.5 0 0 0 1.5-1.5V6a1.5 1.5 0 0 0-1.5-1.5H3.75A1.5 1.5 0 0 0 2.25 6v12a1.5 1.5 0 0 0 1.5 1.5Zm10.5-11.25h.008v.008h-.008V8.25Zm.375 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Z"
          />
        </svg>
      </div>
    </div>

    <div
      v-if="slides.length > 1"
      class="pointer-events-none absolute inset-0 flex items-center justify-between px-2"
    >
      <button
        type="button"
        class="carousel-nav"
        aria-label="Previous image"
        @click="previous"
      >
        <svg
          class="h-5 w-5"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M15 19l-7-7 7-7"
          />
        </svg>
      </button>
      <button
        type="button"
        class="carousel-nav"
        aria-label="Next image"
        @click="next"
      >
        <svg
          class="h-5 w-5"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M9 5l7 7-7 7"
          />
        </svg>
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
.carousel-nav {
  @apply pointer-events-auto inline-flex items-center justify-center rounded-full bg-white/80 p-2 text-slate-600 shadow transition hover:bg-white focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-slate-800/80 dark:text-slate-100 dark:hover:bg-slate-800;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
