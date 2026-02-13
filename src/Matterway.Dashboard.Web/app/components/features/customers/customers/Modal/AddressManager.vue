<script setup lang="ts">
import { useCustomersApi } from "~/composables/useCustomersApi"
import type {
  CustomerAddressResponse,
  PutCustomerAddressRequest,
} from "~/types/customers"
import { useModalCloseReset } from "~/composables/useModalCloseReset"
import { useRequestState } from "~/composables/useRequestState"

type AddressMode = "view" | "manage"
type AddressForm = PutCustomerAddressRequest

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

const api = useCustomersApi()
const loadState = useRequestState()
const saveState = useRequestState()

const mode = ref<AddressMode>("view")
const address = ref<CustomerAddressResponse | null>(null)
const notFound = ref(false)
const form = ref<AddressForm>({
  country: "",
  city: "",
  zipCode: "",
  addressLine1: "",
  addressLine2: "",
  contactPhone: "",
})

const displayName = computed(
  () => props.customerName.trim() || "Selected customer",
)

function resetForm() {
  form.value = {
    country: "",
    city: "",
    zipCode: "",
    addressLine1: "",
    addressLine2: "",
    contactPhone: "",
  }
}

function applyAddressToForm(value: CustomerAddressResponse | null) {
  form.value = {
    country: value?.country || "",
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
    loadState.error = "Select a customer first."
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

    loadState.error = result.error || "Unable to reveal customer address."
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
    saveState.error = "Select a customer first."
    return
  }

  const payload: PutCustomerAddressRequest = {
    country: form.value.country.trim(),
    city: form.value.city.trim(),
    zipCode: form.value.zipCode.trim(),
    addressLine1: form.value.addressLine1.trim(),
    addressLine2: form.value.addressLine2.trim(),
    contactPhone: form.value.contactPhone.trim(),
  }

  if (
    !payload.country ||
    !payload.city ||
    !payload.zipCode ||
    !payload.addressLine1 ||
    !payload.addressLine2 ||
    !payload.contactPhone
  ) {
    saveState.error = "All address fields are required."
    return
  }

  saveState.loading = true
  const result = await api.putAddressByCustomer(customerId, payload)
  saveState.loading = false

  if (!result.ok || !result.data) {
    saveState.error = result.error || "Unable to save customer address."
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
          {{ mode === "manage" ? "Manage Address" : "Customer Address" }}
        </h3>
        <p class="text-muted text-sm">
          {{
            mode === "manage"
              ? `Update address details for ${displayName}.`
              : `Revealed address details for ${displayName}.`
          }}
        </p>
      </div>
    </template>

    <template #body>
      <div v-if="mode === 'view'" class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing customer address.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Address not found"
          description="This customer does not have a saved address yet."
        />

        <dl v-else-if="address" class="grid gap-3 sm:grid-cols-2">
          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Country</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.country || "-" }}
            </dd>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">City</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.city || "-" }}
            </dd>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Zip Code</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.zipCode || "-" }}
            </dd>
          </div>

          <div class="border-default/70 rounded-md border px-3 py-2">
            <dt class="text-muted text-xs">Contact Phone</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.contactPhone || "-" }}
            </dd>
          </div>

          <div
            class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
          >
            <dt class="text-muted text-xs">Address Line 1</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.addressLine1 || "-" }}
            </dd>
          </div>

          <div
            class="border-default/70 rounded-md border px-3 py-2 sm:col-span-2"
          >
            <dt class="text-muted text-xs">Address Line 2</dt>
            <dd class="text-foreground mt-1 text-sm font-medium">
              {{ address.addressLine2 || "-" }}
            </dd>
          </div>
        </dl>
      </div>

      <div v-else class="space-y-4">
        <div class="grid gap-4 md:grid-cols-2">
          <UFormField label="Country" required>
            <UInput
              v-model="form.country"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>

          <UFormField label="City" required>
            <UInput
              v-model="form.city"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>

          <UFormField label="Zip Code" required>
            <UInput
              v-model="form.zipCode"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>

          <UFormField label="Contact Phone" required>
            <UInput
              v-model="form.contactPhone"
              :disabled="saveState.loading || !canEdit"
            />
          </UFormField>
        </div>

        <UFormField label="Address Line 1" required>
          <UInput
            v-model="form.addressLine1"
            :disabled="saveState.loading || !canEdit"
          />
        </UFormField>

        <UFormField label="Address Line 2" required>
          <UInput
            v-model="form.addressLine2"
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
          Manage Address
        </UButton>
        <UButton variant="ghost" @click="isOpen = false">Close</UButton>
      </div>

      <div v-else class="flex w-full justify-between gap-2">
        <UButton
          variant="ghost"
          icon="i-lucide-arrow-left"
          :disabled="saveState.loading"
          @click="backToView"
        >
          Back
        </UButton>

        <div class="flex items-center gap-2">
          <UButton
            variant="ghost"
            :disabled="saveState.loading"
            @click="isOpen = false"
          >
            Cancel
          </UButton>
          <UButton
            color="primary"
            :loading="saveState.loading"
            :disabled="!canEdit"
            @click="saveAddress"
          >
            Save Address
          </UButton>
        </div>
      </div>
    </template>
  </UModal>
</template>
