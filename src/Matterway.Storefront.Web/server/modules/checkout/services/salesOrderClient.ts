import type { Context } from "@opentelemetry/api"
import { fetchWithTrace } from "../../shared/fetchWithTrace"
import type { CheckoutOrderInput, SalesOrderResponse } from "../contracts/types"

function extractErrorMessage(payload: unknown, fallback: string) {
  if (typeof payload === "string" && payload.trim()) {
    return payload.trim()
  }

  if (!payload || typeof payload !== "object") {
    return fallback
  }

  const candidate = payload as {
    detail?: unknown
    title?: unknown
    message?: unknown
  }

  if (typeof candidate.detail === "string" && candidate.detail.trim()) {
    return candidate.detail.trim()
  }
  if (typeof candidate.title === "string" && candidate.title.trim()) {
    return candidate.title.trim()
  }
  if (typeof candidate.message === "string" && candidate.message.trim()) {
    return candidate.message.trim()
  }

  return fallback
}

export async function createSalesOrder(
  order: CheckoutOrderInput,
  authorization: string,
  salesApiBaseUrl: string,
  requestContext?: Context,
): Promise<SalesOrderResponse> {
  const response = await fetchWithTrace(
    `${salesApiBaseUrl}/api/self/v1/orders`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
        Authorization: authorization,
      },
      body: JSON.stringify(order),
    },
    requestContext,
  )

  const contentType = response.headers.get("content-type") || ""
  const payload = contentType.includes("json")
    ? await response.json().catch(() => null)
    : await response.text().catch(() => null)

  if (!response.ok) {
    throw createError({
      statusCode: response.status,
      statusMessage: extractErrorMessage(payload, "Order creation failed."),
    })
  }

  if (!payload || typeof payload !== "object") {
    throw createError({
      statusCode: 500,
      statusMessage: "Order response payload is invalid.",
    })
  }

  return payload as SalesOrderResponse
}
