// composable/useSessionData.ts

import { ref, Ref, watchEffect } from "vue";

interface TokenData {
  userId: number;
  username: string;
  rawToken: string;
}

export default function useSessionData(): {
  sessionData: Ref<TokenData | undefined>;
  getSessionData: () => void;
} {
  const sessionData: Ref<TokenData | undefined> = ref(undefined);
  const rawToken = useState<string | undefined>("jwtToken", undefined);

  const extractDataFromToken = (
    token: string | undefined
  ): TokenData | undefined => {
    if (!token) return undefined;

    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );

    const { userId, username } = JSON.parse(jsonPayload) as {
      userId: number;
      username: string;
      rawRefreshToken: string;
    };

    return {
      userId,
      username,
      rawToken: token,
    } satisfies TokenData;
  };

  const getSessionData = (): void => {
    const token = rawToken.value;
    sessionData.value = extractDataFromToken(token);
  };

  watchEffect(() => {
    const token = rawToken.value;
    sessionData.value = extractDataFromToken(token);
  });

  return {
    sessionData,
    getSessionData,
  };
}
