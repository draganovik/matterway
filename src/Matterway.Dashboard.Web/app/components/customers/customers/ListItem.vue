<script setup lang="ts">
const { item } = defineProps<{
  item: {
    systemUserId?: string | null
    firstName?: string | null
    lastName?: string | null
    birthDate?: string | null
  }
}>()

const fullName = computed(() => {
  const first = item.firstName?.trim() || ''
  const last = item.lastName?.trim() || ''
  const combined = `${first} ${last}`.trim()
  return combined || 'Unnamed customer'
})
</script>

<template>
  <div class="flex items-start justify-between gap-3">
    <div class="min-w-0">
      <p class="text-foreground truncate text-base font-medium">
        {{ fullName }}
      </p>
      <p class="text-muted truncate font-mono text-xs">
        {{ item.systemUserId || 'No ID' }}
      </p>
    </div>

    <UBadge
      color="neutral"
      variant="subtle"
      class="shrink-0 font-normal whitespace-nowrap"
    >
      {{ item.birthDate || 'No birth date' }}
    </UBadge>
  </div>
</template>
