<script setup lang="ts">
import { useDelayedLoading } from "~/composables/workflows/state/useDelayedLoading"

const props = defineProps<{
  error?: string | boolean
  success?: string | boolean
  loading?: string | boolean
  empty?: string | boolean
}>()

function hasState(value?: string | boolean) {
  if (value === true) return true
  if (typeof value === "string") return value.trim().length > 0
  return false
}

function resolveMessage(value: string | boolean | undefined, fallback: string) {
  if (typeof value === "string" && value.trim().length > 0) return value
  return fallback
}

const showError = computed(() => hasState(props.error))
const showSuccess = computed(() => hasState(props.success))
const loadingRequested = computed(() => hasState(props.loading))
const delayedLoading = useDelayedLoading(loadingRequested)
const showEmpty = computed(
  () =>
    hasState(props.empty) &&
    !loadingRequested.value &&
    !showError.value &&
    !showSuccess.value,
)
const showLoading = computed(
  () =>
    delayedLoading.value &&
    !showError.value &&
    !showSuccess.value &&
    !showEmpty.value,
)

const loadingMessage = computed(() =>
  resolveMessage(props.loading, "Učitavanje..."),
)
const errorMessage = computed(() =>
  resolveMessage(props.error, "Došlo je do greške."),
)
const successMessage = computed(() =>
  resolveMessage(props.success, "Operacija je uspešno završena."),
)
const emptyMessage = computed(() =>
  resolveMessage(props.empty, "Nema dostupnih podataka."),
)
</script>

<template>
  <div class="space-y-3">
    <UAlert
      v-if="showError"
      icon="i-lucide-alert-triangle"
      color="error"
      variant="soft"
      :title="errorMessage"
    />
    <UAlert
      v-if="showLoading"
      icon="i-lucide-loader"
      color="neutral"
      variant="soft"
      :title="loadingMessage"
    />
    <UAlert
      v-if="showSuccess"
      icon="i-lucide-check-circle"
      color="success"
      variant="soft"
      :title="successMessage"
    />
    <UAlert
      v-if="showEmpty"
      icon="i-lucide-inbox"
      color="neutral"
      variant="soft"
      :title="emptyMessage"
    />
  </div>
</template>
