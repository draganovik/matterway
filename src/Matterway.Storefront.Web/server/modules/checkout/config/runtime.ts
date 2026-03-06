import type { SystemPaymentConfig } from "../contracts/types"

function asRequiredRuntimeValue(value: unknown, message: string): string {
  const normalized = typeof value === "string" ? value.trim() : ""
  if (normalized) {
    return normalized
  }

  throw createError({
    statusCode: 500,
    statusMessage: message,
  })
}

export function getServerSalesApiBaseUrl(): string {
  return asRequiredRuntimeValue(
    useRuntimeConfig().serverSalesApiBaseUrl,
    "Missing serverSalesApiBaseUrl runtime config",
  )
}

export function getStripeSecretKey(): string {
  return asRequiredRuntimeValue(
    useRuntimeConfig().stripeSecretKey,
    "Missing Stripe secret key configuration.",
  )
}

export function getSystemPaymentConfig(): SystemPaymentConfig {
  const config = useRuntimeConfig()
  const serverSalesApiBaseUrl = asRequiredRuntimeValue(
    config.serverSalesApiBaseUrl,
    "Missing serverSalesApiBaseUrl runtime config",
  )
  const systemAccessKey = asRequiredRuntimeValue(
    config.systemAccessKey,
    "Missing systemAccessKey runtime config",
  )

  return {
    serverSalesApiBaseUrl,
    systemAccessKey,
  }
}
