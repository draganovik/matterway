import { useSessionStore } from "@stores/session";
import LoginModel from "#models/LoginModel";

export async function createUser(
  firstName: string,
  lastName: string,
  birthDate: string, // expected format yyyy-MM-dd
  email: string,
  password: string,
): Promise<Response> {
  const config = useRuntimeConfig();

  const registerResponse = await fetch(
    `${config.public.customersApiBaseUrl}/api/v1.0/public/register`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email,
        password,
        firstName,
        lastName,
        birthDate, // DateOnly on API; keep as yyyy-MM-dd
      }),
    },
  );

  if (registerResponse.ok) {
    // auto-login with provided credentials
    const loginModel = new LoginModel();
    loginModel.email = email;
    loginModel.password = password;

    const session = useSessionStore();
    await session.login(loginModel);

    return registerResponse;
  }
  return registerResponse;
}
