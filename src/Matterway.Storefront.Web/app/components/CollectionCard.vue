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
        <svg
          v-if="icon === 'lighting'"
          class="h-5 w-5"
          fill="none"
          stroke="currentColor"
          stroke-width="1.6"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M12 3v9m0 0l3.5 3.5M12 12 8.5 15.5M6 21h12"
          />
        </svg>
        <svg
          v-else-if="icon === 'energy'"
          class="h-5 w-5"
          fill="none"
          stroke="currentColor"
          stroke-width="1.6"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M13 2 3 14h9l-1 8 10-12h-9l1-8z"
          />
        </svg>
        <svg
          v-else-if="icon === 'security'"
          class="h-5 w-5"
          fill="none"
          stroke="currentColor"
          stroke-width="1.6"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M12 12a5 5 0 1 0 0-10 5 5 0 0 0 0 10zm0 0v9m-4 0h8"
          />
        </svg>
        <svg
          v-else
          class="h-5 w-5"
          fill="none"
          stroke="currentColor"
          stroke-width="1.6"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M12 5v14m-7-7h14"
          />
        </svg>
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
      <svg
        class="h-4 w-4"
        fill="none"
        stroke="currentColor"
        stroke-width="1.8"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          d="m13.5 4.5 7.5 7.5m0 0-7.5 7.5M21 12H3"
        />
      </svg>
    </span>
  </button>
</template>
