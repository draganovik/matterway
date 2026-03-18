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
    "Broj kartice je obavezan.",
  ).replace(/\D/g, "")
  if (!/^\d{13,19}$/.test(cardNumber)) {
    throw new Error("Broj kartice nije ispravan.")
  }

  const expMonth = Math.trunc(
    asNumber(body, "expMonth", "Mesec isteka nije ispravan."),
  )
  if (expMonth < 1 || expMonth > 12) {
    throw new Error("Mesec isteka nije ispravan.")
  }

  const expYear = Math.trunc(
    asNumber(body, "expYear", "Godina isteka nije ispravna."),
  )
  if (expYear < 2000) {
    throw new Error("Godina isteka nije ispravna.")
  }

  const cvc = asRequiredString(body, "cvc", "CVC je obavezan.").replace(
    /\D/g,
    "",
  )
  if (!/^\d{3,4}$/.test(cvc)) {
    throw new Error("CVC nije ispravan.")
  }

  return {
    cardNumber,
    expMonth,
    expYear,
    cvc,
    amount: asPositiveNumber(body, "amount", "Iznos uplate nije ispravan."),
  }
}

function parseAddress(body: Record<string, unknown>): PaymentAddress {
  return {
    receiverName: asRequiredString(
      body,
      "receiverName",
      "Ime primaoca je obavezno.",
    ),
    residence: asRequiredString(body, "residence", "Adresa je obavezna."),
    street: asRequiredString(body, "street", "Ulica i broj su obavezni."),
    city: asRequiredString(body, "city", "Grad je obavezan."),
    zipCode: asRequiredString(body, "zipCode", "Poštanski broj je obavezan."),
    country: "Serbia",
    contactPhone: asOptionalString(body, "contactPhone"),
    note: asOptionalString(body, "note"),
  }
}

function parseOrder(body: Record<string, unknown>): CheckoutOrderInput {
  const type = asRequiredString(body, "type", "Tip porudžbine je obavezan.")
  if (type.toLowerCase() !== "ecommerce") {
    throw new Error("Tip porudžbine nije ispravan.")
  }

  const deliveryInfo = asObject(
    body.deliveryInfo,
    "Podaci za dostavu su obavezni.",
  )

  return {
    type: "Ecommerce",
    deliveryInfo: {
      country: "Serbia",
      city: asRequiredString(
        deliveryInfo,
        "city",
        "Grad za dostavu je obavezan.",
      ),
      zipCode: asRequiredString(
        deliveryInfo,
        "zipCode",
        "Poštanski broj za dostavu je obavezan.",
      ),
      addressLine1: asRequiredString(
        deliveryInfo,
        "addressLine1",
        "Adresa za dostavu je obavezna.",
      ),
      addressLine2: asOptionalString(deliveryInfo, "addressLine2"),
      contactPhone: asOptionalString(deliveryInfo, "contactPhone"),
    },
  }
}

export function validateCheckoutOrderRequest(
  data: unknown,
): CheckoutOrderRequest {
  const body = asObject(data, "Telo zahteva nije ispravno.")
  const orderBody = asObject(body.order, "Podaci o porudžbini su obavezni.")
  const paymentBody = asObject(body.payment, "Podaci o plaćanju su obavezni.")
  const addressBody = asObject(body.address, "Podaci o adresi su obavezni.")
  const cardPaymentBody = asObject(
    paymentBody.cardPayment,
    "Podaci o kartici su obavezni.",
  )

  const paymentType = asRequiredString(
    paymentBody,
    "type",
    "Tip plaćanja je obavezan.",
  )
  if (paymentType.toLowerCase() !== "stripe") {
    throw new Error("Izabrani tip plaćanja nije podržan.")
  }

  return {
    customerId: asRequiredString(body, "customerId", "ID kupca je obavezan."),
    order: parseOrder(orderBody),
    payment: {
      type: "stripe",
      cardPayment: parseCardPayment(cardPaymentBody),
    },
    address: parseAddress(addressBody),
  }
}
