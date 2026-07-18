<script setup lang="ts">
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { useSalesClient } from "~/composables/api/useSalesClient"
import type { OrderStatusType } from "~/types/sales"

const props = withDefaults(
  defineProps<{
    orderId?: string | null
    orderLabel?: string
    canEdit?: boolean
  }>(),
  {
    orderId: null,
    orderLabel: "",
    canEdit: false,
  },
)

const emit = defineEmits<{
  (event: "created"): void
}>()

const isOpen = defineModel<boolean>("open", { required: true })
const api = useSalesClient()
const createState = useRequestState()

const statusOptions: Array<{ label: string; value: OrderStatusType }> = [
  { label: "Obrada", value: "Processing" },
  { label: "Rezervisano", value: "Reserved" },
  { label: "Dostava", value: "Delivery" },
  { label: "Završeno", value: "Completed" },
  { label: "Otkazano", value: "Cancelled" },
]

const status = ref<OrderStatusType | "">("")
const note = ref("")

const displayLabel = computed(
  () =>
    props.orderLabel?.trim() || props.orderId?.trim() || "Izabrana porudžbina",
)

const canSubmit = computed(
  () =>
    props.canEdit &&
    Boolean(props.orderId?.trim()) &&
    Boolean(String(status.value).trim()) &&
    !createState.loading,
)

function resetForm() {
  status.value = ""
  note.value = ""
  createState.error = ""
  createState.success = ""
}

useResetOnModalOpen(isOpen, resetForm)

async function createStatus() {
  createState.error = ""
  if (!props.canEdit) return

  const orderId = props.orderId?.trim()
  if (!orderId) {
    createState.error = "Najpre izaberite porudžbinu."
    return
  }

  const statusValue = String(status.value).trim()
  if (!statusValue) {
    createState.error = "Status je obavezan."
    return
  }

  createState.loading = true

  const result = await api.addOrderStatus(orderId, {
    status: statusValue as OrderStatusType,
    note: note.value.trim() || null,
  })

  createState.loading = false

  if (!result.ok) {
    createState.error = result.error || "Kreiranje statusa nije uspelo."
    return
  }

  emit("created")
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-highlighted text-base font-semibold">Novi status</h3>
        <p class="text-muted text-sm">
          Dodajte novu promenu statusa za porudžbinu {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField label="Status" required>
          <USelectMenu
            v-model="status"
            :items="statusOptions"
            value-key="value"
            label-key="label"
            :disabled="!canEdit || createState.loading"
            placeholder="Izaberite status"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Napomena">
          <UTextarea
            v-model="note"
            :rows="3"
            placeholder="Opciona napomena za ovu promenu statusa."
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <StatusMessages :error="createState.error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="createState.loading"
          @click="isOpen = false"
        >
          Otkaži
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canSubmit"
          @click="createStatus"
        >
          {{ createState.loading ? "Kreiranje statusa" : "Kreiraj status" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
