export function getApiErrorMessage(payload: unknown) {
  if (typeof payload === "string") {
    return payload.trim() || null
  }

  if (!payload || typeof payload !== "object") return null

  const candidate = payload as Record<string, unknown>
  for (const key of [
    "detail",
    "title",
    "message",
    "statusMessage",
    "statusText",
  ]) {
    const value = candidate[key]
    if (typeof value === "string" && value.trim()) return value.trim()
  }

  return null
}
