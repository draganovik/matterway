<script setup lang="ts">
const props = withDefaults(defineProps<{ detailLoading?: boolean }>(), {
  detailLoading: false,
})
</script>

<template>
  <div
    class="grid h-full min-h-0 gap-6 lg:grid-cols-[minmax(0,420px)_minmax(0,1fr)]"
  >
    <section
      class="dashboard-panel-surface flex min-h-0 flex-col gap-5 overflow-hidden rounded-2xl p-5"
    >
      <slot name="list" />
    </section>

    <section
      class="dashboard-panel-surface relative flex min-h-0 flex-col gap-6 rounded-2xl p-5"
      :style="props.detailLoading ? { overflow: 'hidden' } : undefined"
    >
      <div
        class="relative min-h-0 flex-1 overflow-y-auto"
        :class="props.detailLoading ? 'pointer-events-none select-none' : ''"
        :aria-busy="props.detailLoading"
      >
        <slot name="detail" />
      </div>

      <div
        v-if="props.detailLoading"
        class="bg-default/55 absolute inset-0 z-10 grid place-items-center rounded-2xl backdrop-blur-[2px]"
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
