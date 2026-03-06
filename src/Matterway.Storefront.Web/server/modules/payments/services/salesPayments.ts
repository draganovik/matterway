import type { Context } from "@opentelemetry/api"
import { fetchWithTelemetry } from "../../../../app/utils/telemetry"
import type {
  StripeChargeSucceeded,
  SystemPaymentConfig,
} from "../contracts/types"

const systemAccessKeyHeaderName = "X-System-Access-Key"

export async function registerStripePayment(
  charge: StripeChargeSucceeded,
  config: SystemPaymentConfig,
  requestContext?: Context,
) {
  const response = await fetchWithTelemetry(
    `${config.serverSalesApiBaseUrl}/api/system/v1/payments`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
        [systemAccessKeyHeaderName]: config.systemAccessKey,
      },
      body: JSON.stringify({
        orderId: charge.orderId,
        provider: "Stripe",
        referenceId: charge.referenceId,
        amount: charge.amount,
        status: "Charged",
        createdAt: charge.createdAt,
      }),
    },
    requestContext,
  )

  if (!response.ok) {
    console.error("[stripe] failed to register payment", await response.text())
    throw createError({
      statusCode: 500,
      statusMessage: "Failed to register payment",
    })
  }

  return await response.json()
}
