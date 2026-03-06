<script setup lang="ts">
const props = defineProps<{
  totalCount: number
  featuredCount: number
  cartItems: number
}>()
</script>

<template>
  <UCard class="border-default bg-elevated/60 border">
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
        <UCard class="border-default bg-default border">
          <p class="text-muted text-xs">Stavke u katalogu</p>
          <p class="text-2xl font-semibold">{{ props.totalCount }}</p>
        </UCard>

        <UCard class="border-default bg-default border">
          <p class="text-muted text-xs">Trenutno Izdvojeni</p>
          <p class="text-2xl font-semibold">{{ props.featuredCount }}</p>
        </UCard>

        <UCard class="border-default bg-default col-span-2 border">
          <p class="text-muted text-xs">Stavki u vašoj korpi</p>
          <p class="text-2xl font-semibold">{{ props.cartItems }}</p>
        </UCard>
      </div>
    </div>
  </UCard>
</template>
