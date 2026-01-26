export function formatDateTime(value?: string | Date | null) {
  if (!value) return '—'
  const date = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(date.getTime())) return '—'
  return date.toLocaleString()
}

export function formatMoney(value?: number | string | null) {
  if (value === null || value === undefined) return '—'
  const parsed = typeof value === 'string' ? Number(value) : value
  if (Number.isNaN(parsed)) return '—'
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'RSD',
    maximumFractionDigits: 2
  }).format(parsed)
}
