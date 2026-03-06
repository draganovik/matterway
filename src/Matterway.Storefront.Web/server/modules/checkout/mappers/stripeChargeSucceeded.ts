import type {
  StripeChargeSucceeded,
  StripeEventWebhookPayload,
} from "../contracts/types"

export function parseStripeChargeSucceeded(
  event: StripeEventWebhookPayload,
): StripeChargeSucceeded {
  const stripeObject = event.data?.object
  if (!stripeObject) {
    throw createError({
      statusCode: 400,
      statusMessage: "Missing Stripe charge object",
    })
  }

  const orderId = stripeObject.metadata?.order_id?.trim()
  if (!orderId) {
    throw createError({
      statusCode: 400,
      statusMessage: "Missing Stripe order_id metadata",
    })
  }

  if (
    typeof stripeObject.amount !== "number" ||
    !Number.isFinite(stripeObject.amount)
  ) {
    throw createError({
      statusCode: 400,
      statusMessage: "Missing Stripe amount in event payload",
    })
  }

  return {
    orderId,
    amount: stripeObject.amount / 100,
    createdAt:
      typeof stripeObject.created === "number"
        ? new Date(stripeObject.created * 1000)
        : new Date(),
  }
}
