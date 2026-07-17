<script setup lang="ts">
const { item } = defineProps<{
  item: {
    systemUserId?: string | null
    firstName?: string | null
    lastName?: string | null
  }
}>()

const fullName = computed(() => {
  const first = item.firstName?.trim() || ""
  const last = item.lastName?.trim() || ""
  const combined = `${first} ${last}`.trim()
  return combined || "Kupac bez imena"
})
</script>

<template>
  <div class="min-w-0">
    <p class="text-highlighted truncate text-sm font-medium">
      {{ fullName }}
    </p>
    <p class="text-muted truncate font-mono text-xs">
      {{ item.systemUserId || "Bez ID-ja" }}
    </p>
  </div>
</template>
