<script setup lang="ts">
import CartItemsFeature from '@components/customers/CartItemsFeature.vue'

definePageMeta({
  title: 'Cart Items',
  service: 'customers',
  level: 'observer'
})
</script>

<template>
  <CartItemsFeature />
</template>
