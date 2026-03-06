export type SalesOrderItem = {
  quantity?: number
  articleTitle?: string
  unitPrice?: number
}

export type SalesOrderStatus = {
  status?: string
  changedAt?: string
  note?: string
}

export type SalesOrder = {
  id: string
  totalAmount?: number
  placedAt?: string
  createdAt?: string
  items?: SalesOrderItem[]
  statusHistory?: SalesOrderStatus[]
  deliveryInfo?: {
    addressLine1?: string
  }
}
