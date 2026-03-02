<template>
  <footer class="border-default bg-elevated/60 mt-auto border-t pb-4">
    <div
      class="mx-auto flex w-full max-w-7xl flex-col gap-3 px-4 py-4 text-sm text-slate-500 sm:flex-row sm:items-center sm:justify-between sm:px-6 lg:px-8"
    >
      <p>© {{ new Date().getFullYear() }} Matterway prodavnica</p>
      <div class="flex items-center gap-4">
        <NuxtLink class="hover:text-primary" to="/">Pregled</NuxtLink>
        <NuxtLink class="hover:text-primary" to="/articles">Artikli</NuxtLink>
        <NuxtLink class="hover:text-primary" to="/cart">Korpa</NuxtLink>
      </div>
    </div>
  </footer>
</template>
