import { resolve } from "node:path";
import { fileURLToPath } from "node:url";
import tailwindcss from "@tailwindcss/vite";

const projectRoot = fileURLToPath(new URL(".", import.meta.url));
const withRoot = (...segments: string[]) => resolve(projectRoot, ...segments);

const alias = {
  "@assets": withRoot("app/assets"),
  "@components": withRoot("app/components"),
  "@composables": withRoot("app/composables"),
  "@layouts": withRoot("app/layouts"),
  "@middleware": withRoot("app/middleware"),
  "@pages": withRoot("app/pages"),
  "@plugins": withRoot("app/plugins"),
  "@stores": withRoot("app/stores"),
  $api: withRoot("server/api"),
  "#models": withRoot("models"),
  "#services": withRoot("services"),
} satisfies Record<string, string>;
const isDev = process.env.NODE_ENV !== "production";
const noCacheHeaders = {
  "Cache-Control":
    "no-store, no-cache, must-revalidate, proxy-revalidate, max-age=0",
};

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  alias,
  app: {
    head: {
      titleTemplate: "%s - Matterway Storefront",
      link: [
        {
          rel: "icon",
          href: "/favicon.svg",
        },
      ],
    },
  },
  components: [
    {
      path: alias["@components"],
      pathPrefix: false,
    },
  ],
  runtimeConfig: {
    stripeSecretKey: process.env.STRIPE_SECRET_KEY,
    serverSalesApiBaseUrl:
      process.env.NUXT_SERVER_SALES_API_BASE_URL ??
      process.env.SERVER_SALES_API_BASE_URL ??
      process.env.SALES_API_BASE_URL,
    public: {
      appDomain: "localhost",
      authApiBaseUrl: process.env.AUTH_API_BASE_URL,
      catalogApiBaseUrl: process.env.CATALOG_API_BASE_URL,
      customersApiBaseUrl: process.env.CUSTOMERS_API_BASE_URL,
      salesApiBaseUrl: process.env.SALES_API_BASE_URL,
    },
  },
  css: ["@assets/css/main.css"],
  routeRules: isDev
    ? {
        "/**": {
          headers: noCacheHeaders,
        },
      }
    : undefined,
  vite: {
    plugins: [tailwindcss()],
    ...(isDev
      ? {
          server: {
            headers: noCacheHeaders,
          },
        }
      : {}),
  },
  modules: ["@pinia/nuxt", "@nuxt/icon"],
});
