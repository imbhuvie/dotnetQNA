# DotNetTechnicalMastery

A professional **.NET Technical Q&A Learning Platform** — 1000+ .NET/C# questions with rich,
structured answers (short answer, detailed explanation, code, internals, real-world usage,
common mistakes, discussion points, interview follow-ups, key takeaways) for study, not just
interview prep.

> **Primary architectural goal:** the frontend can be replaced (WPF → mobile) without
> rewriting the backend. All clients talk to the same HTTP + JSON API; the WPF app never
> touches the database.

## Architecture

```text
WPF (MVVM → typed HttpClient) ── HTTP + JSON (DTOs only) ──▶ ASP.NET Core API
  (thin controllers) → Application services (business rules + validation)
  → Infrastructure repositories (EF Core) → SQLite (technical_mastery.db)
```

Dependency direction: `Wpf ╌HTTP╌▶ Api → Application → Domain`;
`Infrastructure → Application + Domain`. Full rules and scope:
[`docs/project-requirements.md`](docs/project-requirements.md).
What was built in each phase: [`docs/implementation-log.md`](docs/implementation-log.md).
Original 40-section brief: [`docs/initial_instructions.md`](docs/initial_instructions.md).

## Solution layout

```text
DotNetTechnicalMastery.slnx
src/
  TechnicalMastery.Domain/          Entities, enums, base types (no dependencies)
  TechnicalMastery.Application/     Interfaces, services, DTOs, validators
  TechnicalMastery.Infrastructure/  AppDbContext, configurations, repositories, migrations, seed data
  TechnicalMastery.Api/             Controllers, middleware, configuration
  TechnicalMastery.Wpf/             MVVM views, view models, API clients
tests/TechnicalMastery.Tests/       xUnit test suite
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (10.0.102+)
- EF Core CLI is pinned per-repo: `dotnet tool restore` (no global install needed)

## Getting started

```powershell
# 1. Restore tools + packages
dotnet tool restore
dotnet build DotNetTechnicalMastery.slnx

# 2. Create / upgrade the local database
dotnet ef database update `
  --project src/TechnicalMastery.Infrastructure `
  --startup-project src/TechnicalMastery.Api

# 3. Run the API (SQLite file 'technical_mastery.db' is created beside the API)
dotnet run --project src/TechnicalMastery.Api
```

API base URL is configurable (`ConnectionStrings:DefaultConnection` in
`src/TechnicalMastery.Api/appsettings.json`); the WPF client keeps its own configurable
API base URL (no hardcoded endpoints).

## Progress

| Milestone | Phases | Status |
|---|---|---|
| M1 Solution + Domain | 1–2 | ✅ Done |
| M2 Data layer (DbContext, SQLite, migrations, repositories) | 3–6 | ✅ Done (seeding in M4) |
| M3 Backend vertical (services → controllers → Serilog/Swagger) | 7–13 | ✅ Done |
| M4 Content seeding (30/1000+ so far — see `docs/question-authoring-guide.md` tracker) + API tests 40/40 | 14–15 | ✅ Infra done, batches ongoing |
| M5 WPF client (dashboard, browse, reader, search, bookmarks, progress, notes) | 16–25 | ✅ Done |
| M6 Polish + themes, 24 xUnit tests green, final architecture review | 26–28 | ✅ Done |

## Contributing / conventions

- Explicit types over `var`, small methods, XML docs on public types.
- Business logic → Application services; controllers stay thin; DTOs over the wire (never EF entities).
- Migrations for every schema change — never `EnsureCreated()`.
