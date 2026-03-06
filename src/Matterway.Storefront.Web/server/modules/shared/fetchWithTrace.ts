import {
  context,
  trace,
  type Context,
  type SpanContext,
} from "@opentelemetry/api"

const correlationHeaderName = "x-correlation-id"
const invalidTraceId = "00000000000000000000000000000000"
const invalidSpanId = "0000000000000000"

function toTraceParentValue(
  traceId: string,
  spanId: string,
  traceFlags: number,
) {
  return `00-${traceId}-${spanId}-${traceFlags.toString(16).padStart(2, "0")}`
}

function hasValidSpanContext(
  spanContext: SpanContext | undefined,
): spanContext is SpanContext {
  if (!spanContext) return false
  return (
    spanContext.traceId.length === 32 &&
    spanContext.spanId.length === 16 &&
    spanContext.traceId !== invalidTraceId &&
    spanContext.spanId !== invalidSpanId
  )
}

function applyTraceHeaders(headers: Headers, requestContext?: Context) {
  const sourceContext = requestContext ?? context.active()
  const spanContext = trace.getSpanContext(sourceContext)
  if (!hasValidSpanContext(spanContext)) return

  headers.set(
    "traceparent",
    toTraceParentValue(
      spanContext.traceId,
      spanContext.spanId,
      spanContext.traceFlags,
    ),
  )

  const traceState = spanContext.traceState?.serialize()
  if (traceState) {
    headers.set("tracestate", traceState)
  }

  headers.set(correlationHeaderName, spanContext.traceId)
}

export function fetchWithTrace(
  input: RequestInfo | URL,
  init: RequestInit = {},
  requestContext?: Context,
) {
  const headers = new Headers(init.headers ?? {})
  applyTraceHeaders(headers, requestContext)

  return fetch(input, {
    ...init,
    headers,
  })
}
