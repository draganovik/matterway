import { payWithStripe } from "../services/stripeService"
import type { H3Event } from "h3"
import type { CardPaymentInput, PaymentAddress } from "../types/payments"

const config = useRuntimeConfig()

type PaymentRequestBody = {
  cardNumber?: unknown
  expMonth?: unknown
  expYear?: unknown
  cvc?: unknown
  amount?: unknown
  receiverName?: unknown
  residence?: unknown
  street?: unknown
  city?: unknown
  zipCode?: unknown
  country?: unknown
  contactPhone?: unknown
  note?: unknown
  orderId?: unknown
  userId?: unknown
}

function asString(value: unknown) {
  return typeof value === "string" ? value.trim() : ""
}

function asNumber(value: unknown) {
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : NaN
}

function toPositiveAmount(value: unknown) {
  const parsed = asNumber(value)
  return parsed > 0 ? parsed : NaN
}

function isAddressValid(address: PaymentAddress) {
  return [
    address.receiverName,
    address.residence,
    address.street,
    address.city,
    address.zipCode,
  ].every((field) => field.length > 0)
}

async function parseBody(event: H3Event) {
  const rawBody = await readBody(event)
  if (typeof rawBody === "string") {
    const trimmed = rawBody.trim()
    if (!trimmed) return {} as PaymentRequestBody
    try {
      return JSON.parse(trimmed) as PaymentRequestBody
    } catch (error) {
      console.error("[payments] failed to parse JSON body", error)
      throw createError({
        statusCode: 400,
        message: "Neispravno telo zahteva",
      })
    }
  }
  if (rawBody && typeof rawBody === "object") {
    return rawBody as PaymentRequestBody
  }
  return {} as PaymentRequestBody
}

export default defineEventHandler(async (event) => {
  const body = await parseBody(event)
  const orderId = asString(body.orderId)
  if (!orderId) {
    throw createError({
      statusCode: 400,
      message: "OrderId je obavezan",
    })
  }
  const userId = asString(body.userId)
  if (!userId) {
    throw createError({
      statusCode: 400,
      message: "UserId je obavezan",
    })
  }

  const cardPayment: CardPaymentInput = {
    cardNumber: asString(body.cardNumber),
    expMonth: Math.trunc(asNumber(body.expMonth)),
    expYear: Math.trunc(asNumber(body.expYear)),
    cvc: asString(body.cvc),
    amount: toPositiveAmount(body.amount),
  }

  if (
    !cardPayment.cardNumber ||
    !cardPayment.cvc ||
    !Number.isFinite(cardPayment.amount) ||
    cardPayment.expMonth < 1 ||
    cardPayment.expMonth > 12 ||
    cardPayment.expYear < 2000
  ) {
    throw createError({
      statusCode: 400,
      message: "Podaci za kartično plaćanje nisu ispravni",
    })
  }

  const address: PaymentAddress = {
    receiverName: asString(body.receiverName),
    residence: asString(body.residence),
    street: asString(body.street),
    city: asString(body.city),
    zipCode: asString(body.zipCode),
    country: asString(body.country) || undefined,
    contactPhone: asString(body.contactPhone) || undefined,
    note: asString(body.note) || undefined,
  }

  if (!isAddressValid(address)) {
    throw createError({
      statusCode: 400,
      message: "Adresa nije ispravna",
    })
  }

  if (typeof config.stripeSecretKey !== "string" || !config.stripeSecretKey) {
    throw createError({
      statusCode: 500,
      message: "Nedostaje konfiguracija Stripe secret ključa.",
    })
  }

  const clientSecret = await payWithStripe(
    cardPayment,
    address,
    userId,
    orderId,
    config.stripeSecretKey,
  )
  return { clientSecret }
})
