# DevJourney Performance and Infrastructure Implementation Brief

You are working in the DevJourney repository, an ASP.NET Core Web API targeting .NET 10.

The solution currently uses ASP.NET Core controllers, SQL Server with Entity Framework Core, Dapper for selected read queries, MediatR, Autofac, JWT authentication and ASP.NET Core Identity, Docker, `AddMemoryCache()`, response compression, CV uploads, and competition cover-image URLs.

The goal is to make the application feel faster and remain stable as traffic and data volume grow. Prioritize user-perceived latency, database round-trips, payload size, cache effectiveness, and reliable background work.

Do not optimize by replacing MediatR, EF Core, AutoMapper, or Autofac unless profiling proves that one of them is a significant bottleneck. Do not introduce a new database engine or a distributed-system component merely for theoretical performance.

## Operating rules

1. Inspect the repository before changing anything.
2. Preserve existing API contracts unless a change is essential and documented.
3. Preserve authentication, authorization, validation, and data-integrity behavior.
4. Never cache personalized responses in a way that could expose one user’s data to another user.
5. Never cache commands, mutation responses, or responses containing sensitive data.
6. Use cancellation tokens for database calls, cache calls, storage calls, and background work.
7. Prefer built-in .NET and ASP.NET Core capabilities before adding third-party packages.
8. Keep local development simple. Every external dependency must have a local Docker/dev alternative or a clearly documented fallback.
9. Do not put uploaded files inside the application container’s local filesystem as the production design.
10. Do not run destructive database operations, delete data, or rewrite migrations without explicit approval.
11. Before changing a query, inspect its current handler, repository, entity configuration, migration/index state, and tests.
12. Do not claim a performance improvement without measurements or a reproducible verification method.

## Phase 0: Baseline and inventory

Before implementation, create a short baseline report in the final response or in `PERFORMANCE_IMPLEMENTATION_NOTES.md` containing:

- build and test status before changes
- current project/package versions
- hot list/search/detail endpoints
- lookup/reference-data endpoints
- public endpoints that may be safely output-cached
- upload/download/file-related code paths
- `Include`, `ToListAsync`, and repository methods that can cause N+1 or unbounded queries
- current database indexes from EF configurations and migrations
- current runtime, request, SQL, and exception logging

Important observations to verify rather than blindly trust:

- `Devjourney/Program.cs` already calls `AddMemoryCache()`.
- `Devjourney/Program.cs` already calls `AddResponseCompression()` and `UseResponseCompression()`.
- The existing performance report may be stale; validate every finding against current source code.
- `StudentProfileRepository` contains multiple collection includes and should be reviewed for split-query or projection use.
- The competition area should be reviewed for repeated queries and in-memory aggregation.
- `ProfileController` contains CV upload functionality, while competition entities contain cover-image URL functionality.

Do not implement changes until this inventory is complete.

## Phase 1: ASP.NET Core output caching

Add and configure ASP.NET Core Output Caching.

### Requirements

- Register output caching in the service collection.
- Add the middleware in the correct pipeline position.
- Ensure it runs after authentication and authorization for protected endpoints.
- Do not enable global caching by default unless a safe policy explicitly excludes personalized and mutation endpoints.
- Use opt-in caching for individual controller actions or named policies.
- Vary cache entries by route values and relevant query-string parameters.
- Define short, explicit TTLs.
- Define tags or another invalidation strategy where practical.
- Prevent caching of responses containing user-specific, authorization-sensitive, or private data.
- Document invalidation after updates to competitions, posts, lookups, profiles, and scoreboards.

### Candidate endpoints

Inspect actual routes and choose only safe candidates. Likely candidates include public lookup data, public competition details, public post listings/details, and non-personalized scoreboard data with a short TTL.

For authenticated endpoints, include every relevant identity/tenant dimension in the cache key, or avoid output caching and use application-level caching instead.

### Tests and verification

Add tests proving that:

- a cacheable endpoint returns the cached response within its TTL
- query-string changes produce separate entries when they affect the result
- authenticated users cannot receive another user’s cached response
- mutation endpoints are not cached
- expiration or invalidation returns fresh data

Measure database query count and p50/p95 latency before and after caching.

## Phase 2: File storage and CDN-ready asset delivery

Design a storage abstraction for uploaded files and media. The application should depend on an interface rather than directly on a provider SDK.

### Requirements

- Create an abstraction such as `IFileStorage` or `IObjectStorage`.
- Support local development storage and an S3-compatible production implementation, or choose a provider after inspecting the deployment target.
- Keep provider credentials in configuration/user secrets/environment variables, never source code.
- Validate file size, extension, MIME type, and content where appropriate.
- Do not trust the original filename.
- Generate collision-resistant object keys.
- Avoid loading large files fully into memory; stream when possible.
- Return stable asset identifiers or URLs rather than internal filesystem paths.
- Make storage operations cancellation-aware.
- Define behavior for missing, deleted, and inaccessible objects.
- Keep private CVs private. Do not publish them through a public CDN URL.
- Treat public competition cover images separately from private user documents.

### Upload architecture

For production, prefer direct browser-to-object-storage uploads using short-lived pre-signed upload URLs where supported. The API should authorize the upload and persist metadata, while the browser transfers the file directly.

If direct uploads are too large for the current scope, implement safe streamed API uploads first and leave a clean path to pre-signed uploads.

### CDN architecture

- Serve public images through a CDN-compatible URL.
- Serve private files through short-lived signed download URLs or an authorized streaming endpoint.
- Add appropriate `Cache-Control`, `ETag`, and content-type headers.
- Use immutable object keys when possible so public assets can have long cache lifetimes.
- Never use a broad CDN cache rule for authenticated API responses.

### Tests and verification

Add tests for rejected oversized/invalid uploads, safe object-key generation, private/public visibility rules, cancellation, missing objects, local storage, and provider failure handling. Document production configuration and local Docker/development setup.

## Phase 3: SQL Server and data-access performance

Improve the existing SQL Server access layer without changing database engines.

### Query requirements

- Use `AsNoTracking()` for read-only EF queries.
- Prefer projection directly into DTOs instead of loading entire entity graphs.
- Add pagination to every user-controlled collection endpoint and enforce a maximum page size.
- Avoid `SELECT *` and unbounded `ToListAsync()` calls.
- Eliminate N+1 queries by batching IDs and grouping results in memory.
- Avoid repeated enumeration and repeated materialization.
- Use `AsSplitQuery()` where it prevents collection-include explosion; do not apply it indiscriminately.
- Use Dapper only where it produces a clear measured benefit or is naturally better expressed as SQL.
- Pass cancellation tokens through all query paths.
- Dispose database connections correctly.

### Specific review targets

Inspect and improve at minimum:

- `StudentProfileRepository`
- `DapperPagedRepositoryBase`
- competition participant/evaluation/scoreboard queries
- partner competition queries
- list/search endpoints under `Application/Modules`
- repository methods returning full tables or unbounded collections
- queries with multiple collection `Include` calls

For scoreboards and aggregates, prefer database-side aggregation or a single batched query. If computation remains in memory, use a lookup/dictionary and avoid scanning all evaluations once per participant.

### Indexing requirements

Use actual query filters, joins, foreign keys, and ordering to propose indexes. Do not add speculative indexes everywhere.

For every new index, explain the query it supports, add it through the project’s normal EF Core migration workflow, consider write overhead and index width, and check that an existing index does not already cover it. Avoid duplicating foreign-key indexes.

Use SQL Server Query Store and execution plans where available. If a production database is unavailable, document the exact commands and metrics required for validation.

### Tests and verification

- Add or update tests for pagination, filtering, ordering, and authorization.
- Verify query count for formerly N+1 paths.
- Verify generated SQL or use a query interceptor/test logger where practical.
- Ensure large result sets cannot bypass page-size limits.
- Do not silently modify production data.

## Phase 4: OpenTelemetry and performance observability

Add OpenTelemetry-based observability so future optimizations are evidence-driven.

### Instrumentation

Capture, with privacy-conscious defaults:

- incoming ASP.NET Core request traces and metrics
- outgoing HTTP client traces
- SQL client/database traces
- request duration and status code
- exception counts
- runtime counters such as GC, allocation, thread pool, and request rate where supported
- cache hit/miss metrics for application-level caches
- background queue depth, job duration, failures, and retries

Never record passwords, JWTs, authorization headers, CV contents, file contents, or sensitive personal data in telemetry.

### Exporters and environments

- In development, provide a console exporter and/or optional local OTLP/Jaeger/Prometheus setup.
- In production, use OTLP configuration through environment variables.
- Make telemetry optional or safely disabled when no exporter is configured.
- Avoid high-cardinality labels such as raw user IDs, arbitrary URLs, tokens, or full query strings.
- Use sampling appropriate for production traffic.

### Instrumentation design

- Add custom activities/metrics around cache operations, scoreboard generation, file storage, and background jobs.
- Add a correlation/request identifier to logs and traces.
- Preserve existing logging while improving structured fields.
- Add a slow-request or slow-query threshold that logs actionable information without secrets.

### Verification

Document how to inspect locally an HTTP request trace, its SQL span, cache hit/miss metrics, a background-job trace, and an exception with its correlation ID.

## Phase 5: Background processing

Move non-critical work off the request path using a reliable abstraction.

### Candidate work

Inspect the codebase for email/notification delivery, CV parsing, image processing, cache warming, recommendation or scoreboard recalculation, and non-critical audit/event work.

Do not move work to the background if the API must guarantee that it completed before returning success.

### Initial implementation

Prefer a built-in `BackgroundService` backed by `System.Threading.Channels` for simple local/single-instance work.

Requirements:

- bounded queue capacity
- cancellation support
- graceful shutdown
- structured logging
- bounded retries
- failure handling and error visibility
- queue-depth and processing-duration metrics
- no unbounded memory growth
- no request-scoped services captured by long-lived workers
- create a new DI scope for each job

If the application will run multiple replicas or jobs must survive restarts, document whether Hangfire, a persistent queue, or Redis Streams is required. Do not add one automatically without demonstrating that an in-process queue is insufficient.

### API behavior

For accepted asynchronous work, return an appropriate status and job identifier. Add a status endpoint only if the product needs it. Do not make clients poll aggressively; document retry/backoff behavior.

### Tests

Test successful execution, shutdown cancellation, bounded queue backpressure/rejection, retry/final failure behavior, scoped dependency creation, and duplicate/idempotent job handling where relevant.

## Cross-cutting safeguards

Inspect and configure, where appropriate, response compression, `Cache-Control`/ETag headers, maximum request and upload sizes, connection-pool behavior, Kestrel/reverse-proxy settings, HTTP/2/HTTP/3 support, health checks for SQL Server/Redis/storage/telemetry/workers, and startup migration/seeding behavior in multi-instance deployments.

Do not trade away security for speed. Never disable JWT validation, authorization, TLS, request limits, or input validation.

## Deliverables

Produce:

1. Source-code implementation for the approved phases.
2. Configuration examples with secrets omitted.
3. Local development instructions, including Docker services if needed.
4. EF Core migrations for approved indexes only.
5. Automated tests for caching, storage, query behavior, telemetry hooks, and background processing.
6. A concise `PERFORMANCE_IMPLEMENTATION_NOTES.md` containing baseline measurements, changed files, new dependencies and their rationale, cache policies/invalidation, storage/CDN design, database/index rationale, telemetry setup, background-job guarantees/limitations, remaining risks, and recommended next measurements.

## Acceptance criteria

The work is complete only when:

- the solution builds successfully
- existing tests pass
- new tests pass
- no endpoint leaks cached personalized data
- public cache policies are explicit and documented
- uploads do not require local container persistence in the production design
- unbounded list endpoints have safe pagination and limits
- identified N+1 paths are removed or explicitly justified
- new indexes are justified by actual query patterns
- telemetry can be inspected locally
- workers shut down safely and cannot grow without bound
- configuration contains no hard-coded secrets
- performance claims include before/after measurements or a documented benchmark plan

At the end, report what was implemented, what was intentionally not implemented, test results, and decisions requiring human approval.
