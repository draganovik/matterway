import type { ParsedCardExpiry } from "~/types/checkout"
import type { CheckoutAddress } from "~/types/customers"
import type { SalesOrder } from "~/types/sales"
import { useApiClient } from "~/composables/api/useApiClient"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { getApiErrorMessage } from "~/utils/apiErrors"

type CreateCheckoutOrderRequest = {
  customerId: string
  amount: number
  address: CheckoutAddress
  cardNumber: string
  cvc: string
  expiry: ParsedCardExpiry
}

type CheckoutOrderResult = {
  ok: boolean
  data?: SalesOrder
  error?: string
}

type CheckoutOrderApiResponse = {
  order?: SalesOrder
  message?: string
}

function noSessionResult(): CheckoutOrderResult {
  return {
    ok: false,
    error: "Sesija je istekla. Prijavite se ponovo.",
  }
}

function buildCreateOrderPayload(payload: CreateCheckoutOrderRequest) {
  return {
    customerId: payload.customerId,
    order: {
      type: "Ecommerce" as const,
      deliveryInfo: {
        country: "Serbia",
        city: payload.address.city,
        zipCode: payload.address.zipCode,
        addressLine1: payload.address.street,
        addressLine2: payload.address.residence || undefined,
        contactPhone: payload.address.contactPhone || undefined,
      },
    },
    payment: {
      type: "stripe" as const,
      cardPayment: {
        cardNumber: payload.cardNumber,
        expMonth: payload.expiry.month,
        expYear: payload.expiry.year,
        cvc: payload.cvc,
        amount: payload.amount,
      },
    },
    address: {
      receiverName: payload.address.receiverName,
      residence: payload.address.residence,
      street: payload.address.street,
      city: payload.address.city,
      zipCode: payload.address.zipCode,
      country: "Serbia",
      contactPhone: payload.address.contactPhone || undefined,
      note: payload.address.note || undefined,
    },
  }
}

export function useStorefrontOrdersClient() {
  const api = useApiClient()
  const auth = useAuthSessionStore()

  async function createOrder(
    payload: CreateCheckoutOrderRequest,
  ): Promise<CheckoutOrderResult> {
    await auth.initialize()

    if (!auth.getAccessToken()) {
      return noSessionResult()
    }

    const response = await api.requestRaw<CheckoutOrderApiResponse>(
      "/api/storefront/checkout",
      {
        method: "POST",
        body: buildCreateOrderPayload(payload),
      },
    )

    if (!response) {
      return {
        ok: false,
        error: "Porudžbina trenutno ne može da se završi. Pokušajte ponovo.",
      }
    }

    if (response.status === 401 && !auth.getAccessToken())
      return noSessionResult()

    const body = response._data as CheckoutOrderApiResponse | null
    if (!response.ok) {
      return {
        ok: false,
        error: getApiErrorMessage(body) || undefined,
      }
    }

    return {
      ok: true,
      data: body?.order,
    }
  }

  return {
    createOrder,
  }
}
