<script setup lang="ts">
import { useCart } from "~/composables/useCart"

type ControlSize = "xs" | "sm" | "md" | "lg" | "xl"

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

const cart = useCart()
const isOpen = ref(false)
const loading = ref(false)
const quantityInput = ref<string | number>("")
const error = ref("")

const resolvedQuantity = computed(() => {
  const parsed = Number(props.quantity)
  if (!Number.isFinite(parsed) || parsed < 0) return 0
  return Math.trunc(parsed)
})

function openEditor() {
  if (props.disabled) return
  quantityInput.value = String(Math.max(1, resolvedQuantity.value))
  error.value = ""
  isOpen.value = true
}

function parseQuantity(value: string | number) {
  const trimmed = String(value ?? "").trim()
  if (!/^\d+$/.test(trimmed)) return null
  const parsed = Number(trimmed)
  if (!Number.isInteger(parsed) || parsed < 1) return null
  return parsed
}

async function applyQuantity() {
  if (loading.value) return

  const nextQuantity = parseQuantity(quantityInput.value)
  if (!nextQuantity) {
    error.value = "Unesite količinu 1 ili više."
    return
  }

  if (nextQuantity === resolvedQuantity.value) {
    isOpen.value = false
    return
  }

  loading.value = true
  error.value = ""

  try {
    // Close immediately for deterministic UX; cart state updates are synced right after.
    isOpen.value = false
    await cart.setQuantity(props.articleId, nextQuantity)
  } catch {
    isOpen.value = true
    error.value = "Ažuriranje količine nije uspelo."
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex items-center gap-2">
    <UButton
      color="primary"
      variant="soft"
      :size="props.size"
      :disabled="props.disabled"
      trailing-icon="i-lucide-pencil"
      @click="openEditor"
    >
      <span
        class="leading-none font-semibold tabular-nums"
        :class="props.size === 'lg' ? 'text-lg' : ''"
      >
        {{ resolvedQuantity }}
      </span>
    </UButton>

    <UButton
      v-if="props.showRemove"
      color="error"
      variant="ghost"
      icon="i-lucide-trash"
      square
      :size="props.size"
      :disabled="props.disabled"
      @click="emit('remove')"
    />
  </div>

  <UModal v-model:open="isOpen" :ui="{ content: 'sm:max-w-md' }">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-base font-semibold">Izmena količine</h3>
        <p class="text-muted text-sm">Unesite željeni broj artikala.</p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <UFormField label="Količina" required>
          <UInput
            v-model="quantityInput"
            type="number"
            :size="props.size"
            min="1"
            step="1"
            inputmode="numeric"
            placeholder="npr. 2"
            class="w-full"
            required
          />
        </UFormField>

        <StatusMessages v-if="error" :error="error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-end gap-2">
        <UButton
          type="button"
          variant="ghost"
          :size="props.size"
          :disabled="loading"
          @click="isOpen = false"
        >
          Odustani
        </UButton>
        <UButton
          type="button"
          color="primary"
          :size="props.size"
          :loading="loading"
          @click.stop="applyQuantity"
        >
          Potvrdi
        </UButton>
      </div>
    </template>
  </UModal>
</template>
