import {
  getSystemPaymentConfig,
  getStripeSecretKey,
} from "../../modules/checkout/config/runtime"
import { createSalesOrder } from "../../modules/checkout/services/salesOrderClient"
import {
  cancelStripePaymentIntent,
  confirmStripePaymentIntent,
  createStripePaymentIntent,
} from "../../modules/checkout/services/stripePaymentIntent"
import { validateCheckoutOrderRequest } from "../../modules/checkout/validators/checkoutOrderRequest"
import { validateNoQueryParams } from "../../modules/checkout/validators/emptyQuery"
import { getRequestTraceContext } from "../../modules/shared/requestContext"

function normalizeCheckoutError(error: unknown) {
  const candidate =
    error && typeof error === "object"
      ? (error as {
          statusCode?: unknown
          statusMessage?: unknown
          message?: unknown
        })
      : {}

  const statusCode =
    typeof candidate.statusCode === "number" && candidate.statusCode >= 400
      ? candidate.statusCode
      : 500

  const internalMessage =
    (typeof candidate.statusMessage === "string" && candidate.statusMessage) ||
    (typeof candidate.message === "string" && candidate.message) ||
    "Poručivanje nije uspelo."

  let publicMessage = internalMessage
  if (statusCode >= 500) {
    publicMessage =
      "Porudžbina trenutno ne može da se završi. Pokušajte ponovo."
  } else if (statusCode === 401 || statusCode === 403) {
    publicMessage =
      "Vaša sesija više nije važeća. Prijavite se ponovo i pokušajte još jednom."
  } else if (statusCode === 404) {
    publicMessage =
      "Neke podatke za poručivanje nije moguće pronaći. Osvežite stranicu i pokušajte ponovo."
  }

  return {
    statusCode,
    internalMessage,
    publicMessage,
  }
}

export default defineEventHandler(async (event) => {
  await getValidatedQuery(event, validateNoQueryParams)
  const orderRequest = await readValidatedBody(
    event,
    validateCheckoutOrderRequest,
  )

  const authorization = event.node.req.headers.authorization?.trim()
  if (!authorization) {
    throw createError({
      statusCode: 401,
      statusMessage: "Nedostaje Authorization zaglavlje.",
    })
  }

  const requestContext = getRequestTraceContext(event)
  const stripeSecretKey = getStripeSecretKey()
  const systemPaymentConfig = getSystemPaymentConfig()

  let paymentIntentId: string | null = null

  try {
    const stripeSession = await createStripePaymentIntent(
      orderRequest.payment.cardPayment,
      orderRequest.address,
      orderRequest.customerId,
      stripeSecretKey,
      requestContext,
    )

    paymentIntentId = stripeSession.paymentIntentId

    const createdOrder = await createSalesOrder(
      orderRequest.order,
      authorization,
      systemPaymentConfig,
      requestContext,
    )

    if (!createdOrder.id) {
      throw createError({
        statusCode: 500,
        statusMessage: "Odgovor porudžbine ne sadrži ID porudžbine.",
      })
    }

    if (typeof createdOrder.totalAmount === "number") {
      const amountMismatch =
        Math.abs(
          createdOrder.totalAmount - orderRequest.payment.cardPayment.amount,
        ) > 0.009

      if (amountMismatch) {
        throw createError({
          statusCode: 400,
          statusMessage: "Iznos uplate se ne poklapa sa iznosom porudžbine.",
        })
      }
    }

    await confirmStripePaymentIntent(
      stripeSession.paymentIntentId,
      createdOrder.id,
      stripeSecretKey,
      requestContext,
    )

    return {
      order: createdOrder,
    }
  } catch (error) {
    const normalizedError = normalizeCheckoutError(error)

    if (paymentIntentId) {
      await cancelStripePaymentIntent(
        paymentIntentId,
        stripeSecretKey,
        requestContext,
      )
    }

    console.error("[checkout] order submission failed", {
      statusCode: normalizedError.statusCode,
      message: normalizedError.internalMessage,
      error,
    })

    event.node.res.statusCode = normalizedError.statusCode
    event.node.res.statusMessage = normalizedError.publicMessage

    return {
      message: normalizedError.publicMessage,
    }
  }
})
