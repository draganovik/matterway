<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    listClass?: string
    detailClass?: string
    detailLoading?: boolean
  }>(),
  {
    listClass: "",
    detailClass: "",
    detailLoading: false,
  },
)
</script>

<template>
  <div
    class="grid h-full min-h-0 gap-6 lg:grid-cols-[minmax(0,420px)_minmax(0,1fr)]"
  >
    <section
      class="border-default bg-elevated/70 flex min-h-0 flex-col gap-5 rounded-2xl border p-5"
      :class="props.listClass"
    >
      <slot name="list" />
    </section>

    <section
      class="border-default bg-elevated/70 relative flex min-h-0 flex-col gap-6 rounded-2xl border p-5"
      :class="props.detailClass"
      :style="props.detailLoading ? { overflow: 'hidden' } : undefined"
    >
      <div
        class="relative min-h-0 flex-1"
        :class="props.detailLoading ? 'pointer-events-none select-none' : ''"
        :aria-busy="props.detailLoading"
      >
        <slot name="detail" />
      </div>

      <div
        v-if="props.detailLoading"
        class="bg-elevated/45 absolute inset-0 z-10 grid place-items-center rounded-2xl backdrop-blur-[1px]"
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
