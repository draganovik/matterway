export interface CardPaymentInput {
  cardNumber: string
  expMonth: number
  expYear: number
  cvc: string
  amount: number
}

export interface PaymentAddress {
  receiverName: string
  residence: string
  street: string
  city: string
  zipCode: string
  country?: string
  contactPhone?: string
  note?: string
}

export type EmptyQuery = Record<string, never>

export interface DeliveryInfoInput {
  country: string
  city: string
  zipCode: string
  addressLine1: string
  addressLine2?: string
  contactPhone?: string
}

export interface CheckoutOrderInput {
  type: "Ecommerce"
  deliveryInfo: DeliveryInfoInput
}

export interface CheckoutPaymentInput {
  type: "stripe"
  cardPayment: CardPaymentInput
}

export interface CheckoutOrderRequest {
  customerId: string
  order: CheckoutOrderInput
  payment: CheckoutPaymentInput
  address: PaymentAddress
}

export interface SalesOrderResponse {
  id?: string
  totalAmount?: number
}

export interface StripeEventWebhookPayload {
  type: string
  data?: {
    object?: {
      amount?: number
      created?: number
      metadata?: Record<string, string | undefined>
    }
  }
}

export type SystemPaymentConfig = {
  serverSalesApiBaseUrl: string
  systemAccessKey: string
}

export type StripeChargeSucceeded = {
  orderId: string
  amount: number
  referenceId: string
  createdAt: Date
}

export type StripePaymentSession = {
  paymentIntentId: string
  referenceId: string
}
