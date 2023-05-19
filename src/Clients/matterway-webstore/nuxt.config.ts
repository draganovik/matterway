// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  app: {
    head: {
      titleTemplate: "%s - Matterway Web Store",
    },
  },
  runtimeConfig: {
    public: {
      auth_api_base_url: process.env.AUTH_API_BASE_URI,
      catalog_api_base_url: process.env.CATALOG_API_BASE_URI,
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
