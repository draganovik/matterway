function sanitizeNumberString(value: string) {
  const raw = value.trim().replace(/\s+/g, '')
  if (!raw) return ''

  let normalized = raw.replace(/[^\d,.-]/g, '')
  if (!normalized) return ''

  const hasComma = normalized.includes(',')
  const hasDot = normalized.includes('.')

  if (hasComma && hasDot) {
    const lastComma = normalized.lastIndexOf(',')
    const lastDot = normalized.lastIndexOf('.')

    if (lastComma > lastDot) {
      normalized = normalized.replace(/\./g, '').replace(',', '.')
    } else {
      normalized = normalized.replace(/,/g, '')
    }
  } else if (hasComma) {
    normalized = normalized.replace(',', '.')
  }

  return normalized
}

export function parseLooseAmount(value: unknown): number {
  if (typeof value === 'number') {
    return Number.isFinite(value) ? value : 0
  }

  if (typeof value === 'bigint') {
    return Number(value)
  }

  if (typeof value === 'string') {
    const normalized = sanitizeNumberString(value)
    if (!normalized) return 0
    const parsed = Number(normalized)
    return Number.isFinite(parsed) ? parsed : 0
  }

  if (!value || typeof value !== 'object') return 0

  const record = value as Record<string, unknown>
  if ('amount' in record) return parseLooseAmount(record.amount)
  if ('Amount' in record) return parseLooseAmount(record.Amount)
  if ('value' in record) return parseLooseAmount(record.value)
  if ('Value' in record) return parseLooseAmount(record.Value)

  return 0
}

export function roundCurrency(value: number) {
  return Math.round((value + Number.EPSILON) * 100) / 100
}

export function sumPaymentAmounts(payments: unknown[] | null | undefined) {
  return roundCurrency(
    (payments || []).reduce<number>((sum, row) => {
      if (!row || typeof row !== 'object') return sum + parseLooseAmount(row)
      const record = row as Record<string, unknown>
      return sum + parseLooseAmount(record.amount ?? record.Amount)
    }, 0)
  )
}

export function amountsMatch(left: unknown, right: unknown) {
  const leftValue = roundCurrency(parseLooseAmount(left))
  const rightValue = roundCurrency(parseLooseAmount(right))
  return Math.abs(leftValue - rightValue) < 0.01
}
