<script setup lang="ts">
const props = defineProps<{
  totalCount: number
  cartItems: number
}>()
</script>

<template>
  <section
    class="border-default grid gap-6 border-b pb-6 lg:grid-cols-[minmax(0,1fr)_auto] lg:items-end"
  >
    <div class="max-w-3xl space-y-4">
      <div class="space-y-2">
        <p class="text-primary text-sm font-medium">Matterway prodavnica</p>
        <h1 class="text-3xl font-semibold tracking-tight sm:text-4xl">
          Pametna oprema za bolji dom
        </h1>
        <p class="text-muted max-w-2xl text-sm sm:text-base">
          Istražite najnoviju opremu za pametan dom, od rasvete do sigurnosnih
          kamera, i završite kupovinu na jednom mestu.
        </p>
      </div>

      <div class="flex flex-wrap gap-2">
        <UButton to="/articles" trailing-icon="i-lucide-arrow-right">
          Pogledaj artikle
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

    <dl class="flex gap-8 lg:justify-end">
      <div>
        <dt class="text-muted text-xs">Artikala u katalogu</dt>
        <dd class="mt-1 text-2xl leading-none font-semibold">
          {{ props.totalCount }}
        </dd>
      </div>
      <div>
        <dt class="text-muted text-xs">U vašoj korpi</dt>
        <dd class="mt-1 text-2xl leading-none font-semibold">
          {{ props.cartItems }}
        </dd>
      </div>
    </dl>
  </section>
</template>
