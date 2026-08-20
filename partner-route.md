# DevJourney Partner Portal — Backend API Specification & Route Contract

> **Document Type:** Production Backend Route & API Integration Contract  
> **Target Audience:** Backend Engineering Team (.NET 9 / ASP.NET Core Web API, Node.js, Go, or Python)  
> **Frontend Stack:** React 18, TanStack Router, TypeScript, Tailwind CSS, Axios  
> **Scope:** Complete Partner / Organizer / Company lifecycle and workspace APIs  
> **Date:** 2026-08-20  
> **Status:** Authoritative Implementation Specification  

---

## Table of Contents

1. [Architectural Overview & Multi-Tenant Security Model](#1-architectural-overview--multi-tenant-security-model)
2. [Global Conventions, Headers, and Error Schemas](#2-global-conventions-headers-and-error-schemas)
3. [Domain Enums, Constants, and Data Types](#3-domain-enums-constants-and-data-types)
4. [Complete Endpoint Directory & Specifications](#4-complete-endpoint-directory--specifications)
   - [Module 1: Partner Authentication & Organization Profile](#module-1-partner-authentication--organization-profile)
   - [Module 2: Partner Dashboard & Metrics Overview](#module-2-partner-dashboard--metrics-overview)
   - [Module 3: Competition Creation & Event Management](#module-3-competition-creation--event-management)
   - [Module 4: Participant Pipeline & Application Review](#module-4-participant-pipeline--application-review)
   - [Module 5: On-Site Physical Attendance & Check-In System](#module-5-on-site-physical-attendance--check-in-system)
   - [Module 6: Partner Staff Accounts (Jury & Support)](#module-6-partner-staff-accounts-jury--support)
   - [Module 7: Digital Certificate Issuance & Verification](#module-7-digital-certificate-issuance--verification)
   - [Module 8: Notification Broadcasts & Support Tickets Hub](#module-8-notification-broadcasts--support-tickets-hub)
   - [Module 9: Partner Scoreboard & Evaluation Visibility](#module-9-partner-scoreboard--evaluation-visibility)
5. [Database Schema & Aggregate Invariants](#5-database-schema--aggregate-invariants)
6. [Business Logic, Edge Cases, and Concurrency Rules](#6-business-logic-edge-cases-and-concurrency-rules)
7. [Error Code Reference Dictionary](#7-error-code-reference-dictionary)
8. [Summary Checklist for Backend Developers](#8-summary-checklist-for-backend-developers)

---

## 1. Architectural Overview & Multi-Tenant Security Model

### 1.1 Tenant Isolation Invariant
The DevJourney platform operates on a multi-tenant hierarchy where **Company / Partner** is the principal organizer tenant:
1. Every `Competition` belongs to exactly one `Company` / `Partner` (`companyId`).
2. Staff accounts (`COMPANY_ADMIN`, `JURY`, `SUPPORTER`) belong strictly to one `Company`.
3. **Tenant Scoping Requirement:** The backend MUST ALWAYS derive `companyId` directly from the authenticated JWT session context (claims/database rehydration). The backend MUST NEVER allow a client to specify or override `companyId` via URL query parameters or request body payloads.
4. Attempting to query, mutate, export, or access another company's competitions, participants, jury evaluations, or attendance MUST return `403 Forbidden` or `404 Not Found`.

### 1.2 Role Hierarchy in Partner Portal

| Role String | Code Claim | Description | Permissions in Partner Scope |
| :--- | :--- | :--- | :--- |
| `Company` / `Partner` | `COMPANY_ADMIN` | Primary company administrator / organizer | Full control over owned competitions, applicants, agenda, staff accounts, certificates, broadcasts, and support. |
| `Jury` | `JURY` | Assigned evaluation judge | Read access to assigned teams and criteria; writes weighted evaluation scores while `isJuryActive = true`. |
| `Supporter` | `SUPPORTER` | On-site event staff | Venue check-in and attendance toggling; **MUST NEVER** receive finalist flags, scores, or sensitive PII. |
| `Volunteer` | `VOLUNTEER` | Event volunteer | Display-only in UI; no independent API write access without explicit role promotion. |
| `SuperAdmin` | `SUPER_ADMIN` | Global platform administrator | Cross-tenant oversight, tenant invitations, master lifecycle overrides, and dispute resolution. |

```
+-------------------------------------------------------------------------------+
|                                SUPER_ADMIN                                    |
|                         (Platform Tenant Oversight)                           |
+---------------------------------------+---------------------------------------+
                                        |
                   +--------------------+--------------------+
                   |                                         |
+------------------v--------------------+ +------------------v------------------+
|      COMPANY / PARTNER TENANT A       | |      COMPANY / PARTNER TENANT B       |
|  (Baku Higher Oil School / AzTU / ...) | |        (Pasha Bank / SOCAR / ...)     |
+------------------+--------------------+ +------------------+--------------------+
                   |                                         |
     +-------------+-------------+             +-------------+-------------+
     |             |             |             |             |             |
+----v----+   +----v----+   +----v----+   +----v----+   +----v----+   +----v----+
|  Jury   |   | Support |   | Volunteer|  |  Jury   |   | Support |   | Volunteer|
| Staff   |   | Staff   |   | (Display)|  | Staff   |   | Staff   |   | (Display)|
+---------+   +---------+   +---------+   +---------+   +---------+   +---------+
```

---

## 2. Global Conventions, Headers, and Error Schemas

### 2.1 Request Headers
All requests interacting with the Partner API MUST specify:
```http
Content-Type: application/json
Accept: application/json
Authorization: Bearer <accessToken>
X-Correlation-ID: 7b817d3d29a54497a7eec434444983e2
```
*Note: For multipart uploads (SVG certificates, cover images, agenda PDFs), omit `Content-Type` so the client boundary is generated automatically.*

### 2.2 Standard Success Envelope
All JSON endpoints MUST respond with a predictable standard envelope:

```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}
```

For paginated collections:
```json
{
  "success": true,
  "data": {
    "items": [ ... ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 142,
    "totalPages": 8
  }
}
```

### 2.3 Standard Error Responses

#### A. Validation Failure (`400 Bad Request` / `422 Unprocessable Entity`)
```json
{
  "success": false,
  "code": "VALIDATION_FAILED",
  "message": "One or more validation errors occurred.",
  "errors": [
    {
      "field": "registrationDeadline",
      "message": "Registration deadline must be earlier than the competition start date."
    },
    {
      "field": "contactEmail",
      "message": "Must be a valid corporate email address."
    }
  ]
}
```

#### B. Tenant Unauthorized / Forbidden (`403 Forbidden`)
```json
{
  "success": false,
  "code": "CROSS_TENANT_ACCESS_DENIED",
  "message": "You do not have permission to access resources outside your organization.",
  "correlationId": "7b817d3d29a54497a7eec434444983e2"
}
```

#### C. Resource Not Found (`404 Not Found`)
```json
{
  "success": false,
  "code": "COMPETITION_NOT_FOUND",
  "message": "The requested competition was not found or does not belong to your organization."
}
```

#### D. Rate Limit Exceeded (`429 Too Many Requests`)
```json
{
  "success": false,
  "code": "RATE_LIMIT_EXCEEDED",
  "message": "Too many requests. Please try again in 30 seconds."
}
```

---

## 3. Domain Enums, Constants, and Data Types

When serializing to and from JSON, the backend should support string representations (preferred) or integer mappings consistently:

### 3.1 `PartnerType`
```typescript
type PartnerType = "COMPANY" | "UNIVERSITY";
```
- `COMPANY` (`1`): Corporate sponsor or enterprise partner.
- `UNIVERSITY` (`2`): Academic institution or university partner.

### 3.2 `ApplicationStatus` / `CandidateStatus`
```typescript
type ApplicationStatus = "PENDING" | "APPROVED" | "ON_HOLD" | "REJECTED";
```
- `PENDING` (`1`): Initial submission awaiting organizer screening.
- `APPROVED` (`2`): Accepted into competition; unlocks team creation/submission rights for student.
- `ON_HOLD` (`3`): Decision deferred/waitlisted; holds `holdAt` timestamp.
- `REJECTED` (`4`): Application rejected; reason logged in audit trail.

### 3.3 `ParticipationFormat`
```typescript
type ParticipationFormat = "TEAM_ONLY" | "TEAM_AND_INDIVIDUAL";
```
- `TEAM_ONLY` (`1`): Must participate in a team of 2–6 members.
- `TEAM_AND_INDIVIDUAL` (`2`): Both solo participants and teams permitted.

### 3.4 `DeploymentRequirement`
```typescript
type DeploymentRequirement = "MANDATORY" | "OPTIONAL" | "NONE";
```
- `MANDATORY` (`1`): Live URL required for final project delivery.
- `OPTIONAL` (`2`): Live URL accepted but not blocking.
- `NONE` (`3`): GitHub repository code is sufficient.

### 3.5 `PitchDeckFormat`
```typescript
type PitchDeckFormat = "FILE" | "LINK" | "BOTH";
```
- `FILE` (`1`): Only PDF / PPTX upload (Max 25 MB).
- `LINK` (`2`): Presentation URL (Figma, Canva, Google Slides).
- `BOTH` (`3`): Accepts file upload, web presentation URL, or both.

### 3.6 `CertificateCategory`
```typescript
type CertificateCategory =
  | "PARTICIPANT"
  | "WINNER"
  | "RUNNER_UP"
  | "FINALIST"
  | "SPECIAL"
  | "SPEAKER"
  | "COMPLETION"
  | "APPRECIATION";
```

### 3.7 `BroadcastType` & `BroadcastAudience`
```typescript
type BroadcastType = "GENERAL" | "URGENT" | "SCHEDULE";

type BroadcastAudience =
  | { kind: "ALL" }
  | { kind: "FINALISTS" }
  | { kind: "TEAM"; teamId: string; teamName: string };
```

### 3.8 `SupportTicketStatus`
```typescript
type SupportTicketStatus = "PENDING" | "RESPONDED" | "CLOSED";
```

---

## 4. Complete Endpoint Directory & Specifications

---

### Module 1: Partner Authentication & Organization Profile

#### 1.1 Verify Partner Invitation Code (Pre-check)
- **Endpoint:** `GET /api/partner-invitations/{code}`
- **Auth:** Public
- **Description:** Validates a single-use invitation code before rendering the registration form on `/register/partner?code=XYZ`.
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "code": "BHOS-INVITE-2026",
    "partnerType": "UNIVERSITY",
    "organizationName": "Baku Higher Oil School",
    "expiresAt": "2026-09-01T23:59:59Z",
    "isValid": true
  }
}
```
- **Error Responses:**
  - `404 Not Found`: `{ "code": "INVITATION_NOT_FOUND", "message": "Invitation code is invalid or does not exist." }`
  - `410 Gone`: `{ "code": "INVITATION_EXPIRED_OR_USED", "message": "This invitation code has already been used or has expired." }`

---

#### 1.2 Register Partner Account via Invite Code
- **Endpoint:** `POST /api/partner-invitations/{code}/register`
- **Auth:** Public
- **Validation Rules:**
  - `confirmCode`: Must match active invitation code.
  - `partnerType`: `"UNIVERSITY"` | `"COMPANY"`.
  - `organizationName`: String, 2–120 characters.
  - `email`: Valid corporate email address (unique across platform).
  - `representativeName`: String, 2–80 characters.
  - `representativeRole`: String, 2–80 characters (e.g. "Head of Career Center").
  - `websiteUrl`: Valid HTTPS URL.
  - `password`: Min 8 chars, at least 1 uppercase letter, 1 number.
- **Request Body:**
```json
{
  "confirmCode": "BHOS-INVITE-2026",
  "partnerType": "UNIVERSITY",
  "organizationName": "Baku Higher Oil School",
  "email": "career@bhos.edu.az",
  "representativeName": "Elmar Gasimov",
  "representativeRole": "Rector / Organizer Lead",
  "websiteUrl": "https://bhos.edu.az",
  "password": "SecurePassword123!"
}
```
- **Response (`201 Created`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "organizationName": "Baku Higher Oil School",
    "partnerType": "UNIVERSITY",
    "email": "career@bhos.edu.az",
    "representativeName": "Elmar Gasimov",
    "verificationStatus": "PENDING_ADMIN_REVIEW",
    "createdAt": "2026-08-20T11:00:00Z"
  },
  "message": "Partner account registered. Awaiting SuperAdmin verification."
}
```

---

#### 1.3 Partner / Company Login
- **Endpoint:** `POST /api/Auth/login/company` (Alias: `POST /api/Auth/login`)
- **Auth:** Public
- **Request Body:**
```json
{
  "email": "career@bhos.edu.az",
  "password": "SecurePassword123!"
}
```
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-08-21T11:00:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "companyId": "9ba85f64-5717-4562-b3fc-2c963f66af99",
      "email": "career@bhos.edu.az",
      "fullName": "Baku Higher Oil School",
      "representativeName": "Elmar Gasimov",
      "role": "Company",
      "partnerType": "UNIVERSITY",
      "isVerified": true
    }
  }
}
```

---

#### 1.4 Get Current Partner Profile
- **Endpoint:** `GET /api/partner/profile`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "9ba85f64-5717-4562-b3fc-2c963f66af99",
    "name": "Baku Higher Oil School",
    "shortName": "BHOS",
    "partnerType": "UNIVERSITY",
    "mission": "Empowering young engineering talents in energy & IT.",
    "website": "https://bhos.edu.az",
    "contactEmail": "career@bhos.edu.az",
    "location": "Baku, Azerbaijan",
    "logoUrl": "https://api.devjourney.az/uploads/logos/bhos.png",
    "bannerUrl": "https://api.devjourney.az/uploads/banners/bhos-banner.jpg",
    "isVerified": true,
    "verificationId": "VRF-BHOS-9921"
  }
}
```

---

#### 1.5 Update Partner Profile
- **Endpoint:** `PUT /api/partner/profile`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "mission": "Innovating the future of Azerbaijani software engineering.",
  "website": "https://bhos.edu.az",
  "contactEmail": "contact@bhos.edu.az",
  "location": "Baku, Azerbaijan",
  "logoUrl": "/uploads/logos/bhos_updated.png",
  "bannerUrl": "/uploads/banners/bhos_updated.png"
}
```
- **Response (`200 OK`):** Updated Profile Object.

---

### Module 2: Partner Dashboard & Metrics Overview

#### 2.1 Get Overview Metrics & Live Status
- **Endpoint:** `GET /api/partner/dashboard`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Description:** Returns aggregate KPI cards and current active competition summary for the authenticated partner.
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "activeCompetition": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "AzTU İnnovativ Həllər Hackathonu 2026",
      "startDate": "2026-07-30T09:00:00Z",
      "endDate": "2026-08-01T20:30:00Z",
      "status": "LIVE"
    },
    "metrics": {
      "totalApplicants": 142,
      "approvedParticipants": 48,
      "checkedInParticipants": 42,
      "formedTeamsCount": 12
    },
    "agendaToday": [
      {
        "id": "day-1-registration",
        "day": 1,
        "time": "09:00 - 10:00",
        "title": "Qeydiyyat və Check-in",
        "description": "İştirakçıların məkana daxil olması və dəstəkçi təsdiqi",
        "status": "COMPLETED"
      },
      {
        "id": "day-1-hacking",
        "day": 1,
        "time": "11:00 - 19:00",
        "title": "Hacking & İdeasiya",
        "description": "Komandaların layihələr üzərində ilk canlı işi",
        "status": "LIVE"
      }
    ]
  }
}
```

---

### Module 3: Competition Creation & Event Management

#### 3.1 Create New Competition
- **Endpoint:** `POST /api/partner/Competitions/new` (Alias: `POST /api/partner/Competitions`)
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Payload Schema:**
  - `title`: String, 3–150 chars (Required)
  - `shortSummary`: String, 10–250 chars (Required)
  - `description`: Markdown text, min 20 chars (Required)
  - `participationFormat`: `"TEAM_ONLY"` (1) or `"TEAM_AND_INDIVIDUAL"` (2)
  - `teamLimit`: Integer between 2 and 6 (Default: 4)
  - `startDate`: ISO 8601 string (Required)
  - `endDate`: ISO 8601 string (Must be >= startDate)
  - `registrationDeadline`: ISO 8601 string (Must be <= startDate)
  - `submissionDeadline`: ISO 8601 string (Optional/Must be <= endDate)
  - `venue`: String, e.g. "AzTU İnnovasiya Mərkəzi" (Required)
  - `locationMapLink`: Valid Google Maps / OpenStreetMap URL (Optional)
  - `tags`: Array of strings or comma-separated tags (e.g. `["Süni İntellekt", "Fintech"]`)
  - `evaluationCriteria`: String or structured rubric criteria summary
  - `coverImageUrl`: String URL of uploaded cover asset (Optional)
  - `contactEmail`: Valid email string (Required)
  - `contactPhone`: String phone number (Optional)
  - `contactSocialLink`: String URL (Telegram, LinkedIn) (Optional)
  - `agendaMode`: `"PDF"` or `"MANUAL"`
  - `agendaPdfUrl`: String URL if `agendaMode == "PDF"`
  - `stages`: Array of agenda rows if `agendaMode == "MANUAL"` (Max 10 rows per day, days 1–3)
  - `gitHubRepositoryRequirement`: `1` (MANDATORY) | `2` (OPTIONAL)
  - `liveDeploymentRequirement`: `1` (MANDATORY) | `2` (OPTIONAL) | `3` (NONE)
  - `pitchDeckFormat`: `1` (FILE) | `2` (LINK) | `3` (BOTH)

- **Request Body:**
```json
{
  "title": "Baku AI & Fintech Hackathon 2026",
  "shortSummary": "Build scalable financial and machine learning micro-services",
  "description": "## Yarış Qaydaları\n48 saatlıq intensiv hakaton...",
  "participationFormat": 1,
  "teamLimit": 4,
  "startDate": "2026-10-15T09:00:00Z",
  "endDate": "2026-10-17T18:00:00Z",
  "registrationDeadline": "2026-10-10T23:59:59Z",
  "submissionDeadline": "2026-10-17T14:00:00Z",
  "venue": "AzTU İnnovasiya Mərkəzi",
  "locationMapLink": "https://maps.google.com/?q=AzTU+Baku",
  "tags": "Süni İntellekt,Fintech,Cloud",
  "evaluationCriteria": "İdeyanın orijinallığı (30%), Texniki icra (40%), Təqdimat (30%)",
  "coverImageUrl": "/uploads/competitions/ai-fintech-cover.png",
  "contactEmail": "hackathon@aztu.edu.az",
  "contactPhone": "+994502341290",
  "contactSocialLink": "https://t.me/aztuhackathon",
  "agendaMode": "MANUAL",
  "gitHubRepositoryRequirement": 1,
  "liveDeploymentRequirement": 2,
  "pitchDeckFormat": 3,
  "stages": [
    {
      "dayNumber": 1,
      "time": "09:00 - 10:00",
      "title": "Qeydiyyat və Check-in"
    },
    {
      "dayNumber": 1,
      "time": "10:00 - 11:30",
      "title": "Açılış Mərasimi və Püşkatma"
    },
    {
      "dayNumber": 2,
      "time": "10:00 - 14:00",
      "title": "Mentorluq Sessiyaları"
    },
    {
      "dayNumber": 3,
      "time": "11:00 - 14:00",
      "title": "Münsif Qiymətləndirməsi və Pitches"
    }
  ]
}
```
- **Response (`201 Created`):**
```json
{
  "success": true,
  "data": {
    "competitionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Baku AI & Fintech Hackathon 2026",
    "slug": "baku-ai-fintech-hackathon-2026",
    "isRegistrationOpen": true,
    "createdAt": "2026-08-20T11:30:00Z"
  },
  "message": "Competition created and published successfully."
}
```

---

#### 3.2 List Partner's Competitions
- **Endpoint:** `GET /api/partner/Competitions`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "AzTU İnnovativ Həllər Hackathonu 2026",
      "startDate": "2026-07-30T09:00:00Z",
      "endDate": "2026-08-01T20:30:00Z",
      "venue": "AzTU İnnovasiya Mərkəzi",
      "coverImageUrl": "/uploads/competitions/aztu-hack.jpg",
      "applicantCount": 142,
      "approvedCount": 48,
      "checkInCount": 42,
      "teamCount": 12,
      "isRegistrationOpen": false,
      "isJuryActive": true,
      "isScoreboardLive": false,
      "isCertificatesPublished": false
    }
  ]
}
```

---

#### 3.3 Get Competition Details & Settings (Partner View)
- **Endpoint:** `GET /api/partner/Competitions/{id}`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Baku Innovation Hackathon 2026",
    "shortSummary": "48 saatlıq rəqəmsal həllər və süni intellekt yarışması.",
    "description": "48 saatlıq rəqəmsal həllər və süni intellekt yarışması...",
    "location": "Bakı İdman Sarayı / A və B zalı",
    "startDate": "2026-10-15",
    "endDate": "2026-10-17",
    "bannerUrl": "/uploads/banners/baku-innovation.jpg",
    "participationFormat": 1,
    "teamLimit": 4,
    "isRegistrationOpen": true,
    "isJuryActive": false,
    "isScoreboardLive": false,
    "isCertificatesPublished": false,
    "tags": ["AI", "Fintech", "GreenTech"],
    "agenda": [
      {
        "id": "day-1-initial",
        "day": 1,
        "time": "09:00 - 10:00",
        "title": "Qeydiyyat və Check-in"
      }
    ]
  }
}
```

---

#### 3.4 Update Competition Settings & Banner
- **Endpoint:** `PUT /api/partner/Competitions/{id}`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "title": "Baku Innovation Hackathon 2026 (Updated)",
  "description": "Yenilənmiş təsvir və qaydalar...",
  "location": "Bakı İdman Sarayı / Əsas Zal",
  "startDate": "2026-10-15",
  "endDate": "2026-10-17",
  "bannerUrl": "/uploads/banners/baku-hack-new.jpg"
}
```
- **Response (`200 OK`):** Updated Competition Object.

---

#### 3.5 Delete Competition
- **Endpoint:** `DELETE /api/partner/Competitions/{id}`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Description:** Deletes competition and associated transient records. *Note: If published certificates or immutable evaluated scores exist, backend SHOULD reject deletion with `409 Conflict` and mandate Archival instead.*
- **Response (`200 OK`):**
```json
{
  "success": true,
  "message": "Competition deleted successfully."
}
```

---

#### 3.6 Toggle Lifecycle Flags
- **Endpoint:** `PATCH /api/partner/Competitions/{id}/lifecycle`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "isRegistrationOpen": false,
  "isJuryActive": true,
  "isScoreboardLive": false,
  "isCertificatesPublished": false
}
```
- **Response (`200 OK`):** Current status of all 4 lifecycle gates.

---

### Module 4: Participant Pipeline & Application Review

#### 4.1 Get Participant Applications (Pipeline Grid)
- **Endpoint:** `GET /api/partner/Competitions/{id}/participants`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Query Parameters:**
  - `stage`: `"requests"` (`PENDING`), `"on_hold"` (`ON_HOLD`), `"approved"` (`APPROVED`), `"checked_in"` (`CHECKED_IN`), or `"rejected"` (`REJECTED`)
  - `q`: String search query (matches candidate name, team name, or university)
  - `page`: Integer (Default: 1)
  - `pageSize`: Integer (Default: 50)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "counts": {
      "requests": 14,
      "on_hold": 3,
      "approved": 48,
      "checked_in": 42
    },
    "items": [
      {
        "id": "candidate-1",
        "name": "Əli Məmmədov",
        "type": "Komanda",
        "teamName": "ByteForge",
        "memberCount": 4,
        "university": "AzTU",
        "appliedAt": "2026-07-21T12:20:00Z",
        "status": "requests",
        "holdAt": null,
        "checkInTime": null,
        "members": [
          {
            "id": "byteforge-ali",
            "username": "ali-mammadov",
            "fullName": "Əli Məmmədov",
            "role": "Kapitan / Fullstack Developer",
            "isLeader": true,
            "university": "AzTU",
            "major": "Kompüter mühəndisliyi",
            "studyYear": "3-cü kurs",
            "email": "ali@byteforge.az",
            "phone": "+994 50 234 12 90",
            "githubUrl": "https://github.com/ali-mammadov",
            "linkedinUrl": "https://linkedin.com/in/ali-mammadov",
            "portfolioUrl": "https://ali-mammadov.dev",
            "cvUrl": "/uploads/cvs/ali-mammadov.pdf",
            "skills": ["React", "TypeScript", "Node.js"]
          },
          {
            "id": "byteforge-aysel",
            "username": "aysel-huseynli",
            "fullName": "Aysel Hüseynli",
            "role": "UI/UX Designer",
            "isLeader": false,
            "university": "AzTU",
            "major": "Dizayn",
            "studyYear": "3-cü kurs",
            "email": "aysel@byteforge.az",
            "phone": "+994 55 410 33 72",
            "skills": ["Figma", "UX Research"]
          }
        ]
      }
    ]
  }
}
```

---

#### 4.2 Update Application Status (Approve / Hold / Reject)
- **Endpoint:** `PUT /api/partner/Competitions/participants/{participantId}/status` (Alias: `PATCH /api/partner/Competitions/{id}/participants/{participantId}`)
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "status": "approved",
  "reason": "Strong portfolio and aligned tech stack."
}
```
*Note: Accepted status values are `"approved"`, `"on_hold"`, `"rejected"`, `"requests"` (or integers `1`, `2`, `3`, `4`).*
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "participantId": "candidate-1",
    "status": "approved",
    "updatedAt": "2026-08-20T11:45:00Z"
  },
  "message": "Candidate application status updated to approved."
}
```

---

### Module 5: On-Site Physical Attendance & Check-In System

#### 5.1 Get Attendance Roster
- **Endpoint:** `GET /api/partner/Competitions/{id}/attendance`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN` or `SUPPORTER`)
- **Description:** Returns full team list with individual member attendance status.
- **Security Invariant:** If caller has role `SUPPORTER`, the backend MUST filter out `isFinalist` or set it to `false` to prevent unauthorized finalist disclosure at check-in desks.
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "totalParticipants": 48,
    "presentCount": 42,
    "teams": [
      {
        "id": "devtrio",
        "teamName": "DevTrio",
        "projectTitle": "EcoPay — Yaşıl ödənişlər",
        "category": "GreenTech",
        "isFinalist": true,
        "members": [
          {
            "id": "devtrio-1",
            "fullName": "Aylin Məmmədova",
            "role": "Komanda rəhbəri",
            "isPresent": true,
            "checkedInAt": "2026-07-30T09:12:00Z"
          },
          {
            "id": "devtrio-2",
            "fullName": "Murad Əliyev",
            "role": "Frontend developer",
            "isPresent": false,
            "checkedInAt": null
          }
        ]
      }
    ]
  }
}
```

---

#### 5.2 Toggle / Update Member Attendance
- **Endpoint:** `PATCH /api/partner/Competitions/{id}/attendance/{memberId}` (Alias: `POST /api/partner/Competitions/{id}/check-in`)
- **Auth:** `Bearer Token` (`COMPANY_ADMIN` or `SUPPORTER`)
- **Request Body:**
```json
{
  "isPresent": true
}
```
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "memberId": "devtrio-1",
    "isPresent": true,
    "checkedInAt": "2026-08-20T09:12:00Z",
    "verifiedBy": "tural.staff@devjourney.az"
  },
  "message": "Attendance status updated."
}
```

---

#### 5.3 Supporter QR Scanner Check-in
- **Endpoint:** `POST /api/supporter/check-in`
- **Auth:** `Bearer Token` (`SUPPORTER` or `COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "competitionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "qrPayload": "DJ-QR-TOKEN-99481a82e9b047"
}
```
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "participantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Əli Məmmədov",
    "teamName": "ByteForge",
    "status": "CHECKED_IN",
    "checkedInAt": "2026-08-20T09:15:00Z"
  },
  "message": "Participant successfully checked in."
}
```

---

### Module 6: Partner Staff Accounts (Jury & Support)

#### 6.1 List Partner Sub-Accounts
- **Endpoint:** `GET /api/partner/accounts`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Description:** Returns all staff accounts provisioned under this partner, grouped by role (`jury`, `support`, `volunteer`).
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": [
    {
      "id": "jury",
      "title": "Münsiflər heyəti",
      "description": "Layihələrin qiymətləndirilməsi və yekun rəylər",
      "countLabel": "hesab",
      "members": [
        {
          "id": "jury-rashad",
          "name": "Dr. Rəşad Əliyev",
          "title": "Süni İntellekt Eksperti",
          "company": "AzTU",
          "email": "rashad@aztu.edu.az",
          "avatarInitial": "R",
          "role": "jury",
          "hasAccessKey": true,
          "createdAt": "2026-07-20T10:00:00Z"
        }
      ]
    },
    {
      "id": "support",
      "title": "Dəstəkçilər (Staff)",
      "description": "Check-in və tədbir məkanı üzrə əməliyyatlar",
      "countLabel": "hesab",
      "members": [
        {
          "id": "support-tural",
          "name": "Tural Qasımov",
          "title": "QR Check-in Məsul",
          "company": "DevJourney",
          "email": "tural.staff@devjourney.az",
          "avatarInitial": "T",
          "role": "support",
          "hasAccessKey": true,
          "createdAt": "2026-07-20T10:00:00Z"
        }
      ]
    },
    {
      "id": "volunteer",
      "title": "Könüllülər korpusu",
      "description": "Qeydiyyat, səhnə və iştirakçı axınına dəstək",
      "countLabel": "şəxs",
      "members": [
        {
          "id": "volunteer-kenan",
          "name": "Kənan Əhmədov",
          "title": "Qeydiyyat Masası Könüllüsü",
          "company": "Volunteers.az",
          "email": "kenan@volunteers.az",
          "avatarInitial": "K",
          "role": "volunteer",
          "hasAccessKey": false
        }
      ]
    }
  ]
}
```

---

#### 6.2 Create Sub-Account (Jury or Support)
- **Endpoint:** `POST /api/partner/accounts`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Security Invariant:** The backend MUST generate a cryptographically random 12-character temporary password and a formatted access code (`JURY-######` or `SUPP-######`). These credentials are ONLY returned in this creation response and must not be stored in plaintext.
- **Request Body:**
```json
{
  "fullName": "Dr. Orxan Rəhimov",
  "email": "orxan.rahimov@company.com",
  "role": "jury",
  "company": "AzTU",
  "competitionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
- **Response (`201 Created`):**
```json
{
  "success": true,
  "data": {
    "account": {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66af77",
      "name": "Dr. Orxan Rəhimov",
      "email": "orxan.rahimov@company.com",
      "role": "jury",
      "title": "Münsif Heyəti",
      "company": "AzTU",
      "avatarInitial": "O",
      "hasAccessKey": true
    },
    "credentials": {
      "temporaryPassword": "X9k#mP2$vL8q",
      "referralCode": "JURY-883192",
      "loginUrl": "https://devjourney.az/login"
    }
  },
  "message": "Account created successfully. Deliver temporary credentials to the user."
}
```

---

#### 6.3 Delete / Deactivate Sub-Account
- **Endpoint:** `DELETE /api/partner/accounts/{id}`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "message": "Account access revoked and removed successfully."
}
```

---

### Module 7: Digital Certificate Issuance & Verification

#### 7.1 Upload & Issue Certificate (Single Student)
- **Endpoint:** `POST /api/certificates/upload`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Content-Type:** `multipart/form-data`
- **Form Data Fields:**
  - `Certificate` or `File`: Binary `.SVG` file stream (Max 5 MB) (Required)
  - `StudentEmail` or `Email`: String recipient email (Required)
  - `Title` or `CompetitionTitle`: String event title (Required)
  - `CompetitionId`: String UUID (Optional)
  - `Desc` or `Description`: String award description/citation (Optional)
  - `Type` or `CertificateType`: `"PARTICIPANT"` | `"WINNER"` | `"RUNNER_UP"` | `"FINALIST"` | `"SPECIAL"` | `"SPEAKER"` (Default: `"PARTICIPANT"`)
  - `IssueDate`: String date `YYYY-MM-DD` (Default: today)

- **Response (`200 OK` / `201 Created`):**
```json
{
  "success": true,
  "data": {
    "id": "CERT-1724151234-892",
    "studentEmail": "ali@byteforge.az",
    "title": "AzTU İnnovativ Həllər Hackathonu 2026",
    "description": "Hackathonda innovativ həll və uğurlu layihə təqdimatına görə təltif olunur.",
    "issueDate": "2026-08-20",
    "type": "WINNER",
    "status": "ISSUED",
    "isPendingUser": false,
    "verificationCode": "DJ-WIN-7821-AZ",
    "svgUrl": "/uploads/certificates/cert-1724151234.svg",
    "pdfUrl": null,
    "issuedAt": "2026-08-20T11:50:00Z"
  },
  "message": "Sertifikat uğurla təqdim edildi."
}
```

*Note on Unregistered Students:* If `studentEmail` is not yet registered in DevJourney, `status` will be `"PENDING"`, `isPendingUser` will be `true`, and the certificate is stored in `PendingCertificates` awaiting user registration.

---

#### 7.2 Bulk Issue Certificates
- **Endpoint:** `POST /api/partner/certificates/bulk-issue`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Content-Type:** `multipart/form-data`
- **Form Data Fields:**
  - `Certificate`: Shared SVG Template File
  - `Emails`: List of emails (comma-separated or JSON array)
  - `Title`: Event title
  - `Type`: Certificate category
  - `IssueDate`: Date string
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "totalCount": 24,
    "successCount": 24,
    "failureCount": 0,
    "results": [
      { "email": "ali@byteforge.az", "verificationCode": "DJ-WIN-1001-AZ", "status": "ISSUED" },
      { "email": "aysel@byteforge.az", "verificationCode": "DJ-WIN-1002-AZ", "status": "ISSUED" }
    ]
  }
}
```

---

#### 7.3 List Partner's Issued Certificates History
- **Endpoint:** `GET /api/partner/certificates`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Query Parameters:** `q` (Search by email, title, code), `type`, `page`, `pageSize`
- **Response (`200 OK`):** Paginated array of issued certificate records.

---

#### 7.4 Public Certificate Verification Lookup
- **Endpoint:** `GET /api/certificates/verify/{codeOrId}`
- **Auth:** Public
- **Description:** Consumed by anyone accessing `/verify/{codeOrId}` or scanning certificate QR codes.
- **Security Invariant:** Never return recipient phone number, email address, or internal scoring data in this public lookup.
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "verificationCode": "DJ-WIN-7821-AZ",
    "recipientFullName": "Əli Məmmədov",
    "competitionTitle": "AzTU İnnovativ Həllər Hackathonu 2026",
    "organizerName": "Baku Higher Oil School",
    "certificateType": "WINNER",
    "issueDate": "2026-08-20",
    "status": "VALID",
    "svgUrl": "/uploads/certificates/cert-1724151234.svg"
  }
}
```

---

### Module 8: Notification Broadcasts & Support Tickets Hub

#### 8.1 List Competition Broadcasts
- **Endpoint:** `GET /api/partner/Competitions/{id}/broadcasts`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": [
    {
      "id": "broadcast-1",
      "title": "Final təqdimat sırası açıqlandı",
      "body": "Finalist komandalar təqdimat saatlarını şəxsi kabinetdə yoxlaya bilərlər.",
      "type": "SCHEDULE",
      "audience": { "kind": "FINALISTS" },
      "createdAt": "2026-07-31T11:40:00+04:00",
      "isUnread": false
    },
    {
      "id": "broadcast-2",
      "title": "Pitch Deck yükləməsi üçün son 30 dəqiqə",
      "body": "Təqdimat faylınızı saat 18:00-dək Yüklənənlər bölməsinə əlavə edin.",
      "type": "URGENT",
      "audience": { "kind": "ALL" },
      "createdAt": "2026-07-31T10:15:00+04:00",
      "isUnread": false
    },
    {
      "id": "broadcast-3",
      "title": "Mentor görüşü təsdiqləndi",
      "body": "CyberGuard komandası üçün mentor görüşü B zalında saat 15:20-də başlayacaq.",
      "type": "GENERAL",
      "audience": { "kind": "TEAM", "teamId": "cyberguard", "teamName": "CyberGuard" },
      "createdAt": "2026-07-31T09:05:00+04:00",
      "isUnread": false
    }
  ]
}
```

---

#### 8.2 Send Broadcast Notification
- **Endpoint:** `POST /api/partner/Competitions/{id}/broadcasts`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "title": "Nahar fasiləsi başladı! 🍕",
  "body": "Ac qarnına kod yazmaq olmaz! Hamını yemək zonasına dəvət edirik.",
  "type": "GENERAL",
  "audience": {
    "kind": "ALL"
  }
}
```
*For Team target:* `"audience": { "kind": "TEAM", "teamId": "cyberguard", "teamName": "CyberGuard" }`  
*For Finalists target:* `"audience": { "kind": "FINALISTS" }`
- **Response (`201 Created`):** Created broadcast object with timestamp.

---

#### 8.3 List Support Tickets
- **Endpoint:** `GET /api/partner/Competitions/{id}/support-tickets`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Query Parameters:** `status` (`"PENDING"`, `"RESPONDED"`, `"CLOSED"`)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "totalUnread": 2,
    "tickets": [
      {
        "id": "ticket-1",
        "teamId": "cyberguard",
        "teamName": "CyberGuard",
        "category": "Texniki dəstək",
        "subject": "Kod deposuna giriş problemi",
        "createdAt": "2026-07-31T12:08:00+04:00",
        "status": "PENDING",
        "unreadCount": 2,
        "messages": [
          {
            "id": "ticket-1-msg-1",
            "author": "TEAM",
            "authorName": "Leyla Rzayeva",
            "body": "Komanda üzvlərindən biri təqdimat deposuna giriş edə bilmir.",
            "createdAt": "2026-07-31T12:05:00+04:00"
          }
        ]
      }
    ]
  }
}
```

---

#### 8.4 Reply to Support Ticket
- **Endpoint:** `POST /api/partner/support-tickets/{ticketId}/messages`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "body": "Dəvət linki komandanızın email ünvanına təkrar göndərildi."
}
```
- **Response (`201 Created`):**
```json
{
  "success": true,
  "data": {
    "id": "ticket-1-msg-2",
    "ticketId": "ticket-1",
    "author": "STAFF",
    "authorName": "Dəstək komandası",
    "body": "Dəvət linki komandanızın email ünvanına təkrar göndərildi.",
    "createdAt": "2026-08-20T11:55:00Z"
  },
  "message": "Message sent; ticket marked as RESPONDED."
}
```

---

#### 8.5 Close Support Ticket
- **Endpoint:** `PATCH /api/partner/support-tickets/{ticketId}`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Request Body:**
```json
{
  "status": "CLOSED"
}
```
- **Response (`200 OK`):** Updated ticket summary.

---

### Module 9: Partner Scoreboard & Evaluation Visibility

#### 9.1 Get Partner Competition Scoreboard
- **Endpoint:** `GET /api/partner/Competitions/{id}/scoreboard`
- **Auth:** `Bearer Token` (`COMPANY_ADMIN`)
- **Query Parameters:**
  - `track`: Filter by Track / Category (`"FinTech"`, `"Kibertəhlükəsizlik"`, `"HealthTech"`, `"AgriTech"`, `"EdTech"`, `"Smart City"`, `"GreenTech"`, `"ALL"`)
  - `q`: Search string (team name or project title)
- **Response (`200 OK`):**
```json
{
  "success": true,
  "data": [
    {
      "id": "cyberguard",
      "teamName": "CyberGuard",
      "projectTitle": "AI Anomaly Detector",
      "track": "Kibertəhlükəsizlik",
      "innovation": 94,
      "technicalExecution": 96,
      "pitch": 91,
      "totalScore": 281,
      "rank": 1
    },
    {
      "id": "healthai",
      "teamName": "HealthAI",
      "projectTitle": "Erkən sağlamlıq risk analizi",
      "track": "HealthTech",
      "innovation": 93,
      "technicalExecution": 91,
      "pitch": 94,
      "totalScore": 278,
      "rank": 2
    }
  ]
}
```

---

## 5. Database Schema & Aggregate Invariants

```
 +-------------------------+            +----------------------------+
 |         Company         | 1        * |      CompanyInvitation     |
 |-------------------------+------------+----------------------------|
 | id (PK, UUID)           |            | id (PK, UUID)              |
 | name (VARCHAR)          |            | code (UNIQUE, VARCHAR)     |
 | partnerType (ENUM)      |            | companyName (VARCHAR)      |
 | isVerified (BOOLEAN)    |            | expiresAt (TIMESTAMPTZ)    |
 +------------+------------+            | isUsed (BOOLEAN)           |
              | 1                       +----------------------------+
              |
              | *
 +------------v------------+            +----------------------------+
 |       Competition       | 1        * |        CheckInLog          |
 |-------------------------+------------+----------------------------|
 | id (PK, UUID)           |            | id (PK, UUID)              |
 | companyId (FK, UUID)    |            | competitionId (FK, UUID)   |
 | title (VARCHAR)         |            | participantId (FK, UUID)   |
 | participationFormat(INT)|            | verifiedBy (UUID)          |
 | teamLimit (INT)         |            | checkedInAt (TIMESTAMPTZ)  |
 | isRegistrationOpen(BOOL)|            +----------------------------+
 | isJuryActive (BOOL)     |
 | isScoreboardLive (BOOL) |            +----------------------------+
 | isCertPublished (BOOL)  | 1        * |        Certificate         |
 +------------+------------+------------+----------------------------|
              | 1                       | id (PK, UUID)              |
              |                         | verificationCode (UNIQUE)  |
              | *                       | recipientEmail (VARCHAR)   |
 +------------v------------+            | recipientId (FK, Nullable) |
 |          Team           |            | type (ENUM)                |
 |-------------------------+            | svgAssetUrl (VARCHAR)      |
 | id (PK, UUID)           |            | status (ISSUED/PENDING)    |
 | competitionId (FK, UUID)|            +----------------------------+
 | name (VARCHAR)          |
 | captainId (FK, UUID)    |
 | isFinalist (BOOL)       |
 | repoUrl (VARCHAR)       |
 | pitchDeckUrl (VARCHAR)  |
 +-------------------------+
```

### Aggregate Invariant Rules
1. **Unique Team Membership:** A student MUST NOT belong to more than one team per `competitionId`.
2. **Atomic Captaincy:** Every team has exactly one captain (`Team.captainId == TeamMember.userId` where `role == 'CAPTAIN'`).
3. **Application Approval Gate:** A student or team cannot create a final submission unless their application status is `APPROVED`.
4. **Jury Score Bounds:** All criteria scores must satisfy `0 <= score <= criterion.maxScore`.
5. **Certificate Uniqueness:** `verificationCode` MUST be globally unique across the entire platform.

---

## 6. Business Logic, Edge Cases, and Concurrency Rules

### 6.1 Certificate Issuance for Unregistered Students (Pending Status)
- When a partner issues a certificate to an email that is not yet registered in DevJourney:
  1. Insert a record into `Certificates` with `recipientId = NULL`, `recipientEmail = targetEmail`, and `status = 'PENDING'`.
  2. When any student registers or verifies their email with `targetEmail`, a database trigger or post-registration hook MUST atomically claim all matching pending certificates by setting `recipientId = newUser.id` and `status = 'ISSUED'`.

### 6.2 Supporter Check-In & Finalist Privacy
- Check-in desks staffed by `SUPPORTER` roles must maintain event integrity.
- **Rule:** The `GET /api/partner/Competitions/{id}/attendance` and `GET /api/supporter/check-in` endpoints MUST sanitize the response and exclude `isFinalist` or any placement rank when called by a user with role `SUPPORTER`. Only `COMPANY_ADMIN` and `SUPER_ADMIN` may view finalist tags.

### 6.3 Competition Deletion vs Archival
- Deleting a competition with active evaluations or issued certificates can corrupt audit history.
- **Rule:** If `isCertificatesPublished == true` or any `JuryEvaluation` rows exist, `DELETE /api/partner/Competitions/{id}` MUST return `409 Conflict`. Partners should be instructed to archive the event instead.

### 6.4 Concurrency & Capacity Limits
- If a competition defines `teamLimit = 4` and max participants, join requests and registration approvals must use database transactional locks (`SELECT FOR UPDATE` or optimistic concurrency tokens) to prevent oversubscribing team rosters.

---

## 7. Error Code Reference Dictionary

| Error Code | HTTP Status | Meaning | Action / Recovery |
| :--- | :--- | :--- | :--- |
| `INVALID_CREDENTIALS` | `401 Unauthorized` | Invalid email or password | Re-enter credentials; do not disclose if account exists. |
| `CROSS_TENANT_ACCESS_DENIED` | `403 Forbidden` | Partner tried to access another company's competition | Ensure requests only query resources belonging to caller's `companyId`. |
| `INVITATION_NOT_FOUND` | `404 Not Found` | Partner invite code does not exist | Check code spelling or contact SuperAdmin. |
| `INVITATION_EXPIRED_OR_USED` | `410 Gone` | Invite code has already been redeemed | Request a new invitation link from platform admin. |
| `COMPETITION_NOT_FOUND` | `404 Not Found` | Competition ID invalid or owned by another tenant | Verify ID in route parameters. |
| `INVALID_SVG_FILE` | `422 Unprocessable` | Uploaded certificate is not valid XML/SVG | Upload a valid `.svg` vector asset. |
| `DUPLICATE_EMAIL` | `409 Conflict` | Sub-account email already registered | Use a different email address or search existing staff. |
| `REGISTRATION_CLOSED` | `403 Forbidden` | Participant actions attempted after deadline | Lifecycle gate is closed. |
| `DELETION_BLOCKED_PUBLISHED_CERTS` | `409 Conflict` | Cannot delete competition with issued certificates | Archive the competition instead of deleting. |
| `RATE_LIMIT_EXCEEDED` | `429 Too Many Requests` | Exceeded 100 requests per minute | Throttle automated requests. |

---

## 8. Summary Checklist for Backend Developers

- [ ] Implement JWT role authorization with strict `COMPANY_ADMIN` role guards on all `/api/partner/*` routes.
- [ ] Ensure all SQL queries filter on `companyId` extracted from the authenticated caller claims.
- [ ] Implement `GET /api/partner-invitations/{code}` and `POST /api/partner-invitations/{code}/register`.
- [ ] Build `POST /api/certificates/upload` with SVG validation and Pending User email resolution.
- [ ] Build `POST /api/partner/accounts` returning temporary 12-char passwords and `JURY-######` access keys.
- [ ] Provide sanitized `GET /api/partner/Competitions/{id}/attendance` masking finalist status for `SUPPORTER` roles.
- [ ] Validate date sequence: `registrationDeadline <= startDate <= endDate`.
- [ ] Return standard error response envelopes with `code`, `message`, and correlation IDs.
