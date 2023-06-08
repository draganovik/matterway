import { payWithStripe } from "@/services/stripeService";
import AddressModel from "~/utils/AddressModel";
import CardPaymentModel from "~/utils/CardPaymentModel";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const {
    cardNumber,
    expMonth,
    expYear,
    cvc,
    amount,
    receiverName,
    residence,
    street,
    city,
    zipCode,
    note,
    userId,
  } = await readBody(event);
  // create Order (needs address)
  // add items to Order from CartItems
  // start payment intent in Stripe
  // webhook: Catch init event and create payment
  // webhook: On different events update payment entity in Payment API
  // note: use payment intent ID "pi_3NGRh4BZYyXRoS3Y1o41PbK2" as reference number in payment and in Order
  const cardPayment = new CardPaymentModel(
    cardNumber,
    expMonth,
    expYear,
    cvc,
    amount,
  );
  const address = new AddressModel(
    receiverName,
    residence,
    street,
    city,
    zipCode,
    note,
  );

  if (!address.validate())
    throw createError({
      statusCode: 400,
      message: "Address is not valid",
    });

  try {
    const clientSecret = await payWithStripe(
      cardPayment,
      address,
      userId,
      config.stripeSecretKey,
    );
    return { clientSecret };
  } catch (error) {
    throw createError({
      statusCode: 500,
    });
  }
});
