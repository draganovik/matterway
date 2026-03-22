// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ["@nuxt/eslint", "@nuxt/ui"],
  ssr: true,
  colorMode: {
    preference: "system",
    storageKey: "mw-dashboard-color-mode",
  },
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

  devtools: {
    enabled: true,
  },

  app: {
    head: {
      link: [{ rel: "icon", href: "/favicon.svg" }],
    },
  },

  css: ["~/assets/css/main.css"],

  runtimeConfig: {
    // Runtime values are injected from container env at startup.
    serverIdentityApiBaseUrl: "",
    serverCatalogApiBaseUrl: "",
    serverCustomersApiBaseUrl: "",
    serverSalesApiBaseUrl: "",
    serverImageCdnBaseUrl: "",
    serverImageCdnBucket: "",
  },

  compatibilityDate: "2025-01-15",
})
