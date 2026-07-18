<template>
  <div class="text-default bg-default">
    <HeaderNavBar />
    <main class="min-h-[calc(100dvh-3.5rem)] w-full px-4 py-6 sm:px-5 lg:px-6">
      <div class="mx-auto w-full max-w-6xl">
        <slot />
      </div>
    </main>
    <FooterAppFooter />
  </div>
</template>
