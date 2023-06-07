import { payWithStripe } from "@/services/stripeService";
import Stripe from "stripe";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const { cardNumber, expMonth, expYear, cvc, amount } = await readBody(event);
  // create Order (needs address)
  // add items to Order from CartItems
  // start payment intent in Stripe
  // webhook: Catch init event and create payment
  // webhook: On different events update payment entity in Payment API
  // note: use payment intent ID "pi_3NGRh4BZYyXRoS3Y1o41PbK2" as reference number in payment and in Order
  try {
    const clientSecret = await payWithStripe(
      cardNumber,
      expMonth,
      expYear,
      cvc,
      amount,
      config.stripeSecretKey,
    );
    return { clientSecret };
  } catch (error) {
    console.log(error);
    throw createError({
      statusCode: 500,
    });
  }
});
