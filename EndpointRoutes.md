# DevJourney API Endpoint Routes & Integration Guide

This document provides the complete, authoritative mapping of all backend API endpoints across the separated Swagger definitions, along with payload contracts, authentication rules, and frontend wiring details.

---

## 1. Global API & Authentication Standards

- **Base URL**: `/api` (or configured via environment variable `VITE_API_BASE_URL` / `https://devjourney-0sy5.onrender.com`)
- **Interactive Swagger UI**: `/swagger` (accessible in development mode)
- **Separate Swagger Document JSON URLs**:
  - **DevJourney Core & Student API**: `/swagger/v1/swagger.json`
  - **Partner Portal API**: `/swagger/partner/swagger.json`
  - **Company API**: `/swagger/company/swagger.json`
  - **Admin API**: `/swagger/admin/swagger.json`

### Standard Response Envelope
All backend endpoints return responses in the standard `ApiResponse<T>` envelope or ASP.NET Core `ProblemDetails` on error:

```ts
interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  error?: {
    code: string;
    message: string;
  };
}
```

### Authentication Header
For all protected routes, attach the JWT token in the `Authorization` header:
```http
Authorization: Bearer <accessToken>
```
*Note: Tokens are automatically attached by `apiClient` in `src/lib/api-client.ts` from `devjourney:auth-session` / `localStorage`.*

---

## 2. Swagger Definitions & Endpoints Overview

```
├── 1. Company API (Swagger: company)
│   ├── Company Authentication (Register & Login)
│   ├── Partner Invitations & Onboarding Flow
│   ├── Partner / Company Organization Profile
│   ├── Sub-Accounts Management (Staff & Jury credentials)
│   ├── Hackathons & Competition Lifecycle Management
│   └── Certificate Issuance
│
├── 2. Partner Portal API (Swagger: partner)
│   ├── Includes all Company API endpoints
│   ├── Jury Member Registration & Login (/api/Auth/login/jury)
│   ├── Jury Workspace Retrieval (/api/Jury/competitions/{id}/workspace)
│   └── Team Evaluation & Scoring (/api/Jury/competitions/{id}/teams/{teamId}/evaluation)
│
├── 3. DevJourney API (Student & Core Platform) (Swagger: v1)
│   ├── Student Authentication & Registration
│   ├── Student Dashboard & Progress (/api/student/dashboard)
│   ├── Public Competitions, Teams & Submissions (/api/competitions/...)
│   ├── Scoreboards & Results (/api/scoreboard, /api/competitions/{id}/results/me)
│   ├── Student & Public Profile Management (/api/me/profile, /api/public/profiles/{idOrSlug})
│   ├── Student Certificates & Public Verification (/api/certificates/...)
│   ├── Global Lookups (Skills, Professions, Roles, Idea Fields)
│   ├── University Profiles (/api/University)
│   └── Notifications & Support Tickets
│
└── 4. Admin API (Swagger: admin)
    └── Platform overview listings (Companies, Users, Teams, Supporters, Certificates)
```

---

## 3. Detailed Endpoint Route Specifications

---

### A. Company API (`/swagger/company/swagger.json`)

Used by: **Company Registration, Company Login, and Partner Dashboard**

| HTTP Method | Route URL | Auth Required | Description | Request Payload / Params | Response Data Shape |
|---|---|---|---|---|---|
| `POST` | `/api/Auth/register/company` | Anonymous | Register a new company account | `{ organizationName, email, password, representativeName, representativeRole, websiteUrl }` | `Guid` (Created User ID) |
| `POST` | `/api/Auth/login/company` | Anonymous | Authenticate company administrator | `{ email, password }` | `{ success: true, data: { accessToken, expiresAt, user: { id, email, fullName, role: "COMPANY" } } }` |
| `GET` | `/api/partner-invitations/{code}` | Anonymous | Verify invitation code validity | Route param: `code` | `{ success: true, data: { organizationName, partnerType, email, isValid } }` |
| `POST` | `/api/partner-invitations/{code}/register` | Anonymous | Complete company/partner onboarding via code | `{ organizationName, partnerType: 1 \| 2 \| 3, email, representativeName, representativeRole, websiteUrl, password }` | `{ success: true, data: { ... }, message: "Partner account registered." }` |
| `GET` | `/api/partner/profile` | `Bearer` (`COMPANY`) | Get company profile details | None | `{ success: true, data: PartnerProfileDto }` |
| `PUT` | `/api/partner/profile` | `Bearer` (`COMPANY`) | Update company profile details | `{ organizationName, websiteUrl, representativeName, representativeRole, bio, logoUrl }` | `{ success: true, data: { ... }, message: "Partner profile updated successfully" }` |
| `GET` | `/api/partner/accounts` | `Bearer` (`COMPANY`) | List all staff, jury & support accounts | None | `{ success: true, data: PartnerAccountDto[] }` |
| `POST` | `/api/partner/accounts` | `Bearer` (`COMPANY`) | Create a sub-account (Jury / Supporter) | `{ fullName, email, role: "JURY" \| "SUPPORTER", company, competitionId }` | `{ success: true, data: { id, temporaryPassword, referralCode, loginUrl }, message: "..." }` |
| `DELETE` | `/api/partner/accounts/{id}` | `Bearer` (`COMPANY`) | Delete/revoke a staff or jury account | Route param: `id` (Guid) | `{ success: true, message: "Account access revoked and removed successfully." }` |
| `GET` | `/api/partner/Competitions` | `Bearer` (`COMPANY`) | Get competitions created by current company | None | `{ success: true, data: PartnerCompetitionDto[] }` |
| `POST` | `/api/partner/Competitions/new` | `Bearer` (`COMPANY`) | Create & publish a new competition | `CreateCompetitionDto` (`title`, `description`, `location`, `startDate`, `endDate`, `bannerUrl`, etc.) | `{ success: true, data: { competitionId }, message: "Competition created and published successfully." }` |
| `GET` | `/api/partner/Competitions/{id}` | `Bearer` (`COMPANY`) | Get single competition details | Route param: `id` (Guid) | `{ success: true, data: CompetitionDetailDto }` |
| `PUT` | `/api/partner/Competitions/{id}` | `Bearer` (`COMPANY`) | Update competition settings & dates | `CreateCompetitionDto` | `{ success: true, data: { competitionId }, message: "Competition updated successfully" }` |
| `DELETE` | `/api/partner/Competitions/{id}` | `Bearer` (`COMPANY`) | Delete competition | Route param: `id` (Guid) | `{ success: true, message: "Competition deleted successfully" }` |
| `PATCH` | `/api/partner/Competitions/{id}/lifecycle` | `Bearer` (`COMPANY`) | Update competition status (e.g. Active, Completed) | `{ status: "Active" \| "Draft" \| "Published" \| "Completed" }` | `{ success: true, data: { ... }, message: "Competition lifecycle updated successfully" }` |
| `GET` | `/api/partner/Competitions/{id}/stages` | `Bearer` (`COMPANY`) | Get timeline stages for competition | Route param: `id` (Guid) | `{ success: true, data: CompetitionStageDto[] }` |
| `GET` | `/api/partner/Competitions/{id}/participants` | `Bearer` (`COMPANY`) | Get registered participants pipeline | Route param: `id`, Query: `status?` | `{ success: true, data: CompetitionParticipantDto[] }` |
| `PUT` | `/api/partner/Competitions/participants/{participantId}/status` | `Bearer` (`COMPANY`) | Approve / Reject / Waitlist participant | `{ status: ApplicationStatus }` | `{ success: true, data: { participantId, status }, message: "Status updated successfully" }` |
| `POST` | `/api/partner/Competitions/{id}/check-in` | `Bearer` (`COMPANY` / `SUPPORTER`) | Toggle participant attendance check-in | `{ studentId: Guid }` | `{ success: true, data: { studentId }, message: "Check-in toggled successfully" }` |
| `GET` | `/api/partner/Competitions/{id}/attendance` | `Bearer` (`COMPANY` / `SUPPORTER`) | Get full attendance matrix | Route param: `id` (Guid) | `{ success: true, data: { teams: [...] } }` |
| `GET` | `/api/partner/Competitions/{id}/scoreboard` | `Bearer` (`COMPANY`) | Get competition scoreboard | Route param: `id` (Guid) | `{ success: true, data: ScoreboardDto[] }` |
| `GET` | `/api/partner/certificates` | `Bearer` (`COMPANY`) | List certificates issued by company | None | `{ success: true, data: CertificateDto[] }` |
| `POST` | `/api/partner/certificates/bulk-issue` | `Bearer` (`COMPANY`) | Bulk issue certificates to students | `multipart/form-data` (`CompetitionId`, `StudentIds`, `TemplateId`, `Type`) | `{ success: true, data: { issuedCount: number } }` |

---

### B. Partner Portal & Jury API (`/swagger/partner/swagger.json`)

Includes all Company API routes above, plus the Jury evaluation subsystem:

| HTTP Method | Route URL | Auth Required | Description | Request Payload / Params | Response Data Shape |
|---|---|---|---|---|---|
| `POST` | `/api/Auth/register/jury` | Anonymous | Register jury profile | `{ fullName, email, password, profession, company, bio }` | `Guid` (Created User ID) |
| `POST` | `/api/Auth/login/jury` | Anonymous | Jury authentication with jury code | `{ juryCode, password }` (or `{ email, password }`) | `{ success: true, data: { accessToken, expiresAt, user: { id, fullName, role: "JURY" } } }` |
| `GET` | `/api/Jury` | `Bearer` | List all jury profiles | None | `JuryProfileDto[]` |
| `GET` | `/api/Jury/{id}` | `Bearer` | Get specific jury profile | Route param: `id` (Guid) | `JuryProfileDto` |
| `GET` | `/api/Jury/competitions/{id}/workspace` | `Bearer` (`JURY`) | Load jury scoring workspace & criteria | Route param: `id` (Competition Guid) | `{ competition: { id, title }, criteria: [{ id, title, weight, maxScore }], assignedTeams: [{ id, teamName, projectTitle, projectDescription, repoUrl, deckUrl, evaluation }] }` |
| `PUT` | `/api/Jury/competitions/{id}/teams/{teamId}/evaluation` | `Bearer` (`JURY`) | Submit or update team scores and feedback | `{ score: number, feedback: string, scores: [{ criterionId, score }] }` | `{ success: true }` |

---

### C. DevJourney Core & Student API (`/swagger/v1/swagger.json`)

Used by: **Student Portal, Public Competitions, Leaderboards, User Profiles, Certificate Verification**

#### 1. Authentication & Session
| HTTP Method | Route URL | Auth Required | Description | Request Payload | Response Data Shape |
|---|---|---|---|---|---|
| `POST` | `/api/Auth/register/student` | Anonymous | Register a student account | `{ fullName, username, email, universityId, password }` | `Guid` (Created User ID) |
| `POST` | `/api/Auth/register/University` | Anonymous | Register a university account | `{ universityName, email, password, contactPerson }` | `Guid` (Created User ID) |
| `POST` | `/api/Auth/login/student` | Anonymous | Student login | `{ email, password }` | `{ success: true, data: { accessToken, expiresAt, user: { id, email, fullName, role: "STUDENT" } } }` |
| `POST` | `/api/Auth/login` | Anonymous | Generic user login | `{ email, password }` | `{ success: true, data: { accessToken, expiresAt, user } }` |
| `POST` | `/api/Auth/logout` | `Bearer` | Revoke session & clear cookies | None | `{ success: true }` |
| `POST` | `/api/Auth/password-reset` | Anonymous | Request password reset email | `{ email }` | `{ success: true }` |
| `POST` | `/api/Auth/password-reset/confirm` | Anonymous | Confirm password reset with token | `{ token, newPassword }` | `{ success: true }` |

#### 2. Student Profile & Dashboard
| HTTP Method | Route URL | Auth Required | Description | Request Payload / Params | Response Data Shape |
|---|---|---|---|---|---|
| `GET` | `/api/student/dashboard` | `Bearer` (`STUDENT`) | Aggregate student dashboard metrics | None | `{ profileCompletion, rank, points, activeCompetitions: [...], recentBadges: [...] }` |
| `GET` | `/api/Student/profile` | `Bearer` (`STUDENT`) | Get logged-in student profile | None | `StudentProfileDto` |
| `PUT` | `/api/Student/profile` | `Bearer` (`STUDENT`) | Update student profile data | `UpdateStudentProfileCommand` | `StudentProfileDto` |
| `GET` | `/api/Student/{id}` | Anonymous / `Bearer` | Get student profile by ID | Route param: `id` | `StudentProfileDto` |
| `GET` | `/api/Student/{id}/completion` | Anonymous / `Bearer` | Get profile completion percentage | Route param: `id` | `{ percentage: number, missingFields: string[] }` |
| `GET` | `/api/me` | `Bearer` | Get current authenticated user info | None | `{ success: true, data: UserDto }` |
| `GET` | `/api/me/profile` | `Bearer` | Get current user's full profile | None | `{ success: true, data: FullProfileDto }` |
| `PUT` | `/api/me/profile` | `Bearer` | Update current user's profile | `UpdateProfileCommand` | `{ success: true, data: FullProfileDto }` |
| `POST` | `/api/uploads/cv` | `Bearer` | Upload CV document (PDF/DOC/DOCX) | `multipart/form-data` (`file`) | `{ success: true, data: { assetId, fileUrl } }` |
| `GET` | `/api/public/profiles/{idOrSlug}` | Anonymous | Public developer profile & badges | Route param: `idOrSlug` | `{ success: true, data: PublicProfileDto }` |

#### 3. Public Competitions, Teams & Submissions
| HTTP Method | Route URL | Auth Required | Description | Request Payload / Params | Response Data Shape |
|---|---|---|---|---|---|
| `GET` | `/api/competitions` | Anonymous | List available competitions | None (Cached: `PublicListings`) | `{ success: true, data: CompetitionListItemDto[] }` |
| `GET` | `/api/competitions/{id}` | Anonymous | Get competition details & agenda | Route param: `id` (Cached: `PublicDetails`) | `{ success: true, data: CompetitionDetailsDto }` |
| `GET` | `/api/competitions/{id}/team` | `Bearer` | Get user's current team in competition | Route param: `id` | `{ success: true, data: TeamDto }` |
| `POST` | `/api/competitions/{id}/teams` | `Bearer` (`STUDENT`) | Create a new team (as captain) | `{ name: string }` | `{ success: true, data: TeamDto }` |
| `POST` | `/api/competitions/{id}/teams/join` | `Bearer` (`STUDENT`) | Join existing team via share code | `{ code: string }` | `{ success: true, data: TeamDto }` |
| `PUT` | `/api/competitions/{id}/submission` | `Bearer` (`STUDENT`) | Submit team pitch deck & GitHub repo | `{ projectTitle, pitchDeckAssetId, githubRepoUrl }` | `{ success: true, data: SubmissionDto }` |

#### 4. Scoreboards, Certificates & Lookups
| HTTP Method | Route URL | Auth Required | Description | Request Payload / Params | Response Data Shape |
|---|---|---|---|---|---|
| `GET` | `/api/scoreboard` | Anonymous | Global platform points leaderboard | None (Cached: `PublicListings`) | `{ success: true, data: ScoreboardEntryDto[] }` |
| `GET` | `/api/competitions/{id}/results/me` | `Bearer` (`STUDENT`) | Get current user's team result & feedback | Route param: `id` | `{ success: true, data: TeamResultDto }` |
| `GET` | `/api/certificates` | `Bearer` | Get authenticated user's certificates | None | `{ success: true, data: CertificateDto[] }` |
| `POST` | `/api/certificates/upload` | `Bearer` | Upload a certificate asset | `multipart/form-data` (`file`, `title`) | `{ success: true, certificateId: Guid }` |
| `GET` | `/api/certificates/verify/{codeOrId}` | Anonymous | Public certificate verification lookup | Route param: `codeOrId` | `{ success: true, data: VerifiedCertificateDto }` |
| `GET` | `/api/Lookups/professions` | Anonymous | List available professions | None | `ProfessionDto[]` |
| `GET` | `/api/Lookups/main-roles` | Anonymous | List primary roles (Frontend, Backend, etc.) | None | `MainRoleDto[]` |
| `GET` | `/api/Lookups/skills` | Anonymous | List developer skills | None | `SkillDto[]` |
| `GET` | `/api/Lookups/languages` | Anonymous | List languages | None | `LanguageDto[]` |
| `GET` | `/api/Lookups/roles` | Anonymous | List user roles | None | `RoleDto[]` |
| `GET` | `/api/Lookups/IdeaFields` | Anonymous | List startup / hackathon idea fields | None | `IdeaFieldDto[]` |
| `GET` | `/api/University` | Anonymous | List all universities | None | `UniversityDto[]` |
| `GET` | `/api/University/{id}` | Anonymous | Get university profile | Route param: `id` | `UniversityDto` |
| `GET` | `/api/notifications` | `Bearer` | Get user notifications feed | None | `{ data: NotificationDto[] }` |
| `GET` | `/api/support-tickets` | `Bearer` | Get support tickets list | None | `{ data: SupportTicketDto[] }` |
| `GET` | `/uploads/{containerName}/{objectKey}` | Anonymous | Secure static file download (CV/Images) | Route params: `containerName`, `objectKey` | File binary stream |

---

### D. Admin API (`/swagger/admin/swagger.json`)

Used by: **Super Admin Dashboard**

| HTTP Method | Route URL | Auth Required | Description | Response Data Shape |
|---|---|---|---|---|
| `GET` | `/api/admin/companies` | `Bearer` (`SUPER_ADMIN`) | List registered companies & verification statuses | `{ success: true, data: CompanyAdminDto[] }` |
| `GET` | `/api/admin/users` | `Bearer` (`SUPER_ADMIN`) | List all platform users | `{ success: true, data: UserAdminDto[] }` |
| `GET` | `/api/admin/teams` | `Bearer` (`SUPER_ADMIN`) | List all competition teams across platform | `{ success: true, data: TeamAdminDto[] }` |
| `GET` | `/api/admin/supporters` | `Bearer` (`SUPER_ADMIN`) | List registered event supporters & volunteers | `{ success: true, data: SupporterAdminDto[] }` |
| `GET` | `/api/admin/certificates` | `Bearer` (`SUPER_ADMIN`) | List all issued platform certificates | `{ success: true, data: CertificateAdminDto[] }` |

---

## 4. Frontend Service-to-Endpoint Mapping

Here is the exact frontend file map showing where each API call is implemented:

| Feature / UI Flow | Frontend API Service / Component | Backend Endpoints Called |
|---|---|---|
| **API Client Core** | [src/lib/api-client.ts](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/lib/api-client.ts) | Interceptors, Bearer token injection, `ApiResponse<T>` unwrapping, 401 handling |
| **Partner Onboarding** | [src/features/partner-registration/PartnerRegistrationPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-registration/PartnerRegistrationPage.tsx) | `GET /api/partner-invitations/{code}`<br/>`POST /api/partner-invitations/{code}/register` |
| **Company Login** | [src/features/company-registration/CompanyLoginPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/company-registration/CompanyLoginPage.tsx) | `POST /api/Auth/login/company` |
| **Jury Login** | [src/features/auth/components/LoginPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/auth/components/LoginPage.tsx) (`JuryLoginForm`) | `POST /api/Auth/login/jury` |
| **Jury Workspace** | [src/features/jury-evaluation/jury-api.ts](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/jury-evaluation/jury-api.ts)<br/>[src/features/jury-evaluation/JuryEvaluationPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/jury-evaluation/JuryEvaluationPage.tsx) | `GET /api/Jury/competitions/{id}/workspace`<br/>`PUT /api/Jury/competitions/{id}/teams/{teamId}/evaluation` |
| **Partner Competitions** | [src/features/partner-dashboard/services/partner-api.ts](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/services/partner-api.ts)<br/>[src/features/partner-dashboard/EventManagementPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/EventManagementPage.tsx) | `GET /api/partner/Competitions`<br/>`POST /api/partner/Competitions/new`<br/>`PUT /api/partner/Competitions/{id}`<br/>`DELETE /api/partner/Competitions/{id}`<br/>`PATCH /api/partner/Competitions/{id}/lifecycle` |
| **Sub-Accounts (Staff/Jury)** | [src/features/partner-dashboard/PartnerAccountsPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/PartnerAccountsPage.tsx)<br/>[src/features/partner-dashboard/PartnerAccountCreationDialog.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/PartnerAccountCreationDialog.tsx) | `GET /api/partner/accounts`<br/>`POST /api/partner/accounts`<br/>`DELETE /api/partner/accounts/{id}` |
| **Attendance & Check-in** | [src/features/partner-dashboard/TeamAttendancePage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/TeamAttendancePage.tsx) | `GET /api/partner/Competitions/{id}/attendance`<br/>`POST /api/partner/Competitions/{id}/check-in` |
| **Certificates Management** | [src/features/partner-dashboard/services/certificate-api.ts](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/services/certificate-api.ts)<br/>[src/features/partner-dashboard/IssueCertificatePage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/partner-dashboard/IssueCertificatePage.tsx) | `GET /api/partner/certificates`<br/>`POST /api/partner/certificates/bulk-issue` |
| **Public Verification** | [src/features/certificate-verification/CertificateVerificationPage.tsx](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/certificate-verification/CertificateVerificationPage.tsx) | `GET /api/certificates/verify/{codeOrId}` |
| **Student Competitions** | [src/features/dashboard/competition-details/competition-details-data.ts](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/dashboard/competition-details/competition-details-data.ts) | `GET /api/competitions`<br/>`GET /api/competitions/{id}`<br/>`GET /api/competitions/{id}/team`<br/>`PUT /api/competitions/{id}/submission` |
| **Student Dashboard** | [src/features/dashboard/account/dashboard-account.ts](file:///home/mahammadjafarli/source/repos/DevJourneyFront/devjourney/src/features/dashboard/account/dashboard-account.ts) | `GET /api/student/dashboard`<br/>`GET /api/me/profile` |

---

## 5. Summary of Enums & Type Mappings

### Partner Type Enum
When registering via `/api/partner-invitations/{code}/register`:
- `1` = **Corporate / Company** (`"COMPANY"` / `"CORPORATE"`)
- `2` = **University / Academic** (`"UNIVERSITY"`)
- `3` = **Community / NGO** (`"COMMUNITY"`)

### Sub-Account Role Strings
When creating staff via `/api/partner/accounts`:
- `"JURY"` = Jury member (evaluates assigned hackathon teams)
- `"SUPPORTER"` = Event staff / volunteer (manages check-in & logistics)

### Competition Lifecycle Statuses
When patching lifecycle via `/api/partner/Competitions/{id}/lifecycle`:
- `"Draft"` = In preparation, visible only to organizer
- `"Published"` = Registrations open, visible on public platform
- `"Active"` = Hackathon ongoing, check-in & submissions live
- `"Completed"` = Event closed, evaluations final, certificates issued
