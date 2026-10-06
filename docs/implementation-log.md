# Implementation Log — Features Built Per Phase

> Living record of *what was actually implemented* in each phase.
> Goals and constraints live in `docs/project-requirements.md`.

## Phase 1 — Solution & Projects ✅ (2026-10-06)

- Created `DotNetTechnicalMastery.slnx` (.NET 10 SDK solution format) with 6 projects,
  all `net10.0` (WPF: `net10.0-windows` + `UseWPF`), nullable + implicit usings on:
  - `src/TechnicalMastery.Domain`, `src/TechnicalMastery.Application`,
    `src/TechnicalMastery.Infrastructure` (classlibs)
  - `src/TechnicalMastery.Api` (webapi, controllers)
  - `src/TechnicalMastery.Wpf` (WPF `WinExe`)
  - `tests/TechnicalMastery.Tests` (xUnit)
- Wired references: Application→Domain; Infrastructure→Domain+Application;
  Api→Application+Infrastructure; Tests→all four. **WPF references no server project.**
- Scaffolded folder structure per architecture (Controllers/Middleware/Extensions/
  Configuration, Interfaces/Services/DTOs/Validators/Mappings, Entities/Enums/Common,
  Data(+Seed)/Repositories/Configurations/Migrations, Views/ViewModels/Models/
  Services/Commands/Converters/Resources); removed template boilerplate
  (`Class1.cs` ×3, `WeatherForecast`); added `.gitignore`.
- Verified: `dotnet build` — 0 errors (6/6 projects; benign NU1903 on transitive
  `Microsoft.OpenApi`, resolved in Phase 13).
- Notes: `dotnet new wpf -f net10.0-windows` is rejected — omit `-f`, template self-targets `-windows`.

## Phase 2 — Domain Entities ✅ (2026-10-06)

- Enums: `DifficultyLevel` (Beginner→SystemDesign, 6 levels), `QuestionType`
  (Conceptual/CodeBased/ScenarioBased/Debugging/Design), `StudyStatus`
  (NotStarted/Learning/Completed/NeedsReview) — all `int`-backed, in `Domain/Enums/`.
- `Common/BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`) for audited entities
  (Category, Question, QuestionNote); Topic/QuestionTag/Bookmark/StudyProgress define
  exactly the spec §5 fields standalone.
- Entities in `Domain/Entities/`, zero dependencies on EF/ASP.NET/WPF:
  - `Category` (Name unique — enforced in Phase 3 config) + `Topics` navigation.
  - `Topic` (`CategoryId` FK + `Category`, `Questions`) — no audit timestamps per spec.
  - `Question` — all 13 content columns incl. `TechnicalConversation` and
    `InterviewFollowUp` (spec §6), `DifficultyLevel`/`QuestionType` enums, `IsActive`;
    navigations: `Topic`, `Tags`, `Bookmarks` (collection — multi-user-proof),
    `Progress` (single), `Notes`.
  - `QuestionTag`, `Bookmark` (with `UserId`-extension note for future auth),
    `StudyProgress` (one row/question, nullable `LastViewedAt`/`CompletedAt`),
    `QuestionNote` (audited via `BaseEntity`).
- Naming/style: explicit types, XML docs on every type, `string.Empty` defaults,
  `null!` on required navigations (populated by EF in Phase 3+).
- Verified: `dotnet build` — 0 errors.

## Phase 3 — EF Core DbContext ✅ (2026-10-06)

- NuGet (Infrastructure): `Microsoft.EntityFrameworkCore.Sqlite 10.0.12`,
  `Microsoft.EntityFrameworkCore.Design 10.0.12` (Design enables CLI migrations in Phase 5).
- 7 Fluent API configurations in `Infrastructure/Configurations/` (no data annotations
  on entities — Domain stays persistence-ignorant):
  - `Category`: `Name` required (200), unique; `Description` ≤1000; `IsActive` default true.
  - `Topic`: composite unique `(CategoryId, Name)`; cascade Category→Topics.
  - `Question`: required text fields; enums stored as `int` (explicit `HasConversion<int>`);
    indexes on `TopicId` and `DifficultyLevel` (list/search filters); cascades to
    Tags/Bookmarks/Notes.
  - `QuestionTag`: `Tag` required (100); composite unique `(QuestionId, Tag)`.
  - `Bookmark`: unique `QuestionId` (one bookmark/question until `UserId` arrives).
  - `StudyProgress`: unique `QuestionId`; `Status` default `NotStarted`; 1-to-1 with
    Question (FK on `StudyProgress`), cascade delete.
  - `QuestionNote`: required `NoteText`; index on `QuestionId`.
- `Infrastructure/Data/AppDbContext.cs`: 7 `DbSet`s (progress exposed as
  `StudyProgressEntries` since `StudyProgress` is also a type name); options-only
  constructor (connection string comes from the API host in Phase 4, never hardcoded);
  `ApplyConfigurationsFromAssembly` auto-discovers configs.
- Verified: `dotnet build` — 0 errors.

## Phase 4 — SQLite Database ✅ (2026-10-06)

- Connection strings: `ConnectionStrings:DefaultConnection = "Data Source=technical_mastery.db"`
  in both `Api/appsettings.json` and `appsettings.Development.json` (Development also raises
  `Microsoft.EntityFrameworkCore` log level to Information).
- `Infrastructure/DependencyInjection.cs`: `AddInfrastructure(connectionString)` extension —
  Api gets the database via one line, without referencing EF Core itself (Rule 8).
- `Api/Program.cs`: resolves the connection string (throws a clear error if missing) and
  calls `AddInfrastructure`. No WeatherForecast remnants; no hardcoded paths.
- Tooling: local `dotnet-tools.json` manifest pinning `dotnet-ef 10.0.12` (matches packages;
  the machine-global 9.0.3 cannot target EF 10). `Microsoft.EntityFrameworkCore.Design 10.0.12`
  also added to Api — the Design package has `PrivateAssets=all`, so it does not flow
  transitively and the startup project needs it directly.
- Database created: `src/TechnicalMastery.Api/technical_mastery.db` (16 KB) via
  `dotnet ef database update` — proves host build + connection string + SQLite provider
  end-to-end. Contains only `__EFMigrationsHistory`; schema tables land with `InitialCreate`
  in Phase 5. No `EnsureCreated()` anywhere (per §30).
- `.gitignore`: `*.db`, `*.db-wal`, `*.db-shm` — the database is generated locally via
  migrations + seeding, never committed.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 5 — Migrations ✅ (2026-10-06)

- `dotnet ef migrations add InitialCreate` → 3 files in `Infrastructure/Migrations/`:
  `20261006170557_InitialCreate.cs`, `.Designer.cs`, `AppDbContextModelSnapshot.cs`.
- `dotnet ef database update` → `InitialCreate` recorded in `__EFMigrationsHistory`
  (ProductVersion 10.0.12). Schema verified table-by-table against spec §5 — all 7 tables
  with exact columns: Categories, Topics, Questions (15 content/fkey columns),
  QuestionTags, Bookmarks, StudyProgress, QuestionNotes (+ EF `__EFMigrationsHistory`).
- How migrations work (§30, as promised): each migration is a versioned `Up()`/`Down()`
  step; `__EFMigrationsHistory` tracks applied steps so `database update` moves any
  environment from any version to latest — unlike `EnsureCreated()`, which builds schema
  once with no history, no upgrades, and no downgrades. Never used here.
- Gotcha hit: `database update --no-build` right after `migrations add` runs against the
  stale assembly ("No migrations were found") — the `migrations add` build does not include
  the just-scaffolded files. Fix: run `database update` with its normal build step.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 6 — Repositories ✅ (2026-10-07)

- 6 interfaces in `Application/Interfaces/` (pure contracts over Domain entities, no EF,
  no `IQueryable` leakage): `ICategoryRepository`, `ITopicRepository`,
  `IQuestionRepository` (paged/filtered/sorted/searched list, random, related, count,
  exists), `IBookmarkRepository`, `IStudyProgressRepository` (upsert), `INoteRepository`.
- 6 EF Core implementations in `Infrastructure/Repositories/`: `AsNoTracking` reads,
  `Include` only where the use case needs it (detail → Topic/Category/Tags),
  clamped pagination (1–100), `EF.Functions.Like` search across question/answer text,
  topic, category and tags (server-side, SQLite-translatable), `EF.Functions.Random()`
  for random picks (fine at this scale), `sortBy` limited to `id/difficulty/newest`
  with safe fallback.
- `DependencyInjection.AddInfrastructure` now registers all six repositories as scoped.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 7 — Application Services ✅ (2026-10-07)

- `Application/Common/Exceptions/`: `NotFoundException(entity, key)` → future HTTP 404,
  `ConflictException(message)` → future HTTP 409. Global middleware translates them in Phase 11.
- 6 service interfaces + implementations in `Application/Interfaces/` + `Application/Services/`
  (services return Domain entities for now; Phase 8 DTOs adapt the return shapes):
  - `CategoryService` / `TopicService` — reads with NotFound guards (topics validate the category).
  - `QuestionService` — detail/paged/random/related/count; validates category/topic filters
    before querying (bad filter → 404, not empty list).
  - `BookmarkService` — question must exist; double-bookmark → Conflict; removing absent → NotFound.
  - `StudyProgressService` — `MarkViewed` (creates Learning / stamps LastViewedAt),
    `MarkCompleted` (stamps CompletedAt), `MarkNeedsReview` (increments ReviewCount);
    all validate the question and work whether or not an entry exists yet.
  - `NoteService` — CRUD with question-existence checks, empty-text rejection (trims),
    automatic CreatedAt/UpdatedAt.
- Small Phase 6 follow-ups for dashboard counts: `IBookmarkRepository.CountAsync`,
  `IStudyProgressRepository.CountByStatusAsync`, `IQuestionRepository.GetCategoryStatsAsync`
  (SQL-translatable projection, in-memory grouping — avoids risky conditional-join translation).
- `Application/DependencyInjection.cs`: `AddApplication()` registers all six services
  (new `Microsoft.Extensions.DependencyInjection.Abstractions 10.0.12` reference);
  `Api/Program.cs` calls it. Controllers will depend on `I*Service` only (Rule 7).
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 8 — DTOs ✅ (2026-10-07)

- `DTOs/Common/`: `ApiResponse<T>` (uniform `success/message/data/errors` envelope with
  `Ok`/`Fail` factories) and `PagedResult<T>` (items + page/pageSize/totalCount/totalPages).
- Entity DTOs: `CategoryDto`, `TopicDto`, `QuestionSummaryDto` (list row + IsBookmarked +
  Status, no answer body), `QuestionDetailDto` (all 13 content sections + placement +
  tags + user state), `BookmarkDto`, `StudyProgressDto`, `QuestionNoteDto`,
  `DashboardSummaryDto` (+ `CategoryProgressDto` with `PercentComplete`).
- Request/query shapes: `QuestionsQuery` (page 1/size 20 defaults, all filters),
  `CreateNoteRequest`/`UpdateNoteRequest`, `UpdateProgressRequest`.
- `Mappings/DtoMapper`: handwritten static entity→DTO mapping (no AutoMapper — every
  mapping is explicit, §37). Null-safe on optional navigations.
- Services now return DTOs (entities never leave Application, Rule 9): all 6 service
  contracts + implementations adapted; `QuestionService` enriches via two bulk lookups
  (`GetBookmarkedQuestionIdsAsync`, `GetStatusMapAsync` — new repo methods, no N+1).
- New `IDashboardService`/`DashboardService`: totals, remaining, bookmark/needs-review
  counts, per-category rows ordered by category; registered in `AddApplication`.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 9 — API Controllers ✅ (2026-10-07)

- 7 thin controllers (services do the work; actions wrap `ApiResponse<T>.Ok`):
  `Categories` (list/get), `Topics` (list with optional `?categoryId=`/get),
  `Questions` (paged list via `QuestionsQuery`, `search`, `random`, `category/{id}`,
  `topic/{id}`, `difficulty/{level}`, `{id}`, `{id}/related`), `Bookmarks`
  (list/add→201/remove), `Progress` (list/set-status→routes Learning/Completed/
  NeedsReview), `Notes` (by-question/get/create→201/update/delete), `Dashboard`
  (`summary`). `{id:int}` constraints prevent `search`/`random` route collisions.
- `Program.cs`: enums serialize as strings (`"difficultyLevel": "Intermediate"`) for
  human-friendly WPF/mobile clients.
- Route note: `POST /api/progress/{questionId}` (+ body `{status}`) instead of the
  spec's bare `POST /api/progress` — question identity belongs in the route; the same
  operation, cleaner REST. Same for notes (`POST /api/notes/question/{questionId}`).
- Smoke test (empty DB, pre-seeding): `/api/categories`, `/api/dashboard/summary`,
  `/api/questions?page=1&pageSize=5` all return `success:true` with the exact envelope
  shape and camelCase JSON. 404 paths still bubble as 500 until Phase 11 middleware —
  expected, fixed next.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 10 — Validation ✅ (2026-10-07)

- NuGet (Application): `FluentValidation` (core only — no DI scanning package).
- 4 validators in `Application/Validators/` (rules live server-side, client-independent):
  `QuestionsQueryValidator` (page ≥ 1, size 1–100, search ≤ 200, sortBy ∈ id/difficulty/
  newest), `Create/UpdateNoteRequestValidator` (text required, ≤ 4000),
  `UpdateProgressRequestValidator` (Learning/Completed/NeedsReview only — NotStarted is
  a system state, never client-set).
- Wired into services (not controllers): `QuestionService` validates the query plus
  guards random/related counts (1–100); `NoteService` takes the request DTOs and
  validates (manual empty-checks removed); new `StudyProgressService.SetStatusAsync`
  validates then dispatches — `ProgressController` is now a one-liner.
  `INoteService` signatures now accept `Create/UpdateNoteRequest`.
- `AddApplication` registers the four validators explicitly (no assembly scanning —
  the full set is visible in one place).
- Runtime check: `?page=0&pageSize=500` is rejected (500 today = ValidationException
  escaping; Phase 11 maps it to a 400 envelope), valid queries still 200.
- Verified: full-solution `dotnet build` — 0 errors.
