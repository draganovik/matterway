<script setup lang="ts">
const props = withDefaults(defineProps<{
  listClass?: string
  detailClass?: string
}>(), {
  listClass: '',
  detailClass: ''
})
</script>

<template>
  <div class="grid gap-8 lg:grid-cols-[minmax(0,420px)_minmax(0,1fr)]">
    <section
      class="flex flex-col gap-5 rounded-2xl border border-default bg-elevated/30 p-5 shadow-sm"
      :class="props.listClass"
    >
      <slot name="list" />
    </section>

    <section
      class="flex flex-col gap-6 rounded-2xl border border-default bg-elevated/30 p-5 shadow-sm"
      :class="props.detailClass"
    >
      <slot name="detail" />
    </section>
  </div>
</template>
