import type { OrderItemResponse, OrderResponse } from "~/types/sales"

function toAmount(value: number | string | null | undefined) {
  if (value === null || value === undefined || value === "") return 0

  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : 0
}

function roundCurrency(value: number) {
  return Math.round((value + Number.EPSILON) * 100) / 100
}

export function paymentSumOf(
  order: Pick<OrderResponse, "payments"> | null | undefined,
) {
  return roundCurrency(
    (order?.payments || []).reduce((sum, row) => sum + toAmount(row.amount), 0),
  )
}

export function paymentsBalanced(
  order: Pick<OrderResponse, "payments" | "totalAmount"> | null | undefined,
) {
  const total = roundCurrency(toAmount(order?.totalAmount))
  return Math.abs(total - paymentSumOf(order)) < 0.01
}

export function quantitySumOf(
  order: Pick<OrderResponse, "items"> | null | undefined,
) {
  return roundCurrency(
    (order?.items || []).reduce((sum, row) => sum + toAmount(row.quantity), 0),
  )
}

export function lineTotalOf(item: OrderItemResponse) {
  return roundCurrency(toAmount(item.unitPrice) * toAmount(item.quantity))
}
