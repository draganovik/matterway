<script setup lang="ts">
const props = defineProps<{
  totalCount: number
  featuredCount: number
  cartItems: number
}>()
</script>

<template>
  <UCard class="border-default border">
    <div class="grid gap-6 lg:grid-cols-[1.2fr_1fr]">
      <div class="space-y-4">
        <p class="text-primary text-xs tracking-[0.3em] uppercase">Pregled</p>
        <h1 class="text-3xl font-semibold">Prodavnica za kupce</h1>
        <p class="text-muted max-w-2xl text-sm">
          Pregledajte proizvode za pametan dom, koristite korpu kao gost ili kao
          prijavljen korisnik i završite kupovinu u jednom toku.
        </p>
        <div class="flex flex-wrap gap-2">
          <UButton to="/articles" color="primary" icon="i-lucide-arrow-right">
            Pregledaj artikle
          </UButton>
          <UButton
            to="/cart"
            color="neutral"
            variant="soft"
            icon="i-lucide-shopping-bag"
          >
            Otvori korpu
          </UButton>
        </div>
      </div>

      <div class="grid grid-cols-2 gap-3">
        <div class="border-default bg-elevated/50 rounded-lg border px-4 py-3">
          <p class="text-muted text-[11px] tracking-[0.12em] uppercase">
            Stavke u katalogu
          </p>
          <p class="mt-1 text-2xl leading-none font-semibold">
            {{ props.totalCount }}
          </p>
        </div>

        <div class="border-default bg-elevated/50 rounded-lg border px-4 py-3">
          <p class="text-muted text-[11px] tracking-[0.12em] uppercase">
            Trenutno izdvojeni
          </p>
          <p class="mt-1 text-2xl leading-none font-semibold">
            {{ props.featuredCount }}
          </p>
        </div>

        <div
          class="border-primary/20 bg-primary/5 col-span-2 rounded-lg border px-4 py-3"
        >
          <p class="text-muted text-[11px] tracking-[0.12em] uppercase">
            Stavki u vašoj korpi
          </p>
          <p class="mt-1 text-2xl leading-none font-semibold">
            {{ props.cartItems }}
          </p>
        </div>
      </div>
    </div>
  </UCard>
</template>
