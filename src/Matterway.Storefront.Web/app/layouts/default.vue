<template>
  <div
    class="text-default flex min-h-[calc(100vh+1rem)] flex-col bg-linear-to-b from-stone-200 via-stone-100 to-stone-100 dark:from-stone-950 dark:via-stone-950 dark:to-stone-950"
  >
    <HeaderNavBar />
    <main
      class="mx-auto w-full max-w-7xl flex-1 px-4 pt-24 pb-8 sm:px-6 lg:px-8"
    >
      <slot />
    </main>
    <FooterAppFooter />
  </div>
</template>
