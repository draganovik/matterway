export interface CardPaymentInput {
  cardNumber: string;
  expMonth: number;
  expYear: number;
  cvc: string;
  amount: number;
}

export interface PaymentAddress {
  receiverName: string;
  residence: string;
  street: string;
  city: string;
  zipCode: string;
  country?: string;
  contactPhone?: string;
  note?: string;
}

export interface StripeEventWebhookPayload {
  type?: string;
  data?: {
    object?: {
      amount?: number;
      created?: number;
      metadata?: Record<string, string | undefined>;
    };
  };
}
