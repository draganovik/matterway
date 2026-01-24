<script setup lang="ts">
import { useFeatureTabs } from '~/composables/useFeatureTabs'

const { tabs, active, setActive } = useFeatureTabs()

const items = computed(() =>
  tabs.value.map(tab => ({ label: tab.label, value: tab.key }))
)
</script>

<template>
  <UTabs
    v-if="items.length > 1"
    :items="items"
    :model-value="active"
    @update:model-value="(value: string) => setActive(value)"
  />
</template>
