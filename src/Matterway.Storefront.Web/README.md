# Matterway Storefront (Nuxt 4 + Nuxt UI)

Customer-facing storefront for Matterway, aligned with the same Nuxt platform style used by `Matterway.Dashboard.Web`.

## Stack

- Nuxt 4
- @nuxt/ui
- Tailwind CSS v4
- TypeScript
- Stripe server endpoints for payment intent + webhook registration

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

- Runtime API URLs are configured via `NUXT_PUBLIC_*` environment variables.
- Cart is intentionally cleared on sign in and sign out.
- Admin/catalog management routes are disabled in Storefront.
