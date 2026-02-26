# LifeSync

A personal productivity app that brings habit tracking, expense management, and workout planning into one place. I built this as a portfolio project to practice clean .NET architecture, React, and proper testing.

## What it does

- **Habits** — create habits, toggle daily completion, track streaks
- **Expenses** — log spending by category, filter by date, see breakdowns
- **Workouts** — build training plans with exercises, log sessions, track history
- **Dashboard** — a quick overview pulling data from all three modules

Everything is user-scoped with JWT authentication and refresh token rotation.

## Tech stack

| Layer    | Technologies                                                               |
| -------- | -------------------------------------------------------------------------- |
| Backend  | .NET 10, EF Core, PostgreSQL, MediatR (CQRS), FluentValidation, AutoMapper |
| Frontend | React 19, TypeScript, Tailwind CSS v4, Vite, Axios                         |
| Auth     | JWT access tokens + refresh token rotation, BCrypt                         |
| Testing  | xUnit, FluentAssertions, Testcontainers, EF Core InMemory                  |
| Infra    | Docker Compose, Nginx                                                      |

## Getting started

### With Docker (easiest)

```bash
docker compose up --build
```

This spins up PostgreSQL, the .NET API, and the React frontend. Open [localhost:3000](http://localhost:3000) and log in with:

```
demo@lifesync.dev / Password123!
```

The database gets migrated and seeded automatically on first run.

### Without Docker

You'll need .NET 10 SDK, Node.js 22+, and PostgreSQL.

**Database:**

```sql
CREATE USER lifesync WITH PASSWORD 'lifesync_dev';
CREATE DATABASE lifesync OWNER lifesync;
```

**Backend:**

```bash
cd server/LifeSync.Api
dotnet run
```

Runs on port 5001. Auto-migrates and seeds in development mode.

**Frontend:**

```bash
cd client
npm install
npm run dev
```

Runs on [localhost:3000](http://localhost:3000) with an API proxy configured.

## Running tests

```bash
cd server
dotnet test
```

**101 tests total:**

- 76 unit tests — validators, handlers, services, AutoMapper profiles
- 25 integration tests — full HTTP cycles against a real PostgreSQL via Testcontainers

Integration tests need Docker running (Testcontainers spins up a temporary Postgres container).

## Project structure

```
server/LifeSync.Api/
├── Common/                 Validation pipeline, error middleware
├── Data/                   Entities, DbContext, seed data
└── Features/               One folder per module, each with:
    ├── Auth/                 Commands (Register, Login, Refresh, Revoke)
    ├── Habits/               Commands + Queries + Controller + DTOs
    ├── Categories/           Commands + Queries + Controller + DTOs
    ├── Expenses/             Commands + Queries + Controller + DTOs
    ├── Workouts/             Commands + Queries + Controller + DTOs
    └── Dashboard/            Aggregated summary query

server/LifeSync.Api.Tests/
├── Unit/                   Validators, Handlers, Services, Mapping
└── Integration/            Full endpoint tests with Testcontainers

client/src/
├── api/                    Axios modules per feature
├── components/             UI primitives + feature components
├── contexts/               Auth context (JWT + refresh)
├── hooks/                  Data-fetching hooks per feature
├── pages/                  Route-level components
└── types/                  TypeScript interfaces matching backend DTOs
```

## How the backend is organized

Each feature follows a **CQRS pattern** where a single file co-locates the command/query record, its FluentValidation validator, and the MediatR handler. For example, `CreateExpense.cs` contains the `CreateExpenseCommand`, `CreateExpenseCommandValidator`, and `CreateExpenseCommandHandler` — all in one place.

This keeps related code together instead of spreading it across `Commands/`, `Validators/`, and `Handlers/` folders.

## API endpoints

| Method              | Endpoint                 | Auth | Description             |
| ------------------- | ------------------------ | ---- | ----------------------- |
| POST                | `/api/auth/register`     | No   | Create account          |
| POST                | `/api/auth/login`        | No   | Get tokens              |
| POST                | `/api/auth/refresh`      | No   | Rotate refresh token    |
| POST                | `/api/auth/revoke`       | No   | Revoke refresh token    |
| GET                 | `/api/dashboard`         | Yes  | Aggregated summary      |
| GET/POST/PUT/DELETE | `/api/habits`            | Yes  | Habit CRUD              |
| POST                | `/api/habits/:id/toggle` | Yes  | Toggle daily completion |
| GET/POST/PUT/DELETE | `/api/categories`        | Yes  | Category CRUD           |
| GET/POST/PUT/DELETE | `/api/expenses`          | Yes  | Expense CRUD            |
| GET                 | `/api/expenses/summary`  | Yes  | Spending breakdown      |
| GET/POST/PUT/DELETE | `/api/workouts/plans`    | Yes  | Training plan CRUD      |
| GET/POST            | `/api/workouts/logs`     | Yes  | Workout session logs    |

Expenses support query filters: `?from=2026-01-01&to=2026-01-31&categoryId=...`

## Design decisions

**Refresh token rotation** — Every refresh revokes the old token and issues a new pair. If a token gets stolen, it can only be used once before the real user's next refresh invalidates it.

**No Redux or Zustand** — React context handles auth state, custom hooks handle data fetching. Good enough for this scope without adding complexity.
