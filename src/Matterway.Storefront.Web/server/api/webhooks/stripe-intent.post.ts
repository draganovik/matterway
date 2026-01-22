import StripeEventWebhookModel from "#models/StripeEventWebhookModel";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const stripeEvent: StripeEventWebhookModel = await readBody(event);

  switch (stripeEvent.type) {
    case "payment_intent.created":
      console.log("created");
      return { success: true, message: "Payment created" };
    case "payment_intent.succeeded":
      console.log("succeeded");
      return { success: true, message: "Payment succeeded" };
    case "charge.succeeded":
      const payment = await postPayment(stripeEvent);
      return { success: true, message: "Data received", data: { payment } };
    default:
      throw createError({
        statusCode: 400,
        message: "Event type not supported",
      });
  }
});

const postPayment = async (event: StripeEventWebhookModel) => {
  const orderId = event.data?.object.metadata.order_id;
  if (!orderId) {
    console.error("[stripe] missing order_id metadata");
    return null;
  }
  const amount = event.data?.object.amount! / 100;
  const referenceId =
    event.data?.object.metadata.reference_id ?? createReferenceId();

  const response = await fetch(
    `${config.serverSalesApiBaseUrl}/api/v1/Payments`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
      },
      body: JSON.stringify({
        orderId,
        provider: "Stripe",
        referenceId,
        amount,
        status: "Charged",
        createdAt: new Date(event.data?.object.created! * 1000),
      }),
    },
  );

  if (!response.ok) {
    console.error("[stripe] failed to register payment", await response.text());
    return null;
  }

  return await response.json();
};

const createReferenceId = () => {
  const segment = () => Math.floor(1000 + Math.random() * 9000).toString();
  return `${segment()}-${segment()}-${segment()}-${segment()}`;
};
