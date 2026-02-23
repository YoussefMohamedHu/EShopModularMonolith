# EShop Modular Monolith

A .NET 8 e-commerce backend built with the **Modular Monolith** architectural pattern. All modules run in a single process but are strictly decoupled — each module owns its own database schema, domain model, and API surface. Cross-module communication uses either in-process MediatR (for synchronous queries) or asynchronous integration events via MassTransit + RabbitMQ (for cross-module side effects).

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Modules](#modules)
- [Shared Libraries](#shared-libraries)
- [Technology Stack](#technology-stack)
- [Infrastructure](#infrastructure)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
- [Key Integration Flows](#key-integration-flows)
- [Project Structure](#project-structure)

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Bootstrapper (ASP.NET Core)           │
│                                                         │
│  ┌──────────┐    ┌──────────┐    ┌──────────────────┐  │
│  │ Catalog  │    │  Basket  │    │      Order       │  │
│  │  Module  │    │  Module  │    │      Module      │  │
│  └────┬─────┘    └────┬─────┘    └────────┬─────────┘  │
│       │               │                   │            │
│  [Catalog DB]    [Basket DB]         [Order DB]        │
│   schema:         schema:             schema:          │
│   Catalog         Basket              Ordering         │
│                                                         │
│  └─────────────────────────────────────────────────────┘
│              Single PostgreSQL Database                  │
└──────────────────────────┬──────────────────────────────┘
                           │ async events
                    ┌──────▼──────┐
                    │  RabbitMQ   │
                    └─────────────┘
```

### Key Architectural Decisions

- **One process, three modules** — modules share a host but are isolated by namespace, assembly, and database schema
- **No direct module-to-module references** — cross-module reads use `Catalog.Contracts` (a thin contracts-only project); cross-module writes use integration events
- **Outbox pattern** — integration events are saved to the database atomically with the business transaction, then published to RabbitMQ by a background processor, guaranteeing at-least-once delivery
- **Price authority is always Catalog** — basket item prices are always fetched from the Catalog module at write time; client-supplied prices are ignored
- **Redis cache decorator** — basket reads go through a `CachedBasketRepository` (Scrutor decorator pattern) backed by Redis

---

## Modules

### Catalog Module

Manages the product catalog. Owns the `Catalog` database schema.

**Responsibilities:**
- CRUD for products (name, description, price, category, image)
- Seeding initial product data on startup
- Publishing `ProductPriceChangedIntegrationEvent` when a product price is updated

**Domain Events:**
| Event | Trigger | Handler |
|-------|---------|---------|
| `ProductCreatedEvent` | Product created | `ProductCreatedEventHandler` (logs) |
| `ProductPriceChangedEvent` | Product price updated | `ProductPriceChangedEventHandler` → publishes `ProductPriceChangedIntegrationEvent` |

---

### Basket Module

Manages shopping carts. Owns the `Basket` database schema and the Redis cache.

**Responsibilities:**
- Create, read, and delete shopping carts
- Add/remove/update items (prices resolved from Catalog via in-process MediatR)
- Checkout: snapshot cart into an integration event, persist to outbox, delete cart
- Listen for `ProductPriceChangedIntegrationEvent` and update affected basket items

**Outbox Pattern:**
`CheckoutBasketHandler` writes a `BasketCheckoutIntegrationEvent` row to the `OutboxMessages` table and deletes the basket in a **single atomic transaction**. `OutboxProcessor` (BackgroundService) polls the table and publishes pending messages to RabbitMQ via MassTransit.

**Integration Event Consumers:**
| Consumer | Event | Action |
|----------|-------|--------|
| `ProductPriceChangedIntegrationEventHandler` | `ProductPriceChangedIntegrationEvent` | Updates price on all basket items matching the product |

---

### Order Module

Manages orders. Owns the `Ordering` database schema.

**Responsibilities:**
- Create, read, and delete orders
- Listen for `BasketCheckoutIntegrationEvent` and create an order from it
- Order contains: shipping address, payment info, line items with price snapshots

**Integration Event Consumers:**
| Consumer | Event | Action |
|----------|-------|--------|
| `BasketCheckoutIntegrationEventHandler` | `BasketCheckoutIntegrationEvent` | Creates an order from the embedded basket items |

> **Note:** The basket items are embedded directly in the integration event (not fetched after the fact). This guarantees the order is created from the exact cart state at checkout time, even though the basket is deleted before the event is consumed.

---

## Shared Libraries

### `Shared.Base`

Framework-level building blocks used by all modules:

| Component | Purpose |
|-----------|---------|
| `Entity<T>` / `Aggregate<T>` | DDD base classes with domain event support |
| `IDomainEvent` | Marker interface for domain events |
| `AuditableEntityInterceptor` | EF Core interceptor — auto-sets `CreatedAt` / `UpdatedAt` |
| `DispatchDomainEventsInterceptor` | EF Core interceptor — dispatches domain events via MediatR before SaveChanges |
| `ValidationBehavior<,>` | MediatR pipeline — runs FluentValidation before every handler |
| `LoggingBehavior<,>` | MediatR pipeline — logs request/response timing |
| `PaginationRequest` / `PaginatedResult<T>` | Shared pagination primitives |
| `IDataSeeder` | Contract for startup data seeders |
| `CarterExtensions` | Registers all Carter modules for minimal API endpoint discovery |

### `Shared.Messaging`

Cross-module messaging contracts and MassTransit configuration:

| Component | Purpose |
|-----------|---------|
| `IntegrationEvent` | Base record — `EventId` (Guid), `OccuredOn` (DateTime) |
| `BasketCheckoutIntegrationEvent` | Published by Basket on checkout; consumed by Order |
| `ProductPriceChangedIntegrationEvent` | Published by Catalog on price change; consumed by Basket |
| `MassTransitExtension` | Configures MassTransit + RabbitMQ, registers all consumers from all module assemblies |

### `Catalog.Contracts`

A thin contracts-only project that allows the Basket module to query Catalog data in-process via MediatR **without taking a hard dependency on the full Catalog assembly**:

| Component | Purpose |
|-----------|---------|
| `GetProductByIdQuery` | MediatR query — returns `ProductMinimalDataDto` |
| `ProductMinimalDataDto` | Minimal product projection (id, name, price) |

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| Framework | ASP.NET Core 8, Minimal APIs |
| ORM | Entity Framework Core 8 (Npgsql) |
| Database | PostgreSQL |
| Cache | Redis (StackExchange.Redis) |
| Messaging | MassTransit 8 + RabbitMQ |
| In-process messaging | MediatR 14 |
| Validation | FluentValidation |
| API routing | Carter |
| Authentication | Keycloak (JWT Bearer via `Keycloak.AuthServices`) |
| Logging | Serilog → Console + Seq |
| DI decoration | Scrutor |

---

## Infrastructure

All infrastructure services are provided via Docker Compose.

### Start infrastructure

```bash
docker compose up -d
```

### Services

| Service | Port | Purpose |
|---------|------|---------|
| PostgreSQL | `5433` | Primary database (all module schemas) |
| Redis | `6379` | Basket cache |
| RabbitMQ | `5672` (AMQP), `15672` (Management UI) | Message broker |
| Keycloak | `9090` | Identity provider (realm: `myrealm`, client: `myclient`) |
| Seq | `5341` | Structured log viewer |

### Connection strings (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5433;Database=eshopdb;User Id=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "MessageBroker": {
    "Host": "amqp://localhost:5672",
    "Username": "guest",
    "Password": "guest"
  }
}
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run

```bash
# 1. Start infrastructure
docker compose up -d

# 2. Restore and build
dotnet build EShopModularMonoliths.sln

# 3. Apply EF Core migrations (run from repo root)
dotnet ef database update --project src/Modules/Catalog/catalog.csproj --startup-project src/Bootstrapper/Eshop.api.csproj
dotnet ef database update --project src/Modules/Basket/basket.csproj --startup-project src/Bootstrapper/Eshop.api.csproj
dotnet ef database update --project src/Modules/Order/order.csproj --startup-project src/Bootstrapper/Eshop.api.csproj

# 4. Run the API
dotnet run --project src/Bootstrapper/Eshop.api.csproj
```

The API will be available at `http://localhost:5000`.

Swagger UI: `http://localhost:5000/swagger`

### API Testing (Bruno)

The `EShopModules/` directory contains a [Bruno](https://www.usebruno.com/) API collection with ready-made requests for every endpoint.

```
EShopModules/
├── environments/local.yml     ← base URL: http://localhost:5000
├── Identity/                  ← get JWT tokens from Keycloak
├── Catalog/                   ← product CRUD
├── Basket/                    ← basket + checkout
└── Order/                     ← order CRUD
```

Open the `EShopModules/` folder as a collection in Bruno to use it.

---

## API Reference

### Catalog

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/products` | Get all products (paginated) |
| `GET` | `/products/{id}` | Get product by ID |
| `GET` | `/products/category/{category}` | Get products by category |
| `POST` | `/products` | Create product |
| `PUT` | `/products` | Update product |
| `DELETE` | `/products/{id}` | Delete product |

### Basket

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/basket/{userName}` | Get basket |
| `POST` | `/basket` | Create or update basket (requires auth) |
| `DELETE` | `/basket/{userName}` | Delete basket |
| `POST` | `/basket/additem` | Add item to basket |
| `DELETE` | `/basket/removeitem/{itemId}` | Remove item from basket |
| `POST` | `/basket/checkout` | Checkout basket → triggers order creation |

### Order

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/orders` | Get all orders (paginated) |
| `GET` | `/orders/{id}` | Get order by ID |
| `POST` | `/orders` | Create order manually |
| `DELETE` | `/orders/{id}` | Delete order |

---

## Key Integration Flows

### 1. Basket Checkout → Order Creation

```
POST /basket/checkout
        │
        ▼
CheckoutBasketHandler
  ├─ Validates basket exists
  ├─ Writes BasketCheckoutIntegrationEvent (with all items) to OutboxMessages table
  └─ Deletes basket
        │  (atomic DB transaction)
        ▼
OutboxProcessor (BackgroundService)
  └─ Polls OutboxMessages, publishes to RabbitMQ via MassTransit
        │
        ▼
BasketCheckoutIntegrationEventHandler (Order module)
  └─ Reads items from event payload → CreateOrderCommand → Order persisted
```

> Basket items are embedded in the event so the order can always be created from the exact cart state at checkout time, regardless of when the message is consumed.

### 2. Product Price Change → Basket Price Sync

```
PUT /products  (price changed)
        │
        ▼
UpdateProductHandler
  └─ Raises ProductPriceChangedEvent (domain event)
        │
        ▼
DispatchDomainEventsInterceptor (EF Core SaveChanges)
        │
        ▼
ProductPriceChangedEventHandler
  └─ Publishes ProductPriceChangedIntegrationEvent via MassTransit
        │
        ▼
ProductPriceChangedIntegrationEventHandler (Basket module)
  └─ Updates price on all basket items matching the product ID
```

---

## Project Structure

```
EShopModularMonolith/
├── docker-compose.yml
├── docker-compose.override.yml
├── EShopModularMonoliths.sln
│
├── src/
│   ├── Bootstrapper/               ← ASP.NET Core host, Program.cs, appsettings.json
│   │
│   ├── Modules/
│   │   ├── Catalog/
│   │   │   ├── catalog.csproj
│   │   │   └── Catalog.Contracts/  ← thin query contracts for cross-module use
│   │   ├── Basket/
│   │   │   └── basket.csproj
│   │   └── Order/
│   │       └── order.csproj
│   │
│   └── Shared/
│       ├── Shared.Base/            ← DDD base classes, EF interceptors, MediatR behaviors
│       └── Shared.Messaging/       ← MassTransit config, integration event contracts
│
└── EShopModules/                   ← Bruno API collection
    ├── environments/
    ├── Identity/
    ├── Catalog/
    ├── Basket/
    └── Order/
```
