import Stripe from "stripe";
import type {
  CardPaymentInput,
  PaymentAddress,
} from "../server/types/payments";

export async function payWithStripe(
  cardPayment: CardPaymentInput,
  address: PaymentAddress,
  userId: string,
  orderId: string,
  secretkey: string,
): Promise<string> {
  const referenceId = createReferenceId();
  const stripe = new Stripe(secretkey, {
    apiVersion: "2025-10-29.clover",
  });
  try {
    const paymentMethod = await stripe.paymentMethods.create({
      type: "card",
      card: {
        number: cardPayment.cardNumber,
        exp_month: cardPayment.expMonth,
        exp_year: cardPayment.expYear,
        cvc: cardPayment.cvc,
      },
    });

    const paymentIntent = await stripe.paymentIntents.create({
      amount: Math.round(cardPayment.amount * 100),
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

    return paymentIntent.client_secret || "";
  } catch (error) {
    console.error("[payments] stripe charge failed", error);
    throw new Error("Plaćanje nije uspelo. Pokušajte ponovo.");
  }
}

const createReferenceId = () => {
  const segment = () => Math.floor(1000 + Math.random() * 9000).toString();
  return `${segment()}-${segment()}-${segment()}-${segment()}`;
};
