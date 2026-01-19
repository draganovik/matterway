<script></script>
<template>
  <div
    role="status"
    class="grid w-full max-w-sm animate-pulse grid-rows-[min-content] flex-col place-items-stretch overflow-hidden rounded-lg border border-slate-200 shadow-sm dark:border-slate-700 dark:bg-slate-800"
  >
    <div
      class="grid h-60 w-full place-items-center bg-slate-300 dark:bg-slate-700"
    >
      <Icon
        name="heroicons-outline:photo"
        class="rounded-t-lg text-7xl text-slate-200 dark:text-slate-600"
        aria-hidden="true"
      />
    </div>
    <div class="flex w-full flex-col justify-start p-5 pb-32 pt-3">
      <div class="mt-2.5 h-2 rounded-full bg-slate-200 dark:bg-slate-700"></div>
      <div class="mt-2.5 h-2 rounded-full bg-slate-200 dark:bg-slate-700"></div>
      <div class="mt-2.5 h-2 rounded-full bg-slate-200 dark:bg-slate-700"></div>
    </div>
    <span class="sr-only">Loading...</span>
  </div>
</template>
<style></style>
