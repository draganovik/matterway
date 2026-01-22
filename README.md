# Matterway Smart Infrastructure Store

Matterway is a full-stack e-commerce platform for smart-home, homelab, server, and networking gear. It runs on a modular .NET microservice stack with a Nuxt storefront, and keeps catalog pricing simple with a single base price in RSD.

## At a glance

- Modular services: catalog, customers, identity, sales
- Nuxt 4 storefront with Pinia, Tailwind, and Flowbite
- ASP.NET Minimal APIs with versioning and OpenAPI
- Docker and .NET Aspire orchestration
- Catalog prices are stored as a single base price in RSD; discounts apply on top

## Architecture

Shared defaults live in `src/Matterway.ServiceDefaults`.

## Services

| Service | Port | Purpose |
| --- | --- | --- |
| Catalog.Api | 2001 | Article listings, imagery, filtering, discounts |
| Customers.Api | 2002 | Customer profiles, carts, address book |
| Identity.Api | 2003 | Identity, sessions, JWT issuance |
| Sales.Api | 2005 | Order creation, lifecycle, payments |
| Matterway.Storefront.Web | 3001 | Nuxt storefront (SSR build) |

## Pricing model

- Each article stores a single `BasePrice` in RSD.
- The API computes `Price` by applying the best active discount (if any).
- RSQL filtering uses the final computed price.

## Quickstart

### Prerequisites

- Docker Desktop 4.x (or Docker Engine 24+)
- .NET SDK 10.0 (preview, see `global.json`)
- Node.js 18+ for storefront development

### 1) Restore dependencies

```bash
dotnet restore Matterway.slnx
cd src/Matterway.Storefront.Web
npm install
cd ../..
```

### 2) Run the full stack with Aspire

```bash
dotnet run --project src/Matterway.AppHost
```

The storefront is exposed at `http://localhost:3001`. APIs are on ports `2001-2003` and `2005`. Swagger/Scalar is available at `/swagger` on each API.

### 3) Run services individually (optional)

```bash
dotnet watch run --project src/Matterway.Catalog.Api
dotnet watch run --project src/Matterway.Customers.Api
dotnet watch run --project src/Matterway.Identity.Api
dotnet watch run --project src/Matterway.Sales.Api
cd src/Matterway.Storefront.Web && npm run dev
```

## Project structure

```
src/
├─ Matterway.AppHost/             # .NET Aspire host
├─ Matterway.Catalog.Api/         # Article catalog service
├─ Matterway.Customers.Api/       # Customers, carts, addresses
├─ Matterway.Identity.Api/        # Identity, roles, JWT issuance
├─ Matterway.Sales.Api/           # Orders and payments
├─ Matterway.Storefront.Web/      # Nuxt storefront
└─ Matterway.ServiceDefaults/     # Shared API defaults
scripts/                          # Database + migrations helpers
```

## Database and migrations

Helper scripts in `scripts/` manage database lifecycle and EF migrations:

```bash
cd scripts
./databases_drop.sh
./migrations_remove.sh
./migrations_add.sh
./databases_update.sh
```

On Windows, use the matching `.cmd` files.

## Docs

- High-level design diagram: `docs/Matterway HLD: Class Diagram - Platform Overview.puml`

## License

This project is licensed under the MIT License. See `LICENSE.txt`.
