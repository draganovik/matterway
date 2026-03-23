import {
  createError,
  defineEventHandler,
  getRequestHeaders,
  getRouterParam,
  proxyRequest,
} from "h3"

const imageIdPattern = /^[a-f0-9]{32}$/i
const forwardHeaderNames = [
  "accept",
  "accept-encoding",
  "if-match",
  "if-none-match",
  "if-modified-since",
  "if-unmodified-since",
  "if-range",
  "range",
] as const

export default defineEventHandler((event) => {
  const imageId = normalizeImageId(getRouterParam(event, "id"))
  if (!imageId) {
    throw createError({
      statusCode: 404,
      statusMessage: "Not Found",
    })
  }

  const config = useRuntimeConfig()
  const baseUrl = config.serverImageCdnBaseUrl?.trim()
  const bucket = config.serverImageCdnBucket?.trim()
  if (!baseUrl || !bucket) {
    throw createError({
      statusCode: 503,
      statusMessage: "Missing image CDN configuration.",
    })
  }

  const targetUrl = new URL(`/${bucket}/images/${imageId}`, baseUrl)

  return proxyRequest(event, targetUrl.toString(), {
    headers: buildProxyHeaders(event),
    streamRequest: true,
  })
})

function normalizeImageId(value: string | null | undefined) {
  const normalized =
    typeof value === "string" ? value.trim().replace(/-/g, "") : ""
  return imageIdPattern.test(normalized) ? normalized.toLowerCase() : null
}

function buildProxyHeaders(event: Parameters<typeof getRequestHeaders>[0]) {
  const requestHeaders = getRequestHeaders(event)
  const headers = new Headers()
  for (const name of forwardHeaderNames) {
    const value = requestHeaders[name]
    if (value == null) continue
    if (Array.isArray(value)) {
      for (const item of value) headers.append(name, item)
      continue
    }
    headers.set(name, value)
  }

  return headers
}
