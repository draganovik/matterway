<script lang="ts" setup>
import { initTabs } from "flowbite";
import { useSessionStore } from "~/store/session";

const sessionStore = useSessionStore();

useHead({
  title: "Naslovna",
});

// initialize components based on data attribute selectors
onMounted(() => {
  initTabs();
  console.log(sessionStore.getSessionData);
});
</script>

<template>
  <FeaturedCarousel />
  <div class="mb-4 border-b border-slate-200 dark:border-slate-700">
    <ul
      class="-mb-px flex flex-wrap text-center text-sm font-medium"
      id="myTab"
      data-tabs-toggle="#myTabContent"
      role="tablist"
    >
      <li class="mr-2" role="presentation">
        <button
          class="inline-block rounded-t-lg border-b-2 p-4"
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
          class="inline-block rounded-t-lg border-b-2 border-transparent p-4 hover:border-slate-300 hover:text-slate-600 dark:hover:text-slate-300"
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
          class="inline-block rounded-t-lg border-b-2 border-transparent p-4 hover:border-slate-300 hover:text-slate-600 dark:hover:text-slate-300"
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
          class="inline-block rounded-t-lg border-b-2 border-transparent p-4 hover:border-slate-300 hover:text-slate-600 dark:hover:text-slate-300"
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
    <section
      class="hidden rounded-lg bg-slate-50 p-4 dark:bg-slate-800"
      id="profile"
      role="tabpanel"
      aria-labelledby="profile-tab"
    >
      <div class="grid grid-cols-2 gap-4 md:grid-cols-3">
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-1.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-2.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-3.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-4.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-5.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-6.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-7.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-8.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-9.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-10.jpg"
            alt=""
          />
        </div>
        <div>
          <img
            class="h-auto max-w-full rounded-lg"
            src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-11.jpg"
            alt=""
          />
        </div>
      </div>
    </section>
    <div
      class="hidden rounded-lg bg-slate-50 p-4 dark:bg-slate-800"
      id="dashboard"
      role="tabpanel"
      aria-labelledby="dashboard-tab"
    >
      <p class="text-sm text-slate-500 dark:text-slate-400">
        This is some placeholder content the
        <strong class="font-medium text-slate-800 dark:text-white"
          >Dashboard tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
    <div
      class="hidden rounded-lg bg-slate-50 p-4 dark:bg-slate-800"
      id="settings"
      role="tabpanel"
      aria-labelledby="settings-tab"
    >
      <p class="text-sm text-slate-500 dark:text-slate-400">
        This is some placeholder content the
        <strong class="font-medium text-slate-800 dark:text-white"
          >Settings tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
    <div
      class="hidden rounded-lg bg-slate-50 p-4 dark:bg-slate-800"
      id="contacts"
      role="tabpanel"
      aria-labelledby="contacts-tab"
    >
      <p class="text-sm text-slate-500 dark:text-slate-400">
        This is some placeholder content the
        <strong class="font-medium text-slate-800 dark:text-white"
          >Contacts tab's associated content</strong
        >. Clicking another tab will toggle the visibility of this one for the
        next. The tab JavaScript swaps classes to control the content visibility
        and styling.
      </p>
    </div>
  </div>
</template>

<style scoped></style>
