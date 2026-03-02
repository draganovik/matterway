<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"
import { useCart } from "~/composables/useCart"
import { useCustomersApi } from "~/composables/useCustomersApi"
import { useSalesApi } from "~/composables/useSalesApi"
import type { CheckoutAddress } from "~/types/customers/address"

const auth = useAuthSession()
const cart = useCart()
const customersApi = useCustomersApi()
const salesApi = useSalesApi()

const address = reactive<CheckoutAddress>({
  receiverName: "",
  residence: "",
  street: "",
  city: "",
  zipCode: "",
  country: "Srbija",
  contactPhone: "",
  note: "",
})

const payment = reactive({
  cardNumber: "",
  expMonth: "",
  expYear: "",
  cvc: "",
})

const error = ref("")
const loading = ref(false)
const currentYear = new Date().getFullYear()

const isCartEmpty = computed(() => cart.totalItems.value === 0)

function getCardDigits() {
  return payment.cardNumber.replace(/\D/g, "")
}

function isLuhnValid(cardDigits: string) {
  let sum = 0
  let shouldDouble = false

  for (let i = cardDigits.length - 1; i >= 0; i -= 1) {
    let digit = Number(cardDigits[i])
    if (!Number.isFinite(digit)) return false

    if (shouldDouble) {
      digit *= 2
      if (digit > 9) digit -= 9
    }

    sum += digit
    shouldDouble = !shouldDouble
  }

  return sum % 10 === 0
}

function isCardNumberValid() {
  const cardDigits = getCardDigits()
  if (!/^\d{13,19}$/.test(cardDigits)) return false
  return isLuhnValid(cardDigits)
}

function getParsedExpiry() {
  const monthRaw = String(payment.expMonth ?? "").trim()
  const yearRaw = String(payment.expYear ?? "").trim()

  if (!/^\d{1,2}$/.test(monthRaw)) return null
  if (!/^\d{4}$/.test(yearRaw)) return null

  const month = Number(monthRaw)
  const year = Number(yearRaw)
  if (!Number.isInteger(month) || !Number.isInteger(year)) return null
  if (month < 1 || month > 12) return null
  if (year < 2000) return null

  const now = new Date()
  const nowMonth = now.getMonth() + 1
  const nowYear = now.getFullYear()
  if (year < nowYear) return null
  if (year === nowYear && month < nowMonth) return null

  return { month, year }
}

function isExpiryValid() {
  return getParsedExpiry() !== null
}

function isCvcValid() {
  return /^\d{3,4}$/.test(payment.cvc)
}

function getPaymentValidationError() {
  if (!isCardNumberValid()) {
    return "Unesite ispravan broj kartice."
  }
  if (!isExpiryValid()) {
    return "Unesite ispravan datum isteka kartice."
  }
  if (!isCvcValid()) {
    return "Unesite ispravan CVC kod."
  }
  return null
}

function formatCardNumber(value: string) {
  const digits = value.replace(/\D/g, "").slice(0, 19)
  return digits.replace(/(.{4})/g, "$1 ").trim()
}

watch(
  () => payment.cardNumber,
  (value) => {
    const formatted = formatCardNumber(value)
    if (formatted !== value) {
      payment.cardNumber = formatted
    }
  },
)

watch(
  () => payment.cvc,
  (value) => {
    const normalized = value.replace(/\D/g, "").slice(0, 4)
    if (normalized !== value) {
      payment.cvc = normalized
    }
  },
)

watch(
  () => payment.expMonth,
  (value) => {
    const normalized = String(value ?? "")
      .replace(/\D/g, "")
      .slice(0, 2)
    if (normalized !== value) {
      payment.expMonth = normalized
    }
  },
)

watch(
  () => payment.expYear,
  (value) => {
    const normalized = String(value ?? "")
      .replace(/\D/g, "")
      .slice(0, 4)
    if (normalized !== value) {
      payment.expYear = normalized
    }
  },
)

function isAddressValid() {
  return [
    address.receiverName,
    address.residence,
    address.street,
    address.city,
    address.zipCode,
  ].every((item) => item.trim().length > 0)
}

async function loadDefaultAddress() {
  const response = await customersApi.getSelfAddress()
  if (!response.ok || !response.data) return

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

    if (!isAddressValid()) {
      error.value = "Popunite sva obavezna polja za dostavu."
      return
    }

    const paymentValidationError = getPaymentValidationError()
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
      deliveryInfo: {
        country: address.country || "Srbija",
        city: address.city,
        zipCode: address.zipCode,
        addressLine1: address.street,
        addressLine2: address.residence || undefined,
        contactPhone: address.contactPhone || undefined,
      },
    })

    if (!orderResponse.ok || !orderResponse.data?.id) {
      error.value = orderResponse.error || "Kreiranje porudžbine nije uspelo."
      return
    }

    const paymentResponse = await fetch("/api/payments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        cardNumber: payment.cardNumber.replace(/\D/g, ""),
        expMonth: expiry.month,
        expYear: expiry.year,
        cvc: payment.cvc.replace(/\D/g, ""),
        amount: orderResponse.data.totalAmount ?? cart.totalPrice.value,
        ...address,
        orderId: orderResponse.data.id,
        userId: auth.customerId.value,
      }),
    })

    if (!paymentResponse.ok) {
      const body = await paymentResponse.json().catch(() => null)
      error.value = body?.message || "Plaćanje nije uspelo."
      return
    }

    await cart.clear()
    await navigateTo("/orders")
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  if (isCartEmpty.value) {
    await navigateTo("/cart")
    return
  }
  await loadDefaultAddress()
})
</script>

<template>
  <div class="space-y-6">
    <UCard class="border-default bg-elevated/60 border">
      <div class="flex items-center justify-between gap-3">
        <div>
          <p class="text-primary text-xs tracking-[0.3em] uppercase">
            Plaćanje
          </p>
          <h1 class="text-2xl font-semibold">Završi porudžbinu</h1>
        </div>
        <UBadge color="primary" variant="soft">
          {{ cart.totalItems.value }} stavki
        </UBadge>
      </div>
    </UCard>

    <StatusMessages v-if="error" :error="error" />

    <div class="grid gap-6 lg:grid-cols-[1fr_22rem]">
      <div class="space-y-6">
        <CheckoutDeliveryPanel v-model="address" />
        <CheckoutPaymentPanel v-model="payment" :current-year="currentYear" />
      </div>

      <CheckoutOrderActionPanel
        :total-items="cart.totalItems.value"
        :total-price="cart.totalPrice.value"
        :loading="loading"
        @submit="submitCheckout"
      />
    </div>
  </div>
</template>
