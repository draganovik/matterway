// stores/session.ts

import { defineStore } from "pinia";
import { Buffer } from "buffer";
import LoginModel from "#models/LoginModel";
import SessionModel from "#models/SessionModel";
import JwtModel from "#models/JwtModel";
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
      if (this.session && this.session.token) {
        const parts = this.session.token.split(".");
        if (parts.length > 1 && parts[1]) {
          try {
            return JSON.parse(Buffer.from(parts[1], "base64").toString());
          } catch (e) {
            // invalid token payload
            return null;
          }
        }
      }
      return null;
    },
  },

  actions: {
    async login(credentials: LoginModel) {
      const config = useRuntimeConfig();
      const baseUrl = `${config.public.authApiBaseUrl}/api/v1.0/Auth`;
      const response = await fetch(`${baseUrl}/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          accept: "application/json",
        },
        body: JSON.stringify(credentials),
      });
      const data = await response.json();
      if (response.ok) {
        this.setSession(data);
      }
    },

    async logout() {
      const config = useRuntimeConfig();
      const baseUrl = `${config.public.authApiBaseUrl}/api/v1.0/Auth`;
      const response = await request(`${baseUrl}/logout`, {
        method: "POST",
      });
      if (response.ok) {
        const cartStore = useCartStore();
        this.setSession(null);
        cartStore.fetchCartItems();
      }
    },
    async refreshToken() {
      const config = useRuntimeConfig();
      const baseUrl = `${config.public.authApiBaseUrl}/api/v1.0/Auth`;
      const response = await fetch(`${baseUrl}/refresh`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          accept: "application/json",
        },
        credentials: "include",
        body: JSON.stringify({
          refreshToken: this.session?.refreshToken,
        }),
      });
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
