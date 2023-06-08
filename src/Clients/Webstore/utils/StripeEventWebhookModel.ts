class StripeEventWebhookModel {
  id?: string;
  object?: string;
  api_version?: string;
  created?: number;
  data?: {
    object: {
      id: string;
      object: string;
      amount: number;
      amount_capturable: number;
      amount_details: object;
      amount_received: number;
      application: null | string;
      application_fee_amount: null | number;
      automatic_payment_methods: null | object;
      canceled_at: null | number;
      cancellation_reason: null | string;
      capture_method: string;
      client_secret: string;
      confirmation_method: string;
      created: number;
      currency: string;
      customer: null | string;
      description: string;
      invoice: null | string;
      last_payment_error: null | object;
      latest_charge: null | string;
      livemode: boolean;
      metadata: object;
      next_action: null | object;
      transfer_data: null | object;
      transfer_group: null | string;
    };
  };
  livemode?: boolean;
  pending_webhooks?: number;
  request?: {
    id: string;
    idempotency_key: string;
  };
  type?: string;
}
