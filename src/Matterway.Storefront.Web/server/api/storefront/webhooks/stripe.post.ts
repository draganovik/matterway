import { getSystemPaymentConfig } from "../../../modules/checkout/config/runtime"
import { parseStripeChargeSucceeded } from "../../../modules/checkout/mappers/stripeChargeSucceeded"
import { registerSalesPayment } from "../../../modules/checkout/services/salesPaymentClient"
import { validateNoQueryParams } from "../../../modules/checkout/validators/emptyQuery"
import { validateStripeWebhookPayload } from "../../../modules/checkout/validators/stripeWebhookPayload"
import { getRequestTraceContext } from "../../../modules/shared/requestContext"

export default defineEventHandler(async (event) => {
  await getValidatedQuery(event, validateNoQueryParams)
  const stripeEvent = await readValidatedBody(
    event,
    validateStripeWebhookPayload,
  )

  if (stripeEvent.type !== "charge.succeeded") {
    console.log("Stripe event type not handled:", stripeEvent.type)
    setResponseStatus(event, 304, "Event type is not processed")
    return { success: true, ignored: true, eventType: stripeEvent.type }
  }

  const payment = await registerSalesPayment(
    parseStripeChargeSucceeded(stripeEvent),
    getSystemPaymentConfig(),
    getRequestTraceContext(event),
  )

  return { success: true, message: "Payment registered", data: { payment } }
})
