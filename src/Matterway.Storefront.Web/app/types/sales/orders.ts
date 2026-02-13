export type SalesOrderItem = {
  quantity?: number
  articleTitle?: string
}

export type SalesOrder = {
  id: string
  totalAmount?: number
  placedAt?: string
  createdAt?: string
  items?: SalesOrderItem[]
  deliveryInfo?: {
    addressLine1?: string
  }
}

export type PlaceOrderPayload = {
  customerId?: string
  type: "Ecommerce"
  deliveryInfo: {
    country: string
    city: string
    zipCode: string
    addressLine1: string
    addressLine2?: string
    contactPhone?: string
  }
}
