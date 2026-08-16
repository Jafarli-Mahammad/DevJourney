# DevJourney Security and Privacy Implementation Brief

You are working in the DevJourney repository, an ASP.NET Core Web API targeting .NET 10.

The solution currently uses ASP.NET Core controllers, JWT bearer authentication, ASP.NET Core Identity, SQL Server with Entity Framework Core, Dapper, MediatR, Autofac, Docker, Swagger/Swashbuckle, CORS, rate limiting, custom exception middleware, and file-upload-related functionality.

The goal is to protect user privacy, account security, uploaded files, credentials, and application availability. Review the entire repository before changing code. Use OWASP ASVS and OWASP API Security Top 10 as practical reference frameworks, but preserve the project’s existing product behavior unless a security change requires a documented contract change.

Do not claim the system is secure merely because tests pass. Identify assumptions, remaining risks, and deployment responsibilities.

## Non-negotiable rules

1. Never print, commit, or expose passwords, JWT secrets, Redis credentials, database connection strings, API keys, reset tokens, uploaded-file contents, or personal data.
2. Treat all credentials previously placed in source files, chat, logs, screenshots, or shell history as compromised. Recommend rotation and remove them from tracked files.
3. Use secret managers, environment variables, or .NET User Secrets for secrets. Keep only non-secret examples in repository configuration.
4. Do not weaken authentication, authorization, TLS, validation, rate limits, or request-size limits to make tests pass.
5. Do not reveal whether an account exists during login, registration, password-reset, or account-recovery flows unless the product explicitly requires it and the disclosure is accepted as a risk.
6. Apply authorization at the resource level, not just at the controller level. A valid JWT must not imply access to every resource.
7. Use cancellation tokens and bounded resource usage for database, network, file, and background operations.
8. Avoid logging request bodies, authorization headers, cookies, passwords, tokens, CVs, or unnecessary personal data.
9. Do not perform destructive data changes, delete users, revoke all sessions, or rewrite migrations without explicit approval.
10. Preserve existing user data and API compatibility unless a security fix requires a versioned change.

## Phase 0: Threat model and security baseline

Before implementation, create `SECURITY_IMPLEMENTATION_NOTES.md` or include the baseline in the final report.

Document:

- application trust boundaries
- public and authenticated endpoints
- user roles and resource ownership rules
- sensitive data categories stored or returned
- authentication and token lifecycle
- upload and download flows
- database and cache access paths
- external services and deployment boundaries
- likely attackers and abuse cases
- current security controls and known gaps

Inventory at minimum:

- every controller and action
- every `[Authorize]`, `[AllowAnonymous]`, role check, and policy check
- every direct `HttpContext.User` access
- every database query receiving user-controlled IDs or filters
- every file upload/download path
- every outbound HTTP call
- every configuration value containing a secret or credential
- every place where exceptions are logged or returned
- Swagger and health/debug endpoints
- CORS, rate limiter, HTTPS, proxy, and forwarded-header configuration
- Identity password, lockout, cookie, token, and user-confirmation configuration
- JWT creation and validation code

Produce a prioritized list using Critical, High, Medium, and Low severity. Explain exploitability, affected users/data, and remediation.

## Phase 1: Secrets and configuration hygiene

Remove secrets from tracked configuration and source code.

### Requirements

- `appsettings.json` may contain safe defaults and placeholders only.
- `appsettings.Development.json` must not contain real production credentials.
- Use User Secrets for local development and the deployment platform’s secret manager/environment settings for deployment.
- Use standard environment-variable names such as `ConnectionStrings__Redis`, `ConnectionStrings__DefaultConnection`, and `Jwt__SecretKey`.
- Fail fast in non-development environments when required secrets are missing or weak.
- Do not log resolved configuration values.
- Do not use a fallback JWT secret in production.
- Do not silently use a development database or development credentials in production.
- Review Dockerfiles, Compose files, CI files, scripts, documentation, screenshots, and git history for leaked values where feasible.
- Add secret-scanning guidance or a repository-compatible secret scanner.
- Ensure generated local secret files remain ignored by git.

### JWT secrets

- Require a strong, randomly generated signing key.
- Validate issuer, audience, lifetime, signing key, and expected signing algorithm.
- Configure an appropriate clock skew rather than accepting overly long token validity.
- Use short-lived access tokens.
- Design refresh-token rotation, revocation, reuse detection, and secure storage if refresh tokens are added.
- Never put secrets or sensitive personal data in JWT claims.
- Consider key rotation and `kid` support before production.
- Document what happens to existing tokens after key rotation.

### Database and Redis credentials

- Use TLS for remote Redis and database connections where supported.
- Restrict Redis network access and avoid exposing local Redis beyond loopback.
- Never cache secrets or sensitive data without a clear encryption/access policy.
- Use separate credentials and least-privilege accounts for development, staging, and production.

### Verification

Add checks that fail deployment or startup when production configuration contains placeholder secrets, weak JWT keys, or missing required credentials. Do not expose secret values in error messages.

## Phase 2: Authentication hardening

Review and harden ASP.NET Core Identity and all login/registration flows.

### Requirements

- Configure a strong password policy consistently across every registration path.
- Verify that the current `PasswordResetConfirm` flow enforces the same password policy.
- Enable appropriate account lockout or progressive delay after repeated failures.
- Prevent username/email enumeration through consistent responses and timing where practical.
- Normalize email/usernames consistently before lookup.
- Confirm email addresses before enabling sensitive functionality if required by the product.
- Add MFA/TOTP or an extensible MFA plan for privileged roles and high-risk operations.
- Do not store plaintext passwords; use Identity’s password hasher only.
- Ensure password reset tokens are single-use, expire quickly, and do not appear in logs or URLs where avoidable.
- Revoke or invalidate relevant sessions/tokens after password reset, account disablement, or credential compromise.
- Prevent login endpoints from becoming an unbounded expensive hashing oracle.
- Return generic authentication errors to clients while logging safe internal diagnostics.

### Authorization

- Define explicit policies for student, partner/company, jury, organizer, and administrative actions.
- Verify ownership/tenant boundaries for every route containing a user, profile, post, competition, participant, evaluation, or file ID.
- Prevent IDOR/BOLA vulnerabilities by loading the resource through an authorized query rather than loading by ID and checking later.
- Ensure mutation handlers verify both authentication and authorization.
- Review `VerifiedAuthorBehaviour` and similar behaviors for bypasses, incorrect identity assumptions, and race conditions.
- Never trust role or ownership fields submitted by the client.
- Test unauthorized, cross-user, cross-role, and cross-tenant access explicitly.

### Tests

Add integration tests for:

- invalid, expired, wrong-issuer, wrong-audience, and wrong-signature JWTs
- missing authentication
- each role/policy boundary
- accessing another user’s profile, posts, files, competitions, participants, and evaluations
- password reset expiration and reuse
- lockout/progressive delay behavior
- disabled/deleted users

## Phase 3: API and HTTP security

Review the complete ASP.NET Core pipeline and endpoint exposure.

### CORS

- Replace `AllowAll` in production with an explicit allowlist of trusted origins.
- Never combine unrestricted origins with credentials.
- Make origins configuration-driven and environment-specific.
- Test preflight, allowed-origin, disallowed-origin, and credential behavior.

### HTTPS and headers

- Require HTTPS outside local development.
- Configure HSTS only for domains where HTTPS is guaranteed.
- Configure forwarded headers correctly when behind a trusted reverse proxy.
- Add suitable security headers at the edge or application layer, including CSP where applicable, `X-Content-Type-Options`, `Referrer-Policy`, and a restrictive permissions policy.
- Do not add obsolete or misleading headers merely to satisfy a checklist.
- Ensure cookies, if introduced, use Secure, HttpOnly, and SameSite settings appropriate to the client architecture.

### Rate limiting and abuse prevention

- Keep global rate limiting, but review whether IP-only partitioning is appropriate behind proxies, NAT, and IPv6.
- Use trusted forwarded IP handling only after configuring trusted proxies.
- Add endpoint-specific policies for login, registration, password reset, file upload, search, and expensive scoreboard/report operations.
- Consider combined user/IP/device/application limits where appropriate.
- Ensure rejected requests return `Retry-After` where useful.
- Make limits configurable per environment and monitor rejection rates.
- Avoid creating an unbounded rate-limiter partition for attacker-controlled keys.

### Request limits and validation

- Set maximum request-body and multipart upload sizes.
- Enforce maximum lengths for every string, collection, page size, sort field, and filter.
- Validate enum values, GUIDs, dates, numeric ranges, and cross-field invariants.
- Reject unexpected or malformed content types.
- Use FluentValidation consistently, including every password and file-upload path.
- Avoid expensive validation or regexes vulnerable to catastrophic backtracking.
- Reject duplicate or conflicting mutation requests where idempotency is required.

### Swagger and operational endpoints

- Disable Swagger UI and sensitive API descriptions in production unless explicitly protected.
- Do not expose stack traces, environment details, connection status secrets, or internal paths.
- Protect health/readiness endpoints appropriately and return only necessary information.
- Ensure `/`, diagnostics, metrics, and debug routes are intentional and safe.

## Phase 4: Data privacy and database security

Map personal and sensitive data and reduce exposure.

### Requirements

- Document what personal data is collected, why it is collected, and who can access it.
- Return the minimum fields needed for each endpoint.
- Use DTO projections rather than serializing entities or navigation graphs.
- Avoid returning email, phone, internal IDs, audit fields, or private profile data in public responses unless required.
- Review logs, telemetry, caches, exports, backups, and error responses for personal data leakage.
- Apply least-privilege database permissions to the application account.
- Use encrypted transport to SQL Server and encrypted backups where supported.
- Review soft-delete behavior, authorization filters, and accidental resurrection of deleted records.
- Add audit trails for privileged access, profile changes, role changes, file access, and sensitive mutations.
- Define retention and deletion behavior for accounts, uploaded files, logs, caches, and backups.
- Support account/data deletion or document why a retention exception exists.
- Protect against mass assignment by using explicit request DTOs and never binding entities directly.

### Concurrency and integrity

- Add concurrency tokens or atomic update conditions for sensitive toggles and role/status changes.
- Use transactions for multi-entity security-sensitive operations.
- Enforce uniqueness and ownership constraints in the database, not only in application code.
- Avoid trusting a read-then-write authorization decision when the resource can change concurrently.

### SQL injection and query safety

- Use EF parameterization and Dapper parameters exclusively.
- Review dynamic SQL, sort fields, filters, search expressions, and raw SQL.
- Use an allowlist for dynamic column names and sort directions.
- Never concatenate user input into SQL, shell commands, filesystem paths, or URLs.
- Add regression tests for injection payloads and authorization bypasses.

## Phase 5: File-upload and media security

Treat CVs, PDFs, presentations, images, and other uploads as hostile input.

### Requirements

- Enforce strict size limits before buffering.
- Validate extension, declared MIME type, detected file signature, and content type.
- Use an allowlist, not a blocklist.
- Generate random storage keys; never use user filenames as paths.
- Store uploads outside the web root and outside the application container filesystem in production.
- Prevent path traversal, archive bombs, decompression bombs, and polyglot files.
- Do not execute, render, or parse untrusted files in the API process without isolation.
- Scan uploads with an antivirus/malware scanner where the deployment supports it.
- Strip or sanitize metadata when privacy requires it.
- Serve downloads with safe content-disposition and content-type headers.
- Keep private CVs behind authorization or short-lived signed URLs.
- Do not make user uploads executable or publicly listable.
- Delete associated files when a user/account is deleted, subject to retention policy.
- Protect upload endpoints with authentication, authorization, rate limits, and quotas.

Review `IFormFile`, `LocalFileStorage`, `UploadCvCommand`, and every future media path. The current mock storage URL must not be treated as a production security design.

### Tests

Test oversized files, wrong extensions, mismatched signatures, malicious filenames, traversal strings, empty files, malformed multipart requests, unauthorized downloads, and cross-user file access.

## Phase 6: Error handling, logging, and monitoring

Harden `GlobalExceptionMiddleware` and all logging.

### Requirements

- Return consistent problem responses without stack traces or internal exception messages in production.
- Generate a correlation ID and include it in the response and structured logs.
- Log security events such as failed logins, lockouts, password resets, privilege changes, suspicious access, rate-limit abuse, and file access failures.
- Do not log credentials, tokens, request bodies, CV contents, or unnecessary personal data.
- Use structured logging with stable event names.
- Separate client-safe messages from internal diagnostic details.
- Ensure exception handling itself cannot throw or leak sensitive values.
- Alert on unusual authentication failures, authorization failures, reset abuse, upload abuse, and data-access anomalies.
- Ensure telemetry sampling does not drop critical security events.

## Phase 7: Dependency, container, and deployment security

Secure the supply chain and runtime.

### Requirements

- Audit NuGet packages for vulnerabilities and outdated versions.
- Enable automated dependency update/security scanning.
- Pin or lock dependency versions where appropriate.
- Build and scan container images for known vulnerabilities.
- Use minimal runtime images and do not run as root.
- Keep Docker build context free of secrets.
- Run with a read-only filesystem where practical and provide writable temporary storage explicitly.
- Drop unnecessary Linux capabilities.
- Set resource limits for CPU, memory, processes, and file descriptors.
- Use non-root service identities and least-privilege database accounts.
- Separate development, staging, and production resources.
- Do not run migrations and seeders concurrently from every production replica; use a controlled deployment step.
- Restrict Redis, SQL Server, object storage, and admin interfaces by network policy.
- Enable TLS certificate validation; never use trust-all certificate settings in production.
- Define backup encryption, restoration tests, and incident recovery procedures.

## Phase 8: Security testing and verification

Add automated and manual verification.

### Automated tests

Include unit, integration, and authorization tests for:

- authentication and token validation
- every role and ownership boundary
- CORS behavior
- rate-limit policies
- request-size and input limits
- password reset and lockout
- file validation and private file access
- error-response redaction
- SQL/Dapper parameterization
- cache privacy and invalidation
- sensitive-data minimization in DTOs

### Tooling

Use safe, non-destructive tools where appropriate:

- `dotnet list package --vulnerable`
- NuGet audit/dependency scanning
- secret scanning
- container image scanning
- OWASP ZAP or an equivalent authorized API scan
- static analysis and compiler analyzers
- targeted integration tests with a disposable database/cache

Never run active scanning against systems without authorization. Do not include real user data or production credentials in security tests.

### Manual review checklist

Verify:

- anonymous users cannot access protected resources
- users cannot access another user’s resources by changing IDs
- roles cannot be self-assigned
- reset tokens cannot be reused
- revoked/disabled users cannot continue using sensitive operations
- public responses do not expose private fields
- errors do not expose implementation details
- uploads cannot become public or executable
- Swagger and diagnostics are not unnecessarily public
- production CORS is not unrestricted
- secrets are absent from tracked files, logs, and images

## Deliverables

Produce:

1. Source-code implementation for approved remediations.
2. Security configuration examples with secrets omitted.
3. Updated tests and a security test plan.
4. Database migrations only where integrity/security constraints require them.
5. Deployment hardening documentation.
6. `SECURITY_IMPLEMENTATION_NOTES.md` containing:
   - threat model and assumptions
   - findings by severity
   - changed files
   - credentials rotated or requiring rotation
   - authentication/authorization policy map
   - privacy/data-retention decisions
   - upload security design
   - deployment and container controls
   - test/tool results
   - remaining risks and recommended next steps

## Acceptance criteria

The work is complete only when:

- no real credentials remain in tracked configuration or source
- production startup fails safely when critical secrets are missing or weak
- JWT validation is strict and token handling is documented
- every sensitive endpoint has tested authentication and resource authorization
- CORS is environment-specific and restricted in production
- login, reset, upload, search, and expensive operations have abuse controls
- request and upload sizes are bounded
- private data and files cannot be accessed through ID changes
- error responses and logs do not expose secrets or sensitive personal data
- SQL and Dapper paths are parameterized
- Swagger/diagnostics exposure is intentional
- dependencies and container images have a repeatable vulnerability-scanning process
- security tests pass
- unresolved risks and deployment responsibilities are explicitly documented

At the end, report what was implemented, what was intentionally not implemented, which credentials must be rotated, test results, and all decisions requiring human approval.
