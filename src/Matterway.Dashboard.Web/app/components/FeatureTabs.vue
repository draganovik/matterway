<script setup lang="ts">
import type { FeatureAction } from '~/data/adminFeatures'

defineProps<{
  tabs: FeatureAction[]
  modelValue: string
}>()

const emit = defineEmits<{
  (event: 'update:modelValue', value: string): void
}>()
</script>

<template>
  <UTabs
    :items="tabs.map(tab => ({ label: tab.label, value: tab.key }))"
    :model-value="modelValue"
    @update:model-value="(value: string) => emit('update:modelValue', value)"
  />
</template>
