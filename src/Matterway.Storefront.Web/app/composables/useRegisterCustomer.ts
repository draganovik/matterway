import { useAuthSession } from "~/composables/useAuthSession";
import { useCustomersApi } from "~/composables/useCustomersApi";
import type { RegisterPayload } from "~/types/customers/address";

export function useRegisterCustomer() {
  const customers = useCustomersApi();
  const auth = useAuthSession();

  async function registerAndSignIn(payload: RegisterPayload) {
    const response = await customers.registerCustomer(payload);

    if (!response.ok) {
      throw new Error(response.error || "Registracija nije uspela.");
    }

    await auth.login({
      email: payload.email,
      password: payload.password,
    });
  }

  return { registerAndSignIn };
}
