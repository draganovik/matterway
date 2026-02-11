const env = import.meta.env as Record<string, string | undefined>

const publicEnv = (key: string) => env[`NUXT_PUBLIC_${key}`] ?? env[key]

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ['@nuxt/eslint', '@nuxt/ui'],
  ssr: false,
  components: [
    {
      path: '~/components/common'
    },
    {
      path: '~/components/features'
    },
    {
      path: '~/components/shell'
    }
  ],

  devtools: {
    enabled: true
  },

  app: {
    head: {
      link: [{ rel: 'icon', href: '/favicon.svg' }]
    }
  },

  css: ['~/assets/css/main.css'],

  runtimeConfig: {
    public: {
      identityApiBaseUrl: publicEnv('IDENTITY_API_BASE_URL'),
      catalogApiBaseUrl: publicEnv('CATALOG_API_BASE_URL'),
      customersApiBaseUrl: publicEnv('CUSTOMERS_API_BASE_URL'),
      salesApiBaseUrl: publicEnv('SALES_API_BASE_URL')
    }
  },

  compatibilityDate: '2025-01-15'
})
