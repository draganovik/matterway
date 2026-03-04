import type { StripeEventWebhookPayload } from "../../types/payments"

const config = useRuntimeConfig()
const systemAccessKeyHeaderName = "X-System-Access-Key"

export default defineEventHandler(async (event) => {
  const stripeEvent = (await readBody(event)) as StripeEventWebhookPayload

  if (stripeEvent.type !== "charge.succeeded") {
    console.log("Stripe event type not handled:", stripeEvent.type)
    setResponseStatus(event, 304, "Event type is not processed")
    return { success: true, ignored: true, eventType: stripeEvent.type }
  }

  const payment = await postPayment(stripeEvent)
  if (!payment) {
    throw createError({
      statusCode: 500,
      statusMessage: "Failed to register payment",
    })
  }

  return { success: true, message: "Payment registered", data: { payment } }
})

const postPayment = async (event: StripeEventWebhookPayload) => {
  if (!config.serverSalesApiBaseUrl) {
    console.error("[stripe] missing serverSalesApiBaseUrl runtime config")
    return null
  }

  if (!config.systemAccessKey) {
    console.error("[stripe] missing systemAccessKey runtime config")
    return null
  }

  const stripeObject = event.data?.object
  const orderId = stripeObject?.metadata?.order_id
  if (!orderId) {
    console.error("[stripe] missing order_id metadata")
    return null
  }

  if (typeof stripeObject.amount !== "number") {
    console.error("[stripe] missing amount in event payload")
    return null
  }

  const amount = stripeObject.amount / 100
  const referenceId = stripeObject.metadata?.reference_id ?? createReferenceId()

  const createdAt =
    typeof stripeObject.created === "number"
      ? new Date(stripeObject.created * 1000)
      : new Date()

  const response = await fetch(
    `${config.serverSalesApiBaseUrl}/api/system/v1/payments`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
        [systemAccessKeyHeaderName]: config.systemAccessKey,
      },
      body: JSON.stringify({
        orderId,
        provider: "Stripe",
        referenceId,
        amount,
        status: "Charged",
        createdAt,
      }),
    },
  )

  if (!response.ok) {
    console.error("[stripe] failed to register payment", await response.text())
    return null
  }

  return await response.json()
}

const createReferenceId = () => {
  const segment = () => Math.floor(1000 + Math.random() * 9000).toString()
  return `${segment()}-${segment()}-${segment()}-${segment()}`
}
