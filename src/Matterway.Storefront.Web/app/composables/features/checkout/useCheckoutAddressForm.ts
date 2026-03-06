import type { CheckoutAddress } from "~/types/customers/address"
import { useCustomersClient } from "~/composables/api/useCustomersClient"

function createInitialAddress(): CheckoutAddress {
  return {
    receiverName: "",
    residence: "",
    street: "",
    city: "",
    zipCode: "",
    country: "Srbija",
    contactPhone: "",
    note: "",
  }
}

export function useCheckoutAddressForm() {
  const customersApi = useCustomersClient()
  const address = reactive<CheckoutAddress>(createInitialAddress())

  function hasRequiredAddressFields() {
    return [
      address.receiverName,
      address.residence,
      address.street,
      address.city,
      address.zipCode,
    ].every((item) => item.trim().length > 0)
  }

  function toDeliveryInfo() {
    return {
      country: address.country || "Srbija",
      city: address.city,
      zipCode: address.zipCode,
      addressLine1: address.street,
      addressLine2: address.residence || undefined,
      contactPhone: address.contactPhone || undefined,
    }
  }

  async function loadDefaultAddress() {
    const response = await customersApi.getSelfAddress()
    if (!response.ok || !response.data) {
      return
    }

    if (!address.street && response.data.addressLine1) {
      address.street = response.data.addressLine1
    }
    if (!address.residence && response.data.addressLine2) {
      address.residence = response.data.addressLine2
    }
    if (!address.city && response.data.city) {
      address.city = response.data.city
    }
    if (!address.zipCode && response.data.zipCode) {
      address.zipCode = response.data.zipCode
    }
    if (!address.country && response.data.country) {
      address.country = response.data.country
    }
    if (!address.contactPhone && response.data.contactPhone) {
      address.contactPhone = response.data.contactPhone
    }
  }

  return {
    address,
    hasRequiredAddressFields,
    toDeliveryInfo,
    loadDefaultAddress,
  }
}
