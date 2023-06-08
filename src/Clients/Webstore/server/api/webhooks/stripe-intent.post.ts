import { payWithStripe } from "@/services/stripeService";
import AddressModel from "~/utils/AddressModel";
import CartItemModel from "~/utils/CartItemModel";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const stripeEvent: StripeEventWebhookModel = await readBody(event);
  const stripeAddress = stripeEvent.data?.object.shipping.address;
  console.log(stripeAddress);
  const address: AddressModel = new AddressModel(
    stripeEvent.data?.object.shipping.name!,
    stripeAddress?.line2!,
    stripeAddress?.line1!,
    stripeAddress?.city!,
    stripeAddress?.postal_code!,
  );

  switch (stripeEvent.type) {
    case "payment_intent.created":
      console.log("created");
      break;
    case "payment_intent.succeeded":
      console.log("succeeded");
      break;
    case "charge.succeeded":
      const createdAddress: any = await postAddress(address);
      const createdOrder = await postOrder(
        stripeEvent.data?.object.metadata.client_id!,
        createdAddress.id,
      );
      await postPayment(stripeEvent, createdOrder);
      await postOrderItems(stripeEvent, createdOrder);
      console.log("chargeed");
      break;
    default:
      throw createError({
        statusCode: 400,
        message: "Event type not supported",
      });
  }
});

const postAddress = async (address: AddressModel) => {
  const response = await fetch(
    `${config.public.ordering_api_base_url}/api/Addresses`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
      },
      body: JSON.stringify(address),
    },
  );
  if (response.ok) {
    console.log("--------------------");
    const data = await response.json();
    console.log(data);
    console.log("--------------------");
    return await data;
  }
  console.log(await response.json());
  return null;
};

const postOrder = async (userId: string, addressId: string) => {
  console.log("ADDRESS", userId, addressId);
  const response = await fetch(
    `${config.public.ordering_api_base_url}/api/Orders`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
      },
      body: JSON.stringify({
        customerId: userId,
        deliveryAddressId: addressId,
      }),
    },
  );
  console.log(response);
  if (response.ok) {
    console.log("--------------------");
    const data = await response.json();
    console.log(data);
    console.log("--------------------");
    return await data;
  }
};

const postPayment = async (event: StripeEventWebhookModel, order: any) => {
  const response = await fetch(
    `${config.public.payments_api_base_url}/api/Payments`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
      },
      body: JSON.stringify({
        referenceNumber: order.referenceNumber,
        paymentDate: new Date(event.data?.object.created! * 1000),
        paymentAmount: event.data?.object.amount! / 100,
        cardNumber: "0000-0000-0000-0000",
        cardHolder: "string",
        expirationDate: "00/00",
        securityCode: "000",
        paymentState: "Completed",
      }),
    },
  );
  if (response.ok) {
    console.log("--------------------");
    const data = await response.json();
    console.log(data);
    console.log("--------------------");
    return await data;
  }
};

class OrderItems {
  id?: string;
  quantity?: number;
}

const postOrderItems = async (event: StripeEventWebhookModel, order: any) => {
  const items: OrderItems[] = JSON.parse(event.data?.object.metadata.items!);
  console.log(items);
  items.forEach(async (item) => {
    const response = await fetch(
      `${config.public.ordering_api_base_url}/api/Orders/${order.id}/Items/${item.id}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          accept: "application/json",
        },
        body: JSON.stringify({
          quantity: item.quantity,
        }),
      },
    );
    console.log(response);
    if (response.ok) {
      console.log("--------------------");
      const data = await response.json();
      console.log(data);
      console.log("--------------------");
    }
  });
};
