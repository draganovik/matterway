// store/session.ts

import { defineStore } from "pinia";
import { Buffer } from "buffer";
import LoginModel from "~/utils/LoginModel";
import SessionModel from "~/utils/SessionModel";

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
      if (
        this.session?.expires == undefined ||
        new Date(this.session.expires) <= new Date()
      ) {
        this.session = null;
      }
      return this.session != null;
    },
    getSessionData(): SessionModel | null {
      return this.session;
    },
    getTokenData(): any {
      if (this.session) {
        return JSON.parse(
          Buffer.from(this.session.token.split(".")[1], "base64").toString(),
        );
      }
    },
  },

  actions: {
    async login(credentials: LoginModel) {
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.auth_api_base_url}/api/Sessions/create`,
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
      const response = await fetch(
        `${config.public.auth_api_base_url}/api/Sessions/revoke`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
            Authorization: `${this.session?.tokenType} ${this.session?.token}`,
          },
          credentials: "include",
        },
      );
      if (response.ok) {
        this.setSession(null);
      }
    },

    setSession(session: SessionModel | null) {
      this.session = session;
    },
  },
});
