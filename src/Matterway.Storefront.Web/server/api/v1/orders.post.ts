import {
  getServerSalesApiBaseUrl,
  getStripeSecretKey,
} from "../../modules/checkout/config/runtime"
import { createSalesOrder } from "../../modules/checkout/services/salesOrderClient"
import {
  cancelStripePaymentIntent,
  confirmStripePaymentIntent,
  createStripePaymentIntent,
} from "../../modules/checkout/services/stripePaymentIntent"
import { createOrderReferenceId } from "../../modules/checkout/utils/orderReferenceId"
import { validateCheckoutOrderRequest } from "../../modules/checkout/validators/checkoutOrderRequest"
import { validateNoQueryParams } from "../../modules/checkout/validators/emptyQuery"
import { getRequestTraceContext } from "../../modules/shared/requestContext"

export default defineEventHandler(async (event) => {
  await getValidatedQuery(event, validateNoQueryParams)
  const orderRequest = await readValidatedBody(
    event,
    validateCheckoutOrderRequest,
  )

  const authorization = event.node.req.headers.authorization?.trim()
  if (!authorization) {
    throw createError({
      statusCode: 401,
      statusMessage: "Missing Authorization header.",
    })
  }

  const requestContext = getRequestTraceContext(event)
  const stripeSecretKey = getStripeSecretKey()
  const salesApiBaseUrl = getServerSalesApiBaseUrl()

  const referenceId = createOrderReferenceId()
  let paymentIntentId: string | null = null

  try {
    const stripeSession = await createStripePaymentIntent(
      orderRequest.payment.cardPayment,
      orderRequest.address,
      orderRequest.customerId,
      referenceId,
      stripeSecretKey,
      requestContext,
    )

    paymentIntentId = stripeSession.paymentIntentId

    const createdOrder = await createSalesOrder(
      orderRequest.order,
      authorization,
      salesApiBaseUrl,
      requestContext,
    )

    if (!createdOrder.id) {
      throw createError({
        statusCode: 500,
        statusMessage: "Order response did not include an order ID.",
      })
    }

    if (typeof createdOrder.totalAmount === "number") {
      const amountMismatch =
        Math.abs(createdOrder.totalAmount - orderRequest.payment.cardPayment.amount) >
        0.009

      if (amountMismatch) {
        throw createError({
          statusCode: 400,
          statusMessage: "Payment amount does not match order total.",
        })
      }
    }

    await confirmStripePaymentIntent(
      stripeSession.paymentIntentId,
      createdOrder.id,
      stripeSession.referenceId,
      stripeSecretKey,
      requestContext,
    )

    return {
      order: createdOrder,
      referenceId: stripeSession.referenceId,
    }
  } catch (error) {
    if (paymentIntentId) {
      await cancelStripePaymentIntent(
        paymentIntentId,
        stripeSecretKey,
        requestContext,
      )
    }
    throw error
  }
})
