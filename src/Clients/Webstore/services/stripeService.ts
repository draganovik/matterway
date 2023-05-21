// Import necessary dependencies
import Stripe from "stripe";

// Create a function to handle the payment
export async function payWithStripe(
  cardNumber: string,
  expMonth: number,
  expYear: number,
  cvc: string,
  amount: number,
  secretkey: string,
): Promise<string> {
  // Set up your Stripe API key
  const stripe = new Stripe(secretkey, {
    apiVersion: "2022-11-15",
  });
  try {
    // Create a Stripe payment method using the provided card details
    const paymentMethod = await stripe.paymentMethods.create({
      type: "card",
      card: {
        number: cardNumber,
        exp_month: expMonth,
        exp_year: expYear,
        cvc: cvc,
      },
    });
    console.log(paymentMethod);

    // Create a Stripe payment intent
    const paymentIntent = await stripe.paymentIntents.create({
      amount: amount * 100, // Stripe expects the amount in cents
      currency: "rsd",
      payment_method: paymentMethod.id,
      confirm: true,
    });

    // Return the payment intent's client secret
    return paymentIntent.client_secret || "";
  } catch (error) {
    console.log(error);
    throw new Error("Payment failed. Please try again.");
  }
}
