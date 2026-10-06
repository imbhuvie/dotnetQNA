# DotNetTechnicalMastery — Requirements, Scope & Goals

> Source of truth for *what* we are building and *why*.
> Original brief: `docs/initial_instructions.md` (40 sections — read it for full detail).
> Per-phase *what was built* lives in `docs/implementation-log.md`.

## 1. Goal

A professional **.NET Technical Q&A Learning Platform**: 1000+ .NET/C# questions with rich
answers (short answer, detailed explanation, code, internals, real-world usage, common mistakes,
discussion points, interview follow-ups, key takeaways) for study — not just interview prep.

Primary architectural goal:

> **The frontend can be replaced (WPF → mobile) without rewriting the backend.**

## 2. Architecture (non-negotiable)

Client-server with complete frontend/backend separation:

```text
WPF (MVVM → typed HttpClient) ── HTTP + JSON (DTOs only) ──▶ ASP.NET Core API
  (thin controllers) → Application services (business rules + validation)
  → Infrastructure repositories (EF Core) → SQLite (technical_mastery.db)
```

Dependency direction: `Wpf ╌HTTP╌▶ Api → Application → Domain`; `Infrastructure → Application + Domain`.

### The 10 architectural rules (§38, must never be violated)

1. WPF must not access SQLite directly.
2. WPF must not contain database queries.
3. WPF must not contain business logic.
4. API must not reference WPF.
5. Domain must not reference infrastructure (nor EF, ASP.NET, WPF, SQLite).
6. Controllers stay thin.
7. Business logic lives in the Application/service layer.
8. Database logic lives in Infrastructure.
9. API ↔ clients communicate via DTOs (never EF entities).
10. The API contract stays independent of UI technology.

Backend knows only HTTP / JSON / DTOs / business rules / database — never that the client is WPF.

## 3. Technology stack (locked decisions)

| Area | Choice |
|---|---|
| Framework | .NET 10 LTS (`net10.0`; WPF `net10.0-windows`) |
| Backend | ASP.NET Core Web API, EF Core, SQLite |
| Patterns | Repository + service layer, DTOs, DI, async/await |
| Validation | FluentValidation |
| Mapping | Manual DTO mapping (no AutoMapper — mappings are trivial) |
| Logging | Serilog (no passwords/tokens/secrets/connection strings in logs) |
| API docs | Swagger/OpenAPI, every endpoint documented for future mobile devs |
| WPF | MVVM via **CommunityToolkit.Mvvm**, `IHttpClientFactory` typed clients, no logic in code-behind |
| Tests | xUnit (+ Moq, FluentAssertions, `Microsoft.AspNetCore.Mvc.Testing`) |
| DB strategy | EF Core migrations (never `EnsureCreated()` as the production path) |

## 4. Functional scope

- **Browse**: dashboard (totals, completed, bookmarks, needs-review, per-category progress),
  categories → topics → question list (search/filter/sort/pagination, all server-side) → rich question reader.
- **Search**: global, across question text, answers, category, topic, tags — API-side.
- **Bookmarks**: add/remove/list via API (persisted, not in-memory).
- **Study progress**: NotStarted / Learning / Completed / NeedsReview + percentages.
- **Notes**: personal notes per question, persisted via API.
- **Settings**: configurable API base URL (no hardcoded URLs). **About** screen.
- **API surface** (§8): categories, topics, questions (paged, filtered, search, random, related,
  by category/topic/difficulty), bookmarks, progress, notes — uniform `ApiResponse<T>` envelope
  (`success, message, data, errors`) with correct HTTP status codes.
- **Seed content**: ≥1000 questions, Beginner → Intermediate → Advanced → Production →
  Architecture → SystemDesign, across C#, OOP, collections, LINQ, .NET, DI, ASP.NET Core,
  Web API, security, EF Core, SQL, async/concurrency, testing, production scenarios,
  architecture, system design. Seeded from **JSON files** (`Infrastructure/Data/Seed/`) via a
  maintainable seeder — never inline in `Program.cs`.
- **Resilience/UX**: friendly errors (offline, timeout, 4xx/5xx), responsive layout, clean
  developer-focused reader with styled code blocks, dark/light theme if low-cost.
- **Offline**: interface seam only (`IDataSource` → API now, local cache later) — no offline
  implementation in v1.
- **Security posture**: no plain-text secrets, server-side validation always, no EF entities
  over the wire, architecture ready for future auth (incl. `Bookmark.UserId` slot).

## 5. Quality bar

SOLID / clean / DRY, explicit types over `var`, small methods, XML docs where useful,
understandable by a ~1-year developer — no over-engineering. Tests minimum:
`QuestionService`, `BookmarkService`, `StudyProgressService`, repository and controller tests
(retrieval, search, pagination, bookmark add/remove, progress update, not-found, invalid input).

## 6. Roadmap (spec §35, grouped)

- **M1** Solution + Domain (Ph 1–2) · **M2** Data layer (Ph 3–6) · **M3** Backend vertical
  (Ph 7–13) · **M4** Content seeding + API smoke tests (Ph 14–15) · **M5** WPF client
  (Ph 16–25) · **M6** Polish + tests + final architecture review (Ph 26–28).
