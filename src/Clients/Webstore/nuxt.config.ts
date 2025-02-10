// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  app: {
    head: {
      titleTemplate: "%s - Matterway Web Store",
    },
  },
  runtimeConfig: {
    stripeSecretKey: process.env.STRIPE_SECRET_KEY,
    public: {
      appDomain: "localhost",
      authApiBaseUrl: "http://localhost:2003",
      catalogApiBaseUrl: "http://localhost:2001",
      customersApiBaseUrl: "http://localhost:2002",
      orderingApiBaseUrl: "http://localhost:2005",
      paymentsApiBaseUrl: "http://localhost:2006",
    },
  },
  css: ["@/assets/css/main.css"],
  postcss: {
    plugins: {
      tailwindcss: {},
      autoprefixer: {},
    },
  },
  modules: ["@pinia/nuxt", "@pinia-plugin-persistedstate/nuxt"],
});
