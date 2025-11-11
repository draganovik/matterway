# Matterway Storefront (Nuxt 4)

This repository contains the Nuxt 4 storefront for Matterway. The project now follows the recommended `app` source directory structure and uses componentized form building blocks for better reuse and testability.

## Requirements

- Node.js **20.17** (LTS) or newer
- npm **10.x**

## Getting Started

```bash
npm install
npm run dev
```

The development server listens on `http://localhost:3000` by default.

## Project Structure

- `app/` – Nuxt source (components, layouts, middleware, models, pages, server endpoints, stores, etc.)
- `public/` – Static assets served as-is
- `nuxt.config.ts` – Framework configuration (Pinia modules, runtime config, CSS, etc.)
- `tailwind.config.ts` – Tailwind + Flowbite setup
- `Dockerfile` – Multi-stage production build using Node 20

Forms that previously lived directly in pages (authentication, checkout, product creation) are now extracted into typed components under `app/components/forms`.

## Scripts

```bash
npm run dev       # Start dev server
npm run build     # Production build
npm run preview   # Preview the production build locally
npm run generate  # Static site generation
```

Refer to the [Nuxt documentation](https://nuxt.com/docs) for additional guides and deployment options.
