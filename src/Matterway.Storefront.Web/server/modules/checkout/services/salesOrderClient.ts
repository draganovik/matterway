import type { Context } from "@opentelemetry/api"
import { fetchWithTrace } from "../../shared/fetchWithTrace"
import type {
  CheckoutOrderInput,
  SalesOrderResponse,
  SystemPaymentConfig,
} from "../contracts/types"

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

function getPublicOrderErrorMessage(statusCode: number) {
  switch (statusCode) {
    case 400:
      return "Porudžbina ne može da se kreira. Proverite korpu i podatke za dostavu pa pokušajte ponovo."
    case 401:
    case 403:
      return "Vaša sesija više nije važeća. Prijavite se ponovo i pokušajte još jednom."
    case 404:
      return "Neke podatke za poručivanje nije moguće pronaći. Osvežite stranicu i pokušajte ponovo."
    default:
      return "Porudžbina trenutno ne može da se završi. Pokušajte ponovo."
  }
}

export async function createSalesOrder(
  order: CheckoutOrderInput,
  authorization: string,
  systemConfig: SystemPaymentConfig,
  requestContext?: Context,
): Promise<SalesOrderResponse> {
  let response: Response
  try {
    response = await fetchWithTrace(
      `${systemConfig.serverSalesApiBaseUrl}/api/system/v1/orders`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          accept: "application/json",
          Authorization: authorization,
          "X-System-Access-Key": systemConfig.systemAccessKey,
        },
        body: JSON.stringify(order),
      },
      requestContext,
    )
  } catch (error) {
    console.error(
      "[checkout] sales order request failed before receiving a response",
      {
        error,
      },
    )
    throw createError({
      statusCode: 502,
      statusMessage: getPublicOrderErrorMessage(502),
    })
  }

  const contentType = response.headers.get("content-type") || ""
  const payload = contentType.includes("json")
    ? await response.json().catch(() => null)
    : await response.text().catch(() => null)

  if (!response.ok) {
    const internalMessage = extractErrorMessage(
      payload,
      "Kreiranje porudžbine nije uspelo.",
    )
    console.error("[checkout] sales order request failed", {
      statusCode: response.status,
      message: internalMessage,
      payload,
    })
    throw createError({
      statusCode: response.status,
      statusMessage: getPublicOrderErrorMessage(response.status),
    })
  }

  if (!payload || typeof payload !== "object") {
    console.error("[checkout] sales order response payload is invalid", {
      payload,
    })
    throw createError({
      statusCode: 502,
      statusMessage: getPublicOrderErrorMessage(502),
    })
  }

  return payload as SalesOrderResponse
}
