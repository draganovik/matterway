<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"
import { useCart } from "~/composables/useCart"
import { useCustomersApi } from "~/composables/useCustomersApi"
import { useSalesApi } from "~/composables/useSalesApi"
import { formatMoney } from "~/utils/formatters"
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
  expMonth: new Date().getMonth() + 1,
  expYear: new Date().getFullYear(),
  cvc: "",
})

const error = ref("")
const loading = ref(false)

const isCartEmpty = computed(() => cart.totalItems.value === 0)

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

  if (isCartEmpty.value) {
    error.value = "Korpa je prazna."
    return
  }

  if (!auth.customerId.value) {
    error.value = "Korisnička sesija nije dostupna."
    return
  }

  if (!isAddressValid()) {
    error.value = "Popunite sva obavezna polja za dostavu."
    return
  }

  loading.value = true
  try {
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
        expMonth: payment.expMonth,
        expYear: payment.expYear,
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
      <UCard class="border-default bg-default border">
        <template #header>
          <h2 class="text-lg font-semibold">Dostava</h2>
        </template>

        <div class="grid gap-4 sm:grid-cols-2">
          <UInput
            v-model="address.receiverName"
            label="Ime primaoca"
            placeholder="npr. Petar Petrović"
            required
          />
          <UInput
            v-model="address.contactPhone"
            label="Telefon"
            placeholder="npr. +381 64 123 4567"
          />
          <UInput
            v-model="address.street"
            label="Ulica"
            placeholder="npr. Bulevar oslobođenja 15"
            required
          />
          <UInput
            v-model="address.residence"
            label="Dodatak adrese"
            placeholder="npr. Stan 12, 3. sprat"
            required
          />
          <UInput
            v-model="address.city"
            label="Grad"
            placeholder="npr. Novi Sad"
            required
          />
          <UInput
            v-model="address.zipCode"
            label="Poštanski broj"
            placeholder="npr. 21000"
            required
          />
          <UInput
            v-model="address.country"
            label="Država"
            placeholder="npr. Srbija"
            class="sm:col-span-2"
          />
          <UTextarea
            v-model="address.note"
            label="Napomena"
            placeholder="Napomena za dostavu (opciono)"
            class="sm:col-span-2"
          />
        </div>

        <template #footer>
          <h3 class="mb-3 text-base font-semibold">Plaćanje karticom</h3>
          <div
            class="border-default bg-elevated mb-4 flex items-center gap-2 rounded-lg border px-3 py-2 text-xs"
          >
            <UIcon name="i-lucide-shield-check" class="text-primary h-4 w-4" />
            <span class="text-muted">
              Bezbedna SSL naplata. Podaci o kartici se ne čuvaju.
            </span>
          </div>
          <div class="grid gap-4 sm:grid-cols-3">
            <UInput
              v-model="payment.cardNumber"
              label="Broj kartice"
              icon="i-lucide-credit-card"
              placeholder="1234 5678 9012 3456"
              autocomplete="cc-number"
              inputmode="numeric"
              maxlength="23"
              :ui="{ base: 'font-mono tracking-[0.08em]' }"
              class="sm:col-span-3"
              required
            />
            <UInput
              v-model.number="payment.expMonth"
              type="number"
              min="1"
              max="12"
              label="Mesec"
              placeholder="MM"
              autocomplete="cc-exp-month"
              inputmode="numeric"
              required
            />
            <UInput
              v-model.number="payment.expYear"
              type="number"
              min="2024"
              label="Godina"
              placeholder="GGGG"
              autocomplete="cc-exp-year"
              inputmode="numeric"
              required
            />
            <UInput
              v-model="payment.cvc"
              label="CVC"
              placeholder="123"
              autocomplete="cc-csc"
              inputmode="numeric"
              maxlength="4"
              required
            />
          </div>
        </template>
      </UCard>

      <UCard class="border-default bg-default h-fit border">
        <template #header>
          <h2 class="text-base font-semibold">Pregled porudžbine</h2>
        </template>

        <div class="space-y-3 text-sm">
          <div class="flex items-center justify-between">
            <span class="text-muted">Stavke</span>
            <span>{{ cart.totalItems.value }}</span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-muted">Iznos</span>
            <span>{{ formatMoney(cart.totalPrice.value) }}</span>
          </div>
        </div>

        <template #footer>
          <UButton
            color="primary"
            block
            :loading="loading"
            @click="submitCheckout"
          >
            {{ loading ? "Obrada..." : "Plati odmah" }}
          </UButton>
        </template>
      </UCard>
    </div>
  </div>
</template>
