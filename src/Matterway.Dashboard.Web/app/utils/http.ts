type QueryValue = string | number | boolean | null | undefined

export function buildQuery(params: Record<string, QueryValue | QueryValue[]>) {
  const search = new URLSearchParams()
  for (const [key, value] of Object.entries(params)) {
    if (Array.isArray(value)) {
      for (const item of value) {
        if (item === null || item === undefined || item === "") continue
        search.append(key, String(item))
      }
      continue
    }
    if (value === null || value === undefined || value === "") continue
    search.set(key, String(value))
  }
  const query = search.toString()
  return query ? `?${query}` : ""
}

export function normalizeList<T>(payload: unknown): T[] {
  if (Array.isArray(payload)) return payload as T[]
  if (!payload || typeof payload !== "object") return []
  const record = payload as Record<string, unknown>
  const keys = ["items", "results", "data", "value"]
  for (const key of keys) {
    const candidate = record[key]
    if (Array.isArray(candidate)) return candidate as T[]
  }
  const nestedItems = record.items as Record<string, unknown> | undefined
  if (nestedItems && Array.isArray(nestedItems.items))
    return nestedItems.items as T[]
  return []
}
