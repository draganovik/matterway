<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type { CreateDiscountRequest } from "~/types/catalog"
import { normalizeCode } from "~/utils/normalization"

type DiscountForm = {
  code: string
  percentage: number | null
  validFrom: string
  validTo: string
}

const { canEdit = false } = defineProps<{
  canEdit?: boolean
}>()

const emit = defineEmits<{
  created: [discount: CreateDiscountRequest]
}>()

const isOpen = defineModel<boolean>("open", { required: true })

const api = useCatalogClient()
const createState = useRequestState()
const form = ref<DiscountForm>(emptyForm())
const selectedArticleCodes = ref<string[]>([])

function getDefaultDateTimeLocal() {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, "0")
  const day = String(now.getDate()).padStart(2, "0")
  const hour = String(now.getHours()).padStart(2, "0")
  const minute = String(now.getMinutes()).padStart(2, "0")
  return `${year}-${month}-${day}T${hour}:${minute}`
}

function emptyForm(): DiscountForm {
  return {
    code: "",
    percentage: null,
    validFrom: getDefaultDateTimeLocal(),
    validTo: "",
  }
}

function toIsoDateTime(value: string) {
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return null
  return parsed.toISOString()
}

function resetForm() {
  form.value = emptyForm()
  selectedArticleCodes.value = []
  createState.error = ""
}

useResetOnModalOpen(isOpen, resetForm)

async function createDiscount() {
  createState.error = ""
  if (!canEdit) return

  const code = normalizeCode(form.value.code)
  const percentage = form.value.percentage
  const validFrom = toIsoDateTime(form.value.validFrom)
  const validToInput = form.value.validTo.trim()
  const validTo = validToInput ? toIsoDateTime(validToInput) : null
  const articleCodes = [...new Set(selectedArticleCodes.value)]

  if (!code) {
    createState.error = "Kod je obavezan."
    return
  }
  if (percentage == null || percentage < 0.01 || percentage > 1) {
    createState.error = "Procenat mora biti između 0,01 i 1."
    return
  }
  if (!validFrom) {
    createState.error = "Polje „Važi od“ mora imati ispravan datum i vreme."
    return
  }
  if (validToInput && !validTo) {
    createState.error = "Polje „Važi do“ mora imati ispravan datum i vreme."
    return
  }
  if (validTo && new Date(validTo) < new Date(validFrom)) {
    createState.error =
      "Polje „Važi do“ mora biti veće ili jednako polju „Važi od“."
    return
  }
  if (!articleCodes.length) {
    createState.error = "Izaberite bar jedan artikal."
    return
  }

  const payload: CreateDiscountRequest = {
    code,
    percentage,
    validFrom,
    validTo,
    articleCodes,
  }

  createState.loading = true
  const result = await api.createDiscounts(payload)
  createState.loading = false

  if (!result.ok) {
    createState.error = result.error || "Kreiranje popusta nije uspelo."
    return
  }

  emit("created", payload)
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Novi popust</h3>
        <p class="text-muted text-sm">
          Kreirajte kod popusta i izaberite artikle na koje se primenjuje.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <CatalogDiscountsInformationForm
          v-model="form"
          :disabled="!canEdit || createState.loading"
        />
        <CatalogDiscountsArticleSelectionPanel
          v-model="selectedArticleCodes"
          :can-edit="canEdit && !createState.loading"
        />
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
          :disabled="!canEdit"
          @click="createDiscount"
        >
          {{ createState.loading ? "Kreiranje popusta" : "Kreiraj popust" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
