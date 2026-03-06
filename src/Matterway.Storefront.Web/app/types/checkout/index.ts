export type CheckoutPaymentForm = {
  cardNumber: string
  expMonth: string
  expYear: string
  cvc: string
}

export type ParsedCardExpiry = {
  month: number
  year: number
}
