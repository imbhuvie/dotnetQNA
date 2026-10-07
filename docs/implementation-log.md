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

## Phase 11 — Global Exception Handling ✅ (2026-10-07)

- `Api/Middleware/ExceptionHandlingMiddleware.cs`: try/catch around the whole pipeline,
  mapping exceptions to the `ApiResponse` envelope — NotFound→404, Conflict→409,
  FluentValidation→400 (per-field `Property: message` errors), Argument→400, all
  else→500 with a generic message. Stack traces are logged server-side (ILogger:
  Error for 500s, Warning for handled) and never leave the server (§20).
  Registered first in `Program.cs`, so every failure exits uniformly.
- Api references core `FluentValidation 12.1.1` (exception type only — no rules in Api).
- Live verification: 200 categories; 404 `Category with id 999 was not found.`;
  400 with `["Page: Page must be 1 or greater.", "PageSize: PageSize must be between
  1 and 100."]`; 404 bookmarking nonexistent question. All in-envelope, camelCase.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 12 — Serilog ✅ (2026-10-07)

- NuGet (Api): `Serilog.AspNetCore`, `Serilog.Sinks.File` (console sink ships with AspNetCore).
- `appsettings.json` + `.Development.json`: `Serilog` section — console + rolling daily
  file (`logs/log-.txt`, 7 retained, compact timestamped template); Production-quiet
  overrides, Development shows EF SQL commands.
- `Program.cs`: `Log.Logger` from configuration, `UseSerilog()` on the host,
  `UseSerilogRequestLogging()` (method/path/status/elapsed only — never bodies, tokens,
  passwords or connection strings, §27), startup line plus fatal-catch with
  `CloseAndFlush()` so no event is lost on crash.
- Live verification: startup/hosting lines, per-request `HTTP GET /api/categories
  responded 200`, EF command text in Development, handled-error warning for the 404.
- `.gitignore`: `logs/` — diagnostics stay local.
- Verified: full-solution `dotnet build` — 0 errors.

## Phase 13 — Swagger / OpenAPI ✅ (2026-10-07)

- Replaced the template's `Microsoft.AspNetCore.OpenApi` (built-in JSON only, no UI)
  with `Swashbuckle.AspNetCore 10.2.3`: `AddSwaggerGen` (titled/versioned document with
  a mobile-dev-facing description) + `UseSwagger`/`UseSwaggerUI` in Development.
  Side benefit: the NU1903 `Microsoft.OpenApi` warning is gone — build is now 0 warnings.
- `GenerateDocumentationFile` on Api (+ `NoWarn 1591`): the existing XML summaries on
  every controller action flow into the document as endpoint descriptions (§29).
- Live verification: `/swagger/v1/swagger.json` lists all 19 routes across the 7
  controllers; `/swagger/index.html` returns 200.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 14 — Seed Data (in progress) ✅ infra + batch 1 (2026-10-07)

- `Infrastructure/Data/Seed/`: `SeedModels.cs` (category/topic/question shapes; questions
  reference catalog by NAME, enums parse from names with clear errors on typos),
  `DatabaseSeeder.cs` (applies pending migrations via `MigrateAsync`, then loads content;
  idempotent — categories/topics upsert by name, question batches load only into an
  empty table, tags normalized lowercase/distinct), JSON embedded resources
  (`categories.json`, `topics.json`, `questions/*.json` — new batches auto-discovered
  by naming convention, never `Program.cs`).
- `DatabaseSeeder` registered in `AddInfrastructure`; `Api/Program.cs` runs it at startup
  inside a scope (safe on every boot).
- Content: `categories.json` (all 25 catalog categories), `topics.json` (84 topics across
  every category), `questions/01-csharp-fundamentals.json` (10 full-field questions:
  var/typing, value-vs-reference, boxing, string immutability, properties, records,
  exception practices, ref/out/in, ctors, throw semantics).
- Live verification: 25 categories, 84 topics, 10 questions; dashboard aggregates;
  detail returns tags; `search?query=boxing` finds its question.
- Remaining: batches 02+ continue in following turns until 1000+ (each batch re-verified
  by count query). No schema or contract changes expected.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 14 — batches 02 + 03 ✅ (30 questions total)

- `questions/02-csharp-advanced.json` (10: generics, EqualityComparer, delegates,
  events, closures, patterns, reflection, GC, IDisposable, Span) and
  `questions/03-oop.json` (10: encapsulation, virtual/override/new, interface vs
  abstract, composition, SOLID ×2, LSP, DIP vs DI, access modifiers, decorator,
  overloading).
- Process note: batches load only into an empty Questions table (by design — protects
  user bookmarks/progress). Verifying new batches locally means deleting the dev
  `technical_mastery.db*` and re-running. Also learned: `Remove-Item -LiteralPath`
  does not expand wildcards — use `-Path` for `db*` deletion.
- Bug found by live testing and fixed: list/random/related queries included `Topic`
  but not `Topic.Category` or `Tags`, so summaries returned empty `categoryName`/`tags`
  (detail was unaffected). All three queries now include both.
- Full write-path verification: related lookup, bookmark add → 201, duplicate → 409
  envelope, progress set → Completed with timestamps, note create → 201, dashboard
  reflects 30/1/29/1/0. Follow-up list check confirms populated category/topic/tags.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 15 — API Testing + Question-Authoring Guide ✅ (2026-10-07)

- New doc `docs/question-authoring-guide.md` (concise): how seeding works, batch
  conventions, annotated JSON schema, allowed enums (categories/topics referenced
  from `categories.json`/`topics.json`, never duplicated), quality bar, add-and-verify
  workflow with gotchas, and a remaining-batches tracker (30 done + 1000 target = 1030).
- Endpoint-matrix verification (scripted, fresh DB): **40/40 green** — all 19 routes'
  happy paths, every 404, 409 duplicate bookmark, 400s (query, sort, count, note text,
  NotStarted status), 201 creates, dashboard aggregation, 19 Swagger paths, Swagger UI.
- Consistency fix found by the matrix: invalid-enum routes returned the framework's
  default problem-details instead of our envelope (model binding fails before actions).
  `ApiBehaviorOptions.InvalidModelStateResponseFactory` in `Program.cs` now maps those
  to the same 400 envelope. Re-ran: 40/40.
- Backend (M1–M4) is now fully verified. Next: M5 WPF client (Phase 16+).
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 16 — WPF Project Setup ✅ (2026-10-07)

- NuGet (Wpf): `CommunityToolkit.Mvvm 8.4.2`, `Microsoft.Extensions.Hosting 10.0.12`.
- `App.xaml(.cs)`: generic-host startup (no `StartupUri`) — configuration + DI container
  own the app lifetime; `MainWindow` resolved from the container.
- `appsettings.json` (`Api:BaseUrl`, copied to output): the API address lives in exactly
  one place, never hardcoded (§31).
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 17 — MVVM Infrastructure ✅ (2026-10-07)

- `ViewModels/ViewModelBase.cs`: Toolkit `ObservableObject` + `IsBusy`/`ErrorMessage`
  + virtual `InitializeAsync` (API loading hook — never in constructors).
- `Services/NavigationService.cs`: view-model-first navigation (`NavigateToAsync<T>`,
  DI-resolved, auto-initialized). No view types referenced — screens resolve via
  `DataTemplate`s, code-behind stays logic-free.
- `ViewModels/MainViewModel.cs`: 7 navigation `RelayCommand`s, global `SearchText` +
  `SearchCommand` (routes into Browse), exposes `Navigation` for the content host.
- `MainWindow.xaml`: sidebar + header-search + `ContentControl`; `MainWindow.xaml.cs`
  only assigns the injected VM and shows the dashboard — the project's sole code-behind logic.
- 7 stub views/VMs (Dashboard/Browse/Bookmarks/Progress/Notes/Settings/About) with
  placeholders; real content lands in Phases 19–26. All VMs registered in the host.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 18 — API Client ✅ (2026-10-07)

- `Models/ApiModels.cs`: client-side contract copies (enums + all DTO shapes) —
  deliberately not shared with the server, so this client proves the HTTP+JSON
  contract exactly like a future mobile app would. Enums from JSON strings.
- `Services/ApiException.cs`: status + server message; `UserMessage()` maps network/
  timeout/unknown failures to friendly text (§20, no stack traces to users).
- `Services/ApiClientInterfaces.cs`: 6 narrow contracts (catalog/questions/bookmarks/
  progress/notes/dashboard) — the §21 offline seam (a local cache can implement these
  later without UI changes).
- `Services/StudyApiClient.cs`: single typed `HttpClient` (factory-created, 30s timeout),
  one envelope-unwrapping `SendAsync<T>` (error body → `ApiException` with message).
- `App.xaml.cs`: typed-client registration with `BaseAddress` from config + interface
  mappings. New package: `Microsoft.Extensions.Http` (`AddHttpClient` lives there,
  not in Hosting — build caught it).
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 19 — Dashboard ✅ (2026-10-07)

- `DashboardViewModel`: loads `IDashboardApiClient` summary in `InitializeAsync`
  (busy flag, friendly errors via `ApiException.UserMessage`); `ContinueLearning`
  navigates to Browse (deep-link to a random question's detail arrives with Phase 21).
- `DashboardView`: 5 stat cards, per-category `ProgressBar` rows with counts/percents,
  loading + error states, scrollable responsive layout.
- `Converters/`: `NullToVisibility` + `BoolToVisibility`, registered app-wide.
- Live check: API reseeded (30 questions), WPF exe launched and stayed alive through
  startup/navigation/dashboard load with no crash; API serving alongside.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 20 — Question List ✅ (2026-10-07)

- `BrowseViewModel`: category list → topic combo (reloaded per category) → difficulty
  combo (All + 6 levels) → server-side paged list (20/page, Prev/Next + counts) →
  `OpenQuestionCommand` navigates to detail with the id. Search text executes as an
  API-side query (full search UX refined in Phase 22). Explicit Apply buttons instead
  of selection-changed events — every load path is a command.
- `BrowseView`: filter rail + question cards (text, category/topic, difficulty, status)
  + pager; responsive grid layout.
- `NavigationService.NavigateToAsync<T>(configure)`: configure-before-initialize so
  detail screens receive their id before loading.
- `QuestionDetailViewModel`/view stub + template + DI registration (reader in Phase 21).
- Live check: app launches and stays alive with API serving.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 21 — Question Detail ✅ (2026-10-07)

- `QuestionDetailViewModel`: loads detail + related + notes by id; Prev/Next
  (id-based with graceful errors), bookmark toggle, complete/needs-review,
  related-question opening, note add/delete, back-to-browse. Reloads after every
  mutation so bookmark/status state is always server-truth.
- `QuestionDetailView`: sectioned reader (§13 — all 13 content areas), monospace
  dark code block (selectable), tag pills, action bar + bookmark/status line,
  clickable related list, inline notes with add/delete.
- Fixed a malformed `DataTemplate` close in the tags block (build caught it).
- Live check: app launches and stays alive with API serving.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 22 — Search & Sort ✅ (2026-10-07)

- Header search now routes through configure-before-initialize, so Browse's first
  server-side load already applies the query (previously the text landed after load).
- Browse gains Sort options (Default/Newest/Oldest/Hardest/Easiest) mapped to the
  API's `sortBy`/`descending` — full Search/Filter/Sort/Pagination coverage (§12).
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 23 — Bookmarks ✅ (2026-10-07)

- `BookmarksViewModel`: loads all bookmarks via `IBookmarkApiClient`; `OpenBookmarkCommand`
  navigates to detail; `RemoveBookmarkCommand` calls API then removes locally.
- `BookmarksView`: list of bookmarked questions (title, date), each row has
  "Remove" button; empty state when no bookmarks.
- Added converters: `CountToVisibility` (non-zero → Visible), `ZeroToVisibility` (zero → Visible),
  `DateTimeToString` (formats DateTime as "yyyy-MM-dd HH:mm"); registered app-wide.
- Fixed StringFormat-in-Run XAML issue by using a converter.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings; app launches.

## Phase 24 — Progress Screen ✅ (2026-10-07)

- `ProgressViewModel`: loads all progress via `IProgressApiClient.GetAllProgressAsync`; filter
  by `StudyStatus` (All/NotStarted/Learning/Completed/NeedsReview) with Apply button;
  `OpenQuestionCommand` navigates to detail with the question id.
- `ProgressView`: filter combo + rows with status badge (colored via `StatusToBrushConverter`),
  last viewed/completed dates, review count, "Open" button; empty state when no entries.
- Added converters: `NullableDateTimeToString` (empty for null), `StatusToBrush` (status →
  colored brush: gray/blue/green/orange); registered app-wide.
- API interface updated: `IProgressApiClient.GetAllProgressAsync` + `StudyApiClient` impl.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings; app launches.

## Phase 25 — Notes Screen ✅ (2026-10-07)

- New `GET /api/notes` through proper layers (repo `GetAllAsync` newest-first →
  service → controller) — one call for the overview screen instead of the N+1
  per-question loop the draft VM used (which would fire ~1000 requests at scale).
- `NotesViewModel`: single-load list with inline edit (one shared edit panel),
  delete, and jump-to-question. Creation stays in the question reader, where the
  question context exists. `INotesApiClient.GetAllNotesAsync` + client impl.
- `NotesView`: note cards (Question #, text, updated date, Open/Edit/Delete) +
  shared edit panel + empty state.
- Live check: created note via API, `GET /api/notes` returns it; app launches alive.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 26 — Polish + Themes ✅ (2026-10-07)

- `Resources/LightTheme.xaml` + `DarkTheme.xaml`: 10 named color/brush pairs each
  (background, foreground, borders, muted text, accent, code block, sidebar, tags).
- `Services/ThemeService`: swaps the merged dictionary at runtime; views use
  `{DynamicResource}` so they update instantly. Hardcoded sidebar/code/tag colors
  converted; window background/foreground themed.
- `SettingsViewModel`/view: dark-theme CheckBox toggle + read-only API base URL
  from config (§31). `AboutViewModel`/view: title, version, description, architecture note.
- Build validates both dictionaries (XAML compiler resolves all brush keys).
- Live check: app launches alive on the default light theme with API serving.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

## Phase 27 — Automated Tests ✅ (2026-10-07)

- `tests/`: `TestDatabase` helper (temp-file SQLite + real migrations + 3-question
  seed, fresh per test, file deleted on dispose — dev DB never touched) and 5 classes,
  **24 tests, all green** (`dotnet test`): `QuestionServiceTests` (detail, filters,
  search-by-tag, NotFound/validation guards, random, related), `BookmarkServiceTests`
  (add/list/duplicate-conflict/missing/remove), `StudyProgressServiceTests`
  (complete stamps, review-count increments, learning creation, status validation),
  `QuestionRepositoryTests` (pagination, category stats, counts), `QuestionControllerTests`
  (envelope + status codes, NotFound propagation).
- Real bug found by the suite: `BookmarkService.AddAsync` assigned a detached `Question`
  to the new bookmark's navigation — harmless on fresh production scopes but throws
  when the context already tracks that key. Fixed by building the DTO from the loaded
  question (no graph attach, robust under any context lifetime).
- Verified: `dotnet test` 24/24 passed; full-solution build 0 errors, 0 warnings.

## Phase 28 — Final Architecture Review ✅ (2026-10-07)

All 10 rules (§38) verified, not just asserted:
- R1/R2: zero `EntityFramework`/`Sqlite`/`DbContext` matches in `TechnicalMastery.Wpf`.
- R3: view models orchestrate API-client interfaces only (no queries, no rules).
- R4: zero `System.Windows`/Wpf matches in `TechnicalMastery.Api`.
- R5: zero EF/ASP.NET matches in `TechnicalMastery.Domain`.
- R6/R7/R8: controllers delegate to `I*Service`, services hold rules, EF lives in
  Infrastructure (by construction, reviewed per phase).
- R9: controllers return DTOs; WPF ships its own contract copies — entities never
  cross HTTP in either direction.
- R10: `TechnicalMastery.Wpf` has **zero** `ProjectReference`s; Api references only
  Application + Infrastructure. The frontend is replaceable by construction.
- Hygiene: no `EnsureCreated()` in code (docs prose only); `dotnet ef migrations
  has-pending-model-changes` → none; build 0/0; `dotnet test` 24/24.
- README progress table updated to final state.
- **All 28 phases complete.** Intentionally remaining: question batches 04+ to 1000+
  via `docs/question-authoring-guide.md` (infra proven, no code changes needed).

## Post-build hardening

### About-crash fix ✅ (2026-10-07)

- Root cause (from the real dialog: TwoWay binding on read-only `AppVersion`):
  `Run.Text` bindings default to TwoWay, which requires a settable source.
  AboutViewModel's display properties are get-only → `InvalidOperationException`
  on render. Other screens survived only because their models have setters.
- All 21 `Run` bindings across 7 views now declare `Mode=OneWay` (correct order:
  `{Binding Path, Mode=OneWay}` — path first).
- `App.xaml.cs`: WPF global exception handling (`DispatcherUnhandledException`
  → friendly message + `logs/wpf-*.txt` full-stack log, app survives;
  domain/unobserved-task faults logged). Also removed duplicate VM registrations.
  Next UI fault arrives with evidence instead of killing the process (§20).

### Simple-MVVM conversion ✅ (2026-10-07)

- Dropped `CommunityToolkit.Mvvm` (no generators, no `partial`, no attributes):
  new `Commands/RelayCommand.cs` (sync/async, optional CanExecute) +
  `Commands/RelayCommandOfT.cs` (parameterized), and hand-written
  `ViewModelBase : INotifyPropertyChanged` with `SetProperty` helper.
- Converted all 8 VMs + `NavigationService` + `ThemeService` with **identical
  public names** (`SearchText`, `SearchCommand`, `CurrentViewModel`, `IsDark`,
  …) — zero XAML changes; flow and architecture untouched.
- Verified: full-solution `dotnet build` — 0 errors; zero `CommunityToolkit`
  references in code; app launches alive with API serving.

## Question batches (ongoing)

### Batch 04a — Collections part 1 ✅ (40 total)

- `questions/04-collections-1.json` (10: array vs List, Add/Insert/RemoveAt costs,
  collection expressions, Dictionary keys, HashSet set-ops, IEqualityComparer vs
  IEquatable, collection-modified errors, jagged/multidim/flat grids, ToLookup/GroupBy,
  API surface types). Tracker split into 04a–04d part rows (same 40 total).
- Live verification: 40 total, 10 in Collections, detail + tags correct.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 04b — Collections part 2 ✅ (50 total)

- `questions/04-collections-2.json` (10: Queue/Stack basics, List-as-queue antipattern,
  PriorityQueue, bracket validation, bounded-buffer design, ConcurrentDictionary
  factories, concurrent queue/stack/bag, BlockingCollection vs Channel, Interlocked
  vs lock, false sharing). File initially misnumbered `05-`; renamed to keep
  category prefixes consistent.
- Live verification: 50 total, 20 in Collections.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 04c — Collections part 3 ✅ (60 total)

- `questions/04-collections-3.json` (10: IEnumerable/yield mechanics, Collection<T>
  vs List<T> inheritance, AsReadOnly vs snapshots, immutable collections, Frozen
  collections, sorted variants, ArrayPool, LOH avoidance, capacity planning, LRU design).
- Live verification: 60 total, 30 in Collections.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 04d — Collections part 4 ✅ (70 total, category complete)

- `questions/04-collections-4.json` (10: collection decision guide, cross-thread
  iteration patterns, comparer production bugs, leak diagnosis, paging/chunking,
  MemoryCache, batched DB writes, JSON gotchas, sliding-window rate limiting,
  object pooling).
- Live verification: 70 total, 40 in Collections — first category complete.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 05a — LINQ part 1 ✅ (80 total)

- `questions/05-linq-1.json` (10: Where/Select chaining, SelectMany flattening, DTO
  projection with EF, OrderBy/ThenBy/comparers, GroupBy + Having, First/Single/Find,
  Any/All/Count costs, Sum/Average/Min/Max empties, Take/Skip/Chunk, query vs method
  syntax). Tracker LINQ row split into 05a–05e.
- Live verification: 80 total, 10 in LINQ.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 05b — LINQ part 2 ✅ (90 total)

- `questions/05-linq-2.json` (10: Join/GroupJoin, left-outer-join pattern, By-key
  set ops, Zip/cross joins, MinBy/MaxBy, ToDictionary/ToLookup failure modes,
  Aggregate folds, dataset reconciliation, OfType/Cast, stable grouped paging).
- Live verification: 90 total, 20 in LINQ.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 05c — LINQ part 3 ✅ (100 total)

- `questions/05-linq-3.json` (10: deferred vs immediate, streaming vs buffering,
  IEnumerable vs IQueryable, multiple-enumeration hazards, closure capture, ToList
  timing, EF translation failures, custom operators, PLINQ, pipeline debugging).
  Tracker rows reordered to match files (05d done; joins/sets moves to 05-linq-4).
- Live verification: 100 total, 30 in LINQ — first century milestone.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 05d — LINQ part 4 ✅ (110 total)

- `questions/05-linq-4.json` (10: inner-join syntaxes, composite keys, self-joins,
  full/right joins, Union/Concat/Intersect/Except, case-insensitive joins,
  SequenceEqual validation, join performance, distinct paging, ranking/windows).
- Live verification: 110 total, 40 in LINQ.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 05e — LINQ part 5 ✅ (120 total, category complete)

- `questions/05-linq-5.json` (10: N+1 detection/fix, compiled queries, split queries,
  async streaming, dynamic filters, plan caching, distributed sorting, LINQ testing,
  temporal joins, pipeline observability).
- Live verification: 120 total, 50 in LINQ — second category complete.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 06a — .NET part 1 ✅ (130 total)

- `questions/06-dotnet-1.json` (10: CLR/JIT, assemblies, SDK vs runtime, config
  layering, options lifetimes, reload pitfalls, log levels, structured logging,
  AOT vs ReadyToRun, secrets management). Tracker .NET row split into 06a–06d.
- Live verification: 130 total, 10 in .NET.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 06b — .NET part 2 ✅ (140 total)

- `questions/06-dotnet-2.json` (10: lifetime semantics, captive dependencies,
  keyed services, TryAdd/Replace testing, scopes + correlation, Serilog sinks,
  log volume/cost, DI testing, OpenTelemetry, hosted-service lifetimes).
- Live verification: 140 total, 20 in .NET.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.

### Batch 06c — .NET part 3 ✅ (150 total)

- `questions/06-dotnet-3.json` (10: Generic Host, IHostedService vs BackgroundService,
  graceful shutdown, health checks, PeriodicTimer, Quartz/Hangfire, scoped services
  in workers, K8s probes YAML, Windows/systemd hosting, zero-downtime deploys).
- Live verification: 150 total, 30 in .NET.
- Verified: full-solution `dotnet build` — 0 errors, 0 warnings.
