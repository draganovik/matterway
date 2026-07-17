<template>
  <div class="text-default bg-default flex min-h-screen flex-col">
    <HeaderNavBar />
    <main
      class="mx-auto w-full max-w-7xl flex-1 px-4 pt-20 pb-6 sm:px-5 lg:px-6"
    >
      <slot />
    </main>
    <FooterAppFooter />
  </div>
</template>
