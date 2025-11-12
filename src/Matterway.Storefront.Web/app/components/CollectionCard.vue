<script setup lang="ts">
import { computed } from "vue";
import type { CollectionIconVariant } from "@composables/collections";

const props = withDefaults(
  defineProps<{
    title: string;
    description: string;
    accent?: string;
    ctaLabel?: string;
    icon?: CollectionIconVariant;
    disabled?: boolean;
  }>(),
  {
    accent: "",
    ctaLabel: "Istraži ponudu",
    icon: "generic",
    disabled: false,
  },
);

const emit = defineEmits<{
  (e: "click", event: MouseEvent): void;
}>();

const accentClasses = computed(() =>
  props.accent ? props.accent : "from-slate-100 via-slate-50 to-slate-100",
);

const iconNameMap: Record<CollectionIconVariant, string> = {
  lighting: "heroicons-outline:light-bulb",
  energy: "heroicons-outline:bolt",
  security: "heroicons-outline:lock-closed",
  generic: "heroicons-outline:squares-plus",
};

const collectionIconName = computed(
  () => iconNameMap[props.icon] ?? iconNameMap.generic,
);

const onClick = (event: MouseEvent) => {
  if (props.disabled) {
    return;
  }
  emit("click", event);
};
</script>

<template>
  <button
    type="button"
    class="group flex h-full flex-col gap-3 rounded-2xl border border-slate-200 bg-linear-to-br p-6 text-left transition hover:-translate-y-1 hover:border-blue-200 hover:shadow-lg focus:outline-hidden focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-70 dark:border-slate-700 dark:bg-slate-800/60"
    :class="accentClasses"
    :disabled="disabled"
    @click="onClick"
  >
    <div
      class="inline-flex h-12 w-12 items-center justify-center rounded-full bg-white/70 text-blue-600 shadow-xs transition group-hover:scale-105 dark:bg-slate-900/80 dark:text-blue-300"
    >
      <slot name="icon">
        <Icon :name="collectionIconName" class="text-xl" aria-hidden="true" />
      </slot>
    </div>
    <div class="space-y-1.5">
      <h3 class="text-lg font-semibold text-slate-900 dark:text-slate-100">
        {{ title }}
      </h3>
      <p class="text-sm text-slate-600 dark:text-slate-300">
        {{ description }}
      </p>
    </div>
    <span
      v-if="ctaLabel"
      class="inline-flex items-center gap-2 text-sm font-medium text-blue-600 transition group-hover:translate-x-1 dark:text-blue-300"
    >
      {{ ctaLabel }}
      <Icon
        name="heroicons-outline:arrow-long-right"
        class="text-base"
        aria-hidden="true"
      />
    </span>
  </button>
</template>
