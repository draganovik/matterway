import { useSessionStore } from "@stores/session";
import LoginModel from "#models/LoginModel";

export async function createUser(
  firstName: string,
  lastName: string,
  birthDate: Date,
  email: string,
  password: string,
): Promise<Response> {
  const config = useRuntimeConfig();

  const createUser = await fetch(
    `${config.public.authApiBaseUrl}/api/v1.0/public/auth/signup`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: email,
        password: password,
      }),
    },
  );
  if (createUser.ok) {
    const user = await createUser.json();
    const loginModel = new LoginModel();
    loginModel.email = email;
    loginModel.password = password;

    const session = useSessionStore();
    await session.login(loginModel);

    const createCustomer = await request(
      `${config.public.customersApiBaseUrl}/api/v1.0/self/profile`,
      {
        method: "POST",
        body: JSON.stringify({
          systemUserId: user.id,
          firstName: firstName,
          lastName: lastName,
          birthDate: birthDate,
        }),
      },
    );

    return createCustomer;
  }
  return createUser;
}
