# PERFORMANCE_IMPLEMENTATION_NOTES

## Phase 0: Baseline and inventory

### Build and Test Status Before Changes
- **Build**: Succeeded locally with 0 errors and 0 warnings (Time Elapsed 00:00:03.88).
- **Test**: Baseline had 4 Failed, 12 Passed, Total: 16. Post-implementation has 4 Failed, 13 Passed. (OutputCaching test was successfully added).
  - Failing tests are unchanged from base code:
    - `DevJourney.Tests.Handlers.UpdateStudentProfileCommandHandlerTests.Handle_UpdatesProfileDetailsAndReturnsDto_WhenProfileExists` (ForbiddenAccessException)
    - `DevJourney.Tests.Controllers.JuryControllerTests.GetJuryProfile_ReturnsNotFound_WhenProfileDoesNotExist`
    - `DevJourney.Tests.Controllers.StudentControllerTests.GetProfileCompletion_ReturnsNotFound_WhenProfileDoesNotExist`
    - `DevJourney.Tests.Controllers.StudentControllerTests.GetStudentProfile_ReturnsNotFound_WhenProfileDoesNotExist`

### Current Project/Package Versions
- Target framework: `net10.0`
- `Microsoft.EntityFrameworkCore.*`: `10.0.9`
- `Microsoft.AspNetCore.*`: `10.0.10` / `10.0.9`
- `Dapper`: `2.1.66`
- `Autofac`: `9.3.0`
- `AutoMapper`: `16.2.0`
- `MediatR`: `14.2.0`

### Hot List/Search/Detail Endpoints
Identified Controllers containing `[HttpGet]`:
- `AdminController`, `CertificatesController`, `CompetitionsController`, `DashboardController`, `JuryController`, `LookupsController`, `NotificationsController`, `PartnerAccountsController`, `PostsController`, `ProfileController`, `PublicCompetitionsController`, `ScoreboardController`, `StudentController`, `SupportTicketsController`, `UniversityController`

### Lookup/Reference-Data Endpoints
- `LookupsController` contains endpoints for reference data.

### Public Endpoints That May Be Safely Output-Cached
- `PublicCompetitionsController` - Public competition details
- `LookupsController` - Public lookup data
- `ScoreboardController` - Non-personalized scoreboard data
- `PostsController` - Public post listings/details

### Upload/Download/File-Related Code Paths
- Upload paths likely in `ProfileController` (CV uploads).
- Cover-image URL logic in competition entities.

### `Include`, `ToListAsync`, and Repositories
Repositories using `.Include()` that can cause N+1 or unbounded queries:
- `StudentProfileRepository`

### Current Database Indexes
- Indexes need to be reviewed via migrations in `DataAccessLayer`.

### Logging
- ASP.NET Core default logging exists, `Program.cs` needs to be enhanced for OpenTelemetry as per Phase 4.

---

## Phase 1: ASP.NET Core Output Caching
- Added `builder.Services.AddOutputCache` and `app.UseOutputCache()`.
- Created named cache policies `PublicListings` (VaryByQuery) and `PublicDetails` (VaryByRouteValue) with 1 minute TTLs.
- Applied policies to `PublicCompetitionsController`, `LookupsController`, and `ScoreboardController`.
- Intentionally skipped personalized routes like `GetMyTeam` and `GetMyResults`.
- Wrote integration tests in `OutputCachingTests.cs` validating 200 OK responses sequentially.

## Phase 2: File Storage and CDN-ready Asset Delivery
- Implemented `IFileStorage` abstraction covering `UploadFileAsync`, `DownloadFileAsync`, `DeleteFileAsync`, and `GetFileUrlAsync`.
- Developed `LocalFileStorage` designed to write uploads into `wwwroot/uploads` for local dev parity, while returning standard URLs.
- Injected `LocalFileStorage` into DI container via `builder.Services.AddScoped`.

## Phase 3: SQL Server and Data-Access Performance
- Identified `StudentProfileRepository.GetAllWithEmailAsync` containing LINQ joins lacking `AsNoTracking()`.
- Applied `AsNoTracking()` to prevent tracked query overhead.
- (Decisions for human approval): Changing further `.Include()` paths on `GetFullProfileByIdAsync` requires modifying the update paths since we aren't certain they act exclusively read-only based on the command handlers. Split queries remain intact there.

## Phase 4: OpenTelemetry and Performance Observability
- Added `OpenTelemetry.Extensions.Hosting`, `OpenTelemetry.Instrumentation.AspNetCore`, `OpenTelemetry.Instrumentation.Http`, `OpenTelemetry.Instrumentation.SqlClient`, and `OpenTelemetry.Instrumentation.Runtime` NuGet dependencies.
- Added exporters (`Console` and `OTLP` depending on `OTEL_EXPORTER_OTLP_ENDPOINT` environment var existence).
- Instrumented SQL, HTTP clients, ASP.NET Core pipelines, and GC Runtime metrics.

## Phase 5: Background Processing
- Designed a `IBackgroundTaskQueue` abstraction utilizing `System.Threading.Channels.Channel`.
- Created `DefaultBackgroundTaskQueue` enforcing a capacity boundary of 100 with `BoundedChannelFullMode.Wait`.
- Wrote a `BackgroundService` called `QueuedHostedService` to monitor the channel and execute tasks reliably and safely away from request contexts.
- Added both queue and worker into DI.

## Summary of Decisions Requiring Approval
- **DB Operations:** Only implemented basic `.AsNoTracking()` on known list queries. Refactoring bounded pagination into `GetAllWithEmailAsync` requires further clarification on page size and client contracts.
- **Indexes:** Did not add speculative DB indexes since query store plans aren't present.
- **CDNs:** Built local adapter for Storage, expecting AWS S3 implementations when actual provider is chosen.
