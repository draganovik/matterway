import type { H3Event } from "h3"
import { getSystemPaymentConfig } from "../../../modules/payments/config/runtime"
import { parseStripeChargeSucceeded } from "../../../modules/payments/mappers/stripeCharge"
import { registerStripePayment } from "../../../modules/payments/services/salesPayments"
import { validateEmptyQuery } from "../../../modules/payments/validators/query"
import { validateStripeWebhookPayload } from "../../../modules/payments/validators/stripeWebhookPayload"
import { getRequestTraceContext } from "../../../modules/shared/requestContext"

async function handleStripeWebhook(event: H3Event) {
  await getValidatedQuery(event, validateEmptyQuery)
  const stripeEvent = await readValidatedBody(
    event,
    validateStripeWebhookPayload,
  )

  if (stripeEvent.type !== "charge.succeeded") {
    console.log("Stripe event type not handled:", stripeEvent.type)
    setResponseStatus(event, 304, "Event type is not processed")
    return { success: true, ignored: true, eventType: stripeEvent.type }
  }

  const payment = await registerStripePayment(
    parseStripeChargeSucceeded(stripeEvent),
    getSystemPaymentConfig(),
    getRequestTraceContext(event),
  )

  return { success: true, message: "Payment registered", data: { payment } }
}

export default defineEventHandler(handleStripeWebhook)
