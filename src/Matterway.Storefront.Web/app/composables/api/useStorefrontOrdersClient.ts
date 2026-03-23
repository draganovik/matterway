import type { ParsedCardExpiry } from "~/types/checkout"
import type { CheckoutAddress } from "~/types/customers"
import type { SalesOrder } from "~/types/sales"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"

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
}

function readErrorMessage(payload: unknown): string | undefined {
  if (typeof payload === "string" && payload.trim()) {
    return payload.trim()
  }

  if (!payload || typeof payload !== "object") return undefined

  const withMessage = payload as {
    message?: unknown
    statusMessage?: unknown
    statusText?: unknown
  }

  if (typeof withMessage.message === "string" && withMessage.message.trim()) {
    return withMessage.message.trim()
  }
  if (
    typeof withMessage.statusMessage === "string" &&
    withMessage.statusMessage.trim()
  ) {
    return withMessage.statusMessage.trim()
  }
  if (
    typeof withMessage.statusText === "string" &&
    withMessage.statusText.trim()
  ) {
    return withMessage.statusText.trim()
  }

  return undefined
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
  const auth = useAuthSessionStore()

  async function createOrder(
    payload: CreateCheckoutOrderRequest,
  ): Promise<CheckoutOrderResult> {
    if (!auth.isInitialized.value) {
      await auth.initialize()
    }

    const requestBody = JSON.stringify(buildCreateOrderPayload(payload))

    const runFetch = (authorization: string) =>
      $fetch.raw<CheckoutOrderApiResponse>("/api/storefront/checkout", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: authorization,
        },
        body: requestBody,
        ignoreResponseError: true,
      })

    let authorization = auth.getAccessToken()
    if (!authorization) {
      return noSessionResult()
    }

    let response
    try {
      response = await runFetch(authorization)
    } catch {
      return {
        ok: false,
        error: "Porudžbina trenutno ne može da se završi. Pokušajte ponovo.",
      }
    }
    if (response.status === 401) {
      await auth.refreshTokens()
      authorization = auth.getAccessToken()
      if (!authorization) {
        return noSessionResult()
      }
      try {
        response = await runFetch(authorization)
      } catch {
        return {
          ok: false,
          error: "Porudžbina trenutno ne može da se završi. Pokušajte ponovo.",
        }
      }
    }

    const body = response._data as CheckoutOrderApiResponse | null
    if (!response.ok) {
      return {
        ok: false,
        error: readErrorMessage(body),
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
