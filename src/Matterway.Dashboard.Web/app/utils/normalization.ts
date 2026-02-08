export function normalizeSlug(value: string) {
  return value
    .trim()
    .toLowerCase()
    .replace(/\s+/g, '-')
}

export function normalizeCode(value: string) {
  return value.trim().toUpperCase()
}
