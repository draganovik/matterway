<script setup lang="ts">
import { useCustomersClient } from "~/composables/api/useCustomersClient"
import type {
  CustomerAddressResponse,
  PutCustomerAddressRequest,
} from "~/types/customers"
import { useModalCloseReset } from "~/composables/workflows/modal/useModalCloseReset"
import { useRequestState } from "~/composables/workflows/state/useRequestState"

type AddressMode = "view" | "manage"
type AddressForm = Omit<PutCustomerAddressRequest, "country">
const COUNTRY = "Srbija"

const props = withDefaults(
  defineProps<{
    customerId?: string | null
    customerName?: string
    canEdit?: boolean
  }>(),
  {
    customerId: null,
    customerName: "",
    canEdit: false,
  },
)

const emit = defineEmits<{
  saved: [address: CustomerAddressResponse]
}>()

const isOpen = defineModel<boolean>("open", { required: true })

const api = useCustomersClient()
const loadState = useRequestState()
const saveState = useRequestState()

const mode = ref<AddressMode>("view")
const address = ref<CustomerAddressResponse | null>(null)
const notFound = ref(false)
const form = ref<AddressForm>({
  city: "",
  zipCode: "",
  addressLine1: "",
  addressLine2: "",
  contactPhone: "",
})

const displayName = computed(
  () => props.customerName.trim() || "Izabrani kupac",
)

function resetForm() {
  form.value = {
    city: "",
    zipCode: "",
    addressLine1: "",
    addressLine2: "",
    contactPhone: "",
  }
}

function applyAddressToForm(value: CustomerAddressResponse | null) {
  form.value = {
    city: value?.city || "",
    zipCode: value?.zipCode || "",
    addressLine1: value?.addressLine1 || "",
    addressLine2: value?.addressLine2 || "",
    contactPhone: value?.contactPhone || "",
  }
}

function resetModalState() {
  mode.value = "view"
  loadState.loading = false
  loadState.error = ""
  saveState.loading = false
  saveState.error = ""
  saveState.success = ""
  address.value = null
  notFound.value = false
  resetForm()
}

async function loadAddress() {
  const customerId = props.customerId?.trim()
  if (!customerId) {
    loadState.error = "Najpre izaberite kupca."
    address.value = null
    notFound.value = false
    return
  }

  loadState.loading = true
  loadState.error = ""
  saveState.error = ""
  address.value = null
  notFound.value = false

  const result = await api.getAddressByCustomer(customerId)

  loadState.loading = false

  if (!result.ok) {
    if (result.status === 404) {
      notFound.value = true
      return
    }

    loadState.error = result.error || "Učitavanje adrese kupca nije uspelo."
    return
  }

  if (!result.data) {
    notFound.value = true
    return
  }

  address.value = result.data
}

function beginManage() {
  if (!props.canEdit) return
  applyAddressToForm(address.value)
  saveState.error = ""
  mode.value = "manage"
}

function backToView() {
  mode.value = "view"
  saveState.error = ""
  saveState.success = ""
  void loadAddress()
}

async function saveAddress() {
  saveState.error = ""
  saveState.success = ""

  if (!props.canEdit) return

  const customerId = props.customerId?.trim()
  if (!customerId) {
    saveState.error = "Najpre izaberite kupca."
    return
  }

  const payload: PutCustomerAddressRequest = {
    country: COUNTRY,
    city: form.value.city.trim(),
    zipCode: form.value.zipCode.trim(),
    addressLine1: form.value.addressLine1.trim(),
    addressLine2: form.value.addressLine2.trim(),
    contactPhone: form.value.contactPhone.trim(),
  }

  if (
    !payload.city ||
    !payload.zipCode ||
    !payload.addressLine1 ||
    !payload.addressLine2 ||
    !payload.contactPhone
  ) {
    saveState.error = "Sva polja adrese su obavezna."
    return
  }

  saveState.loading = true
  const result = await api.putAddressByCustomer(customerId, payload)
  saveState.loading = false

  if (!result.ok || !result.data) {
    saveState.error = result.error || "Čuvanje adrese kupca nije uspelo."
    return
  }

  emit("saved", result.data)
  address.value = result.data
  notFound.value = false
  mode.value = "view"
}

useModalCloseReset({
  isOpen,
  watchSources: [toRef(props, "customerId")],
  onCloseReset: resetModalState,
  onOpen: async () => {
    mode.value = "view"
    await loadAddress()
  },
})
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          {{ mode === "manage" ? "Uređivanje adrese" : "Adresa kupca" }}
        </h3>
        <p class="text-muted text-sm">
          {{
            mode === "manage"
              ? `Ažurirajte adresu za kupca ${displayName}.`
              : `Pregled adrese za kupca ${displayName}.`
          }}
        </p>
      </div>
    </template>

    <template #body>
      <div v-if="mode === 'view'" class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Učitavanje adrese kupca.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Adresa nije pronađena"
          description="Ovaj kupac još nema sačuvanu adresu."
        />

        <dl v-else-if="address" class="grid gap-3 sm:grid-cols-2">
          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Država</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.country || "-" }}
            </dd>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Grad</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.city || "-" }}
            </dd>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Poštanski broj</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.zipCode || "-" }}
            </dd>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Kontakt telefon</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.contactPhone || "-" }}
            </dd>
          </div>

          <div
            class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
          >
            <dt class="text-muted text-xs">Adresa 1</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.addressLine1 || "-" }}
            </dd>
          </div>

          <div
            class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
          >
            <dt class="text-muted text-xs">Adresa 2</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.addressLine2 || "-" }}
            </dd>
          </div>
        </dl>
      </div>

      <div v-else class="space-y-4">
        <div class="grid gap-4 md:grid-cols-2">
          <UFormField label="Država" required>
            <UInput :model-value="COUNTRY" disabled readonly />
          </UFormField>

          <UFormField label="Grad" required>
            <UInput
              v-model="form.city"
              placeholder="Beograd"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>

          <UFormField label="Poštanski broj" required>
            <UInput
              v-model="form.zipCode"
              placeholder="11000"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>

          <UFormField label="Kontakt telefon" required>
            <UInput
              v-model="form.contactPhone"
              placeholder="+381641234567"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>
        </div>

        <UFormField label="Adresa 1" required>
          <UInput
            v-model="form.addressLine1"
            placeholder="Bulevar oslobodjenja 15"
            :disabled="saveState.loading || !canEdit"
          />
        </UFormField>

        <UFormField label="Adresa 2" required>
          <UInput
            v-model="form.addressLine2"
            placeholder="Stan 12"
            :disabled="saveState.loading || !canEdit"
          />
        </UFormField>

        <StatusMessages :error="saveState.error" />
      </div>
    </template>

    <template #footer>
      <div v-if="mode === 'view'" class="flex w-full justify-between gap-2">
        <UButton
          color="primary"
          variant="soft"
          :disabled="!canEdit || loadState.loading"
          @click="beginManage"
        >
          Uredi adresu
        </UButton>
        <UButton variant="ghost" @click="isOpen = false">Zatvori</UButton>
      </div>

      <div v-else class="flex w-full justify-between gap-2">
        <UButton
          variant="ghost"
          icon="i-lucide-arrow-left"
          :disabled="saveState.loading"
          @click="backToView"
        >
          Nazad
        </UButton>

        <div class="flex items-center gap-2">
          <UButton
            variant="ghost"
            :disabled="saveState.loading"
            @click="isOpen = false"
          >
            Otkaži
          </UButton>
          <UButton
            color="primary"
            :loading="saveState.loading"
            :disabled="!canEdit"
            @click="saveAddress"
          >
            {{ saveState.loading ? "Čuvanje adrese" : "Sačuvaj adresu" }}
          </UButton>
        </div>
      </div>
    </template>
  </UModal>
</template>
