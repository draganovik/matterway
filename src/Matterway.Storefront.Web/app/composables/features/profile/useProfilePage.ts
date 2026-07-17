import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomersClient } from "~/composables/api/useCustomersClient"
import type {
  CustomerAddressResponse,
  PutSelfAddressRequest,
  SelfProfileResponse,
  SelfProfileUpdateRequest,
} from "~/types/customers"

type ProfileFormState = {
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string | null
}

function normalizeDate(value: string | null | undefined) {
  if (!value) return ""
  const raw = value.trim()
  if (!raw.length) return ""
  return raw.split("T")[0] || raw
}

export function useProfilePage() {
  const auth = useAuthSessionStore()
  const customersApi = useCustomersClient()

  const isLoading = ref(true)
  const profileSaving = ref(false)
  const addressSaving = ref(false)
  const profileLoaded = ref(false)

  const profileError = ref("")
  const profileSuccess = ref("")
  const addressError = ref("")
  const addressSuccess = ref("")
  const addressInfo = ref("")

  const profileForm = reactive<ProfileFormState>({
    systemUserId: "",
    firstName: "",
    lastName: "",
    birthDate: "",
    defaultAddressId: null,
  })

  const addressForm = reactive<PutSelfAddressRequest>({
    country: "Serbia",
    city: "",
    zipCode: "",
    addressLine1: "",
    addressLine2: "",
    contactPhone: "",
  })

  function applyProfile(value: SelfProfileResponse | null) {
    profileForm.systemUserId =
      value?.systemUserId?.trim() || auth.customerId.value || ""
    profileForm.firstName = value?.firstName?.trim() || ""
    profileForm.lastName = value?.lastName?.trim() || ""
    profileForm.birthDate = normalizeDate(value?.birthDate)
    profileForm.defaultAddressId = value?.defaultAddressId || null
  }

  function applyAddress(value: CustomerAddressResponse | null) {
    addressForm.country = "Serbia"
    addressForm.city = value?.city?.trim() || ""
    addressForm.zipCode = value?.zipCode?.trim() || ""
    addressForm.addressLine1 = value?.addressLine1?.trim() || ""
    addressForm.addressLine2 = value?.addressLine2?.trim() || ""
    addressForm.contactPhone = value?.contactPhone?.trim() || ""
  }

  function resetMessages() {
    profileError.value = ""
    profileSuccess.value = ""
    addressError.value = ""
    addressSuccess.value = ""
  }

  async function loadProfile() {
    const response = await customersApi.getSelfProfile()
    if (!response.ok || !response.data) {
      profileLoaded.value = false
      profileError.value =
        response.error || "Podaci o profilu trenutno nisu dostupni."
      if (!profileForm.systemUserId.trim()) {
        profileForm.systemUserId = auth.customerId.value || ""
      }
      return
    }

    profileLoaded.value = true
    profileError.value = ""
    applyProfile(response.data)
  }

  async function loadAddress() {
    const response = await customersApi.getSelfAddress()
    if (!response.ok) {
      if (response.status === 404) {
        addressInfo.value =
          "Nemate sačuvanu adresu. Popunite polja i sačuvajte adresu."
        applyAddress(null)
        return
      }

      addressError.value = response.error || "Učitavanje adrese nije uspelo."
      applyAddress(null)
      return
    }

    addressError.value = ""
    addressInfo.value = ""
    applyAddress(response.data || null)
  }

  async function loadData() {
    isLoading.value = true
    resetMessages()
    addressInfo.value = ""

    await Promise.all([loadProfile(), loadAddress()])

    isLoading.value = false
  }

  async function saveProfile() {
    if (profileSaving.value) return

    profileError.value = ""
    profileSuccess.value = ""

    if (!profileLoaded.value) {
      profileError.value =
        "Najpre osvežite stranicu da bismo učitali postojeće podatke."
      return
    }

    const systemUserId = profileForm.systemUserId.trim()
    const firstName = profileForm.firstName.trim()
    const lastName = profileForm.lastName.trim()
    const birthDate = normalizeDate(profileForm.birthDate)

    if (!systemUserId || !firstName || !lastName || !birthDate) {
      profileError.value = "Popunite sva obavezna polja u ličnim podacima."
      return
    }

    const payload: SelfProfileUpdateRequest = {
      systemUserId,
      firstName,
      lastName,
      birthDate,
      defaultAddressId: profileForm.defaultAddressId,
    }

    profileSaving.value = true
    const response = await customersApi.updateSelfProfile(payload)
    profileSaving.value = false

    if (!response.ok || !response.data) {
      profileError.value =
        response.error || "Čuvanje ličnih podataka nije uspelo."
      return
    }

    applyProfile(response.data)
    profileSuccess.value = "Lični podaci su sačuvani."
  }

  async function saveAddress() {
    if (addressSaving.value) return

    addressError.value = ""
    addressSuccess.value = ""

    const payload: PutSelfAddressRequest = {
      country: "Serbia",
      city: addressForm.city.trim(),
      zipCode: addressForm.zipCode.trim(),
      addressLine1: addressForm.addressLine1.trim(),
      addressLine2: addressForm.addressLine2.trim(),
      contactPhone: addressForm.contactPhone.trim(),
    }

    if (
      !payload.city ||
      !payload.zipCode ||
      !payload.addressLine1 ||
      !payload.addressLine2 ||
      !payload.contactPhone
    ) {
      addressError.value = "Popunite sva obavezna polja za adresu."
      return
    }

    addressSaving.value = true
    const response = await customersApi.putSelfAddress(payload)
    addressSaving.value = false

    if (!response.ok || !response.data) {
      addressError.value = response.error || "Čuvanje adrese nije uspelo."
      return
    }

    applyAddress(response.data)
    if (response.data.id) {
      profileForm.defaultAddressId = response.data.id
    }
    addressInfo.value = ""
    addressSuccess.value = "Adresa je uspešno sačuvana."
  }

  return {
    isLoading,
    profileSaving,
    addressSaving,
    profileForm,
    addressForm,
    profileError,
    profileSuccess,
    addressError,
    addressSuccess,
    addressInfo,
    loadData,
    saveProfile,
    saveAddress,
  }
}
