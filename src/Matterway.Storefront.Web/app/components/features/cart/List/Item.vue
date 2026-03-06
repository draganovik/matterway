<script setup lang="ts">
import type { CartItem } from "~/types/cart"
import { formatMoney } from "~/utils/formatters"

const props = defineProps<{
  item: CartItem
}>()

const emit = defineEmits<{
  remove: [articleCode: string]
}>()
</script>

<template>
  <tr class="border-default border-t">
    <td class="px-3 py-3">
      <NuxtLink
        :to="`/articles/${props.item.articleCode}`"
        class="truncate text-sm font-semibold hover:text-cyan-700"
      >
        {{ props.item.articleName }}
      </NuxtLink>
    </td>

    <td class="text-muted px-3 py-3 whitespace-nowrap">
      {{ props.item.articleCode ? `#${props.item.articleCode}` : "-" }}
    </td>

    <td class="px-3 py-3 text-right whitespace-nowrap">
      {{ formatMoney(props.item.unitPrice) }}
    </td>

    <td class="px-3 py-3">
      <div class="flex items-center justify-center">
        <CartQuantityInput
          :article-code="props.item.articleCode"
          :quantity="props.item.quantity"
          :show-remove="true"
          @remove="emit('remove', props.item.articleCode)"
        />
      </div>
    </td>

    <td class="px-3 py-3 text-right font-semibold whitespace-nowrap">
      {{ formatMoney(props.item.unitPrice * props.item.quantity) }}
    </td>
  </tr>
</template>
