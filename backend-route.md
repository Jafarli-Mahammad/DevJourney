# DevJourney Backend API Integration Specification

> **Target Audience:** Frontend AI Agent & Frontend Engineering Team  
> **Backend Stack:** ASP.NET Core 9 / .NET 9 Web API, Entity Framework Core, JWT Bearer Auth, Output Caching, OpenTelemetry.  
> **Deployment Target:** Render (`https://<your-backend-service>.onrender.com`)

---

## Table of Contents
1. [Deployment & Environment Setup](#1-deployment--environment-setup)
2. [Global Request & Response Conventions](#2-global-request--response-conventions)
3. [Authentication & Session Flow](#3-authentication--session-flow)
4. [Enum Types & Constants Reference](#4-enum-types--constants-reference)
5. [Complete API Endpoints Directory](#5-complete-api-endpoints-directory)
   - [Authentication & Account Management](#51-authentication--account-management-apiauth)
   - [Current User & Public Profile](#52-current-user--public-profile-api)
   - [Student Profile Management](#53-student-profile-management-apistudent)
   - [Student Dashboard](#54-student-dashboard-apistudentdashboard)
   - [Competitions (Public & Student Flow)](#55-competitions-public--student-flow-apicompetitions)
   - [Scoreboard & Results](#56-scoreboard--results-apiscoreboard--apicompetitions)
   - [Partner Portal Competitions](#57-partner-portal-competitions-apipartnercompetitions)
   - [Partner Accounts](#58-partner-accounts-apipartneraccounts)
   - [Jury Workspace & Evaluation](#59-jury-workspace--evaluation-apijury)
   - [University Profiles](#510-university-profiles-apiuniversity)
   - [Lookups & Metadata](#511-lookups--metadata-apilookups)
   - [File Storage & Downloads](#512-file-storage--downloads-uploads)
   - [Admin & Placeholder Endpoints](#513-admin--placeholder-endpoints)
6. [Frontend TypeScript API Client Implementation Guide](#6-frontend-typescript-api-client-implementation-guide)

---

## 1. Deployment & Environment Setup

### Environment Variables
Configure your Frontend environment files (`.env.production`, `.env.development`, `.env.local`):

```env
# Render deployed backend URL (or localhost for local dev)
VITE_API_BASE_URL=https://devjourney-backend.onrender.com
# For Next.js:
NEXT_PUBLIC_API_BASE_URL=https://devjourney-backend.onrender.com
```

### CORS & Security
- **Development:** Accepts all origins, methods, and headers.
- **Production:** Configured via `AllowedOrigins` in backend settings. Ensure your Render frontend URL (e.g. `https://devjourney-frontend.onrender.com`) is added to backend `AllowedOrigins`.

### Rate Limiting
- **Global Policy:** Fixed window of **100 requests per minute** per client IP / Host.
- **Status Code:** `429 Too Many Requests`
- **Response Body:** `"Too many requests. Please try again later."`

---

## 2. Global Request & Response Conventions

### Headers
Every JSON request must include:
```http
Content-Type: application/json
Accept: application/json
```

For protected routes, include the Bearer JWT token:
```http
Authorization: Bearer <accessToken>
```

### Tracing & Correlation IDs
Every response includes a correlation header for debugging:
```http
X-Correlation-ID: 7b817d3d29a54497a7eec434444983e2
```

### Error Response Schema
The backend middleware returns standard structured JSON responses on failure:

#### Validation Error (`400 Bad Request`)
```json
{
  "message": "Validation failed",
  "errors": [
    {
      "propertyName": "Email",
      "errorMessage": "'Email' must be a valid email address."
    }
  ]
}
```

#### Application Bad Request (`400 Bad Request`)
```json
{
  "message": "Invalid email or password.",
  "errors": {}
}
```

#### Not Found (`404 Not Found`)
```json
{
  "message": "Student profile not found"
}
```

#### Unauthorized (`401 Unauthorized`)
```json
{
  "message": "Unauthorized access attempt."
}
```

#### Forbidden (`403 Forbidden`)
```json
{
  "message": "Forbidden access attempt."
}
```

#### Internal Server Error (`500 Internal Server Error`)
```json
{
  "message": "An internal server error occurred.",
  "correlationId": "7b817d3d29a54497a7eec434444983e2"
}
```

---

## 3. Authentication & Session Flow

### Session Response Structure
Upon successful login (`POST /api/Auth/login`, `POST /api/Auth/login/student`, `POST /api/Auth/login/company`, `POST /api/Auth/login/jury`), the backend returns:

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-08-18T18:00:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "student@example.com",
      "fullName": "Ali Mammadov",
      "role": "Student",
      "universityId": "4ba85f64-5717-4562-b3fc-2c963f66afa7",
      "avatarUrl": null,
      "isVerified": true
    }
  }
}
```

### User Roles
- `"Student"`
- `"Company"`
- `"Partner"`
- `"University"`
- `"Jury"`
- `"Admin"` / `"SuperAdmin"`

---

## 4. Enum Types & Constants Reference

When sending or receiving enum values, use the corresponding integer or string as specified:

### `ApplicationStatus`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Pending` | `1` | Submitted, awaiting organizer review |
| `Approved` | `2` | Accepted into competition / submission unlocked |
| `Hold` | `3` | Application put on hold / waitlisted |
| `Rejected` | `4` | Application declined |

### `ParticipationFormat`
| Name | Value | Description |
| :--- | :--- | :--- |
| `TeamOnly` | `1` | Participants must form/join a team |
| `IndividualAndTeam` | `2` | Both solo and team participation allowed |

### `RequirementLevel`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Mandatory` | `1` | Required for submission |
| `Optional` | `2` | Optional field |
| `NotRequired` | `3` | Disabled / not requested |

### `PitchDeckFormat`
| Name | Value | Description |
| :--- | :--- | :--- |
| `FileUpload` | `1` | PDF / PPTX file upload |
| `PresentationLink` | `2` | Figma, Canva, or Google Slides URL |
| `Both` | `3` | Both file upload and link allowed |

### `CompanySize`
| Name | Value |
| :--- | :--- |
| `S1_10` | `0` |
| `S11_50` | `1` |
| `S51_200` | `2` |
| `S200Plus` | `3` |

### `ExperienceLevel`
| Name | Value |
| :--- | :--- |
| `Junior` | `0` |
| `JuniorPlus` | `1` |
| `Middle` | `2` |
| `Senior` | `3` |

### `WorkFormat`
| Name | Value |
| :--- | :--- |
| `Hybrid` | `0` |
| `Remote` | `1` |
| `OnSite` | `2` |

### `LanguageProficiencyLevel`
| Name | Value |
| :--- | :--- |
| `A1` | `0` |
| `A2` | `1` |
| `B1` | `2` |
| `B2` | `3` |
| `C1` | `4` |
| `C2` | `5` |
| `Native` | `6` |

### `PrimaryRole`
| Name | Value |
| :--- | :--- |
| `Programmer` | `0` |
| `Designer` | `1` |
| `QA` | `2` |
| `DevOps` | `3` |
| `PM` | `4` |
| `DataAnalyst` | `5` |
| `RoboticsDeveloper` | `6` |

---

## 5. Complete API Endpoints Directory

### 5.1 Authentication & Account Management (`/api/Auth`)

#### 1. Register Student
- **Endpoint:** `POST /api/Auth/register/student`
- **Auth:** Public
- **Request Body:**
```json
{
  "email": "student@example.com",
  "password": "Password123!",
  "firstName": "Ali",
  "lastName": "Mammadov",
  "universityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
- **Success Response (`201 Created`):**
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

#### 2. Register Company / Partner
- **Endpoint:** `POST /api/Auth/register/company`
- **Auth:** Public
- **Request Body:**
```json
{
  "userName": "techcorp",
  "email": "contact@techcorp.az",
  "password": "Password123!",
  "companyName": "Tech Corp LLC",
  "companySize": 1,
  "companySector": "FinTech",
  "websiteUrl": "https://techcorp.az",
  "linkedInUrl": "https://linkedin.com/company/techcorp",
  "location": "Baku, Azerbaijan",
  "representativeName": "Samir Aliyev",
  "representativeEmail": "samir@techcorp.az"
}
```
- **Success Response (`201 Created`):**
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

#### 3. Register University
- **Endpoint:** `POST /api/Auth/register/University`
- **Auth:** Public
- **Request Body:**
```json
{
  "userName": "bhos_admin",
  "email": "admin@bhos.edu.az",
  "password": "Password123!",
  "universityName": "Baku Higher Oil School",
  "websiteUrl": "https://bhos.edu.az",
  "location": "Baku, Azerbaijan",
  "representativeName": "Elmar Gasimov",
  "representativeEmail": "elmar@bhos.edu.az"
}
```
- **Success Response (`201 Created`):**
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

#### 4. Register Jury Member
- **Endpoint:** `POST /api/Auth/register/jury`
- **Auth:** Public
- **Request Body:**
```json
{
  "juryCode": "JURY-109283",
  "fullName": "Dr. Rashad Karimov",
  "email": "rashad.k@example.com",
  "password": "Password123!",
  "specialization": "AI & Computer Vision",
  "competitionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
- **Success Response (`201 Created`):**
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

#### 5. Login (Student / General)
- **Endpoint:** `POST /api/Auth/login` (or `POST /api/Auth/login/student`)
- **Auth:** Public
- **Request Body:**
```json
{
  "email": "student@example.com",
  "password": "Password123!"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
    "expiresAt": "2026-08-18T18:00:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "student@example.com",
      "fullName": "Ali Mammadov",
      "role": "Student",
      "universityId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "avatarUrl": null,
      "isVerified": true
    }
  }
}
```

---

#### 6. Login Company
- **Endpoint:** `POST /api/Auth/login/company`
- **Auth:** Public
- **Request Body:**
```json
{
  "email": "contact@techcorp.az",
  "password": "Password123!"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
    "expiresAt": "2026-08-18T18:00:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "contact@techcorp.az",
      "fullName": "Tech Corp",
      "role": "Company",
      "isVerified": true
    }
  }
}
```

---

#### 7. Login Jury
- **Endpoint:** `POST /api/Auth/login/jury`
- **Auth:** Public
- **Request Body:**
```json
{
  "juryCode": "JURY-109283",
  "password": "Password123!"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
    "expiresAt": "2026-08-18T18:00:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "rashad.k@example.com",
      "fullName": "Dr. Rashad Karimov",
      "role": "Jury",
      "isVerified": true
    }
  }
}
```

---

#### 8. Logout
- **Endpoint:** `POST /api/Auth/logout`
- **Auth:** Public / Authenticated
- **Success Response (`200 OK`):**
```json
{
  "success": true
}
```

---

#### 9. Request Password Reset
- **Endpoint:** `POST /api/Auth/password-reset`
- **Auth:** Public
- **Request Body:**
```json
{
  "email": "student@example.com"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true
}
```

---

#### 10. Confirm Password Reset
- **Endpoint:** `POST /api/Auth/password-reset/confirm`
- **Auth:** Public
- **Request Body:**
```json
{
  "email": "student@example.com",
  "token": "reset-token-string",
  "newPassword": "NewSecurePassword123!"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true
}
```

---

### 5.2 Current User & Public Profile (`/api`)

#### 1. Get Me (Session Verification)
- **Endpoint:** `GET /api/me`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "student@example.com",
    "role": "Student"
  }
}
```

---

#### 2. Get My Full Profile
- **Endpoint:** `GET /api/me/profile`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Ali Mammadov",
    "bio": "Full-stack developer passionate about .NET and React",
    "avatarUrl": "https://devjourney-backend.onrender.com/uploads/avatars/ali.png"
  }
}
```

---

#### 3. Update My Profile
- **Endpoint:** `PUT /api/me/profile`
- **Auth:** `Bearer Token` Required
- **Request Body:**
```json
{
  "fullName": "Ali Mammadov",
  "bio": "Updated bio text"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Ali Mammadov",
    "bio": "Updated bio text"
  }
}
```

---

#### 4. Upload CV File
- **Endpoint:** `POST /api/uploads/cv`
- **Auth:** `Bearer Token` Required
- **Content-Type:** `multipart/form-data`
- **Form Data Field:** `File` (Binary PDF / DOCX file, max 5 MB)
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "assetId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "url": "/uploads/cvs/550e8400-e29b-41d4-a716-446655440000.pdf"
  }
}
```

---

#### 5. Get Public Profile by ID or Slug
- **Endpoint:** `GET /api/public/profiles/{idOrSlug}`
- **Auth:** Public
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Ali Mammadov",
    "bio": "Full-stack developer",
    "avatarUrl": null
  }
}
```

---

### 5.3 Student Profile Management (`/api/Student`)

#### 1. Get Student Profile by ID
- **Endpoint:** `GET /api/Student/{id}`
- **Auth:** Public / Authenticated
- **Success Response (`200 OK`):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "applicationUserId": "2fa85f64-5717-4562-b3fc-2c963f66afa5",
  "email": "student@example.com",
  "firstName": "Ali",
  "lastName": "Mammadov",
  "universityId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "universityName": "Baku Higher Oil School",
  "phoneNumber": "+994501234567",
  "professionId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
  "professionName": "Software Engineering",
  "course": "4",
  "gitHubUrl": "https://github.com/alimammadov",
  "linkedinUrl": "https://linkedin.com/in/alimammadov",
  "portfolioUrl": "https://alimammadov.dev",
  "cvUrl": "/uploads/cvs/550e8400-e29b-41d4-a716-446655440000.pdf",
  "mainRoleId": "6fa85f64-5717-4562-b3fc-2c963f66afa9",
  "mainRoleName": "Backend Developer",
  "experienceLevel": 2,
  "bio": "Passionate software engineer building scalable cloud systems.",
  "completionPercentage": 90,
  "skills": [
    { "id": "7fa85f64-5717-4562-b3fc-2c963f66af10", "name": "C# / .NET" },
    { "id": "8fa85f64-5717-4562-b3fc-2c963f66af11", "name": "PostgreSQL" }
  ],
  "languages": [
    {
      "languageId": "9fa85f64-5717-4562-b3fc-2c963f66af12",
      "languageName": "English",
      "proficiencyLevel": 4
    }
  ]
}
```

---

#### 2. Get All Student Profiles
- **Endpoint:** `GET /api/Student`
- **Auth:** Public / Authenticated
- **Success Response (`200 OK`):** Array of Student Profiles.

---

#### 3. Update Student Cabinet Profile
- **Endpoint:** `PUT /api/Student/profile`
- **Auth:** `Bearer Token` Required (Student Role)
- **Request Body:**
```json
{
  "studentProfileId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "universityId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "phoneNumber": "+994501234567",
  "professionId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
  "course": "4",
  "gitHubUrl": "https://github.com/alimammadov",
  "linkedinUrl": "https://linkedin.com/in/alimammadov",
  "portfolioUrl": "https://alimammadov.dev",
  "cvUrl": "/uploads/cvs/550e8400-e29b-41d4-a716-446655440000.pdf",
  "mainRoleId": "6fa85f64-5717-4562-b3fc-2c963f66afa9",
  "experienceLevel": 2,
  "bio": "Experienced .NET & React developer.",
  "skillIds": [
    "7fa85f64-5717-4562-b3fc-2c963f66af10",
    "8fa85f64-5717-4562-b3fc-2c963f66af11"
  ],
  "languages": [
    {
      "languageId": "9fa85f64-5717-4562-b3fc-2c963f66af12",
      "proficiencyLevel": 4
    }
  ]
}
```
- **Success Response (`200 OK`):** Updated `StudentProfileDto` object.

---

#### 4. Get Student Profile Completion Percentage
- **Endpoint:** `GET /api/Student/{id}/completion`
- **Auth:** Public / Authenticated
- **Success Response (`200 OK`):**
```json
{
  "percentage": 85
}
```

---

### 5.4 Student Dashboard (`/api/student/dashboard`)

#### 1. Get Student Dashboard Overview
- **Endpoint:** `GET /api/student/dashboard`
- **Auth:** `Bearer Token` Required (Student Role)
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": []
}
```

---

### 5.5 Competitions (Public & Student Flow) (`/api/competitions`)

#### 1. List Available Public Competitions
- **Endpoint:** `GET /api/competitions`
- **Auth:** Public (Cached via OutputCache)
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Baku AI Hackathon 2026",
      "shortSummary": "Build cutting-edge LLM applications for education",
      "startDate": "2026-09-01T09:00:00Z",
      "endDate": "2026-09-03T18:00:00Z",
      "registrationDeadline": "2026-08-25T23:59:59Z",
      "location": "Baku Convention Center",
      "coverImageUrl": "/uploads/images/hackathon.png",
      "maxTeamSize": 4,
      "participationFormat": 1
    }
  ]
}
```

---

#### 2. Get Competition Details by ID
- **Endpoint:** `GET /api/competitions/{id}`
- **Auth:** Public (Cached via OutputCache)
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Baku AI Hackathon 2026",
    "description": "Full markdown description...",
    "stages": [],
    "evaluationCriteria": "Innovation (40%), Technical Execution (40%), Presentation (20%)"
  }
}
```

---

#### 3. Get My Team in Competition
- **Endpoint:** `GET /api/competitions/{id}/team`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "teamId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
    "teamName": "ByteCrafters",
    "inviteCode": "BC-8921",
    "isCaptain": true,
    "members": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Ali Mammadov",
        "role": "Captain"
      }
    ]
  }
}
```

---

#### 4. Create Team for Competition
- **Endpoint:** `POST /api/competitions/{id}/teams`
- **Auth:** `Bearer Token` Required
- **Request Body:**
```json
{
  "teamName": "ByteCrafters"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "teamId": 1,
    "teamName": "ByteCrafters"
  }
}
```

---

#### 5. Join Team via Code
- **Endpoint:** `POST /api/competitions/{id}/teams/join`
- **Auth:** `Bearer Token` Required
- **Request Body:**
```json
{
  "inviteCode": "BC-8921"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "message": "Joined successfully"
  }
}
```

---

#### 6. Submit / Update Project Delivery
- **Endpoint:** `PUT /api/competitions/{id}/submission`
- **Auth:** `Bearer Token` Required (Team Captain)
- **Request Body:**
```json
{
  "githubUrl": "https://github.com/bytecrafters/ai-project",
  "pitchDeckAssetId": "550e8400-e29b-41d4-a716-446655440000"
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "message": "Submission updated"
  }
}
```

---

### 5.6 Scoreboard & Results (`/api/scoreboard` & `/api/competitions`)

#### 1. Public Global Scoreboard
- **Endpoint:** `GET /api/scoreboard`
- **Auth:** Public
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": []
}
```

---

#### 2. Get My Personal Competition Results
- **Endpoint:** `GET /api/competitions/{id}/results/me`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {
    "score": 100
  }
}
```

---

### 5.7 Partner Portal Competitions (`/api/partner/Competitions`)

#### 1. Create New Competition
- **Endpoint:** `POST /api/partner/Competitions/new`
- **Auth:** `Bearer Token` Required (Partner / Company Role)
- **Request Body:**
```json
{
  "title": "Baku FinTech Challenge 2026",
  "shortSummary": "Build open banking APIs and solutions",
  "description": "Comprehensive description in Markdown...",
  "participationFormat": 1,
  "maxTeamSize": 5,
  "startDate": "2026-10-01T09:00:00Z",
  "endDate": "2026-10-05T18:00:00Z",
  "registrationDeadline": "2026-09-20T23:59:59Z",
  "submissionDeadline": "2026-10-04T18:00:00Z",
  "location": "Baku Expo Center",
  "locationMapLink": "https://maps.google.com/?q=Baku+Expo+Center",
  "tags": "FinTech,OpenBanking,AI",
  "evaluationCriteria": "Quality of code, Business feasibility, UI/UX",
  "coverImageUrl": "/uploads/images/fintech.jpg",
  "contactEmail": "organizer@fintech.az",
  "contactPhone": "+994125555555",
  "contactSocialLink": "https://linkedin.com/company/fintech-az",
  "gitHubRepositoryRequirement": 1,
  "liveDeploymentRequirement": 2,
  "pitchDeckFormat": 3,
  "stages": [
    {
      "dayNumber": 1,
      "title": "Opening Ceremony & Hacking Begins",
      "startTime": "2026-10-01T09:00:00Z",
      "endTime": "2026-10-01T12:00:00Z"
    },
    {
      "dayNumber": 2,
      "title": "Mentorship Sessions",
      "startTime": "2026-10-02T10:00:00Z",
      "endTime": "2026-10-02T16:00:00Z"
    }
  ]
}
```
- **Success Response (`200 OK`):**
```json
{
  "message": "Competition created successfully",
  "competitionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 2. Get All Competitions Created by Current Partner
- **Endpoint:** `GET /api/partner/Competitions`
- **Auth:** `Bearer Token` Required (Partner Role)
- **Success Response (`200 OK`):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Baku FinTech Challenge 2026",
    "applicantCount": 42,
    "approvedCount": 30,
    "checkInCount": 28,
    "teamCount": 8
  }
]
```

---

#### 3. Get Competition Details (Partner View)
- **Endpoint:** `GET /api/partner/Competitions/{id}`
- **Auth:** `Bearer Token` Required (Partner Role)
- **Success Response (`200 OK`):** Full competition details object.

---

#### 4. Get Competition Stages
- **Endpoint:** `GET /api/partner/Competitions/{id}/stages`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "dayNumber": 1,
    "title": "Opening Ceremony",
    "startTime": "2026-10-01T09:00:00Z",
    "endTime": "2026-10-01T12:00:00Z"
  }
]
```

---

#### 5. Get Competition Participants Pipeline
- **Endpoint:** `GET /api/partner/Competitions/{id}/participants?status=1`
- **Query Parameter:** `status` (Optional `ApplicationStatus` integer: `1`=Pending, `2`=Approved, `3`=Hold, `4`=Rejected)
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Ali Mammadov (ByteCrafters)",
    "isTeam": true,
    "status": 1,
    "isCheckedIn": false,
    "appliedAt": "2026-08-15T12:30:00Z",
    "projectName": "FinBot AI"
  }
]
```

---

#### 6. Update Participant Application Status
- **Endpoint:** `PUT /api/partner/Competitions/participants/{participantId}/status`
- **Auth:** `Bearer Token` Required
- **Request Body:**
```json
{
  "status": 2
}
```
- **Success Response (`200 OK`):**
```json
{
  "message": "Status updated successfully"
}
```

---

#### 7. Toggle Participant Check-In
- **Endpoint:** `POST /api/partner/Competitions/{id}/check-in`
- **Auth:** `Bearer Token` Required
- **Request Body:**
```json
{
  "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
- **Success Response (`200 OK`):**
```json
{
  "message": "Check-in toggled successfully"
}
```

---

#### 8. Get Competition Partner Scoreboard
- **Endpoint:** `GET /api/partner/Competitions/{id}/scoreboard`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
[
  {
    "participantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "ByteCrafters",
    "totalScore": 95,
    "rank": 1
  }
]
```

---

#### 9. Get Competition Attendance List
- **Endpoint:** `GET /api/partner/Competitions/{id}/attendance`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": []
}
```

---

### 5.8 Partner Accounts (`/api/partner/accounts`)

#### 1. Get Partner Sub-Accounts (Jury & Support staff)
- **Endpoint:** `GET /api/partner/accounts`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": []
}
```

---

#### 2. Create Sub-Account
- **Endpoint:** `POST /api/partner/accounts`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": {}
}
```

---

### 5.9 Jury Workspace & Evaluation (`/api/Jury`)

#### 1. Get Jury Workspace for a Competition
- **Endpoint:** `GET /api/Jury/competitions/{id}/workspace`
- **Auth:** `Bearer Token` Required (Jury Role)
- **Success Response (`200 OK`):**
```json
{
  "success": true,
  "data": []
}
```

---

#### 2. Evaluate Team
- **Endpoint:** `PUT /api/Jury/competitions/{id}/teams/{teamId}/evaluation`
- **Auth:** `Bearer Token` Required (Jury Role)
- **Request Body:**
```json
{
  "scores": [
    {
      "criterionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "score": 9.5
    },
    {
      "criterionId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "score": 8.0
    }
  ]
}
```
- **Success Response (`200 OK`):**
```json
{
  "success": true
}
```

---

#### 3. Get Jury Profile by ID
- **Endpoint:** `GET /api/Jury/{id}`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "juryCode": "JURY-109283",
  "fullName": "Dr. Rashad Karimov",
  "email": "rashad.k@example.com",
  "specialization": "AI & Computer Vision",
  "competitionId": "4fa85f64-5717-4562-b3fc-2c963f66afa7"
}
```

---

#### 4. Get All Jury Profiles
- **Endpoint:** `GET /api/Jury`
- **Auth:** `Bearer Token` Required
- **Success Response (`200 OK`):** Array of `JuryProfileDto`.

---

### 5.10 University Profiles (`/api/University`)

#### 1. List All Universities
- **Endpoint:** `GET /api/University`
- **Auth:** Public
- **Success Response (`200 OK`):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "universityName": "Baku Higher Oil School",
    "websiteUrl": "https://bhos.edu.az",
    "location": "Baku, Azerbaijan",
    "representativeName": "Elmar Gasimov",
    "representativeEmail": "elmar@bhos.edu.az",
    "isVerified": true
  }
]
```

---

#### 2. Get University Profile by ID
- **Endpoint:** `GET /api/University/{id}`
- **Auth:** Public
- **Success Response (`200 OK`):** Single `UniversityProfileDto` object.

---

### 5.11 Lookups & Metadata (`/api/Lookups`)

All lookup endpoints are cached for high performance. Use them to populate dropdown selects in registration and profile editing.

| Endpoint | Method | Response Structure |
| :--- | :--- | :--- |
| `/api/Lookups/professions` | `GET` | `[ { "id": "guid", "name": "Software Engineering" } ]` |
| `/api/Lookups/main-roles` | `GET` | `[ { "id": "guid", "name": "Backend Developer" } ]` |
| `/api/Lookups/skills` | `GET` | `[ { "id": "guid", "name": "C#" }, { "id": "guid", "name": "React" } ]` |
| `/api/Lookups/languages` | `GET` | `[ { "id": "guid", "name": "Azerbaijani" }, { "id": "guid", "name": "English" } ]` |
| `/api/Lookups/roles` | `GET` | `[ { "id": "guid", "name": "Programmer" } ]` |
| `/api/Lookups/IdeaFields` | `GET` | `[ { "id": "guid", "name": "FinTech" }, { "id": "guid", "name": "HealthTech" } ]` |

---

### 5.12 File Storage & Downloads (`/uploads`)

#### Download Uploaded File
- **Endpoint:** `GET /uploads/{containerName}/{objectKey}`
- **Auth:** Public / Authenticated
- **Example:** `GET /uploads/cvs/550e8400-e29b-41d4-a716-446655440000.pdf`
- **Response:** File stream with `Content-Disposition: attachment` to protect against inline script execution.

---

### 5.13 Admin & Placeholder Endpoints

These endpoints return standard empty or mock collections for initial dashboard rendering:

| Endpoint | Method | Auth | Response |
| :--- | :--- | :--- | :--- |
| `/api/certificates` | `GET` | Authenticated | `{ "data": [] }` |
| `/api/notifications` | `GET` | Authenticated | `{ "data": [] }` |
| `/api/support-tickets` | `GET` | Authenticated | `{ "data": [] }` |
| `/api/admin/companies` | `GET` | Admin | `{ "success": true, "data": [] }` |
| `/api/admin/users` | `GET` | Admin | `{ "success": true, "data": [] }` |
| `/api/admin/teams` | `GET` | Admin | `{ "success": true, "data": [] }` |
| `/api/admin/supporters` | `GET` | Admin | `{ "success": true, "data": [] }` |
| `/api/admin/certificates` | `GET` | Admin | `{ "success": true, "data": [] }` |

---

## 6. Frontend TypeScript API Client Implementation Guide

Here is a recommended Axios setup to integrate into the frontend:

```typescript
// src/lib/api-client.ts
import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios';

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://devjourney-backend.onrender.com';

export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
});

// Request Interceptor: Attach JWT Bearer Token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('accessToken');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Auto-logout on 401 Unauthorized
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('user');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);
```

### Example API Functions

```typescript
// src/services/auth.service.ts
import { apiClient } from '@/lib/api-client';

export interface LoginResponse {
  success: boolean;
  data: {
    accessToken: string;
    expiresAt: string;
    user: {
      id: string;
      email: string;
      fullName: string;
      role: string;
      universityId?: string | null;
      avatarUrl?: string | null;
      isVerified: boolean;
    };
  };
}

export const authService = {
  loginStudent: async (email: string, password: string): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>('/api/Auth/login/student', { email, password });
    if (response.data.success) {
      localStorage.setItem('accessToken', response.data.data.accessToken);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
    }
    return response.data;
  },

  registerStudent: async (payload: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    universityId?: string;
  }) => {
    return (await apiClient.post<string>('/api/Auth/register/student', payload)).data;
  },

  getMe: async () => {
    return (await apiClient.get('/api/me')).data;
  },

  logout: async () => {
    try {
      await apiClient.post('/api/Auth/logout');
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('user');
    }
  },
};
```
