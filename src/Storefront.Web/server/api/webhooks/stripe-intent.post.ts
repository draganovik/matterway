import { payWithStripe } from "@/services/stripeService";
import AddressModel from "~/models/AddressModel";
import CartItemModel from "~/models/CartItemModel";

const config = useRuntimeConfig();

export default defineEventHandler(async (event) => {
  const stripeEvent: StripeEventWebhookModel = await readBody(event);
  const stripeAddress = stripeEvent.data?.object.shipping.address;
  console.log(stripeAddress);
  const address: AddressModel = new AddressModel();
  address.receiverName = stripeEvent.data?.object.shipping.name!;
  address.residence = stripeAddress?.line2!;
  address.street = stripeAddress?.line1!;
  address.city = stripeAddress?.city!;
  address.zipCode = stripeAddress?.postal_code!;

  switch (stripeEvent.type) {
    case "payment_intent.created":
      console.log("created");
      return { success: true, message: "Payment created" };
    case "payment_intent.succeeded":
      console.log("succeeded");
      return { success: true, message: "Payment succeeded" };
    case "charge.succeeded":
      const createdAddress: any = await postAddress(address);
      console.log("address: ", createdAddress);
      const createdOrder = await postOrder(
        stripeEvent.data?.object.metadata.client_id!,
        createdAddress.id,
      );
      console.log("order: ", createdOrder);
      let payment = await postPayment(stripeEvent, createdOrder);
      console.log("payment created");
      await postOrderItems(stripeEvent, createdOrder);
      console.log("chargeed");
      return { success: true, message: "Data received", data: { payment } };
    default:
      throw createError({
        statusCode: 400,
        message: "Event type not supported",
      });
  }
});

const postAddress = async (address: AddressModel) => {
  const response = await fetch(
    `${config.orderingApiServerBaseUrl}/api/v1/Addresses`,
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
    `${config.orderingApiServerBaseUrl}/api/v1/Orders`,
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
    `${config.paymentsApiServerBaseUrl}/api/v1/Payments`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "application/json",
      },
      body: JSON.stringify({
        referenceNumber: order.referenceNumber + "-0000",
        paymentDate: new Date(event.data?.object.created! * 1000),
        paymentAmount: event.data?.object.amount! / 100,
        cardNumber: "0000-0000-0000-0000",
        cardHolder: "string",
        expirationDate: "12/30",
        securityCode: "000",
        paymentState: "Processed",
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
      `${config.orderingApiServerBaseUrl}/api/v1/Orders/${order.id}/Items/${item.id}`,
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
