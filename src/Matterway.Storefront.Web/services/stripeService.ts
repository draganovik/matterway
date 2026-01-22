// Import necessary dependencies
import Stripe from "stripe";
import AddressModel from "#models/AddressModel";
import CardPaymentModel from "#models/CardPaymentModel";
// Create a function to handle the payment
export async function payWithStripe(
  cardPayment: CardPaymentModel,
  address: AddressModel,
  userId: string,
  orderId: string,
  secretkey: string,
): Promise<string> {
  const referenceId = createReferenceId();
  // Set up your Stripe API key
  const stripe = new Stripe(secretkey, {
    apiVersion: "2024-06-20",
  });
  try {
    // Create a Stripe payment method using the provided card details
    const paymentMethod = await stripe.paymentMethods.create({
      type: "card",
      card: {
        number: cardPayment.cardNumber,
        exp_month: cardPayment.expMonth,
        exp_year: cardPayment.expYear,
        cvc: cardPayment.cvc,
      },
    });
    //console.log(paymentMethod);

    // Create a Stripe payment intent
    const paymentIntent = await stripe.paymentIntents.create({
      amount: cardPayment.amount * 100, // Stripe expects the amount in cents
      currency: "rsd",
      payment_method_types: ["card"],
      payment_method: paymentMethod.id,
      confirm: true,
      shipping: {
        name: address.receiverName,
        address: {
          line1: address.street,
          line2: address.residence,
          city: address.city,
          postal_code: address.zipCode,
          country: "RS",
        },
      },
      metadata: {
        client_id: userId,
        order_id: orderId,
        reference_id: referenceId,
        note: address.note || "",
      },
    });

    // Return the payment intent's client secret
    return paymentIntent.client_secret || "";
  } catch (error) {
    console.log(error);
    throw new Error("Payment failed. Please try again.");
  }
}

const createReferenceId = () => {
  const segment = () => Math.floor(1000 + Math.random() * 9000).toString();
  return `${segment()}-${segment()}-${segment()}-${segment()}`;
};
