<script setup lang="ts">
import type { CartItem } from "~/types/cart"
import { formatMoney } from "~/utils/formatters"

const props = defineProps<{
  item: CartItem
}>()

const emit = defineEmits<{
  increase: [articleId: string]
  decrease: [articleId: string]
  remove: [articleId: string]
}>()
</script>

<template>
  <tr class="border-default border-t">
    <td class="px-3 py-3">
      <NuxtLink
        :to="`/articles/${props.item.articleId}`"
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
      <div class="flex items-center justify-center gap-2">
        <UButton
          color="neutral"
          variant="soft"
          icon="i-lucide-minus"
          square
          @click="emit('decrease', props.item.articleId)"
        />
        <UBadge color="primary" variant="subtle">
          {{ props.item.quantity }}
        </UBadge>
        <UButton
          color="primary"
          variant="soft"
          icon="i-lucide-plus"
          square
          @click="emit('increase', props.item.articleId)"
        />
      </div>
    </td>

    <td class="px-3 py-3 text-right font-semibold whitespace-nowrap">
      {{ formatMoney(props.item.unitPrice * props.item.quantity) }}
    </td>

    <td class="px-3 py-3 text-right">
      <UButton
        color="error"
        variant="ghost"
        icon="i-lucide-trash"
        square
        @click="emit('remove', props.item.articleId)"
      />
    </td>
  </tr>
</template>
