<script lang="ts" setup>
import { initFlowbite } from "flowbite";

// initialize components based on data attribute selectors
onMounted(() => {
  initFlowbite();
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

<template>
  <div id="default-carousel" class="relative w-full" data-carousel="slide">
    <!-- Carousel wrapper -->
    <div class="relative overflow-hidden rounded-xl h-[50svh] min-h-[20rem]">
      <div
        v-for="(item, index) in carousel"
        :key="index"
        class="hidden duration-700 w-full h-full ease-in-out"
        data-carousel-item
      >
        <img
          :src="item.url"
          class="absolute flex w-full -translate-x-1/2 -translate-y-1/2 top-1/2 left-1/2 object-cover"
          :alt="index.toString()"
        />
      </div>
    </div>
    <!-- Slider indicators -->
    <div
      class="absolute z-30 flex space-x-3 -translate-x-1/2 bottom-5 left-1/2 indicators"
    >
      <button
        v-for="(_, index) in carousel"
        type="button"
        class="w-3 h-3 rounded-full pill"
        :aria-current="index === 0 ? 'true' : 'false'"
        :aria-label="'Slide ' + index.toString()"
        :data-carousel-slide-to="index"
      ></button>
    </div>
    <!-- Slider controls -->
    <button
      type="button"
      class="absolute top-0 left-0 z-30 flex items-center justify-center h-full px-4 cursor-pointer group focus:outline-none"
      data-carousel-prev
    >
      <span
        class="inline-flex items-center justify-center w-8 h-8 rounded-full sm:w-10 sm:h-10 bg-white/30 dark:bg-slate-800/30 group-hover:bg-white/50 dark:group-hover:bg-slate-800/60 group-focus:ring-4 group-focus:ring-white dark:group-focus:ring-slate-800/70 group-focus:outline-none"
      >
        <svg
          aria-hidden="true"
          class="w-5 h-5 text-white sm:w-6 sm:h-6 dark:text-slate-800"
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
          ></path>
        </svg>
        <span class="sr-only">Previous</span>
      </span>
    </button>
    <button
      type="button"
      class="absolute top-0 right-0 z-30 flex items-center justify-center h-full px-4 cursor-pointer group focus:outline-none"
      data-carousel-next
    >
      <span
        class="inline-flex items-center justify-center w-8 h-8 rounded-full sm:w-10 sm:h-10 bg-white/30 dark:bg-slate-800/30 group-hover:bg-white/50 dark:group-hover:bg-slate-800/60 group-focus:ring-4 group-focus:ring-white dark:group-focus:ring-slate-800/70 group-focus:outline-none"
      >
        <svg
          aria-hidden="true"
          class="w-5 h-5 text-white sm:w-6 sm:h-6 dark:text-slate-800"
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
          ></path>
        </svg>
        <span class="sr-only">Next</span>
      </span>
    </button>
  </div>

  <div class="mb-4 border-b border-gray-200 dark:border-gray-700">
    <ul
      class="flex flex-wrap -mb-px text-sm font-medium text-center"
      id="myTab"
      data-tabs-toggle="#myTabContent"
      role="tablist"
    >
      <li class="mr-2" role="presentation">
        <button
          class="inline-block p-4 border-b-2 rounded-t-lg"
          id="profile-tab"
          data-tabs-target="#profile"
          type="button"
          role="tab"
          aria-controls="profile"
          aria-selected="false"
        >
          Popularno
        </button>
      </li>
      <li class="mr-2" role="presentation">
        <button
          class="inline-block p-4 border-b-2 border-transparent rounded-t-lg hover:text-gray-600 hover:border-gray-300 dark:hover:text-gray-300"
          id="dashboard-tab"
          data-tabs-target="#dashboard"
          type="button"
          role="tab"
          aria-controls="dashboard"
          aria-selected="false"
        >
          Akcijska ponuda
        </button>
      </li>
      <li class="mr-2" role="presentation">
        <button
          class="inline-block p-4 border-b-2 border-transparent rounded-t-lg hover:text-gray-600 hover:border-gray-300 dark:hover:text-gray-300"
          id="settings-tab"
          data-tabs-target="#settings"
          type="button"
          role="tab"
          aria-controls="settings"
          aria-selected="false"
        >
          Rasveta
        </button>
      </li>
      <li role="presentation">
        <button
          class="inline-block p-4 border-b-2 border-transparent rounded-t-lg hover:text-gray-600 hover:border-gray-300 dark:hover:text-gray-300"
          id="contacts-tab"
          data-tabs-target="#contacts"
          type="button"
          role="tab"
          aria-controls="contacts"
          aria-selected="false"
        >
          Automatizacija
        </button>
      </li>
    </ul>
  </div>
  <div id="myTabContent">
    <div
      class="hidden p-4 rounded-lg bg-gray-50 dark:bg-gray-800"
      id="profile"
      role="tabpanel"
      aria-labelledby="profile-tab"
    >
      <p class="text-sm text-gray-500 dark:text-gray-400">
        This is some placeholder content the
        <strong class="font-medium text-gray-800 dark:text-white"
          >Profile tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
    <div
      class="hidden p-4 rounded-lg bg-gray-50 dark:bg-gray-800"
      id="dashboard"
      role="tabpanel"
      aria-labelledby="dashboard-tab"
    >
      <p class="text-sm text-gray-500 dark:text-gray-400">
        This is some placeholder content the
        <strong class="font-medium text-gray-800 dark:text-white"
          >Dashboard tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
    <div
      class="hidden p-4 rounded-lg bg-gray-50 dark:bg-gray-800"
      id="settings"
      role="tabpanel"
      aria-labelledby="settings-tab"
    >
      <p class="text-sm text-gray-500 dark:text-gray-400">
        This is some placeholder content the
        <strong class="font-medium text-gray-800 dark:text-white"
          >Settings tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
    <div
      class="hidden p-4 rounded-lg bg-gray-50 dark:bg-gray-800"
      id="contacts"
      role="tabpanel"
      aria-labelledby="contacts-tab"
    >
      <p class="text-sm text-gray-500 dark:text-gray-400">
        This is some placeholder content the
        <strong class="font-medium text-gray-800 dark:text-white"
          >Contacts tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
  </div>

  <h1 class="text-4xl font-bold mt-28">Welcome to Front Page</h1>
</template>

<style scoped></style>
