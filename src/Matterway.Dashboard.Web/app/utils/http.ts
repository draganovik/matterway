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
