import type { SystemPaymentConfig } from "../contracts/types"

export function getStripeSecretKey(): string {
  const config = useRuntimeConfig()
  const stripeSecretKey =
    typeof config.stripeSecretKey === "string"
      ? config.stripeSecretKey.trim()
      : ""

  if (!stripeSecretKey) {
    throw createError({
      statusCode: 500,
      message: "Missing Stripe secret key configuration.",
    })
  }

  return stripeSecretKey
}

export function getSystemPaymentConfig(): SystemPaymentConfig {
  const config = useRuntimeConfig()
  const serverSalesApiBaseUrl =
    typeof config.serverSalesApiBaseUrl === "string"
      ? config.serverSalesApiBaseUrl.trim()
      : ""
  const systemAccessKey =
    typeof config.systemAccessKey === "string"
      ? config.systemAccessKey.trim()
      : ""

  if (!serverSalesApiBaseUrl) {
    throw createError({
      statusCode: 500,
      statusMessage: "Missing serverSalesApiBaseUrl runtime config",
    })
  }

  if (!systemAccessKey) {
    throw createError({
      statusCode: 500,
      statusMessage: "Missing systemAccessKey runtime config",
    })
  }

  return {
    serverSalesApiBaseUrl,
    systemAccessKey,
  }
}
