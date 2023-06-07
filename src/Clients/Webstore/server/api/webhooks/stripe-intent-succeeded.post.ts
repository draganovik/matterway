import { payWithStripe } from "@/services/stripeService";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const stripeEvent: StripeEventWebhookModel = await readBody(event);

  //await initializeOrder()

  switch (stripeEvent.type) {
    case "payment_intent.created":
      //putPayment()
      console.log('created', stripeEvent)
      break;
    case "payment_intent.succeeded":
      //putPayment()
      console.log('succeeded', stripeEvent)
      break;
    case "charge.succeeded":
      //putPayment()
      console.log('chargeed', stripeEvent)
      break;
    default:
      //putPayment()
      console.log('other', stripeEvent)
      break;
  }
});