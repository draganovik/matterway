// store/session.ts

import { defineStore } from 'pinia';
import LoginModel from '~/utils/LoginModel';
import { Buffer } from 'buffer';
import SessionModel from '~/utils/SessionModel';
import { CookieRef } from 'nuxt/app';

interface SessionState {
  session: CookieRef<SessionModel | null | undefined>;
}

export const useSessionStore = defineStore('session', {
  state: (): SessionState => ({
    session: useCookie('session')
  }),

  getters: {
    isLoggedIn() : boolean {
      return this.session != null
    },
    getSessionData(): SessionModel | null {
      const token = this.session?.token
      if (token) {
        const tokenPayload = token.split('.')[1]
        const decodedToken : SessionModel = JSON.parse(Buffer.from(tokenPayload, 'base64').toString())
        return  this.session as SessionModel
      }
      return null
    },
  },

  actions: {
    async login(credentials: LoginModel) {
      const response = await fetch('https://localhost:2003/api/Sessions/create', {
        headers: {
          "Content-Type": "application/json",
          "accept" : "application/json"
        },
        method: 'POST',
        body: JSON.stringify(credentials),
      });
      console.log(JSON.stringify(credentials))
      const data = await response.json();
      if (response.ok) {
        this.setSession(data);
      }
    },

    async logout() {
      const response = await fetch('https://localhost:2003/api/Sessions/revoke', {
        method: 'POST',
      });
      if (response.ok) {
        this.setSession(null);
      }
    },

    setSession(session: SessionModel | null) {
      this.session = session;
    },
  },
});
