import { payWithStripe } from "@/services/stripeService";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const stripeEvent: StripeEventWebhookModel = await readBody(event);

  switch (stripeEvent.type) {
    case "payment_intent.created":
      //updatePayment()
      console.log("created");
      break;
    case "payment_intent.succeeded":
      //updatePayment()
      console.log("succeeded");
      break;
    case "charge.succeeded":
      //postAddress()
      //postOrder()
      //putPayment()
      console.log("chargeed");
      break;
    default:
      throw createError({
        statusCode: 400,
        message: "Event type not supported",
      });
      break;
  }
});
