<script setup lang="ts">
import { useDelayedLoading } from "~/composables/workflows/state/useDelayedLoading"

const props = withDefaults(defineProps<{ detailLoading?: boolean }>(), {
  detailLoading: false,
})

const showDetailLoading = useDelayedLoading(() => props.detailLoading)
</script>

<template>
  <div class="grid h-full min-h-0 lg:grid-cols-[minmax(0,400px)_minmax(0,1fr)]">
    <section
      class="border-muted flex min-h-0 flex-col gap-3 overflow-hidden border-b pb-3 lg:border-e lg:border-b-0 lg:pe-3 lg:pb-0"
    >
      <slot name="list" />
    </section>

    <section
      class="relative flex min-h-0 flex-col gap-3 pt-3 lg:ps-3 lg:pt-0"
      :style="props.detailLoading ? { overflow: 'hidden' } : undefined"
    >
      <div
        class="relative min-h-0 flex-1 overflow-y-auto pe-3"
        :class="props.detailLoading ? 'pointer-events-none select-none' : ''"
        :aria-busy="props.detailLoading"
      >
        <slot name="detail" />
      </div>

      <div
        v-if="showDetailLoading"
        class="bg-default/55 absolute inset-0 z-10 grid place-items-center backdrop-blur-[2px]"
      >
        <UIcon
          name="i-lucide-loader"
          class="text-primary h-8 w-8 animate-spin"
          style="animation-duration: 2s"
        />
      </div>
    </section>
  </div>
</template>
