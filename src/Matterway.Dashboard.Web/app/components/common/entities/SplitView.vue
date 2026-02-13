<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    listClass?: string
    detailClass?: string
  }>(),
  {
    listClass: "",
    detailClass: "",
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
      class="border-default bg-elevated/70 flex min-h-0 flex-col gap-6 rounded-2xl border p-5"
      :class="props.detailClass"
    >
      <slot name="detail" />
    </section>
  </div>
</template>
