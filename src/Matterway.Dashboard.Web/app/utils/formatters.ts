export function formatDateTime(value?: string | Date | null) {
  if (!value) return "—"
  const date = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(date.getTime())) return "—"
  return new Intl.DateTimeFormat("sr-RS", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date)
}

export function formatMoney(value?: number | string | null) {
  if (value === null || value === undefined) return "—"
  const parsed = typeof value === "string" ? Number(value) : value
  if (Number.isNaN(parsed)) return "—"
  return new Intl.NumberFormat("sr-RS", {
    style: "currency",
    currency: "RSD",
    maximumFractionDigits: 2,
  }).format(parsed)
}
