// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  srcDir: "app",
  app: {
    head: {
      titleTemplate: "%s - Matterway Web Store",
    },
  },
  components: [
    {
      path: "~/components",
      pathPrefix: false,
    },
  ],
  runtimeConfig: {
    stripeSecretKey: process.env.STRIPE_SECRET_KEY,
    serverOrderingApiBaseUrl:
      process.env.SERVER_ORDERING_API_BASE_URL ??
      process.env.ORDERING_API_BASE_URL,
    serverPaymentsApiBaseUrl:
      process.env.SERVER_PAYMENTS_API_BASE_URL ??
      process.env.PAYMENTS_API_BASE_URL,
    public: {
      appDomain: "localhost",
      authApiBaseUrl: process.env.AUTH_API_BASE_URL,
      catalogApiBaseUrl: process.env.CATALOG_API_BASE_URL,
      customersApiBaseUrl: process.env.CUSTOMERS_API_BASE_URL,
      orderingApiBaseUrl: process.env.ORDERING_API_BASE_URL,
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
