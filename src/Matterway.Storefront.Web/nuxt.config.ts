const env = import.meta.env as Record<string, string | undefined>

const publicEnv = (key: string) => env[`NUXT_PUBLIC_${key}`] ?? env[key]

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
    stripeSecretKey: env.STRIPE_SECRET_KEY,
    serverSalesApiBaseUrl: env.SERVER_SALES_API_BASE_URL,
    public: {
      identityApiBaseUrl:
        publicEnv("IDENTITY_API_BASE_URL") ?? publicEnv("AUTH_API_BASE_URL"),
      catalogApiBaseUrl: publicEnv("CATALOG_API_BASE_URL"),
      customersApiBaseUrl: publicEnv("CUSTOMERS_API_BASE_URL"),
      salesApiBaseUrl: publicEnv("SALES_API_BASE_URL"),
    },
  },

  devtools: {
    enabled: true,
  },

  compatibilityDate: "2025-01-15",
})
