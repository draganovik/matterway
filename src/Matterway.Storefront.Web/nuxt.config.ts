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
// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  alias,
  app: {
    head: {
      titleTemplate: "%s - Matterway Web Store",
      link: [
        {
          rel: "icon",
          type: "image/svg+xml",
          href: "/logo.svg",
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
  css: ["@assets/css/main.css"],
  vite: {
    plugins: [tailwindcss()],
  },
  modules: ["@pinia/nuxt", "@nuxt/icon"],
});
