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

export interface CreatePaymentRequest {
  orderId: string
  userId: string
  cardPayment: CardPaymentInput
  address: PaymentAddress
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
