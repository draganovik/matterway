<template>
  <div id="default-carousel" class="relative w-full" data-carousel="slide">
    <!-- Carousel wrapper -->
    <div class="relative h-[60svh] min-h-80 overflow-hidden rounded-xl">
      <div
        v-for="(item, index) in carousel"
        :key="index"
        class="hidden h-full w-full duration-700 ease-in-out"
        data-carousel-item
      >
        <img
          :src="item.url"
          class="absolute left-1/2 top-1/2 flex h-full w-full -translate-x-1/2 -translate-y-1/2 object-cover opacity-70"
          :alt="index.toString()"
        />
        <div
          class="flex h-full w-full flex-col items-center justify-center gap-6 px-14 pb-16 pt-4 text-center sm:px-16 md:items-start md:text-left"
        >
          <h1 class="relative text-4xl md:text-5xl">Kupi novi proizvod sad!</h1>
          <NuxtLink
            to="/products"
            class="group relative mb-2 mr-2 flex w-max items-center justify-center overflow-hidden rounded-lg bg-linear-to-br from-green-400 to-blue-600 p-0.5 text-sm font-medium text-slate-900 hover:text-white focus:outline-hidden focus:ring-4 focus:ring-green-200 group-hover:from-green-400 group-hover:to-blue-600 dark:text-white dark:focus:ring-green-800"
          >
            <span
              class="relative rounded-md bg-white px-5 py-2.5 transition-all duration-75 ease-in group-hover:bg-opacity-0 dark:bg-slate-900"
            >
              Poseti stranicu
            </span>
          </NuxtLink>
        </div>
      </div>
    </div>
    <!-- Slider indicators -->
    <div
      class="indicators absolute bottom-5 left-1/2 z-30 flex -translate-x-1/2 space-x-3"
    >
      <button
        v-for="(_, index) in carousel"
        type="button"
        class="pill h-3 w-3 rounded-full"
        :aria-current="index === 0 ? 'true' : 'false'"
        :aria-label="'Slide ' + index.toString()"
        :data-carousel-slide-to="index"
      ></button>
    </div>
    <!-- Slider controls -->
    <button
      type="button"
      class="group absolute left-0 top-0 z-30 flex h-full cursor-pointer items-center justify-center px-4 focus:outline-hidden"
      data-carousel-prev
    >
      <span
        class="inline-flex h-8 w-8 items-center justify-center rounded-full bg-white/30 group-hover:bg-white/50 group-focus:outline-hidden group-focus:ring-4 group-focus:ring-white dark:bg-slate-800/30 dark:group-hover:bg-slate-800/60 dark:group-focus:ring-slate-800/70 sm:h-10 sm:w-10"
      >
        <Icon
          name="heroicons-outline:chevron-left"
          aria-hidden="true"
          class="text-xl text-white dark:text-slate-800 sm:text-2xl"
        />
        <span class="sr-only">Previous</span>
      </span>
    </button>
    <button
      type="button"
      class="group absolute right-0 top-0 z-30 flex h-full cursor-pointer items-center justify-center px-4 focus:outline-hidden"
      data-carousel-next
    >
      <span
        class="inline-flex h-8 w-8 items-center justify-center rounded-full bg-white/30 group-hover:bg-white/50 group-focus:outline-hidden group-focus:ring-4 group-focus:ring-white dark:bg-slate-800/30 dark:group-hover:bg-slate-800/60 dark:group-focus:ring-slate-800/70 sm:h-10 sm:w-10"
      >
        <Icon
          name="heroicons-outline:chevron-right"
          aria-hidden="true"
          class="text-xl text-white dark:text-slate-800 sm:text-2xl"
        />
        <span class="sr-only">Next</span>
      </span>
    </button>
  </div>
</template>

<script lang="ts" setup>
import { initCarousels } from "flowbite";

// initialize components based on data attribute selectors
onMounted(() => {
  initCarousels();
});

const carousel = ref([
  {
    position: 0,
    url: "https://www.cnet.com/a/img/resize/a4e110a6b717ddb98cca2d44f7aff4ab1f704aa4/hub/2020/10/21/a532eddf-2122-4ca0-bc56-539295329d2b/amazon-echo-2020-2.jpg?auto=webp&fit=crop&height=675&width=1200",
  },
  {
    position: 1,
    url: "https://www.digitaltrends.com/wp-content/uploads/2021/06/amazon-echo-show-5-2nd-gen-2021-review-1-of-11.jpg?p=1",
  },
  {
    position: 2,
    url: "https://i.kinja-img.com/gawker-media/image/upload/c_fill,f_auto,fl_progressive,g_center,h_675,pg_1,q_80,w_1200/95a5ed6fb10e3420012279924d668ab0.jpg",
  },
  {
    position: 3,
    url: "https://media-cldnry.s-nbcnews.com/image/upload/t_nbcnews-fp-1200-630,f_auto,q_auto:best/newscms/2018_21/2445331/180525-amazon-echo-dot-ew-437p.jpg",
  },
]);
</script>

<style>
/* Add any necessary CSS styles here */
</style>
