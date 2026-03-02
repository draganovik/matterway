<script setup lang="ts">
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

const showLoading = computed(() => hasState(props.loading))
const showError = computed(() => hasState(props.error))
const showSuccess = computed(() => hasState(props.success))
const showEmpty = computed(() => hasState(props.empty))

const loadingMessage = computed(() =>
  resolveMessage(props.loading, "Loading..."),
)
const errorMessage = computed(() =>
  resolveMessage(props.error, "Something went wrong."),
)
const successMessage = computed(() =>
  resolveMessage(props.success, "Operation completed successfully."),
)
const emptyMessage = computed(() =>
  resolveMessage(props.empty, "No data available."),
)
</script>

<template>
  <div class="space-y-3">
    <UAlert
      v-if="showLoading"
      icon="i-lucide-loader"
      color="neutral"
      variant="soft"
      :title="loadingMessage"
    />
    <UAlert
      v-if="showError"
      icon="i-lucide-alert-triangle"
      color="error"
      variant="soft"
      :title="errorMessage"
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
