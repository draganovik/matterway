import Stripe from "stripe"
import {
  SpanKind,
  SpanStatusCode,
  trace,
  type Context,
  type Span,
} from "@opentelemetry/api"
import type {
  CardPaymentInput,
  PaymentAddress,
  StripePaymentSession,
} from "../contracts/types"

type StripeSpanOptions = {
  operation: string
  endpoint: string
  requestAttributes?: Record<string, string | number | boolean | undefined>
}

function setStripeResponseAttributes(span: Span, value: unknown) {
  if (!value || typeof value !== "object") return

  const withResponse = value as {
    id?: unknown
    object?: unknown
    lastResponse?: {
      requestId?: unknown
      statusCode?: unknown
    }
  }

  if (typeof withResponse.id === "string") {
    span.setAttribute("stripe.resource.id", withResponse.id)
  }
  if (typeof withResponse.object === "string") {
    span.setAttribute("stripe.resource.object", withResponse.object)
  }
  if (typeof withResponse.lastResponse?.requestId === "string") {
    span.setAttribute("stripe.request_id", withResponse.lastResponse.requestId)
  }
  if (typeof withResponse.lastResponse?.statusCode === "number") {
    span.setAttribute("http.status_code", withResponse.lastResponse.statusCode)
    return
  }

  span.setAttribute("http.status_code", 200)
}

async function recordStripeSpan<T>(
  options: StripeSpanOptions,
  parentContext: Context | undefined,
  fn: () => Promise<T>,
) {
  const tracer = trace.getTracer(
    process.env.OTEL_SERVICE_NAME || "mtw-storefront-web",
  )
  const span = tracer.startSpan(
    `Stripe ${options.operation}`,
    { kind: SpanKind.CLIENT },
    parentContext,
  )

  try {
    span.setAttribute("rpc.system", "stripe")
    span.setAttribute("stripe.sdk", "stripe-node")
    span.setAttribute("rpc.service", "stripe")
    span.setAttribute("rpc.method", options.operation)
    span.setAttribute("stripe.operation", options.operation)
    span.setAttribute("http.method", "POST")
    span.setAttribute("http.url", `https://api.stripe.com${options.endpoint}`)
    span.setAttribute("server.address", "api.stripe.com")
    span.setAttribute("stripe.endpoint", options.endpoint)

    for (const [key, value] of Object.entries(
      options.requestAttributes || {},
    )) {
      if (value !== undefined) {
        span.setAttribute(key, value)
      }
    }

    const result = await fn()
    setStripeResponseAttributes(span, result)
    return result
  } catch (error) {
    const message = error instanceof Error ? error.message : "stripe error"
    if (error && typeof error === "object") {
      const withStripeDetails = error as {
        type?: unknown
        code?: unknown
        requestId?: unknown
        statusCode?: unknown
      }
      if (typeof withStripeDetails.type === "string") {
        span.setAttribute("stripe.error.type", withStripeDetails.type)
      }
      if (typeof withStripeDetails.code === "string") {
        span.setAttribute("stripe.error.code", withStripeDetails.code)
      }
      if (typeof withStripeDetails.requestId === "string") {
        span.setAttribute("stripe.request_id", withStripeDetails.requestId)
      }
      if (typeof withStripeDetails.statusCode === "number") {
        span.setAttribute("http.status_code", withStripeDetails.statusCode)
      }
    }

    span.setStatus({ code: SpanStatusCode.ERROR, message })
    span.recordException(error as Error)
    throw error
  } finally {
    span.end()
  }
}

function buildStripeClient(secretKey: string) {
  return new Stripe(secretKey, {
    apiVersion: "2025-10-29.clover",
  })
}

function toStripeShippingAddress(address: PaymentAddress) {
  return {
    line1: address.street,
    line2: address.residence,
    city: address.city,
    postal_code: address.zipCode,
    country: "RS",
  }
}

function wrapStripeError(error: unknown, fallback: string) {
  console.error("[checkout] stripe request failed", error)
  const message = error instanceof Error ? error.message : fallback
  return createError({
    statusCode: 502,
    statusMessage: message || fallback,
  })
}

export async function createStripePaymentIntent(
  cardPayment: CardPaymentInput,
  address: PaymentAddress,
  userId: string,
  secretKey: string,
  parentContext?: Context,
): Promise<StripePaymentSession> {
  const amountInMinor = Math.round(cardPayment.amount * 100)
  const stripe = buildStripeClient(secretKey)

  try {
    const paymentMethod = await recordStripeSpan(
      {
        operation: "paymentMethods.create",
        endpoint: "/v1/payment_methods",
        requestAttributes: {
          "sales.customer_id": userId,
        },
      },
      parentContext,
      async () =>
        stripe.paymentMethods.create({
          type: "card",
          card: {
            number: cardPayment.cardNumber,
            exp_month: cardPayment.expMonth,
            exp_year: cardPayment.expYear,
            cvc: cardPayment.cvc,
          },
        }),
    )

    const paymentIntent = await recordStripeSpan(
      {
        operation: "paymentIntents.create",
        endpoint: "/v1/payment_intents",
        requestAttributes: {
          "sales.customer_id": userId,
          "payment.amount_minor": amountInMinor,
          "payment.currency": "RSD",
          "payment.method_id": paymentMethod.id,
        },
      },
      parentContext,
      async () =>
        stripe.paymentIntents.create({
          amount: amountInMinor,
          currency: "rsd",
          payment_method_types: ["card"],
          payment_method: paymentMethod.id,
          confirm: false,
          shipping: {
            name: address.receiverName,
            address: toStripeShippingAddress(address),
          },
          metadata: {
            client_id: userId,
            note: address.note || "",
          },
        }),
    )

    return {
      paymentIntentId: paymentIntent.id,
    }
  } catch (error) {
    throw wrapStripeError(error, "Pokretanje plaćanja nije uspelo.")
  }
}

export async function confirmStripePaymentIntent(
  paymentIntentId: string,
  orderId: string,
  secretKey: string,
  parentContext?: Context,
) {
  const stripe = buildStripeClient(secretKey)

  try {
    await recordStripeSpan(
      {
        operation: "paymentIntents.update",
        endpoint: `/v1/payment_intents/${paymentIntentId}`,
        requestAttributes: {
          "sales.order_id": orderId,
        },
      },
      parentContext,
      async () =>
        stripe.paymentIntents.update(paymentIntentId, {
          metadata: {
            order_id: orderId,
          },
        }),
    )

    const confirmedIntent = await recordStripeSpan(
      {
        operation: "paymentIntents.confirm",
        endpoint: `/v1/payment_intents/${paymentIntentId}/confirm`,
        requestAttributes: {
          "sales.order_id": orderId,
        },
      },
      parentContext,
      async () => stripe.paymentIntents.confirm(paymentIntentId),
    )

    if (
      confirmedIntent.status === "requires_payment_method" ||
      confirmedIntent.status === "canceled"
    ) {
      throw createError({
        statusCode: 402,
        statusMessage: "Potvrda plaćanja nije uspela.",
      })
    }
  } catch (error) {
    if (error && typeof error === "object" && "statusCode" in error) {
      throw error
    }
    throw wrapStripeError(error, "Potvrda plaćanja nije uspela.")
  }
}

export async function cancelStripePaymentIntent(
  paymentIntentId: string,
  secretKey: string,
  parentContext?: Context,
) {
  const stripe = buildStripeClient(secretKey)

  try {
    await recordStripeSpan(
      {
        operation: "paymentIntents.cancel",
        endpoint: `/v1/payment_intents/${paymentIntentId}/cancel`,
      },
      parentContext,
      async () => stripe.paymentIntents.cancel(paymentIntentId),
    )
  } catch (error) {
    console.error(
      `[checkout] failed to cancel stripe intent ${paymentIntentId}`,
      error,
    )
  }
}
