import type { CreatePaymentRequest } from "../contracts/types"
import {
  asNumber,
  asObject,
  asOptionalString,
  asRequiredString,
} from "./parsers"

function asPositiveNumber(
  body: Record<string, unknown>,
  field: string,
  message: string,
): number {
  const value = asNumber(body, field, message)
  if (value <= 0) {
    throw new Error(message)
  }
  return value
}

export function validateCreatePaymentRequest(
  data: unknown,
): CreatePaymentRequest {
  const body = asObject(data, "Invalid request body.")

  const cardNumber = asRequiredString(
    body,
    "cardNumber",
    "Card number is required.",
  ).replace(/\D/g, "")
  if (!/^\d{13,19}$/.test(cardNumber)) {
    throw new Error("Card number is invalid.")
  }

  const expMonth = Math.trunc(
    asNumber(body, "expMonth", "Expiration month is invalid."),
  )
  if (expMonth < 1 || expMonth > 12) {
    throw new Error("Expiration month is invalid.")
  }

  const expYear = Math.trunc(
    asNumber(body, "expYear", "Expiration year is invalid."),
  )
  if (expYear < 2000) {
    throw new Error("Expiration year is invalid.")
  }

  const cvc = asRequiredString(body, "cvc", "CVC is required.").replace(
    /\D/g,
    "",
  )
  if (!/^\d{3,4}$/.test(cvc)) {
    throw new Error("CVC is invalid.")
  }

  return {
    orderId: asRequiredString(body, "orderId", "Order ID is required."),
    userId: asRequiredString(body, "userId", "User ID is required."),
    cardPayment: {
      cardNumber,
      expMonth,
      expYear,
      cvc,
      amount: asPositiveNumber(body, "amount", "Payment amount is invalid."),
    },
    address: {
      receiverName: asRequiredString(
        body,
        "receiverName",
        "Receiver name is required.",
      ),
      residence: asRequiredString(body, "residence", "Residence is required."),
      street: asRequiredString(body, "street", "Street is required."),
      city: asRequiredString(body, "city", "City is required."),
      zipCode: asRequiredString(body, "zipCode", "Zip code is required."),
      country: asOptionalString(body, "country"),
      contactPhone: asOptionalString(body, "contactPhone"),
      note: asOptionalString(body, "note"),
    },
  }
}
