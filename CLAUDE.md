# Project: Currency Exchange Microservices

## Architecture

The project follows **Clean Architecture** and **CQRS** principles (via MediatR).

### Clean Architecture

Each microservice (UserService, FinanceService) is organized into layers:

```
Domain/          — entities and repository interfaces; no external dependencies
Application/     — commands, queries, handlers, DTOs; depends only on Domain
Infrastructure/  — EF Core, Redis, JWT; implements interfaces defined in Domain
API/             — controllers, middleware, Program.cs; depends only on Application
```

Rule: each layer only knows about the layers below it. Infrastructure is never imported from Application.

### CQRS

- **Commands** (mutate state): `RegisterUserCommand`, `LoginUserCommand`, `LogoutUserCommand`
- **Queries** (read-only): `GetUserFavoriteCurrenciesQuery`
- Each command/query has its own Handler located in the same folder

## Solution Structure

```
TrueCodeTestProject/
├── DbMigrator/                 # Console App — applies migrations and exits
├── CurrencyWorker/             # Worker Service — syncs exchange rates from CBR
├── UserService/
│   ├── UserService/            # Web API (Clean Architecture + CQRS)
│   └── UserService.Tests/      # xUnit unit tests
├── FinanceService/
│   ├── FinanceService/         # Web API (Clean Architecture + CQRS)
│   └── FinanceService.Tests/   # xUnit unit tests
├── ApiGateway/                 # YARP Gateway
├── docker-compose.yml
└── CurrencyExchange.sln
```

## Conventions

- Business logic lives in Application handlers, never in controllers
- Controllers only route the request → MediatR → response
- All dependencies are injected via constructor (primary constructors, C# 12)
- Interfaces for everything that needs to be mocked in tests
- `record` for commands, queries, and DTOs
- Errors are returned as ProblemDetails (RFC 7807)

## Running

```bash
docker-compose up
```

Starts: PostgreSQL, Redis, DbMigrator, CurrencyWorker, UserService (:5001), FinanceService (:5002), ApiGateway (:5000).

All requests are routed through the Gateway on port 5000.
