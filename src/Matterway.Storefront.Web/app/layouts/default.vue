<template>
  <div
    class="flex min-h-[calc(100vh+1rem)] flex-col bg-linear-to-b from-slate-200 via-slate-100 to-slate-100 text-default dark:from-slate-950 dark:via-slate-950 dark:to-slate-950"
  >
    <ShellHeaderNavBar />
    <main
      class="mx-auto w-full max-w-7xl flex-1 px-4 pb-8 pt-24 sm:px-6 lg:px-8"
    >
      <slot />
    </main>
    <ShellFooterAppFooter />
  </div>
</template>
