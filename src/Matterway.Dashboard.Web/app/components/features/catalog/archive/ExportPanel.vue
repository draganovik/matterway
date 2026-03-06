<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    canOperate?: boolean
    loading?: boolean
    error?: string
  }>(),
  {
    canOperate: false,
    loading: false,
    error: "",
  },
)

const emit = defineEmits<{
  (event: "download"): void
}>()
</script>

<template>
  <UCard
    class="dashboard-panel-surface !border-default h-full rounded-2xl !border !shadow-sm !ring-0"
    :ui="{ header: 'p-5 sm:p-5', body: 'px-5 pb-5 pt-0 sm:px-5 sm:pb-5' }"
  >
    <template #header>
      <div>
        <h3 class="text-foreground text-sm font-semibold">Export Archive</h3>
        <p class="text-muted text-xs">
          Download a full snapshot of catalog data and image binaries.
        </p>
      </div>
    </template>

    <div class="space-y-4">
      <UButton
        color="primary"
        icon="i-lucide-download"
        :loading="props.loading"
        :disabled="!props.canOperate"
        @click="emit('download')"
      >
        Download Archive
      </UButton>

      <StatusMessages
        :loading="props.loading && 'Preparing archive...'"
        :error="props.error"
      />
    </div>
  </UCard>
</template>
