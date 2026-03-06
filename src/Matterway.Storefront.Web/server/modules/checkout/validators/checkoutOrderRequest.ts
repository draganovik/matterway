import type {
  CardPaymentInput,
  CheckoutOrderRequest,
  CheckoutOrderInput,
  PaymentAddress,
} from "../contracts/types"
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

function parseCardPayment(body: Record<string, unknown>): CardPaymentInput {
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
    cardNumber,
    expMonth,
    expYear,
    cvc,
    amount: asPositiveNumber(body, "amount", "Payment amount is invalid."),
  }
}

function parseAddress(body: Record<string, unknown>): PaymentAddress {
  return {
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
  }
}

function parseOrder(body: Record<string, unknown>): CheckoutOrderInput {
  const type = asRequiredString(body, "type", "Order type is required.")
  if (type.toLowerCase() !== "ecommerce") {
    throw new Error("Order type is invalid.")
  }

  const deliveryInfo = asObject(body.deliveryInfo, "Delivery info is required.")

  return {
    type: "Ecommerce",
    deliveryInfo: {
      country: asOptionalString(deliveryInfo, "country") || "Serbia",
      city: asRequiredString(
        deliveryInfo,
        "city",
        "Delivery city is required.",
      ),
      zipCode: asRequiredString(
        deliveryInfo,
        "zipCode",
        "Delivery zip code is required.",
      ),
      addressLine1: asRequiredString(
        deliveryInfo,
        "addressLine1",
        "Delivery address line 1 is required.",
      ),
      addressLine2: asOptionalString(deliveryInfo, "addressLine2"),
      contactPhone: asOptionalString(deliveryInfo, "contactPhone"),
    },
  }
}

export function validateCheckoutOrderRequest(
  data: unknown,
): CheckoutOrderRequest {
  const body = asObject(data, "Invalid request body.")
  const orderBody = asObject(body.order, "Order payload is required.")
  const paymentBody = asObject(body.payment, "Payment payload is required.")
  const addressBody = asObject(body.address, "Address payload is required.")
  const cardPaymentBody = asObject(
    paymentBody.cardPayment,
    "Card payment payload is required.",
  )

  const paymentType = asRequiredString(
    paymentBody,
    "type",
    "Payment type is required.",
  )
  if (paymentType.toLowerCase() !== "stripe") {
    throw new Error("Payment type is not supported.")
  }

  return {
    customerId: asRequiredString(
      body,
      "customerId",
      "Customer ID is required.",
    ),
    order: parseOrder(orderBody),
    payment: {
      type: "stripe",
      cardPayment: parseCardPayment(cardPaymentBody),
    },
    address: parseAddress(addressBody),
  }
}
