// Import necessary dependencies
import Stripe from "stripe";
import AddressModel from "~/utils/AddressModel";
import CardPaymentModel from "~/utils/CardPaymentModel";
import CartItemModel from "~/utils/CartItemModel";

// Create a function to handle the payment
export async function payWithStripe(
  cardPayment: CardPaymentModel,
  address: AddressModel,
  userId: string,
  items: CartItemModel[],
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
        items: JSON.stringify(
          items.map((item) => {
            return {
              id: item.productId,
              quantity: item.quantity,
            };
          }),
        ),
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
