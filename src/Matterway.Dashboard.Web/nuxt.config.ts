import { resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const projectRoot = fileURLToPath(new URL('.', import.meta.url))
const withRoot = (...segments: string[]) => resolve(projectRoot, ...segments)

const alias = {
  '@assets': withRoot('app/assets'),
  '@components': withRoot('app/components'),
  '@composables': withRoot('app/composables'),
  '@layouts': withRoot('app/layouts'),
  '@middleware': withRoot('app/middleware'),
  '@pages': withRoot('app/pages'),
  '@plugins': withRoot('app/plugins')
} satisfies Record<string, string>

const isDev = process.env.NODE_ENV !== 'production'
const noCacheHeaders = {
  'Cache-Control': 'no-store, no-cache, must-revalidate, proxy-revalidate, max-age=0'
}

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: [
    '@nuxt/eslint',
    '@nuxt/ui'
  ],

  devtools: {
    enabled: true
  },

  app: {
    head: {
      titleTemplate: '%s - Matterway Dashboard',
      link: [
        { rel: 'icon', href: '/favicon.ico' }
      ]
    }
  },

  css: ['~/assets/css/main.css'],

  runtimeConfig: {
    public: {
      identityApiBaseUrl: process.env.IDENTITY_API_BASE_URL,
      catalogApiBaseUrl: process.env.CATALOG_API_BASE_URL,
      customersApiBaseUrl: process.env.CUSTOMERS_API_BASE_URL,
      salesApiBaseUrl: process.env.SALES_API_BASE_URL
    }
  },
  alias,
  routeRules: isDev
    ? {
        '/**': {
          headers: noCacheHeaders
        }
      }
    : undefined,

  compatibilityDate: '2025-01-15',
  vite: isDev
    ? {
        server: {
          headers: noCacheHeaders
        }
      }
    : undefined,

  eslint: {
    config: {
      stylistic: {
        commaDangle: 'never',
        braceStyle: '1tbs'
      }
    }
  }
})
