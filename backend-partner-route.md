# Partner Portal Backend Integration Guide

This document is designed for the Frontend Coding Agent tasked with building the UI for the Partner Portal. It details the backend architecture, authentication flows, response schemas, and available API endpoints implemented in the .NET Web API backend.

## 1. Global API Conventions

### Base URL
All general Partner Portal endpoints are prefixed with `/api/partner` unless otherwise specified (e.g., Auth endpoints).

### Authentication
The API uses **JWT Bearer Token Authentication**. 
Tokens should be passed in the `Authorization` header:
`Authorization: Bearer <your_jwt_token>`

### Standard Response Envelope
**Every** backend endpoint conforms to a standard JSON envelope structure. Do not assume raw models are returned. You must unwrap `data`.

**Success Response (2xx):**
```json
{
  "success": true,
  "data": { ... }, // Can be an object, array, or null
  "message": "Optional success message"
}
```

**Error Response (4xx/5xx):**
If an error occurs, the API returns a standard `ProblemDetails` response or a custom error envelope. Look for HTTP status codes (400, 401, 403, 404).

## 2. Authentication & Invitation Flow

### 2.1 Verify Invitation
Users land on the registration page via an invite link. Verify the code before showing the registration form.
- **GET** `/api/partner-invitations/{code}`
- **Auth:** Anonymous
- **Response `data`:**
  ```json
  {
    "code": "INVITE-ABC12345",
    "partnerType": "UNIVERSITY", // or COMPANY, GOVERNMENT, etc.
    "organizationName": "Baku Higher Oil School",
    "expiresAt": "2026-12-31T23:59:59Z",
    "isValid": true
  }
  ```

### 2.2 Register Partner Account
Submits the partner registration form using the valid invite code.
- **POST** `/api/partner-invitations/{code}/register`
- **Auth:** Anonymous
- **Payload:**
  ```json
  {
    "partnerType": "UNIVERSITY",
    "organizationName": "Baku Higher Oil School",
    "email": "career@bhos.edu.az",
    "representativeName": "Elmar Gasimov",
    "representativeRole": "Rector",
    "websiteUrl": "https://bhos.edu.az",
    "password": "SecurePassword123!"
  }
  ```
- **Response:** The partner account is created with `COMPANY_ADMIN` role but marked as `PENDING_ADMIN_REVIEW`.

### 2.3 Login
- **POST** `/api/Auth/login/company`
- **Auth:** Anonymous
- **Payload:** `{ "email": "...", "password": "..." }`
- **Response `data`:**
  ```json
  {
    "accessToken": "eyJhbGciOi...",
    "expiresAt": "2026-08-21T10:00:00Z",
    "user": {
      "id": "guid",
      "companyId": "guid", // Store this in state
      "email": "career@bhos.edu.az",
      "fullName": "Baku Higher Oil School",
      "representativeName": "Elmar Gasimov",
      "role": "Company",
      "partnerType": "UNIVERSITY",
      "isVerified": true
    }
  }
  ```

## 3. Partner Profile

- **GET** `/api/partner/profile` -> Fetch the current partner profile details.
- **PUT** `/api/partner/profile` -> Update partner profile details.

## 4. Competitions Management

- **GET** `/api/partner/Competitions`
  - Fetches all competitions owned by the logged-in partner.
- **GET** `/api/partner/Competitions/{id}`
  - Fetches details for a specific competition.
- **POST** `/api/partner/Competitions/new`
  - Creates a new competition.
- **PUT** `/api/partner/Competitions/{id}`
  - Updates competition metadata.
- **DELETE** `/api/partner/Competitions/{id}`
  - Deletes a competition.
- **PATCH** `/api/partner/Competitions/{id}/lifecycle`
  - Toggles lifecycle stages (e.g. `isRegistrationOpen`, `isJuryActive`).

## 5. Applications & Scoreboard (Competition Scope)

These operations apply to specific competitions owned by the partner.

- **GET** `/api/partner/Competitions/{id}/participants`
  - Fetch list of applicants/teams. Supports `?status=PENDING` query param.
- **PUT** `/api/partner/Competitions/participants/{participantId}/status`
  - **Payload:** `{ "status": "APPROVED" }` // or REJECTED
- **POST** `/api/partner/Competitions/{id}/check-in`
  - Toggles attendance for a participant on the event day.
  - **Payload:** `{ "studentId": "guid" }`
- **GET** `/api/partner/Competitions/{id}/scoreboard`
  - Fetches aggregated jury evaluation scores for teams.

## 6. Sub-Accounts & Jury Management

Used to invite jury members or event supporters (volunteers). Note: Emails are mocked in the backend and printed to the console. The frontend should read the temporary password from the API response to display to the inviter.

- **GET** `/api/partner/accounts`
  - List all created sub-accounts.
- **POST** `/api/partner/accounts`
  - **Payload:**
    ```json
    {
      "fullName": "Dr. Orxan Rəhimov",
      "email": "orxan@company.com",
      "role": "jury", // or "supporter"
      "company": "AzTU",
      "competitionId": "guid"
    }
    ```
  - **Response `data`:** Will contain `account` details and a `credentials` object containing the `temporaryPassword` and `referralCode`.
- **DELETE** `/api/partner/accounts/{id}`
  - Revokes access for a sub-account.

## 7. Certificates

- **POST** `/api/partner/certificates/bulk-issue`
  - Issues certificates to users. Accepts `multipart/form-data` with CSV/Excel lists.
- **GET** `/api/partner/certificates`
  - Fetches history of certificates issued by the partner.
- **GET** `/api/certificates/verify/{codeOrId}`
  - Anonymous endpoint to verify a certificate's authenticity.

## Notes for the Frontend Agent
1. **Roles:** Ensure the user has the `COMPANY_ADMIN` claim (returned as `role: "Company"` in the login response) before rendering the Partner Portal layout.
2. **Mocked Features:** The backend mocks the actual sending of emails (e.g., for sub-account passwords). Read passwords from the API response payload and display them in a UI modal for the admin to copy/share manually for now.
3. **Data Wrapping:** Always destructure `response.data.data` when using Axios/Fetch since the backend wraps payloads in a `data` field. 
4. **Error Handling:** Gracefully catch 400 (Validation) and 404 (Not Found) errors, extracting the message from the standard envelope.
