export function normalizeSlug(value: string) {
  return String(value ?? "")
    .trim()
    .toLowerCase()
    .replace(/\s+/g, "-")
}

export function normalizeCode(value: string) {
  return String(value ?? "")
    .trim()
    .toUpperCase()
}
