# Matterway Platform

> A modular, service-oriented commerce platform built with .NET 10,
> PostgreSQL, MinIO, and Nuxt 4 --- orchestrated through .NET Aspire.

Matterway is a full-stack commerce platform designed as a
service-oriented backend with two modern Nuxt-based web applications:

-   **Storefront.Web** --- Customer-facing SSR shopping experience
-   **Dashboard.Web** --- Internal SSR ERP / administration console

The system follows strict service ownership boundaries (Catalog,
Customers, Identity, Sales), centralized schema migration execution, and
standardized platform defaults via `Matterway.ServiceDefaults`.

------------------------------------------------------------------------

## Architecture Overview

Matterway follows a distributed service architecture where:

-   Each API owns its data model and persistence schema
-   Cross-service communication is performed via typed HTTP clients
-   Relational data is stored in PostgreSQL (separate database per
    service)
-   Media assets are stored in MinIO (S3-compatible object storage)
-   Orchestration and environment wiring are handled by `.NET Aspire`

``` mermaid
flowchart LR
  Storefront["Storefront Web"] --> Catalog["Catalog API"]
  Storefront --> Customers["Customers API"]
  Storefront --> Identity["Identity API"]
  Storefront --> Sales["Sales API"]

  Dashboard["Dashboard Web"] --> Catalog
  Dashboard --> Customers
  Dashboard --> Identity
  Dashboard --> Sales

  Customers --> Catalog
  Customers --> Identity
  Sales --> Customers

  Migrations["Migration Runner"] --> Postgres
  Catalog --> Minio["MinIO Object Storage"]
  Catalog --> Postgres["PostgreSQL"]
  Customers --> Postgres
  Identity --> Postgres
  Sales --> Postgres
```

------------------------------------------------------------------------

## Core Design Principles

### Backend Architecture

-   **Style**: ASP.NET Core Minimal APIs
-   **Structure**: Feature-first layout (`Endpoints/<Scope>/<Endpoint>`)
-   **Versioning**: URL segment versioning (`v1`)
-   **Data ownership**: Each service owns its schema and database
-   **Cross-service calls**: Typed HttpClients with resilient defaults

### API Routing Convention

All APIs follow a consistent route format:

    /api/{scope}/v{version}/...

Where scope is:

-   `public` --- Anonymous endpoints
-   `self` --- Authenticated user accessing own resources
-   `admin` --- Operator / employee workflows
-   `system` --- Trusted service-to-service endpoints

Example:

    /api/public/v1/auth
    /api/self/v1/orders
    /api/admin/v1/orders

------------------------------------------------------------------------

## Authentication & Authorization

-   JWT access + refresh tokens issued by **Identity.Api**
-   Endpoints can explicitly require shared system access key (`X-System-Access-Key`)
-   Role-based and permission-claim (`perm`) authorization
-   Permission levels:
    -   `observer`
    -   `operator`
    -   `administrator`
-   Enforcement handled via shared helpers in
    `Matterway.ServiceDefaults`

------------------------------------------------------------------------

## Services and Ports (Default Development Setup)

| Component           | Port  | Responsibility                                     |
|---------------------|-------|----------------------------------------------------|
| Catalog.Api         | 2001  | Articles, pricing, discounts, image metadata       |
| Customers.Api       | 2002  | Profiles, addresses, carts, order mirror           |
| Identity.Api        | 2003  | Authentication, token issuance, permissions        |
| Sales.Api           | 2005  | Order lifecycle, payments, status tracking         |
| Storefront.Web      | 3001  | Customer SSR app (Nuxt 4)                          |
| Dashboard.Web       | 3002  | Admin SSR app (Nuxt 4)                             |
| PostgreSQL          | 15432 | Relational datastore                               |
| MinIO API           | 19000 | Object storage API                                 |
| MinIO Console       | 19001 | Storage admin console                              |
| Aspire Dashboard    | 18888 | Local orchestration + telemetry                    |

## Repository Structure

    src/
      Matterway.AppHost/          Aspire orchestration & composition root
      Matterway.ServiceDefaults/  Shared platform defaults (auth, OpenAPI, telemetry)
      Matterway.Catalog.Api/      Catalog domain + MinIO integration
      Matterway.Customers.Api/    Customer domain
      Matterway.Identity.Api/     ASP.NET Identity + JWT service
      Matterway.Migrations/       EF Core migration runner
      Matterway.Sales.Api/        Sales domain
      Matterway.Storefront.Web/   Nuxt customer application
      Matterway.Dashboard.Web/    Nuxt admin application

    scripts/                      EF migration + DB lifecycle helpers
    data/                         Local artifacts (catalog archives, etc.)
    docs/                         Architecture diagrams (PlantUML)

------------------------------------------------------------------------

## Local Development

### Prerequisites

-   Docker (PostgreSQL + MinIO)
-   .NET SDK 10.x
-   Node.js 20+
-   dotnet-ef tool
-   Aspire CLI
-   Stripe CLI (for webhook testing)

------------------------------------------------------------------------

### 1️⃣ Restore Dependencies

``` bash
dotnet restore Matterway.slnx
npm install --prefix src/Matterway.Storefront.Web
npm install --prefix src/Matterway.Dashboard.Web
```

------------------------------------------------------------------------

### 2️⃣ Configure AppHost Settings (Development)

This section is for local development.

AppHost currently includes shared defaults in `src/Matterway.AppHost/appsettings.json`.
You can add optional environment-specific overrides in the same folder (for example
`appsettings.Development.json`, `appsettings.Production.json`, or other
`appsettings.{Environment}.json` files) for non-secret values.

Set required secret parameters via user-secrets:

``` bash
dotnet user-secrets set "Parameters:JwtSigningKey" "<value>" --project src/Matterway.AppHost
dotnet user-secrets set "Parameters:SystemAccessKey" "<value>" --project src/Matterway.AppHost
dotnet user-secrets set "Parameters:PostgresPassword" "<value>" --project src/Matterway.AppHost
dotnet user-secrets set "Parameters:MinioRootUser" "<value>" --project src/Matterway.AppHost
dotnet user-secrets set "Parameters:MinioRootPassword" "<value>" --project src/Matterway.AppHost
dotnet user-secrets set "Parameters:StripeSecretKey" "<value>" --project src/Matterway.AppHost
```

For deployed environments, prefer environment variables or your platform's secret
store for sensitive values.

Optional non-secret configuration:
- `Apis:AccessOrigins:*`
- `Cors:AllowedOrigins`

------------------------------------------------------------------------

### 3️⃣ Run in Debug Mode

``` bash
dotnet run --project src/Matterway.AppHost
```

Apply database migrations:

``` bash
./scripts/databases_update.sh
```

Stripe webhook forwarding:

``` bash
stripe listen --forward-to http://localhost:3001/api/v1/webhooks/stripe
```

Checkout orchestration endpoint:

``` bash
POST http://localhost:3001/api/v1/orders
```

------------------------------------------------------------------------

### 4️⃣ E2E Deployment (Containerized)

``` bash
aspire deploy --environment production
```

Ensure production configuration values are set before deploying.

------------------------------------------------------------------------

## Database & Migration Strategy

Migration management is separated into:

-   `Matterway.Migrations` --- Runtime migrator resource
-   `scripts/*` --- Development EF Core CLI helpers

Common workflows:

``` bash
./scripts/databases_drop.sh
./scripts/databases_update.sh
```

------------------------------------------------------------------------

## Observability

Provided by `Matterway.ServiceDefaults`:

-   OpenTelemetry instrumentation
-   Resilient HttpClient defaults
-   ProblemDetails standardization
-   OpenAPI generation
-   Centralized auth & CORS configuration

### Web app tracing (Storefront server only)

Nuxt OTEL server tracing is currently wired only for `Matterway.Storefront.Web`.

The storefront Nuxt server reads OTLP settings from environment (`OTEL_EXPORTER_OTLP_*`,
`OTEL_SERVICE_NAME`) and falls back to
`DOTNET_DASHBOARD_OTLP_ENDPOINT_URL` when no explicit OTLP endpoint is provided.

`Matterway.Dashboard.Web` currently has no server-side OTLP tracing setup.

------------------------------------------------------------------------

## Documentation Assets

Located in `docs/`:

-   Platform class diagram
-   Checkout sequence diagram

------------------------------------------------------------------------

## License

MIT License --- see `LICENSE.txt`.
