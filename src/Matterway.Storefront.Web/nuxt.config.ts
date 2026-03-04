export default defineNuxtConfig({
  modules: ["@nuxt/eslint", "@nuxt/ui"],
  ssr: false,

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
      titleTemplate: "%s - Matterway prodavnica",
      link: [{ rel: "icon", href: "/favicon.svg" }],
    },
  },

  css: ["~/assets/css/main.css"],

  runtimeConfig: {
    // Runtime values are injected from container env at startup.
    stripeSecretKey: "",
    serverSalesApiBaseUrl: "",
    systemAccessKey: "",
    public: {
      identityApiBaseUrl: "",
      catalogApiBaseUrl: "",
      customersApiBaseUrl: "",
      salesApiBaseUrl: "",
    },
  },

  devtools: {
    enabled: true,
  },

  compatibilityDate: "2025-01-15",
})
