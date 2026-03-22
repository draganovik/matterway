import type { ApiService } from "~/types/common/api"

const apiServices = [
  "catalog",
  "customers",
  "identity",
  "sales",
] as const satisfies readonly ApiService[]

const apiEndpointKinds = ["self", "admin", "public", "system"] as const

type ApiEndpointKind = (typeof apiEndpointKinds)[number]

const apiEndpointKindSet = new Set<string>(apiEndpointKinds)
const apiServiceSet = new Set<string>(apiServices)

export function isApiService(value: string): value is ApiService {
  return apiServiceSet.has(value)
}

export function normalizeApiPath(path: string) {
  return path.replace(/^\/+/, "")
}

export function buildServiceApiPath(
  service: ApiService,
  endpointKind: ApiEndpointKind,
  resourcePath: string,
) {
  return `/api/${service}/${endpointKind}/v1/${normalizeApiPath(resourcePath)}`
}

export function buildServiceApiPathFromRequestPath(
  service: ApiService,
  path: string,
) {
  const normalizedPath = normalizeApiPath(path)
  const [endpointKind, ...resourcePath] = normalizedPath.split("/")

  if (
    !endpointKind ||
    !apiEndpointKindSet.has(endpointKind) ||
    resourcePath.length === 0
  ) {
    return null
  }

  return buildServiceApiPath(
    service,
    endpointKind as ApiEndpointKind,
    resourcePath.join("/"),
  )
}
