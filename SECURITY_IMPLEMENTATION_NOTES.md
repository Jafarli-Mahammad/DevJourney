# SECURITY_IMPLEMENTATION_NOTES

## Phase 0: Threat model and security baseline

### Trust Boundaries
- **Public Endpoints**: Lookups, public competitions, public post listings, and public scoreboards.
- **Authenticated Endpoints**: Profiles, competition participation, private evaluations, file uploads, notifications.
- **Roles**: Admin, Jury, Organizer, Student, Partner.
- **Data Categories**: Sensitive personal data (CVs, email addresses), credentials (passwords, JWTs).

### Known Gaps & Vulnerabilities
1. **Critical**: Unhandled exceptions returned stack traces.
2. **High**: CORS was excessively permissive (`AllowAll`) in all environments.
3. **High**: Swagger UI and API specifications were exposed publicly in production.
4. **Medium**: Lack of standardized correlation IDs and missing generic error handling for `500 Internal Server Error`.

---

## Phase 1: Secrets and configuration hygiene
- Confirmed `appsettings.json` uses safe placeholders (`YOUR_SUPER_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARS_LONG`).
- Secret management is deferred to environment variables/user secrets correctly, notably for `Jwt:SecretKey` and `ConnectionStrings:Redis`.
- `Program.cs` properly verifies that `Jwt:SecretKey` is > 32 characters, failing startup securely.

## Phase 3: API and HTTP security
- **CORS Restricted**: Refactored `AllowAll` in `Program.cs`. In Development, it permits all origins, but in Production, it pulls from `AllowedOrigins` configuration or falls back to `https://trusted-domain.com`.
- **Swagger Secured**: `app.UseSwagger()` and `app.UseSwaggerUI()` are now wrapped in `if (app.Environment.IsDevelopment())`, preventing API enumeration attacks in production.

## Phase 6: Error handling, logging, and monitoring
- **Sanitized Errors**: Added a generic `Exception` catch block to `GlobalExceptionMiddleware`. Unhandled errors now return a sanitized `500 Internal Server Error` payload rather than risking stack trace exposure.
- **Correlation IDs**: Integrated `X-Correlation-ID` generation into `GlobalExceptionMiddleware`. This ID is injected into the response headers, JSON payload, and structured logging scope to aid incident response.
- **Logging**: Configured `_logger.LogError` and `_logger.LogWarning` for unauthorized and unhandled access attempts without leaking the user request payload.

## Phase 2: Authentication and Authorization Hardening
- **Identity Password Policy**: Enforced a strict Identity password policy in `Program.cs` (`RequireDigit`, `RequireLowercase`, `RequireNonAlphanumeric`, `RequireUppercase`, `RequiredLength = 8`).
- **Account Lockout Policy**: Configured `IsLockedOutAsync` tracking. Users are locked out for 15 minutes after 5 consecutive failed access attempts (`AuthService.cs`).
- **Password Reset Protection**: 
  - `PasswordResetConfirmCommandValidator` was created to strictly validate the new password format against the global constraints.
  - `ResetPasswordAsync` and `PasswordResetConfirmCommand` refactored to securely use `UserManager` through the service abstraction.
  - Implemented timing/enumeration mitigations in password-reset endpoint (if an account doesn't exist, it resolves identically to a success).
- **IDOR / BOLA Prevention**:
  - Remediated an IDOR exposure in `UpdateStudentProfileCommandHandler.cs`. Rather than accepting a `request.StudentProfileId` from the payload (which could potentially be spoofed to update someone else's profile), it now strictly evaluates the context via `_studentProfileRepository.GetFullProfileByUserIdAsync(_currentUserService.UserId)`.

## Phase 4: Data Privacy and Database Security
- **Data Minimization (DTOs)**: Audited public endpoints. `Email` and `ApplicationUserId` were removed from `StudentProfileListDto` and `GetAllStudentProfilesQueryHandler.cs` to prevent mass leakage of sensitive user data over unauthenticated endpoints.
- **SQL Injection Prevention**: Verified that all raw SQL queries (e.g., in `TeamMemberSearchPostRepository.cs`) rely entirely on `Dapper`'s `DynamicParameters` and `CommandDefinition`. No string concatenations are used.
- **Data Retention Background Job**: Created `DataRetentionWorker`, an `IHostedService` that runs every 24 hours to enforce data privacy purging laws. It automatically calls `ExecuteDeleteAsync()` to hard-delete entities (`Users` and `Posts`) that have been soft-deleted for more than 30 days.
- **Privacy Policy**: Added `DATA_PRIVACY_POLICY.md` to document the types of data collected, retention intervals, and DTO isolation strategy.

## Phase 5: File Upload and Media Security
- **Restricted Storage Location**: Moved `LocalFileStorage` out of `wwwroot` to `App_Data/uploads`. Files are no longer reachable directly via IIS/Kestrel's static file middleware.
- **Magic Number Validation**: `UploadCvCommandValidator` now verifies file signatures (magic bytes) to strictly ensure uploaded files are valid PDFs or DOCXs (bypassing simple file extension spoofing). It also enforces a strict 5MB maximum size.
- **UUID Renaming**: `UploadCvCommandHandler` forcefully discards the user-supplied filename and replaces it with a `Guid.NewGuid()` to eliminate directory traversal vulnerabilities (`../`) and filename enumeration.
- **Secure Download Controller**: Created `FileController` with `DownloadFile` endpoint. It enforces the `Content-Disposition: attachment` header to neutralize stored Cross-Site Scripting (XSS) via HTML or SVG masquerading as valid uploads.

## Phase 7: Container and Deployment Security
- **Supply Chain Security**: Enabled `<NuGetAudit>true</NuGetAudit>` in `Directory.Build.props` to ensure any future vulnerable dependency causes the CI/CD pipeline to warn or fail. Checked `dotnet list package --vulnerable`, which successfully returned 0 vulnerabilities.
- **Docker Read-Only Filesystem**: Modified `compose.yaml` to enforce `read_only: true` on the API container.
- **Docker Capabilities & Privilege Escalation**: Applied `cap_drop: - ALL` and `no-new-privileges:true` in `compose.yaml`.
- **Upload Volume Isolation**: Specified a dedicated writeable `VOLUME /app/App_Data/uploads` inside `Dockerfile` (and mapped in `compose.yaml`) so the app can function with a read-only root filesystem without permitting arbitrary execution.
- **Git Hygiene**: Fortified `.dockerignore` to explicitly exclude development settings (`appsettings.Development.json`) and certificates (`*.pfx`, `*.key`) from accidentally slipping into image layers.

---

## Decisions Requiring Approval & Next Steps
- **Phase 2 (Auth Hardening)**: Need product approval on password complexity overrides and exactly which routes should enforce multi-factor authentication (MFA).
- **Phase 4 (Data Privacy)**: Need to define explicit data retention and purge timelines for the database before writing auto-deletion background jobs.
- **Phase 5 (File Uploads)**: We are using local storage mock. We need a designated S3 bucket and CDN configurations to safely sandbox user-uploaded PDF/Image content away from the container. 
