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
        this.session?.expires <= new Date()
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
      const response = await fetch(
        "https://localhost:2003/api/Sessions/create",
        {
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
          },
          method: "POST",
          body: JSON.stringify(credentials),
        },
      );
      const data = await response.json();
      if (response.ok) {
        this.setSession(data);
      }
    },

    async logout() {
      const response = await fetch(
        "https://localhost:2003/api/Sessions/revoke",
        {
          method: "DELETE",
          headers: {
            Authorization: `${this.session?.tokenType} ${this.session?.token}`,
            "Content-Type": "application/json",
            accept: "application/json",
          },
          credentials: "include",
        },
      );
      if (response.ok) {
        this.setSession(null);
      }
      console.log(response);
    },

    setSession(session: SessionModel | null) {
      this.session = session;
    },
  },
});
