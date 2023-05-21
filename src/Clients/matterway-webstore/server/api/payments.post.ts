import { payWithStripe } from "@/services/stripeService";
import Stripe from "stripe";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const { cardNumber, expMonth, expYear, cvc, amount } = await readBody(event);
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
