import type { H3Event } from "h3"
import { getStripeSecretKey } from "../../modules/payments/config/runtime"
import { payWithStripe } from "../../modules/payments/services/stripeGateway"
import { validateCreatePaymentRequest } from "../../modules/payments/validators/createPaymentRequest"
import { validateEmptyQuery } from "../../modules/payments/validators/query"
import { getRequestTraceContext } from "../../modules/shared/requestContext"

async function handleCreatePayment(event: H3Event) {
  await getValidatedQuery(event, validateEmptyQuery)
  const paymentRequest = await readValidatedBody(
    event,
    validateCreatePaymentRequest,
  )

  const clientSecret = await payWithStripe(
    paymentRequest.cardPayment,
    paymentRequest.address,
    paymentRequest.userId,
    paymentRequest.orderId,
    getStripeSecretKey(),
    getRequestTraceContext(event),
  )

  return { clientSecret }
}

export default defineEventHandler(handleCreatePayment)
