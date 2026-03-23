import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomersClient } from "~/composables/api/useCustomersClient"
import type { RegisterPayload } from "~/types/customers"

export function useAuthRegistrationWorkflow() {
  const customers = useCustomersClient()
  const auth = useAuthSessionStore()

  async function registerAndSignIn(payload: RegisterPayload) {
    const response = await customers.registerCustomer(payload)

    if (!response.ok) {
      throw new Error(response.error || "Registracija nije uspela.")
    }

    await auth.login({
      email: payload.email,
      password: payload.password,
    })
  }

  return { registerAndSignIn }
}
