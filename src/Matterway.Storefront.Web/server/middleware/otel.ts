import {
  context,
  propagation,
  Span,
  SpanKind,
  SpanStatusCode,
  trace,
  type Context,
} from "@opentelemetry/api"
import {
  BasicTracerProvider,
  BatchSpanProcessor,
} from "@opentelemetry/sdk-trace-base"
import { OTLPTraceExporter as GrpcOTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-grpc"
import { OTLPTraceExporter as HttpOTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-http"
import { resourceFromAttributes } from "@opentelemetry/resources"
import type { H3Event } from "h3"

type HeaderMap = Record<string, string | string[] | undefined>

type NuxtOtelProvider = {
  isEnabled: boolean
  tracer: ReturnType<typeof trace.getTracer>
}

type OtelH3Context = H3Event["context"] & {
  __otelRequestContext?: Context
}

function normalizeHttpEndpoint(rawEndpoint: string): string | null {
  try {
    const endpoint = new URL(rawEndpoint.trim())
    const scheme = endpoint.protocol
    if (scheme !== "http:" && scheme !== "https:") {
      return null
    }

    const path = endpoint.pathname.replace(/\/+$/, "")
    if (path === "") {
      return endpoint.origin
    }

    if (path.toLowerCase().endsWith("/v1/traces")) {
      return `${endpoint.origin}${path.slice(0, -"/v1/traces".length)}`
    }

    return `${endpoint.origin}${path}`
  } catch {
    return null
  }
}

function normalizeOtlpBaseEndpoint(rawEndpoint: string): string | null {
  const normalized = normalizeHttpEndpoint(rawEndpoint)
  if (!normalized) return null
  return normalized.toLowerCase().endsWith("/v1/traces")
    ? normalized.slice(0, -"/v1/traces".length)
    : normalized
}

function normalizeOtlpTraceEndpoint(rawEndpoint: string): string | null {
  const baseEndpoint = normalizeOtlpBaseEndpoint(rawEndpoint)
  if (!baseEndpoint) return null
  return `${baseEndpoint.replace(/\/+$/, "")}/v1/traces`
}

function normalizeOtlpProtocol(): "grpc" | "http" {
  const protocol = (process.env.OTEL_EXPORTER_OTLP_PROTOCOL || "")
    .trim()
    .toLowerCase()

  if (protocol.startsWith("http")) return "http"
  return "grpc"
}

function createExporter(endpoint: string, protocol: "grpc" | "http") {
  if (protocol === "grpc") {
    return new GrpcOTLPTraceExporter({ url: endpoint })
  }

  return new HttpOTLPTraceExporter({ url: endpoint })
}

function createInstrumentation(): NuxtOtelProvider | null {
  const protocol = normalizeOtlpProtocol()
  const serviceName = process.env.OTEL_SERVICE_NAME || "storefront-web"

  const tracesEndpoint =
    normalizeOtlpTraceEndpoint(
      process.env.OTEL_EXPORTER_OTLP_TRACES_ENDPOINT || "",
    ) ??
    normalizeOtlpTraceEndpoint(process.env.OTEL_EXPORTER_OTLP_ENDPOINT || "") ??
    normalizeOtlpTraceEndpoint(
      process.env.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL || "",
    )
  const baseEndpoint =
    normalizeOtlpBaseEndpoint(process.env.OTEL_EXPORTER_OTLP_ENDPOINT || "") ??
    normalizeOtlpBaseEndpoint(
      process.env.OTEL_EXPORTER_OTLP_TRACES_ENDPOINT || "",
    ) ??
    normalizeOtlpBaseEndpoint(
      process.env.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL || "",
    )
  const endpoint = protocol === "http" ? tracesEndpoint : baseEndpoint

  if (!endpoint) {
    console.warn(
      `[otel] server tracing disabled for ${serviceName}; endpoint is missing. Set OTEL_EXPORTER_OTLP_TRACES_ENDPOINT or OTEL_EXPORTER_OTLP_ENDPOINT.`,
    )
    return null
  }

  try {
    const exporter = createExporter(endpoint, protocol)
    const provider = new BasicTracerProvider({
      resource: resourceFromAttributes({
        "service.name": serviceName,
      }),
      spanProcessors: [new BatchSpanProcessor(exporter)],
    })

    trace.setGlobalTracerProvider(provider)

    process.once("beforeExit", async () => {
      await provider.shutdown()
    })

    console.info(
      `[otel] server tracing enabled for ${serviceName}; protocol=${protocol}; endpoint=${endpoint}`,
    )

    return {
      isEnabled: true,
      tracer: trace.getTracer(serviceName),
    }
  } catch (error) {
    console.error("[otel] failed to initialize server tracer", error)
    return null
  }
}

const otelProvider = createInstrumentation()
const getter = {
  get: (carrier: HeaderMap, key: string) => {
    const headerValue = carrier[key] ?? carrier[key.toLowerCase()]
    return Array.isArray(headerValue) ? headerValue[0] : headerValue
  },
  keys: (carrier: HeaderMap) => Object.keys(carrier),
}

function finishSpan(span: Span, event: H3Event, error?: Error) {
  const statusCode = event.node.res.statusCode
  span.setAttribute("http.status_code", statusCode)
  span.setAttribute(
    "http.response_content_length",
    Number(event.node.res.getHeader("content-length") || "0"),
  )

  if (error) {
    span.setStatus({
      code: SpanStatusCode.ERROR,
      message: error.message,
    })
    span.recordException(error)
  } else if (statusCode >= 400) {
    span.setStatus({
      code: SpanStatusCode.ERROR,
      message: `HTTP ${statusCode}`,
    })
  }

  span.end()
}

function getMiddlewareTraceUrl(event: H3Event) {
  return event.node.req.url || event.path || "/"
}

export default defineEventHandler((event: H3Event) => {
  if (!otelProvider?.isEnabled) return

  const method = event.node.req.method || "GET"
  const route = event.path || "/"
  const rawUrl = getMiddlewareTraceUrl(event)
  const requestContext = propagation.extract(
    context.active(),
    event.node.req.headers as HeaderMap,
    getter,
  )
  const span = otelProvider.tracer.startSpan(
    `${method} ${route}`,
    {
      kind: SpanKind.SERVER,
      attributes: {
        "http.method": method,
        "http.route": route,
        "http.target": rawUrl,
      },
    },
    requestContext,
  )

  ;(event.context as OtelH3Context).__otelRequestContext = trace.setSpan(
    requestContext,
    span,
  )

  let isFinished = false
  const finalize = (error?: Error) => {
    if (isFinished) return
    isFinished = true
    finishSpan(span, event, error)
  }

  event.node.res.once("finish", () => finalize())
  event.node.res.once("close", () => finalize())
  event.node.res.once("error", (error: Error) => finalize(error))
})
