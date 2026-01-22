import { payWithStripe } from "#services/stripeService";
import { Console } from "console";
import AddressModel from "#models/AddressModel";
import CardPaymentModel from "#models/CardPaymentModel";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const rawBody = await readBody(event);
  let body =
    rawBody && typeof rawBody === "string" ? rawBody.trim() : (rawBody ?? {});
  if (typeof body === "string") {
    if (!body.length) {
      body = {};
    } else {
      try {
        body = JSON.parse(body);
      } catch (error) {
        console.error("[payments] failed to parse JSON body", error);
        throw createError({
          statusCode: 400,
          message: "Invalid request body",
        });
      }
    }
  }

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
    orderId,
    userId,
  } = body;
  if (!orderId) {
    throw createError({
      statusCode: 400,
      message: "OrderId is required",
    });
  }
  // create Stripe payment intent
  // webhook: On charge.succeeded, register payment in Sales
  const payloadSummary = {
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
    orderId,
    userId,
  };
  console.log("[payments] received payload", payloadSummary);

  const cardPayment = new CardPaymentModel(
    cardNumber,
    expMonth,
    expYear,
    cvc,
    amount,
  );
  const address = new AddressModel();
  address.receiverName = receiverName;
  address.residence = residence;
  address.street = street;
  address.city = city;
  address.zipCode = zipCode;

  if (!address.validate()) {
    console.error("[payments] invalid address", address);
    throw createError({
      statusCode: 400,
      message: "Address is not valid",
    });
  }

  const clientSecret = await payWithStripe(
    cardPayment,
    address,
    userId,
    orderId,
    config.stripeSecretKey,
  );
  return { clientSecret };
});
