import type { StripeEventWebhookPayload } from "../contracts/types"
import {
  asObject,
  asOptionalNumber,
  asRequiredString,
  isRecord,
} from "./parsers"

function asMetadata(
  value: unknown,
): Record<string, string | undefined> | undefined {
  if (!isRecord(value)) {
    return undefined
  }

  return Object.fromEntries(
    Object.entries(value).map(([key, entry]) => [
      key,
      typeof entry === "string" ? entry.trim() || undefined : undefined,
    ]),
  )
}

export function validateStripeWebhookPayload(
  data: unknown,
): StripeEventWebhookPayload {
  const body = asObject(data, "Invalid webhook body.")
  const type = asRequiredString(body, "type", "Stripe event type is required.")

  const eventData = body.data
  if (eventData === undefined || eventData === null) {
    return { type }
  }

  const parsedData = asObject(eventData, "Stripe event data is invalid.")
  const rawObject = parsedData.object
  if (rawObject === undefined || rawObject === null) {
    return { type, data: {} }
  }

  const parsedObject = asObject(rawObject, "Stripe event object is invalid.")
  const amount = asOptionalNumber(
    parsedObject.amount,
    "Stripe amount must be numeric.",
  )
  const created = asOptionalNumber(
    parsedObject.created,
    "Stripe created must be numeric.",
  )

  return {
    type,
    data: {
      object: {
        amount,
        created: created === undefined ? undefined : Math.trunc(created),
        metadata: asMetadata(parsedObject.metadata),
      },
    },
  }
}
