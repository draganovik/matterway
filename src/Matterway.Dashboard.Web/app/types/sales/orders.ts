import type { PaginationResponse } from '../common/pagination'

export type QueryOrdersParams = {
  page: number
  pageSize: number
  customerId?: string
}

export type OrderType = 'Retail' | 'Ecommerce' | string

export type OrderStatusType =
  | 'Processing'
  | 'Reserved'
  | 'Delivery'
  | 'Completed'
  | 'Cancelled'
  | string

export type PaymentStatusType =
  | 'Reserved'
  | 'Charged'
  | 'Failed'
  | 'Refunded'
  | string

export type OrderDeliveryInfoResponse = {
  country?: string | null
  city?: string | null
  zipCode?: string | null
  addressLine1?: string | null
  addressLine2?: string | null
  contactPhone?: string | null
}

export type OrderItemResponse = {
  id: string
  articleId: string
  articleTitle?: string | null
  unitPrice: number | string
  quantity: number | string
}

export type OrderStatusResponse = {
  status: OrderStatusType
  changedAt: string
  note?: string | null
}

export type PaymentSnapshotResponse = {
  id: string
  provider?: string | null
  referenceId?: string | null
  amount: number | string
  status: PaymentStatusType
  createdAt: string
}

export type OrderResponse = {
  id: string
  customerId?: string | null
  type: OrderType
  totalAmount: number | string
  placedAt: string
  deliveryInfo?: OrderDeliveryInfoResponse | null
  items: OrderItemResponse[]
  statusHistory: OrderStatusResponse[]
  payments: PaymentSnapshotResponse[]
}

export type QueryOrdersResponse = PaginationResponse<OrderResponse>
