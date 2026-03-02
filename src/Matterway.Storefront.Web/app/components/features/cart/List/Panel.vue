<script setup lang="ts">
import type { CartItem } from "~/types/cart"

const props = defineProps<{
  items: CartItem[]
}>()

const emit = defineEmits<{
  increase: [articleId: string]
  decrease: [articleId: string]
  remove: [articleId: string]
}>()
</script>

<template>
  <UCard
    :ui="{ body: 'p-0 sm:p-0' }"
    class="border-default bg-default overflow-hidden border"
  >
    <div class="overflow-x-auto">
      <table class="w-full text-left text-sm">
        <thead class="bg-elevated text-muted">
          <tr>
            <th class="px-3 py-2 font-medium">Artikal</th>
            <th class="px-3 py-2 font-medium">Sifra</th>
            <th class="px-3 py-2 text-right font-medium">Cena</th>
            <th class="px-3 py-2 text-center font-medium">Kolicina</th>
            <th class="px-3 py-2 text-right font-medium">Ukupno</th>
            <th class="px-3 py-2 text-right font-medium">Akcije</th>
          </tr>
        </thead>
        <tbody>
          <CartListItem
            v-for="item in props.items"
            :key="item.articleId"
            :item="item"
            @increase="emit('increase', $event)"
            @decrease="emit('decrease', $event)"
            @remove="emit('remove', $event)"
          />
        </tbody>
      </table>
    </div>
  </UCard>
</template>
