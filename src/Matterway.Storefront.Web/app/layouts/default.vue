<template>
  <div
    class="text-default flex min-h-[calc(100vh+1rem)] flex-col bg-linear-to-b from-slate-200 via-slate-100 to-slate-100 dark:from-slate-950 dark:via-slate-950 dark:to-slate-950"
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
