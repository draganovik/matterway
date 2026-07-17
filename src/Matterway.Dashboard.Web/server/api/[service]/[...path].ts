import { isApiService, normalizeApiPath } from "~/utils/apiProxy"
import type { ApiService } from "~/types/common/api"
import {
  createError,
  defineEventHandler,
  getRequestHeaders,
  getRequestURL,
  getRouterParam,
  proxyRequest,
} from "h3"

export default defineEventHandler(async (event) => {
  const service = getRouterParam(event, "service")
  if (!service || !isApiService(service)) {
    throw createError({
      statusCode: 404,
      statusMessage: "Not Found",
    })
  }

  const path = readRouterPath(event)
  if (!path) {
    throw createError({
      statusCode: 404,
      statusMessage: "Not Found",
    })
  }

  const baseUrl = resolveServiceBaseUrl(service)
  const targetUrl = buildProxyTargetUrl(
    baseUrl,
    path,
    getRequestURL(event).search,
  )
  const headers = buildForwardHeaders(event)

  return proxyRequest(event, targetUrl, {
    headers,
    streamRequest: true,
  })
})

function resolveServiceBaseUrl(service: ApiService) {
  const config = useRuntimeConfig()
  const baseUrls: Record<ApiService, string | undefined> = {
    catalog: config.serverCatalogApiBaseUrl,
    customers: config.serverCustomersApiBaseUrl,
    identity: config.serverIdentityApiBaseUrl,
    sales: config.serverSalesApiBaseUrl,
  }

  const baseUrl = baseUrls[service]?.trim()
  if (!baseUrl) {
    throw createError({
      statusCode: 503,
      statusMessage: `Missing upstream API base URL for service '${service}'.`,
    })
  }

  return baseUrl
}

function buildProxyTargetUrl(baseUrl: string, path: string, search: string) {
  const target = new URL(`/api/${normalizeApiPath(path)}`, baseUrl)
  target.search = search
  return target.toString()
}

function buildForwardHeaders(event: Parameters<typeof getRequestHeaders>[0]) {
  const headers = new Headers()
  for (const [name, value] of Object.entries(getRequestHeaders(event))) {
    if (value == null) continue
    if (Array.isArray(value)) {
      for (const item of value) headers.append(name, item)
      continue
    }
    headers.set(name, value)
  }
  headers.delete("connection")
  headers.delete("cookie")
  headers.delete("expect")
  headers.delete("host")
  headers.delete("keep-alive")
  headers.delete("transfer-encoding")
  headers.delete("upgrade")
  return headers
}

function readRouterPath(event: Parameters<typeof getRouterParam>[0]) {
  const path = getRouterParam(event, "path")
  if (Array.isArray(path)) {
    return path.join("/")
  }

  return path?.trim() || null
}
