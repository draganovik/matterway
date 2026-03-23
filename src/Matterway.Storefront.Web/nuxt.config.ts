export default defineNuxtConfig({
  modules: ["@nuxt/eslint", "@nuxt/ui"],
  ssr: true,

  components: [
    {
      path: "~/components/common",
    },
    {
      path: "~/components/features",
    },
    {
      path: "~/components/shell",
    },
  ],

  app: {
    head: {
      titleTemplate: "%s - Matterway Prodavnica",
      link: [{ rel: "icon", href: "/favicon.svg" }],
    },
  },

  css: ["~/assets/css/main.css"],

  runtimeConfig: {
    // Runtime values are injected from container env at startup.
    stripeSecretKey: "",
    serverIdentityApiBaseUrl: "",
    serverCatalogApiBaseUrl: "",
    serverCustomersApiBaseUrl: "",
    serverSalesApiBaseUrl: "",
    serverImageCdnBaseUrl: "",
    serverImageCdnBucket: "",
    systemAccessKey: "",
  },

  devtools: {
    enabled: true,
  },

  compatibilityDate: "2025-01-15",
})
