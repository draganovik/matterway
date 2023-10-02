// store/session.ts

import { defineStore } from "pinia";
import { Buffer } from "buffer";
import LoginModel from "~/utils/LoginModel";
import SessionModel from "~/utils/SessionModel";
import JwtModel from "~/utils/JwtModel";
import { useCartStore } from "./cart";

interface SessionState {
  session: SessionModel | null;
}

export const useSessionStore = defineStore("session", {
  persist: true,
  state: (): SessionState => ({
    session: null,
  }),

  getters: {
    isLoggedIn(): boolean {
      const cartStore = useCartStore();
      cartStore.fetchCartItems();
      if (this.isSessionExpired) {
        this.session = null;
      }
      return this.session != null;
    },
    isSessionExpired(): boolean {
      return (
        this.session != null &&
        (this.session.expires == undefined ||
          new Date(this.session.expires) <= new Date())
      );
    },
    getSessionData(): SessionModel | null {
      return this.session;
    },
    getTokenData(): JwtModel | null {
      if (this.session) {
        return JSON.parse(
          Buffer.from(this.session.token.split(".")[1], "base64").toString(),
        );
      }
      return null;
    },
  },

  actions: {
    async login(credentials: LoginModel) {
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.authApiBaseUrl}/api/Sessions/create`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
          },
          body: JSON.stringify(credentials),
        },
      );
      const data = await response.json();
      if (response.ok) {
        this.setSession(data);
      }
    },

    async logout() {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.authApiBaseUrl}/api/Sessions/revoke`,
        {
          method: "DELETE",
        },
      );
      if (response.ok) {
        const cartStore = useCartStore();
        this.setSession(null);
        cartStore.fetchCartItems();
      }
    },
    async refreshToken() {
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.authApiBaseUrl}/api/Sessions/refresh`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
          },
          credentials: "include",
          body: JSON.stringify({
            refreshToken: this.session?.refreshToken,
            tokenType: this.session?.tokenType,
          }),
        },
      );
      const data = await response.json();
      if (response.ok) {
        this.setSession(data);
      }
    },

    setSession(session: SessionModel | null) {
      this.session = session;
    },
  },
});
