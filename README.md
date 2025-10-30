
# Matterway Smart Infrastructure Store

[![License: MIT](https://img.shields.io/badge/License-MIT-0f766e.svg)](https://choosealicense.com/licenses/mit/) ![Status](https://img.shields.io/badge/status-alpha-f97316.svg) ![Built with .NET](https://img.shields.io/badge/.NET-10.0-512bd4.svg) ![Nuxt](https://img.shields.io/badge/Nuxt-3-00dc82.svg)

Matterway is a full-stack e-commerce platform for smart-home, homelab, server, and networking equipment. It combines a modular microservice architecture with a modern Vue/Nuxt storefront, giving teams a solid foundation for selling premium infrastructure gear online.

> _“Matterway brings enterprise-grade network hardware, edge devices, and smart-home kits together under one cohesive shopping experience.”_

---

## Table of Contents

1. [Highlights](#highlights)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Getting Started](#getting-started)
5. [Project Structure](#project-structure)
6. [Service Catalog](#service-catalog)
7. [Database & Migrations](#database--migrations)
8. [Developer Tooling](#developer-tooling)
9. [Contributing](#contributing)
10. [License](#license)

---

## Highlights

- **Composable Storefront** – Built with Nuxt 3, Pinia, Tailwind, and Flowbite for a fast, responsive shopping experience.
- **Domain-Driven Services** – Each core capability (catalog, customers, ordering, identity, payments, inventory) is isolated in its own .NET service.
- **Modern API Surface** – ASP.NET Minimal APIs, versioned endpoints, Swagger/Scalar documentation, and structured pagination utilities.
- **Secure & Extensible** – Centralized identity service with JWT auth, shared infrastructure SDK (`Matterway.Common`), and Stripe checkout integration.
- **Developer-Friendly** – Docker-first workflow, database provisioning scripts, and consistent naming conventions across the stack.

---

## System Architecture

```
                            ┌───────────────────────────┐
                            │        Storefront         │
                            │   (Nuxt 3 / Tailwind)     │
                            └────────────┬──────────────┘
                                         │
                           API Gateway / BFF (future-ready)
                                         │
 ┌──────────────┬──────────────┬─────────┼──────────┬──────────────┬──────────────┐
 │ Catalog.Api   │ Customers.Api│ Identity.Api │ Ordering.Api │ Payments.Api │ Inventory.Api │
 │ Product data  │ Customer mesh│ Auth & JWT   │ Orders & cart│ Stripe, billing│ Stock control │
 └──────┬────────┴──────┬───────┴──────┬─────┴──────┬─────────────┴───────┬────────┘
        │               │              │            │                     │
                              SQL Server (dockerized)
```

Shared cross-cutting concerns are packaged inside `Matterway.Common` and imported by each service.

---

## Technology Stack

| Layer            | Technology |
| ---------------- | ---------- |
| Frontend         | Nuxt 3, Vue 3, Pinia, Tailwind CSS, Flowbite, Stripe |
| APIs / Services  | ASP.NET 10 Minimal APIs, Entity Framework Core, Swashbuckle, Scalar |
| Identity & Auth  | JWT Bearer, custom identity service |
| Data Layer       | Microsoft SQL Server 2022 (containerized) |
| DevOps & Infra   | Docker Compose, .NET CLI, EF Core migrations |
| Tooling          | Prettier, TypeScript, AutoMapper, Scalar UI |

---

## Getting Started

### Prerequisites

- Docker Desktop 4.x (or Docker Engine 24+)
- .NET SDK 10.0 (Preview) – aligns with the current `net10.0` target
- Node.js 18+ (for local Nuxt development)

### 1. Clone the repository

```bash
git clone https://github.com/draganovik/Matterway.git
cd Matterway
```

### 2. Bootstrap the environment

```bash
# restore .NET workloads
dotnet restore Matterway.sln

# install frontend dependencies
cd src/Matterway.Storefront.Web
npm install
cd ../..
```

### 3. Launch the stack with Docker

```bash
docker-compose up --build
```

The storefront will be available at `http://localhost:3001`, while the APIs are exposed on ports `2001-2006`. Scalar or Swagger UI for each service can be accessed via `/swagger` once the containers are up.

> Prefer running services individually? Each API is a standalone ASP.NET Minimal API – use `dotnet watch run --project src/<Service>.Api` and Nuxt’s `npm run dev` for the storefront.

---

## Project Structure

```
src/
├─ Catalog.Api/        # Product catalog service
├─ Customers.Api/      # Customer accounts & carts
├─ Identity.Api/       # Identity, sessions, JWT issuance
├─ Inventory.Api/      # Inventory tracking
├─ Ordering.Api/       # Order orchestration
├─ Payments.Api/       # Payments & Stripe integration
├─ Matterway.Common/  # Shared contracts, brokers, helpers
└─ Matterway.Storefront.Web/     # Nuxt storefront
scripts/               # Migration & database automation (sh/cmd)
docker-compose.yml     # Multi-service orchestration
```

---

## Service Catalog

| Service          | Port | Description |
| ---------------- | ---- | ----------- |
| Catalog.Api      | 2001 | Product listings, imagery, filtering, pagination |
| Customers.Api    | 2002 | Customer profiles, carts, and onboarding flows |
| Identity.Api     | 2003 | Token introspection, session lifecycle, user roles |
| Inventory.Api    | 2004 | Stock levels, warehouse sync (stub for expansion) |
| Ordering.Api     | 2005 | Order processing, order history, addresses |
| Payments.Api     | 2006 | Payment intents, Stripe webhook processing |
| Matterway.Storefront.Web   | 3001 | Nuxt storefront (SSR build) |

Each service ships with dedicated features (endpoints, mapping profiles, repositories) following a consistent folder structure.

---

## Database & Migrations

All EF Core migrations/scripts live under `scripts/`. The helpers accept both `.sh` (Unix) and `.cmd` (Windows) workflows.

```bash
# from repo root
cd scripts

# wipe & recreate databases
./databases_drop.sh

# regenerate migrations (shared conventions)
./migrations_remove.sh
./migrations_add.sh

# apply migrations
./databases_update.sh
```

On Windows run the matching `.cmd` files. Each script iterates over `src/*.Api` automatically—no manual path tweaks required.

---

## Developer Tooling

- **Code Quality**: Prettier for the frontend, dotnet format recommended for APIs.
- **API Documentation**: Swagger/Scalar is auto-registered; run any service and browse `/swagger`.
- **Testing (roadmap)**: Unit/integration test harnesses will be staged as the domain stabilizes.
- **Conventions**: Services share DTOs/helpers via `Matterway.Common`. Keep shared logic in that library to avoid duplication.

---

## Contributing

We welcome contributions that improve the storefront experience, extend microservice capabilities, or tighten DevOps workflows.

1. Fork the repo and create a feature branch.
2. Follow existing naming conventions (`<Domain>.Api` etc.).
3. Add or update documentation when altering public contracts.
4. Submit a PR with a clear description of changes and test notes.

---

## License

This project is licensed under the [MIT License](LICENSE.txt).

---

Maintained by [@draganovik](https://github.com/draganovik). Reach out for partnership opportunities or to explore how Matterway can power your smart infrastructure storefront. 🚀
