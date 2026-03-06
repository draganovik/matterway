export type UnknownRecord = Record<string, unknown>

export function isRecord(value: unknown): value is UnknownRecord {
  return typeof value === "object" && value !== null && !Array.isArray(value)
}

export function asObject(value: unknown, message: string): UnknownRecord {
  if (!isRecord(value)) {
    throw new Error(message)
  }
  return value
}

export function asRequiredString(
  data: UnknownRecord,
  field: string,
  message: string,
): string {
  const value = data[field]
  if (typeof value !== "string" || !value.trim()) {
    throw new Error(message)
  }
  return value.trim()
}

export function asOptionalString(
  data: UnknownRecord,
  field: string,
): string | undefined {
  const value = data[field]
  if (value === undefined || value === null || value === "") {
    return undefined
  }

  if (typeof value !== "string") {
    throw new Error(`Field "${field}" must be a string.`)
  }

  return value.trim() || undefined
}

export function asNumber(
  data: UnknownRecord,
  field: string,
  message: string,
): number {
  const value = Number(data[field])
  if (!Number.isFinite(value)) {
    throw new Error(message)
  }
  return value
}

export function asOptionalNumber(
  value: unknown,
  message: string,
): number | undefined {
  if (value === undefined || value === null || value === "") {
    return undefined
  }
  const parsed = Number(value)
  if (!Number.isFinite(parsed)) {
    throw new Error(message)
  }
  return parsed
}
