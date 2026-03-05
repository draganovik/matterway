import {
  context,
  SpanKind,
  SpanStatusCode,
  trace,
  type Context,
} from "@opentelemetry/api"

const INVALID_TRACE_ID = "00000000000000000000000000000000"
const INVALID_SPAN_ID = "0000000000000000"
const CORRELATION_HEADER = "x-correlation-id"

function buildRequestUrl(input: RequestInfo | URL) {
  if (typeof input === "string") return input
  if (input instanceof URL) return input.toString()
  if (typeof globalThis.Request !== "undefined" && input instanceof Request) {
    return input.url
  }
  return String(input)
}

function toTraceParentValue(
  traceId: string,
  spanId: string,
  traceFlags: number,
) {
  const formattedFlags = traceFlags.toString(16).padStart(2, "0")
  return `00-${traceId}-${spanId}-${formattedFlags}`
}

function hasValidContext(traceId: string, spanId: string) {
  return (
    traceId.length === 32 &&
    spanId.length === 16 &&
    traceId !== INVALID_TRACE_ID &&
    spanId !== INVALID_SPAN_ID
  )
}

export async function fetchWithTelemetry(
  input: RequestInfo | URL,
  init: RequestInit = {},
  parentContext?: Context,
): Promise<Response> {
  const url = buildRequestUrl(input)
  const method = init.method
    ? init.method.toUpperCase()
    : typeof globalThis.Request !== "undefined" && input instanceof Request
      ? input.method.toUpperCase()
      : "GET"

  const tracer = trace.getTracer("matterway-storefront-web")
  const requestContext = parentContext ?? context.active()
  const span = tracer.startSpan(
    `${method} ${url}`,
    { kind: SpanKind.CLIENT },
    requestContext,
  )
  const spanContext = span.spanContext()
  const headers = new Headers(init.headers || {})

  if (hasValidContext(spanContext.traceId, spanContext.spanId)) {
    headers.set(
      "traceparent",
      toTraceParentValue(
        spanContext.traceId,
        spanContext.spanId,
        spanContext.traceFlags,
      ),
    )
    headers.set(CORRELATION_HEADER, spanContext.traceId)
  }

  span.setAttribute("http.method", method)
  span.setAttribute("http.url", url)

  try {
    const response = await fetch(url, {
      ...init,
      headers,
    })

    span.setAttribute("http.status_code", response.status)
    if (response.status >= 400) {
      span.setStatus({
        code: SpanStatusCode.ERROR,
        message: `HTTP ${response.status}`,
      })
    }

    return response
  } catch (error) {
    span.recordException(error as Error)
    span.setStatus({
      code: SpanStatusCode.ERROR,
      message: (error as Error).message,
    })
    throw error
  } finally {
    span.end()
  }
}
