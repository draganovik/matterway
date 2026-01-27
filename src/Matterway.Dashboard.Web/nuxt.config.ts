import { fileURLToPath } from 'node:url'
import { resolve } from 'node:path'

const projectRoot = fileURLToPath(new URL('.', import.meta.url))
const withRoot = (...segments: string[]) => resolve(projectRoot, ...segments)
const env = process.env

const publicEnv = (key: string) => env[`NUXT_PUBLIC_${key}`] ?? env[key]

const alias = {
  '@assets': withRoot('app/assets'),
  '@components': withRoot('app/components'),
  '@composables': withRoot('app/composables'),
  '@layouts': withRoot('app/layouts'),
  '@middleware': withRoot('app/middleware'),
  '@pages': withRoot('app/pages'),
  '@plugins': withRoot('app/plugins')
} satisfies Record<string, string>

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({

  modules: [
    '@nuxt/eslint',
    '@nuxt/ui'
  ],
  ssr: false,
  components: [
    {
      path: '~/components',
      pathPrefix: false
    }
  ],

  devtools: {
    enabled: true
  },

  app: {
    head: {
      titleTemplate: '%s - Matterway Dashboard',
      link: [
        { rel: 'icon', href: '/favicon.svg' }
      ]
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
  alias,

  compatibilityDate: '2025-01-15',

  eslint: {
    config: {
      stylistic: {
        commaDangle: 'never',
        braceStyle: '1tbs'
      }
    }
  }
})
