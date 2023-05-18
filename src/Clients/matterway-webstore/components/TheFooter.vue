<script lang="ts" setup>
// method to get the current year
const getCurrentYear = () => {
  return new Date().getFullYear();
};
</script>

<template>
  <footer class="rounded-lg bg-white shadow dark:bg-slate-900">
    <div class="mx-auto w-full max-w-screen-xl p-4 md:py-8">
      <div class="sm:flex sm:items-center sm:justify-between">
        <a href="https://flowbite.com/" class="mb-4 flex items-center sm:mb-0">
          <img
            src="https://flowbite.com/docs/images/logo.svg"
            class="mr-3 h-8"
            alt="Flowbite Logo"
          />
          <span
            class="self-center whitespace-nowrap text-2xl font-semibold dark:text-white"
            >Flowbite</span
          >
        </a>
        <ul
          class="mb-6 flex flex-wrap items-center text-sm font-medium text-slate-500 dark:text-slate-400 sm:mb-0"
        >
          <li>
            <a href="#" class="mr-4 hover:underline md:mr-6">About</a>
          </li>
          <li>
            <a href="#" class="mr-4 hover:underline md:mr-6">Privacy Policy</a>
          </li>
          <li>
            <a href="#" class="mr-4 hover:underline md:mr-6">Licensing</a>
          </li>
          <li>
            <a href="#" class="hover:underline">Contact</a>
          </li>
        </ul>
      </div>
      <hr
        class="my-6 border-slate-200 dark:border-slate-700 sm:mx-auto lg:my-8"
      />
      <span
        class="block text-sm text-slate-500 dark:text-slate-400 sm:text-center"
        >© {{ getCurrentYear() }}
        <a href="https://draganovik.com/" class="hover:underline"
          >Mladen Draganović</a
        >. Sva prava zadržana.</span
      >
    </div>
  </footer>
</template>

<style scoped></style>
