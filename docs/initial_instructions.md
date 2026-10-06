# MASTER PROMPT — .NET TECHNICAL Q&A KNOWLEDGE APPLICATION

Act as a **Senior .NET Software Architect, C# Developer, WPF Developer, API Architect, Database Designer, UI/UX Designer, and Code Reviewer**.

I want you to design and develop a professional **.NET Technical Questions & Answers Learning Application** for studying and improving technical knowledge.

The application will contain **1000+ .NET/C# technical questions with detailed answers**, similar to a personal technical knowledge book, but instead of reading PDFs, I want to browse, search, filter, bookmark, and study the questions through an application.

The application must be designed with **complete frontend/backend separation** so that the WPF desktop application can later be replaced by a **mobile application without modifying the backend business logic or API contracts**.

---

# 1. CORE ARCHITECTURE — VERY IMPORTANT

Do NOT create a monolithic WPF application where:

- WPF directly accesses SQLite
- WPF contains business logic
- WPF contains database queries
- WPF contains Entity Framework Core code
- WPF contains question/answer processing logic

Instead use a proper client-server architecture.

The architecture must be:

```text
                    ┌──────────────────────┐
                    │     WPF Desktop      │
                    │      Frontend        │
                    └──────────┬───────────┘
                               │
                               │ HTTP / REST API
                               ▼
                    ┌──────────────────────┐
                    │    ASP.NET Core      │
                    │       Web API        │
                    ├──────────────────────┤
                    │ Controllers          │
                    │ Application Services │
                    │ Business Logic       │
                    │ Validation           │
                    │ DTOs                  │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │   Data Access Layer  │
                    │      EF Core         │
                    │    Repository        │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │       SQLite         │
                    │      Database        │
                    └──────────────────────┘
```

Later the architecture should support:

```text
                    ┌──────────────────────┐
                    │    Mobile App        │
                    │ Android / iOS         │
                    └──────────┬───────────┘
                               │
                               │ HTTP / REST API
                               ▼
                    ┌──────────────────────┐
                    │ SAME ASP.NET CORE    │
                    │      WEB API         │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │ SAME BUSINESS LOGIC  │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │       SQLite         │
                    └──────────────────────┘
```

The WPF application must NEVER directly access SQLite.

The future mobile application must also NEVER directly access SQLite.

All clients communicate through the API.

---

# 2. TECHNOLOGY STACK

Use the following technologies.

## Backend

- C#
- ASP.NET Core Web API
- .NET 8 or later LTS version
- Entity Framework Core
- SQLite
- REST API
- Dependency Injection
- Repository Pattern where appropriate
- Service Layer
- DTOs
- FluentValidation or equivalent validation approach
- AutoMapper only if it provides real value
- Serilog for logging
- Swagger / OpenAPI
- Global exception handling
- Async/Await
- LINQ

## Frontend

Use:

- WPF
- C#
- MVVM architecture
- XAML
- Data Binding
- Commands
- ObservableCollection
- HttpClient
- Dependency Injection where appropriate

Do NOT put API calls directly inside XAML code-behind.

---

# 3. SOLUTION STRUCTURE

Create a professional multi-project solution.

Recommended structure:

```text
DotNetTechnicalMastery.sln

src/
│
├── TechnicalMastery.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Extensions/
│   ├── Configuration/
│   └── Program.cs
│
├── TechnicalMastery.Application/
│   ├── Interfaces/
│   ├── Services/
│   ├── DTOs/
│   ├── Validators/
│   └── Mappings/
│
├── TechnicalMastery.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Common/
│
├── TechnicalMastery.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   ├── Configurations/
│   └── Migrations/
│
└── TechnicalMastery.Wpf/
    ├── Views/
    ├── ViewModels/
    ├── Models/
    ├── Services/
    ├── Commands/
    ├── Converters/
    ├── Resources/
    └── App.xaml
```

Explain why each project exists.

---

# 4. DEPENDENCY FLOW

Maintain this dependency direction:

```text
WPF
 │
 ▼
API
 │
 ▼
Application
 │
 ▼
Domain

Infrastructure
 └── implements Application interfaces
```

The Domain project should not depend on:

- WPF
- ASP.NET Core
- Entity Framework Core
- SQLite

The Application layer should contain business/use-case logic and interfaces.

Infrastructure should contain database implementation.

API should expose HTTP endpoints.

WPF should consume the API.

---

# 5. DATABASE

Use SQLite.

Database name:

```text
technical_mastery.db
```

Use Entity Framework Core migrations.

Design a normalized database.

At minimum create these entities:

### Category

```text
Category
---------
Id
Name
Description
DisplayOrder
IsActive
CreatedAt
UpdatedAt
```

Examples:

```text
C# Fundamentals
C# Advanced
OOP
SOLID
Collections
LINQ
.NET
Dependency Injection
ASP.NET Core
Web API
REST
Authentication
Security
Entity Framework Core
SQL
PostgreSQL
Async Programming
Multithreading
Testing
Logging
Performance
Debugging
Git
Docker
Architecture
System Design
Production Scenarios
```

---

### Topic

```text
Topic
-----
Id
CategoryId
Name
Description
DisplayOrder
IsActive
```

---

### Question

```text
Question
--------
Id
TopicId
QuestionText
ShortAnswer
DetailedAnswer
DifficultyLevel
QuestionType
CodeExample
InternalWorking
RealWorldUsage
CommonMistake
KeyTakeaway
CreatedAt
UpdatedAt
IsActive
```

---

### QuestionTag

```text
QuestionTag
-----------
Id
QuestionId
Tag
```

---

### Bookmark

```text
Bookmark
--------
Id
QuestionId
CreatedAt
```

If bookmarks should belong to users in the future, design the model so that adding:

```text
UserId
```

later will be straightforward.

---

### StudyProgress

```text
StudyProgress
-------------
Id
QuestionId
Status
LastViewedAt
CompletedAt
ReviewCount
```

Possible status:

```text
NotStarted
Learning
Completed
NeedsReview
```

---

### QuestionNote

```text
QuestionNote
------------
Id
QuestionId
NoteText
CreatedAt
UpdatedAt
```

This allows the user to write personal notes.

---

# 6. QUESTION DATA STRUCTURE

Each question should contain much more than a simple answer.

For example:

```text
Question:
What is Dependency Injection in .NET?

Short Answer:
Dependency Injection is a design technique where required dependencies
are provided to a class instead of the class creating them itself.

Detailed Explanation:
...

Example:
...

What Happens Internally:
...

Real World Usage:
...

Common Mistake:
...

Technical Conversation:
...

Interview Follow-up:
...

Key Takeaway:
...
```

Questions should be designed for learning, not only interview preparation.

---

# 7. 1000+ QUESTIONS

Seed the SQLite database with at least **1000 high-quality technical questions**.

Do NOT generate meaningless variations of the same question.

Questions should progressively move from:

```text
Beginner
   ↓
Intermediate
   ↓
Advanced
   ↓
Production
   ↓
Architecture
   ↓
System Design
```

Cover at least:

### C#

- Variables
- Data types
- Value vs reference types
- Boxing/unboxing
- Strings
- Arrays
- Classes
- Structs
- Records
- Enums
- Methods
- Properties
- Access modifiers
- Constructors
- Static members
- Exception handling
- Generics
- Delegates
- Events
- Lambda expressions
- Nullable reference types
- Pattern matching
- Reflection
- Attributes
- Memory management
- Garbage Collection
- IDisposable
- IAsyncDisposable
- Span
- Memory
- Thread safety

### OOP

- Encapsulation
- Abstraction
- Inheritance
- Polymorphism
- Composition
- Aggregation
- Association
- Interface
- Abstract class
- Virtual
- Override
- New keyword
- Sealed
- SOLID

### Collections

- Array
- List
- Dictionary
- HashSet
- Queue
- Stack
- IEnumerable
- ICollection
- IList
- IQueryable
- Concurrent collections

### LINQ

- Where
- Select
- SelectMany
- OrderBy
- GroupBy
- Join
- Any
- All
- Contains
- First
- FirstOrDefault
- Single
- SingleOrDefault
- Count
- Sum
- Average
- Min
- Max
- Deferred execution
- Immediate execution
- IEnumerable vs IQueryable

### .NET

- CLR
- CTS
- CLS
- Assemblies
- NuGet
- SDK
- Runtime
- Middleware
- Configuration
- Options pattern
- Logging
- Environment configuration

### Dependency Injection

- DI
- IoC
- Constructor injection
- Method injection
- Service locator
- Transient
- Scoped
- Singleton
- Lifetime mismatch
- Captive dependency
- DI best practices

### ASP.NET Core

- Middleware
- Routing
- Controllers
- Model binding
- Model validation
- Filters
- Action results
- TempData
- Session
- Configuration
- Options
- CORS
- Authentication
- Authorization

### Web API

- REST
- HTTP
- GET
- POST
- PUT
- PATCH
- DELETE
- Status codes
- Headers
- Query parameters
- Route parameters
- Request body
- DTO
- Pagination
- Filtering
- Sorting
- Searching
- API versioning

### Security

- Authentication
- Authorization
- JWT
- Cookies
- Refresh tokens
- Password hashing
- BCrypt
- Claims
- Roles
- Policies
- CORS
- CSRF
- XSS
- SQL Injection
- HTTPS
- Secrets management

### EF Core

- DbContext
- DbSet
- Migrations
- Tracking
- NoTracking
- Include
- ThenInclude
- Relationships
- Fluent API
- Data annotations
- Transactions
- Concurrency
- Query optimization
- N+1 problem
- Lazy loading
- Eager loading
- Explicit loading

### SQL

- SELECT
- INSERT
- UPDATE
- DELETE
- JOIN
- GROUP BY
- HAVING
- Subqueries
- CTE
- Indexes
- Primary key
- Foreign key
- Unique constraints
- Transactions
- ACID
- Normalization
- Deadlocks
- Query optimization

### Async / Concurrency

- async
- await
- Task
- Task<T>
- ValueTask
- Thread
- ThreadPool
- Parallel
- CancellationToken
- Race conditions
- Locks
- SemaphoreSlim
- Deadlocks

### Testing

- Unit testing
- Integration testing
- Mocking
- Test doubles
- xUnit
- Assertions
- Arrange Act Assert

### Production

Include real production questions such as:

- API suddenly becomes slow
- Database query becomes slow
- Memory usage continuously increases
- CPU usage reaches 100%
- API returns intermittent 500 errors
- Deadlocks occur
- Connection pool exhaustion
- Too many database calls
- N+1 queries
- Large response payload
- API timeout
- External API unavailable
- Logging strategy
- Retry strategy
- Caching
- Rate limiting

### Architecture

Include:

- Layered architecture
- Clean Architecture
- Onion Architecture
- Hexagonal Architecture
- Repository pattern
- Unit of Work
- CQRS
- Mediator
- Microservices
- Monolith
- Modular monolith
- Event-driven architecture
- SOLID
- Design patterns

### System Design

Include:

- URL shortener
- Chat application
- Notification system
- File upload system
- Authentication system
- E-commerce backend
- Restaurant booking system
- Learning platform
- Question bank
- Logging system
- API gateway
- Caching system

---

# 8. API DESIGN

Create RESTful APIs.

Example:

```http
GET /api/categories
GET /api/categories/{id}

GET /api/topics
GET /api/topics/{id}

GET /api/questions
GET /api/questions/{id}

GET /api/questions/search?query=dependency

GET /api/questions/category/{categoryId}

GET /api/questions/topic/{topicId}

GET /api/questions/difficulty/{difficulty}

GET /api/questions/random

GET /api/questions/{id}/related

POST /api/bookmarks/{questionId}

DELETE /api/bookmarks/{questionId}

GET /api/bookmarks

POST /api/progress

GET /api/progress

POST /api/notes

PUT /api/notes/{id}

DELETE /api/notes/{id}
```

Support:

```text
Pagination
Filtering
Sorting
Searching
Category filtering
Topic filtering
Difficulty filtering
Question type filtering
```

Example:

```http
GET /api/questions?page=1&pageSize=20&difficulty=Intermediate
```

Return proper DTOs.

Do not expose EF Core entities directly from controllers.

---

# 9. API RESPONSE FORMAT

Use a consistent API response model.

Example:

```json
{
    "success": true,
    "message": "Questions retrieved successfully",
    "data": [],
    "errors": []
}
```

For errors:

```json
{
    "success": false,
    "message": "Question not found",
    "data": null,
    "errors": []
}
```

Use appropriate HTTP status codes.

For example:

```text
200 OK
201 Created
204 No Content
400 Bad Request
401 Unauthorized
403 Forbidden
404 Not Found
409 Conflict
500 Internal Server Error
```

---

# 10. WPF APPLICATION

Build a professional WPF desktop application.

Use MVVM.

Main screens:

```text
Dashboard
Categories
Topics
Question List
Question Details
Search
Bookmarks
Study Progress
Notes
Settings
About
```

---

# 11. DASHBOARD

Create a modern dashboard.

Show:

```text
Total Questions
Questions Completed
Questions Remaining
Bookmarks
Questions Needing Review
Current Category
Study Progress
```

Example:

```text
-----------------------------------------------
       .NET TECHNICAL MASTERY
-----------------------------------------------

Total Questions        1000+
Completed              245
Bookmarks              56
Needs Review           23

[ Continue Learning ]

Categories
--------------------------------
C#                         120
ASP.NET Core               150
EF Core                    100
SQL                        120
Architecture                80
...
```

---

# 12. QUESTION LIST

Display questions in a clean list.

Each item should show:

```text
Question
Category
Topic
Difficulty
Bookmark status
Completion status
```

Support:

```text
Search
Filter
Sort
Pagination
```

---

# 13. QUESTION DETAIL SCREEN

Design a high-quality reading experience.

Example:

```text
Question #245

What is Dependency Injection?

Difficulty: Intermediate
Category: ASP.NET Core

---------------------------------------

SHORT ANSWER

...

---------------------------------------

DETAILED EXPLANATION

...

---------------------------------------

EXAMPLE

...

---------------------------------------

WHAT HAPPENS INTERNALLY?

...

---------------------------------------

REAL WORLD USAGE

...

---------------------------------------

COMMON MISTAKE

...

---------------------------------------

TECHNICAL CONVERSATION

...

---------------------------------------

KEY TAKEAWAY

...

[ Previous ] [ Mark Complete ] [ Bookmark ] [ Next ]
```

Code blocks should have a visually different appearance.

---

# 14. SEARCH

Implement global question search.

Search should support:

```text
Question text
Answer
Category
Topic
Tags
```

Example:

```text
dependency injection
```

should return questions containing relevant concepts.

Use API-side search rather than downloading all 1000+ questions into WPF.

---

# 15. BOOKMARK SYSTEM

Allow the user to:

```text
Bookmark question
Remove bookmark
View all bookmarks
```

The bookmark operation must call the API.

Do not store bookmark state only in WPF memory.

---

# 16. STUDY PROGRESS

Track:

```text
Not Started
Learning
Completed
Needs Review
```

Show progress percentage.

Example:

```text
C#                  ████████████░░ 80%
ASP.NET Core        ████████░░░░░ 60%
EF Core             ██████░░░░░░░ 45%
SQL                 █████████░░░░ 70%
```

---

# 17. NOTES

Allow users to attach personal notes to questions.

Example:

```text
My Note:

"Remember that Scoped is normally one instance
per HTTP request in ASP.NET Core."
```

Notes should be persisted through the backend API.

---

# 18. API CLIENT ARCHITECTURE IN WPF

Create an API client abstraction.

Example:

```csharp
public interface IQuestionApiClient
{
    Task<QuestionDto?> GetQuestionAsync(int id);
    Task<PagedResult<QuestionDto>> GetQuestionsAsync(...);
    Task<PagedResult<QuestionDto>> SearchQuestionsAsync(...);
}
```

Implementation:

```text
QuestionApiClient
```

Use HttpClient.

Do NOT write code such as:

```csharp
HttpClient client = new HttpClient();
```

every time a request is made.

Configure HttpClient properly through Dependency Injection.

---

# 19. MVVM

Do not put business logic inside code-behind.

Avoid:

```csharp
private void Button_Click(...)
{
    // API call
    // database logic
    // business logic
}
```

Instead:

```text
View
 ↓
ViewModel
 ↓
API Client
 ↓
REST API
```

Use commands:

```text
SearchCommand
NextQuestionCommand
PreviousQuestionCommand
BookmarkCommand
MarkCompletedCommand
```

---

# 20. ERROR HANDLING

Handle:

```text
Network unavailable
API unavailable
Timeout
404
400
500
Invalid response
SQLite/database errors
```

Show user-friendly messages.

Do not expose technical stack traces to the user.

Example:

```text
Unable to connect to the server.

Please check your internet connection and try again.
```

---

# 21. OFFLINE CONSIDERATION

Initially keep SQLite only on the backend.

The WPF client should communicate through the API.

However, design the WPF architecture so that an offline cache can be added later without changing the UI.

For example:

```text
IQuestionDataSource
       │
       ├── ApiQuestionDataSource
       │
       └── LocalQuestionDataSource
```

Do not implement unnecessary offline functionality initially unless it is useful.

---

# 22. SECURITY

Even though this is initially a personal learning application, design the backend correctly.

Do not:

- Store passwords as plain text
- Expose database connection strings through API responses
- Trust client-side validation
- Return EF entities directly
- Allow unrestricted database queries
- Hardcode secrets

Prepare the architecture for future authentication.

---

# 23. FUTURE MOBILE APPLICATION

This requirement is VERY IMPORTANT.

The WPF application is only one client.

The backend API must be completely independent from WPF.

Do NOT reference:

```text
TechnicalMastery.Wpf
```

from:

```text
TechnicalMastery.Api
```

The backend must not contain:

```text
System.Windows
Window
UserControl
Dispatcher
XAML
WPF ViewModel
```

The backend should only expose platform-independent HTTP APIs.

Future clients could be:

```text
WPF
MAUI
Android
iOS
Web
React
Blazor
```

All should consume the same API.

---

# 24. FUTURE MOBILE ARCHITECTURE

The final architecture should allow:

```text
                ┌───────────────┐
                │ WPF Desktop   │
                └───────┬───────┘
                        │
                        │
                ┌───────▼───────┐
                │               │
                │ ASP.NET Core  │
                │    Web API    │
                │               │
                └───────┬───────┘
                        │
                ┌───────▼───────┐
                │ Application   │
                │ Business Logic│
                └───────┬───────┘
                        │
                ┌───────▼───────┐
                │ Infrastructure│
                │   EF Core     │
                └───────┬───────┘
                        │
                ┌───────▼───────┐
                │    SQLite     │
                └───────────────┘


Future:

                ┌───────────────┐
                │ Mobile App    │
                └───────┬───────┘
                        │
                        ▼
                 SAME REST API
```

No backend rewrite should be necessary merely because the frontend changes from WPF to mobile.

---

# 25. SEED DATA

Create a database seeding mechanism.

Seed:

- Categories
- Topics
- At least 1000 questions
- Tags

The application should automatically seed the database when appropriate.

Do not put 1000 questions directly inside `Program.cs`.

Create:

```text
QuestionSeedData.cs
CategorySeedData.cs
TopicSeedData.cs
```

or an equivalent maintainable mechanism.

---

# 26. CODE QUALITY

Follow:

- SOLID
- Clean Code
- DRY
- Separation of Concerns
- Dependency Injection
- Async/Await
- CancellationToken where appropriate
- Meaningful naming
- Small methods
- Proper exception handling
- XML documentation where useful

Avoid unnecessary complexity.

This application should be understandable by a developer with approximately 1 year of professional experience.

Do not over-engineer.

---

# 27. LOGGING

Use Serilog.

Log:

```text
Application startup
API requests
Important business operations
Exceptions
Database errors
External API errors
```

Do not log:

```text
Passwords
JWT tokens
Sensitive user information
Secrets
Connection strings
```

---

# 28. TESTING

Create tests for important backend functionality.

At minimum:

```text
QuestionServiceTests
BookmarkServiceTests
StudyProgressServiceTests
QuestionRepositoryTests
QuestionControllerTests
```

Test:

```text
Question retrieval
Search
Pagination
Bookmark creation
Bookmark removal
Progress update
Question not found
Invalid input
```

---

# 29. API DOCUMENTATION

Configure Swagger/OpenAPI.

Every API endpoint should have:

- HTTP method
- Route
- Description
- Request model
- Response model
- Status codes

The API must be easy for a future mobile developer to consume.

---

# 30. DATABASE MIGRATIONS

Use:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Explain how migrations work.

Do not use:

```csharp
Database.EnsureCreated()
```

as the primary production database strategy.

---

# 31. PROJECT CONFIGURATION

Provide:

```text
appsettings.json
appsettings.Development.json
```

Configure:

```text
ConnectionStrings
Logging
API configuration
```

The WPF application should have a configurable API base URL.

Example:

```text
http://localhost:5000/api/
```

Do not hardcode API URLs throughout the code.

---

# 32. UI DESIGN

Create a modern professional technical-learning UI.

Style:

- Clean
- Minimal
- Professional
- Developer-focused
- Easy to read for long study sessions

Include:

- Sidebar navigation
- Header
- Search box
- Cards
- Question reader
- Code blocks
- Progress indicators
- Dark/light theme if practical

Prioritize usability over unnecessary animations.

---

# 33. RESPONSIVENESS

The WPF application should resize correctly.

Avoid hardcoded absolute positioning.

Use:

```text
Grid
DockPanel
StackPanel
ScrollViewer
Styles
Templates
Resources
```

appropriately.

---

# 34. API INDEPENDENCE RULE

The following rule must always be respected:

> The backend must never know that the client is WPF.

The backend should only know:

```text
HTTP
JSON
DTOs
Business Rules
Database
```

Therefore, if tomorrow I replace:

```text
WPF
```

with:

```text
.NET MAUI
```

the backend should continue working.

---

# 35. DEVELOPMENT ORDER

Do not attempt to write the entire application randomly.

Develop it in this order:

### Phase 1

Create solution and projects.

### Phase 2

Create Domain entities.

### Phase 3

Create EF Core DbContext.

### Phase 4

Create SQLite database.

### Phase 5

Create migrations.

### Phase 6

Create repositories.

### Phase 7

Create application services.

### Phase 8

Create DTOs.

### Phase 9

Create API controllers.

### Phase 10

Add validation.

### Phase 11

Add global exception handling.

### Phase 12

Add Serilog.

### Phase 13

Add Swagger.

### Phase 14

Add seed data.

### Phase 15

Test APIs using Swagger/Postman.

### Phase 16

Create WPF project.

### Phase 17

Implement MVVM.

### Phase 18

Implement API client.

### Phase 19

Create dashboard.

### Phase 20

Create question list.

### Phase 21

Create question details.

### Phase 22

Implement search/filter.

### Phase 23

Implement bookmarks.

### Phase 24

Implement progress tracking.

### Phase 25

Implement notes.

### Phase 26

Improve UI/UX.

### Phase 27

Add automated tests.

### Phase 28

Perform final architecture/code review.

---

# 36. OUTPUT REQUIREMENT

Do not simply give me a theoretical architecture.

Actually develop the project step by step.

For every phase provide:

1. Files to create
2. Folder structure
3. Complete code
4. Explanation
5. NuGet packages
6. Configuration
7. Commands to run
8. Expected result
9. Common errors and solutions

Never provide incomplete placeholder code such as:

```csharp
// implement this
```

or:

```csharp
// remaining code
```

When code is required, provide complete compilable code.

---

# 37. CODING STYLE

Use explicit types instead of excessive `var`.

Prefer:

```csharp
Question question = new Question();
```

over:

```csharp
var question = new Question();
```

Keep code beginner-friendly while following professional practices.

Do not introduce advanced patterns merely to make the architecture look complicated.

Explain advanced concepts when they are necessary.

---

# 38. IMPORTANT ARCHITECTURAL RULES

Never violate these rules:

### Rule 1

WPF must not access SQLite directly.

### Rule 2

WPF must not contain database queries.

### Rule 3

WPF must not contain business logic.

### Rule 4

API must not reference WPF.

### Rule 5

Domain must not reference infrastructure.

### Rule 6

Controllers should remain thin.

### Rule 7

Business logic belongs in Application/Service layer.

### Rule 8

Database logic belongs in Infrastructure.

### Rule 9

Use DTOs between API and clients.

### Rule 10

The API contract must remain independent of the UI technology.

---

# 39. FINAL ARCHITECTURE

The final solution should look approximately like:

```text
DotNetTechnicalMastery
│
├── TechnicalMastery.Domain
│       └── Entities
│
├── TechnicalMastery.Application
│       ├── DTOs
│       ├── Interfaces
│       ├── Services
│       └── Validators
│
├── TechnicalMastery.Infrastructure
│       ├── Data
│       ├── Repositories
│       └── Migrations
│
├── TechnicalMastery.Api
│       ├── Controllers
│       ├── Middleware
│       └── Configuration
│
└── TechnicalMastery.Wpf
        ├── Views
        ├── ViewModels
        ├── Services
        ├── Commands
        └── Resources
```

Communication:

```text
WPF
 │
 │ HTTP + JSON
 ▼
ASP.NET Core Web API
 │
 ▼
Application Services
 │
 ▼
Repositories
 │
 ▼
EF Core
 │
 ▼
SQLite
```

Future:

```text
Mobile App
 │
 │ HTTP + JSON
 ▼
SAME ASP.NET Core API
 │
 ▼
SAME Application Layer
 │
 ▼
SAME Infrastructure
 │
 ▼
SAME Database
```

---

# 40. FIRST RESPONSE REQUIREMENT

Before writing code, provide:

1. Complete architecture explanation
2. Architecture diagram
3. Project dependency diagram
4. Database ER diagram
5. API architecture
6. WPF MVVM architecture
7. Complete folder structure
8. NuGet package list
9. Development roadmap
10. Explanation of how the architecture allows WPF to be replaced by a mobile application later

Then wait for my instruction:

**"Start Phase 1"**

When I say:

**"Start Phase 1"**

begin creating the actual solution step by step.

Do not skip architectural decisions.

Do not connect WPF directly to SQLite.

Do not create a monolithic application.

The primary goal is:

> **Build a professional, maintainable, scalable .NET Technical Q&A Learning Platform where the frontend can be replaced without rewriting the backend.**