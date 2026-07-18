# Matterway Storefront (Nuxt 4 + Nuxt UI)

Customer-facing storefront for Matterway, aligned with the same Nuxt platform style used by `Matterway.Dashboard.Web`.

## Stack

- Nuxt 4
- @nuxt/ui
- Tailwind CSS v4
- TypeScript
- Server checkout orchestration via `/api/storefront/checkout` + `/api/storefront/webhooks/stripe`

## Key Features

- Overview (front page)
- Articles browser with query-driven filters (RSQL generation kept)
- Cart for guest and signed-in customers
- Checkout flow
- Previous orders (customer accounts only)
- Customer-only login strategy (non-customer roles are rejected)

## Commands

```bash
npm run dev
npm run typecheck
npm run lint
npm run build
```

## Notes

- Server-side API URLs are configured via `NUXT_SERVER_*` environment variables.
- Browser-triggered API calls go through same-origin `/api/<service>/...` proxy routes.
- Guest cart items are merged into the customer's server cart after sign-in or
  registration; the local cart is cleared on sign-out.
- Admin/catalog management routes are disabled in Storefront.
