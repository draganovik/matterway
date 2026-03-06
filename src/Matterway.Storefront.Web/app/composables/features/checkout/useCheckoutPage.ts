import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"
import { useSalesClient } from "~/composables/api/useSalesClient"
import { useStorefrontPaymentsClient } from "~/composables/api/useStorefrontPaymentsClient"
import { useCheckoutAddressForm } from "./useCheckoutAddressForm"
import { useCheckoutPaymentForm } from "./useCheckoutPaymentForm"

export function useCheckoutPage() {
  const auth = useAuthSessionStore()
  const cart = useCartStore()
  const salesApi = useSalesClient()
  const paymentsApi = useStorefrontPaymentsClient()

  const {
    address,
    hasRequiredAddressFields,
    toDeliveryInfo,
    loadDefaultAddress,
  } = useCheckoutAddressForm()
  const {
    payment,
    currentYear,
    getCardDigits,
    getParsedExpiry,
    getValidationError,
  } = useCheckoutPaymentForm()

  const error = ref("")
  const loading = ref(false)

  const totalItems = computed(() => cart.totalItems.value)
  const totalPrice = computed(() => cart.totalPrice.value)
  const isCartEmpty = computed(() => totalItems.value === 0)

  async function initialize() {
    await cart.refreshFromRemote().catch(() => null)

    if (isCartEmpty.value) {
      await navigateTo("/cart")
      return
    }

    await loadDefaultAddress()
  }

  async function submitCheckout() {
    if (loading.value) return
    error.value = ""

    if (!auth.customerId.value) {
      error.value = "Korisnička sesija nije dostupna."
      return
    }

    loading.value = true
    try {
      if (isCartEmpty.value) {
        error.value = "Korpa je prazna."
        return
      }

      if (!hasRequiredAddressFields()) {
        error.value = "Popunite sva obavezna polja za dostavu."
        return
      }

      const paymentValidationError = getValidationError()
      if (paymentValidationError) {
        error.value = paymentValidationError
        return
      }

      const expiry = getParsedExpiry()
      if (!expiry) {
        error.value = "Unesite ispravan datum isteka kartice."
        return
      }

      const orderResponse = await salesApi.placeSelfOrder({
        customerId: auth.customerId.value,
        type: "Ecommerce",
        deliveryInfo: toDeliveryInfo(),
      })

      if (!orderResponse.ok || !orderResponse.data?.id) {
        error.value = orderResponse.error || "Kreiranje porudžbine nije uspelo."
        return
      }

      const paymentResult = await paymentsApi.createPayment({
        orderId: orderResponse.data.id,
        userId: auth.customerId.value,
        amount: orderResponse.data.totalAmount ?? cart.totalPrice.value,
        address,
        cardNumber: getCardDigits(),
        cvc: payment.cvc.replace(/\D/g, ""),
        expiry,
      })

      if (!paymentResult.ok) {
        error.value = paymentResult.error || "Plaćanje nije uspelo."
        return
      }

      await cart.clear()
      await navigateTo("/orders")
    } finally {
      loading.value = false
    }
  }

  return {
    address,
    payment,
    currentYear,
    totalItems,
    totalPrice,
    error,
    loading,
    initialize,
    submitCheckout,
  }
}
