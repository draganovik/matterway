import { useSessionStore } from "~/store/session";

function baseOptions(token: string): RequestInit {
  return {
    headers: {
      "Content-Type": "application/json",
      accept: "application/json",
      Authorization: token,
    },
  };
}

export async function request(
  url: string,
  options: RequestInit,
): Promise<Response> {
  const currentSession = useSessionStore();
  options = {
    ...baseOptions(
      currentSession.getSessionData?.token
        ? `${currentSession.getSessionData?.tokenType} ${currentSession.getSessionData?.token}`
        : "",
    ),
    ...options,
  };

  if (currentSession.isSessionExpired) {
    currentSession.refreshToken();
  }

  console.log(options);
  const response = await fetch(url, options);

  if (response.ok) {
    return response;
  }
  const error = await response.json();
  throw new Error(error);
}
