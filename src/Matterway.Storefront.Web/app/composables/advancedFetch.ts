import { useSessionStore } from "@stores/session";

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
  const authToken = currentSession.getSessionData?.token
    ? `${currentSession.getSessionData?.tokenType} ${currentSession.getSessionData?.token}`
    : "";

  const base = baseOptions(authToken);
  const mergedHeaders = {
    ...(base.headers as Record<string, string>),
    ...((options.headers as Record<string, string>) ?? {}),
  };

  const finalOptions: RequestInit = {
    ...base,
    ...options,
    headers: mergedHeaders,
  };

  if (options.body instanceof FormData) {
    delete (finalOptions.headers as Record<string, string>)["Content-Type"];
  }

  if (currentSession.isSessionExpired) {
    currentSession.refreshToken();
  }

  const response = await fetch(url, finalOptions);

  if (response.ok) {
    return response;
  }
  const error = await response.json();
  throw new Error(error.title);
}
