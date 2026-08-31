# DevJourney Backend Audit & Security Assessment Report

**Target URL:** [https://devjourney-0sy5.onrender.com](https://devjourney-0sy5.onrender.com)  
**Swagger Docs:** [https://devjourney-0sy5.onrender.com/swagger/index.html](https://devjourney-0sy5.onrender.com/swagger/index.html)  
**Date of Assessment:** August 30, 2026  
**Environment:** Production / Cloudflare CDN + Render Web Service (.NET Core / Kestrel)

---

## Executive Summary

A comprehensive automated security and functional inspection of the deployed DevJourney backend API was conducted using Playwright MCP and live endpoint diagnostics against the production environment and source codebase.

The deployment is operational and handles core authentication and lookup endpoints properly. However, several **critical security vulnerabilities**, **broken access control issues**, and **stubbed/unfinished features** were identified.

---

## 1. Security & Authorization Vulnerabilities

### Critical Findings

#### 1.1 Unauthenticated Partner Account Administration (Data Leakage & Unauthorized Mutation)
* **File:** [`Devjourney/Controllers/PartnerAccountsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PartnerAccountsController.cs#L14-L48)
* **Endpoints:**
  * `GET /api/partner/accounts`
  * `POST /api/partner/accounts`
  * `DELETE /api/partner/accounts/{id}`
* **Issue:** Missing `[Authorize]` attribute entirely on the controller and its action methods.
* **Impact:** Any unauthenticated third party can dump all staff and jury accounts (revealing full names, emails, roles, organization titles, and referral codes) as well as create or revoke partner account access without credentials.

#### 1.2 Tenant Impersonation / Broken Object-Level Authorization (BOLA)
* **File:** [`Devjourney/Controllers/CompetitionsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/CompetitionsController.cs#L60-L64) and [Lines 127-131](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/CompetitionsController.cs#L127-L131)
* **Issue:** When an authenticated user's claim does not resolve to an existing partner organization, the controller defaults to fetching the first partner from the database:
  ```csharp
  if (partner == null)
  {
      var allPartners = await _partnerProfileRepository.GetAllAsync(null, cancellationToken);
      partner = System.Linq.Enumerable.FirstOrDefault(allPartners);
  }
  ```
* **Impact:** Any authenticated user (e.g., standard student account) can create, edit, delete, or inspect competitions belonging to the primary partner organization.

#### 1.3 Unprotected Mock / Seed Endpoints Exposed in Production
* **File:** [`Devjourney/Controllers/CertificatesController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/CertificatesController.cs#L82-L122)
  * **Endpoint:** `POST /api/certificates/seed-mock`
  * **Issue:** `[AllowAnonymous]` enables arbitrary users on the internet to inject fake competition certificates for all students into the production database.
* **File:** [`Devjourney/Controllers/PartnerInvitationsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PartnerInvitationsController.cs#L47-L53)
  * **Endpoint:** `POST /api/partner-invitations/generate-mock`
  * **Issue:** `[AllowAnonymous]` enables unauthenticated users to create valid 7-day registration invitation tokens for arbitrary company names.

#### 1.4 Unrestricted File Upload Endpoints
* **File:** [`Devjourney/Controllers/FileController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/FileController.cs#L18-L63)
* **Endpoints:**
  * `POST /uploads/image`
  * `POST /uploads/document`
  * `POST /uploads/file`
* **Issue:** Lacks `[Authorize]`, allowing anonymous users to upload arbitrary quantities of image and document files to the server storage, presenting denial-of-service and storage exhaustion risks.

#### 1.5 Unauthenticated Competitions Mutation
* **File:** [`Devjourney/Controllers/PublicCompetitionsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PublicCompetitionsController.cs#L56-L84)
* **Endpoints:**
  * `POST /api/competitions/{id}/teams`
  * `POST /api/competitions/{id}/teams/join`
  * `PUT /api/competitions/{id}/submission`
* **Issue:** Missing `[Authorize]`. Anonymous users can invoke team and submission mutation handlers.

#### 1.6 Unauthenticated Student PII Data Scraping
* **File:** [`Devjourney/Controllers/StudentController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/StudentController.cs#L39-L44)
* **Endpoint:** `GET /api/Student`
* **Issue:** Missing `[Authorize]` and pagination limits. An unauthenticated scraper can retrieve student profiles with full names, phone numbers, university names, and CV links.

#### 1.7 Unprotected Admin Management Routes
* **File:** [`Devjourney/Controllers/AdminController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/AdminController.cs#L6-L26)
* **Endpoints:**
  * `GET /api/admin/companies`
  * `GET /api/admin/users`
  * `GET /api/admin/teams`
  * `GET /api/admin/supporters`
  * `GET /api/admin/certificates`
* **Issue:** Lacks `[Authorize(Roles = "SuperAdmin")]` or `[Authorize(Roles = "Admin")]`.

---

## 2. Incomplete Features & Hardcoded Stubs

| Feature | Location | Status / Observation |
| :--- | :--- | :--- |
| **Posts & Feeds** | [`Devjourney/Controllers/PostsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PostsController.cs#L30-L225) | **100% Commented Out** (`/* ... */`). All endpoints for team search, team member search, networking events, corporate events, and promo courses are inactive. |
| **Admin Portal** | [`Devjourney/Controllers/AdminController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/AdminController.cs#L12-L26) | All 5 actions return dummy empty arrays (`Array.Empty<object>()`). |
| **Support Tickets** | [`Devjourney/Controllers/SupportTicketsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/SupportTicketsController.cs#L12-L35) | Returns empty arrays or mock success responses without database persistence. |
| **Notifications** | [`Devjourney/Controllers/NotificationsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/NotificationsController.cs#L12-L15) & [`NotifyMatchingStudentsHandler.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Posts/Notifications/NotifyMatchingStudentsHandler.cs#L15) | Returns empty array. Handler contains `// TODO: matching logic — deferred`. |
| **Public Profiles** | [`GetPublicProfileQuery.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Profile/Queries/GetPublicProfile/GetPublicProfileQuery.cs#L16-L19) | Hardcoded mock handler returning `"Public Mock User"`. |
| **Public Team & Results Queries** | [`GetMyTeamQuery.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Competitions/Queries/GetMyTeam/GetMyTeamQuery.cs#L16), [`CreateTeamCommand.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Competitions/Commands/CreateTeam/CreateTeamCommand.cs#L16), [`GetMyResultsQuery.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Competitions/Queries/GetMyResults/GetMyResultsQuery.cs#L16), [`JoinTeamCommand.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Competitions/Commands/JoinTeam/JoinTeamCommand.cs#L16), [`UpdateSubmissionCommand.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Application/Modules/Competitions/Commands/UpdateSubmission/UpdateSubmissionCommand.cs#L16) | Returns static stubs (e.g. `Score: 100`, `TeamName: "My Team"`) rather than querying or updating `DataContext`. |
| **Broadcast Messaging** | [`Devjourney/Controllers/CompetitionsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/CompetitionsController.cs#L229-L242) | `GetBroadcasts` and `SendBroadcast` return dummy empty lists / payload echos. |

---

## 3. Recommended Remediation Plan

1. **Enforce Role-Based Authorization:**
   * Add `[Authorize(Roles = "COMPANY_ADMIN,ADMIN")]` to [`PartnerAccountsController`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PartnerAccountsController.cs).
   * Add `[Authorize(Roles = "SUPERADMIN,ADMIN")]` to [`AdminController`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/AdminController.cs).
   * Add `[Authorize]` to [`FileController`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/FileController.cs) and [`PublicCompetitionsController`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PublicCompetitionsController.cs).
   * Add pagination and `[Authorize]` to `GET /api/Student`.

2. **Fix Tenant Isolation Fallback:**
   * In [`CompetitionsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/CompetitionsController.cs), remove the `allPartners.FirstOrDefault()` fallback. Return `403 Forbidden` or `401 Unauthorized` if the authenticated user has no association with the target partner.

3. **Restrict or Remove Mock Endpoints in Production:**
   * Restrict `POST /api/certificates/seed-mock` and `POST /api/partner-invitations/generate-mock` behind environment checks (`if (!_environment.IsDevelopment()) return NotFound();`) or require Admin authentication.

4. **Implement Persistent Handlers for Competitions and Posts:**
   * Uncomment and verify [`PostsController.cs`](file:///home/mahammadjafarli/source/repos/DevJourney/Devjourney/Controllers/PostsController.cs).
   * Implement real database logic in `GetMyTeamQueryHandler`, `CreateTeamCommandHandler`, `JoinTeamCommandHandler`, `UpdateSubmissionCommandHandler`, and `GetPublicProfileQueryHandler`.
