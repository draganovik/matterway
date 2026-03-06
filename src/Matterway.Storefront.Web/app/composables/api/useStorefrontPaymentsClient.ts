import type { ParsedCardExpiry } from "~/types/checkout"
import type { CheckoutAddress } from "~/types/customers/address"

type CreateCheckoutPaymentRequest = {
  orderId: string
  userId: string
  amount: number
  address: CheckoutAddress
  cardNumber: string
  cvc: string
  expiry: ParsedCardExpiry
}

type CheckoutPaymentResult = {
  ok: boolean
  error?: string
}

export function useStorefrontPaymentsClient() {
  async function createPayment(
    payload: CreateCheckoutPaymentRequest,
  ): Promise<CheckoutPaymentResult> {
    const response = await fetch("/api/v1/payments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        cardNumber: payload.cardNumber,
        expMonth: payload.expiry.month,
        expYear: payload.expiry.year,
        cvc: payload.cvc,
        amount: payload.amount,
        ...payload.address,
        orderId: payload.orderId,
        userId: payload.userId,
      }),
    })

    if (response.ok) {
      return { ok: true }
    }

    const body = await response.json().catch(() => null)
    return {
      ok: false,
      error: typeof body?.message === "string" ? body.message : undefined,
    }
  }

  return {
    createPayment,
  }
}
