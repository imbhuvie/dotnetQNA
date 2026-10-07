# Question-Authoring Guide — Adding the Remaining ~970 Questions

> How to add question batches after all phases are complete.
> Technical companion: `implementation-log.md` Phase 14 entries; seeder code:
> `src/TechnicalMastery.Infrastructure/Data/Seed/DatabaseSeeder.cs`.

## 1. How seeding works (the 5 lines that matter)

- Question batches are **embedded JSON resources** (`Infrastructure/Data/Seed/questions/*.json`).
- Any `*.json` added under `questions/` is **auto-discovered** — no code changes, no registration.
- On API startup the seeder applies pending migrations, then loads content.
- **Categories/topics upsert by name; questions load only into an empty Questions table**
  (protects real user bookmarks/progress — see §5 workflow).
- Tags are normalized (trimmed, lowercased, deduplicated) automatically.

## 2. Batch file conventions

- Name: `questions/NN-slug.json`, sequential (`04-collections.json`, `05-linq.json`, …).
- Size: ~10 questions per file (reviewable diffs, safe commits).
- Scope: one category per file preferred; `category`/`topic` values must match the
  catalog **exactly** (case-sensitive).

## 3. JSON schema (one annotated example)

```json
[
  {
    "category": "C# Fundamentals",
    "topic": "Value vs Reference Types",
    "questionText": "What is ...?",
    "shortAnswer": "2-3 sentence direct answer. Always filled.",
    "detailedAnswer": "Full explanation with reasoning and trade-offs. Always filled.",
    "difficultyLevel": "Beginner",
    "questionType": "Conceptual",
    "codeExample": "Compilable C# snippet, // comments explain. Empty string allowed for pure-concept questions.",
    "internalWorking": "What happens under the hood (compiler/CLR/runtime). Always filled.",
    "realWorldUsage": "Where this appears in production code. Always filled.",
    "commonMistake": "The classic error + why it happens. Always filled.",
    "technicalConversation": "How this comes up in review/discussion. Always filled.",
    "interviewFollowUp": "One sharp follow-up question. Always filled.",
    "keyTakeaway": "Single-sentence rule to remember. Always filled.",
    "tags": ["lowercase", "hyphenated", "2-6 tags"]
  }
]
```

Field rules: `category`, `topic`, `questionText`, `shortAnswer`, `detailedAnswer` required
(seeder throws naming the question otherwise). All other text fields: fill them — empty
strings render as empty reader sections. No placeholders (`TODO`, `...`, `lorem`) ever.
English only.

## 4. Allowed values

`difficultyLevel`: `Beginner` · `Intermediate` · `Advanced` · `Production` · `Architecture` · `SystemDesign`
`questionType`: `Conceptual` · `CodeBased` · `ScenarioBased` · `Debugging` · `Design`

Typos fail fast at startup with the offending question text in the error — read it, fix the value.
**Categories/topics are NOT listed here** — `categories.json` (25) and `topics.json` (84)
are the source of truth. Copy names exactly from those files.

## 5. Quality bar (spec §§6–7)

- No near-duplicate variations of one question; each entry teaches something distinct.
- Progress difficulty within a category: Beginner → Intermediate → Advanced → Production.
- Prefer `ScenarioBased`/`Debugging` types at Production level (real incidents, not definitions).
- Code examples must compile mentally (correct syntax, realistic APIs).

## 6. Add-and-verify workflow

```powershell
# 1. Drop the new batch file into Infrastructure/Data/Seed/questions/
# 2. Rebuild (embeds the JSON) and reset the DEV database (batches need an empty table):
dotnet build DotNetTechnicalMastery.slnx
Remove-Item -Path "src/TechnicalMastery.Api/technical_mastery.db*" -Force
# NOTE: -Path, not -LiteralPath (-LiteralPath does not expand the * wildcard)
# 3. Run the API (seeding happens at startup), then verify the count grew correctly:
#    GET /api/questions?page=1&pageSize=1  →  data.totalCount must equal old total + new batch
# 4. Spot-check one question: GET /api/questions/{id} (sections + tags populated)
# 5. Commit + push
```

Never delete or rewrite a committed batch to "fix" content — add corrections as new
commits so history stays reviewable. Never commit `*.db` (gitignored).

## 7. Remaining-batches tracker (30 done → 1000+)

| # | Batch file | Category | Target | Status |
|---|---|---|---|---|
| 01 | `01-csharp-fundamentals.json` | C# Fundamentals | 10 | ✅ Done |
| 02 | `02-csharp-advanced.json` | C# Advanced | 10 | ✅ Done |
| 03 | `03-oop.json` | OOP | 10 | ✅ Done |
| 04a | `04-collections-1.json` | Collections (lists, dictionaries) | 10 | ✅ Done |
| 04b | `04-collections-2.json` | Collections (queues, concurrent) | 10 | ✅ Done |
| 04c | `04-collections-3.json` | Collections (enumerables, spans) | 10 | ✅ Done |
| 04d | `04-collections-4.json` | Collections (production scenarios) | 10 | ✅ Done |
| 05a | `05-linq-1.json` | LINQ (filtering, projection) | 10 | ✅ Done |
| 05b | `05-linq-2.json` | LINQ (ordering, grouping) | 10 | ✅ Done |
| 05c | `05-linq-4.json` | LINQ (joins, sets) | 10 | ✅ Done |
| 05d | `05-linq-3.json` | LINQ (execution semantics) | 10 | ✅ Done |
| 05e | `05-linq-5.json` | LINQ (production scenarios) | 10 | ✅ Done |
| 06 | `06-dotnet.json` | .NET | 40 | ⬜ |
| 07 | `07-dependency-injection.json` | Dependency Injection | 40 | ⬜ |
| 08 | `08-aspnetcore.json` | ASP.NET Core | 60 | ⬜ |
| 09 | `09-webapi.json` | Web API | 50 | ⬜ |
| 10 | `10-auth.json` | Authentication | 35 | ⬜ |
| 11 | `11-security.json` | Security | 40 | ⬜ |
| 12 | `12-efcore.json` | Entity Framework Core | 60 | ⬜ |
| 13 | `13-sql.json` | SQL | 50 | ⬜ |
| 14 | `14-postgres.json` | PostgreSQL | 25 | ⬜ |
| 15 | `15-async.json` | Async Programming | 45 | ⬜ |
| 16 | `16-multithreading.json` | Multithreading | 35 | ⬜ |
| 17 | `17-testing.json` | Testing | 35 | ⬜ |
| 18 | `18-logging.json` | Logging | 25 | ⬜ |
| 19 | `19-performance.json` | Performance | 35 | ⬜ |
| 20 | `20-debugging.json` | Debugging | 25 | ⬜ |
| 21 | `21-git.json` | Git | 20 | ⬜ |
| 22 | `22-docker.json` | Docker | 25 | ⬜ |
| 23 | `23-architecture.json` | Architecture | 60 | ⬜ |
| 24 | `24-system-design.json` | System Design | 50 | ⬜ |
| 25 | `25-production.json` | Production Scenarios | 60 | ⬜ |
| 26 | `26-csharp-fundamentals-2.json` | C# Fundamentals (top-up) | 30 | ⬜ |
| 27 | `27-csharp-advanced-2.json` | C# Advanced (top-up) | 30 | ⬜ |
| 28 | `28-oop-2.json` | OOP (top-up) | 30 | ⬜ |

Targets sum to 30 + 1000 = **1030**. Mark rows ✅ as batches land; adjust targets freely
as long as the total stays ≥1000.
