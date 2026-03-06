import type { Context } from "@opentelemetry/api"
import type { H3Event } from "h3"

type OtelEventContext = H3Event["context"] & { __otelRequestContext?: Context }

export function getRequestTraceContext(event: H3Event): Context | undefined {
  return (event.context as OtelEventContext).__otelRequestContext
}
