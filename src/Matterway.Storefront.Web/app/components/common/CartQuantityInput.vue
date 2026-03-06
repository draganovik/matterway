<script setup lang="ts">
import { useCartStore } from "~/composables/stores/useCartStore"

type ControlSize = "xs" | "sm" | "md" | "lg" | "xl"
const maxQuantity = 100

const props = withDefaults(
  defineProps<{
    articleId: string
    quantity: number
    disabled?: boolean
    showRemove?: boolean
    size?: ControlSize
  }>(),
  {
    disabled: false,
    showRemove: false,
    size: "md",
  },
)

const emit = defineEmits<{
  remove: []
}>()

const cart = useCartStore()
const inputValue = ref<number | null>(1)
const loading = ref(false)
const error = ref("")
let syncTimeout: ReturnType<typeof setTimeout> | undefined

const widthClass = computed(() => {
  switch (props.size) {
    case "xs":
      return "w-20"
    case "sm":
      return "w-24"
    case "lg":
      return "w-32"
    case "xl":
      return "w-36"
    default:
      return "w-28"
  }
})

function normalizeQuantity(value: unknown) {
  const parsed = Number(value)
  if (!Number.isFinite(parsed) || parsed < 1) return 1
  return Math.min(maxQuantity, Math.trunc(parsed))
}

const resolvedQuantity = computed(() => normalizeQuantity(props.quantity))
const resolvedInputQuantity = computed(() =>
  normalizeQuantity(inputValue.value),
)

watch(
  () => props.quantity,
  (quantity) => {
    if (loading.value) return
    inputValue.value = normalizeQuantity(quantity)
  },
  { immediate: true },
)

function clearPendingSync() {
  if (!syncTimeout) return
  clearTimeout(syncTimeout)
  syncTimeout = undefined
}

async function applyQuantity(nextValue?: number | null) {
  clearPendingSync()

  const nextQuantity = normalizeQuantity(nextValue ?? inputValue.value)
  inputValue.value = nextQuantity
  error.value = ""

  if (nextQuantity === resolvedQuantity.value) return

  loading.value = true

  try {
    await cart.setQuantity(props.articleId, nextQuantity)
  } catch {
    inputValue.value = resolvedQuantity.value
    error.value = "Ažuriranje količine nije uspelo."
  } finally {
    loading.value = false
  }
}

function queueQuantityUpdate(nextValue: number | null) {
  if (nextValue != null && nextValue > maxQuantity) {
    return
  }

  inputValue.value = nextValue
  error.value = ""

  if (nextValue == null) {
    clearPendingSync()
    return
  }

  clearPendingSync()
  syncTimeout = setTimeout(() => {
    void applyQuantity(nextValue)
  }, 250)
}

function handleBlur() {
  void applyQuantity(inputValue.value)
}

onBeforeUnmount(() => {
  clearPendingSync()
})
</script>

<template>
  <div class="flex flex-col items-start gap-1">
    <div class="flex items-center gap-2">
      <UInputNumber
        :model-value="inputValue"
        :min="1"
        :max="maxQuantity"
        :step="1"
        step-snapping
        variant="outline"
        :size="props.size"
        :disabled="props.disabled || loading"
        :decrement-disabled="resolvedInputQuantity <= 1"
        :increment-disabled="resolvedInputQuantity >= maxQuantity"
        :class="widthClass"
        :ui="{ base: 'text-center font-semibold tabular-nums' }"
        @update:model-value="queueQuantityUpdate"
        @blur="handleBlur"
      />

      <UButton
        v-if="props.showRemove"
        color="error"
        variant="ghost"
        icon="i-lucide-trash"
        square
        :size="props.size"
        :disabled="props.disabled || loading"
        @click="emit('remove')"
      />
    </div>

    <p v-if="error" class="text-error text-xs">
      {{ error }}
    </p>
  </div>
</template>
