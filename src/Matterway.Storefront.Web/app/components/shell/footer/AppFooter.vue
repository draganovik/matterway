<template>
  <footer class="border-default bg-default mt-auto border-t">
    <div
      class="text-muted mx-auto flex w-full max-w-7xl flex-col gap-3 px-4 py-3 text-sm sm:flex-row sm:items-center sm:justify-between sm:px-5 lg:px-6"
    >
      <p>© {{ new Date().getFullYear() }} Matterway Prodavnica</p>
      <div class="flex items-center gap-4">
        <NuxtLink class="hover:text-primary" to="/">Početna</NuxtLink>
        <NuxtLink class="hover:text-primary" to="/articles">Artikli</NuxtLink>
        <NuxtLink class="hover:text-primary" to="/cart">Korpa</NuxtLink>
      </div>
    </div>
  </footer>
</template>
