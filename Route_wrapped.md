# DevJourney frontend → backend contract

## Status, scope, and reading guide

This is the backend handoff derived from the current frontend source in `src/` (including its route
files, form schemas, mock data, client state, and the in-repo prototype server). It describes what
the product UI currently asks the backend to support; it does **not** prescribe an internal
architecture.

### Important implementation fact

**Fact:** The browser application currently makes no HTTP requests. Almost every feature uses hard-
coded fixtures, component state, or `localStorage`; some mutations merely display a success message.
Therefore, endpoint names in **Recommended API** sections are contract recommendations for replacing
those mocks, not existing frontend calls.

**Fact:** There is an embedded, unconsumed prototype API at `src/server/competition-platform/api-
router.ts` and a Supabase migration. Its routes are listed in [Existing prototype API](#existing-
prototype-api). Where it agrees with a UI behavior, it is a valuable ready-made contract. It does
not cover the full frontend surface and sometimes uses different models/role names; do not silently
treat it as the product source of truth.

### Conventions

- All date-times should be ISO 8601 strings with offset; dates are `YYYY-MM-DD`.
- IDs are opaque strings in the UI. Use UUIDs internally/at the API unless a human-facing code is
explicitly requested.
- Every recommended JSON endpoint returns either `{ "data": ... }` or `{ "success": true, "data":
..., "error": null }`. The latter matches the prototype. Errors should consistently contain a stable
`code`, a user-safe `message`, and optional field errors.
- Protected requests use `Authorization: Bearer <accessToken>`. The current browser session
validator expects `accessToken`, Unix-second `expiresAt`, and a user object; see
[Authentication](#authentication-and-authorization).
- “Fact” means observed code. “Recommendation” is a minimal backend interpretation required to
connect the observed UI.

## Application routes and access expectations

The route tree has presentation namespaces but, apart from the checks noted below, **does not
enforce authentication or roles**. The backend must enforce authorization for every protected
resource even if the route currently renders for anyone.

| Frontend URL                                | Screen / purpose
| Intended audience and data required
|
| ------------------------------------------- | ----------------------------------------------------
------------------------------------------------------------------------- | ------------------------
----------------------------------------------------------------------------------------------------
---- |
| `/`                                         | Landing page
| Public; static marketing only.
|
| `/login`, `/register`                       | Student login/registration; `/login` also has a Jury
tab                                                                      | Public.
|
| `/register/partner?code=&type=`             | Partner invitation registration
| Public only with invite code. `type` is `UNIVERSITY` or `COMPANY`; code is required by the route.
|
| `/company/login`, `/company/register?code=` | Separate company sign-in/invited registration
| Public. Note the existing prototype instead creates `COMPANY_ADMIN`; see ambiguities.
|
| `/verify`, `/verify/:id`                    | Public certificate lookup
| Public.
|
| `/u/:id`                                    | Public developer profile and verified participation
badges                                                                    | Public.
|
| `/jury-evaluation`                          | Jury’s assigned projects and scoring workspace
| Jury only.
|
| `/supporter/check-in`                       | Supporter check-in workspace
| Supporter only; no finalist data must be disclosed.
|
| `/dashboard/*`                              | Student home, competitions, details/registration,
submission, scoreboard, certificates/results, notifications, profile        | Authenticated student.
A submitted-project route additionally requires approved registration and team/captain submission
access. |
| `/partner/dashboard/*`                      | Partner overview, create/event management,
participant pipeline, attendance, scoreboard, managed accounts, broadcasts/support | Partner/company
organizer. Supporter attendance view must hide finalists.
|
| `/admin/*`                                  | Super Admin overview, users, companies/admins,
teams, jury, supporters, certificates, media, global settings                  | Super Admin only.
`/admin` redirects to `/admin/dashboard`; `/admin/check-in` redirects to `/admin/supporters`.
|
| `/test-certificate`                         | Client-only certificate laboratory
| Developer tool; no production API requirement.
|

**Route facts:** Unknown student competition IDs render not-found.
`/dashboard/competitions/:id/submit` redirects to the competition detail page with
`?notice=submission-restricted` unless the local account is approved for that competition and has a
team. `/register/partner` redirects to `/register` with no `code`.

## Domain model implied by the UI

The following is the smallest shared vocabulary that lets all screens operate. Fields listed are
required by one or more rendered views; extra internal fields are not specified.

| Entity                                    | Required fields / relationships
|
| ----------------------------------------- | ------------------------------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
--------------------------------------------------------- |
| **User / profile**                        | `id`, `email`, `fullName`, username/participant code,
role, account status, verification status, university; optional avatar, gender,
contact/academic/developer profile, social links, CV asset. A user can have profile, notifications,
team memberships, certificates, participation/results and attendance records. |
| **University**                            | `id`, `name`, optionally short name/location. The
registration/profile selectors currently show AzTU, ADA, BDU, BHOS, UNEC; backend must not depend on
labels as IDs.
|
| **Organization / company / partner**      | Organization name/type, representative/contact data,
website, verification/status, organization admin, child staff accounts, hosted competitions. Current
partner registration permits `UNIVERSITY` and `COMPANY`; company admin UI uses
company/leader/invitation fields.                                              |
| **Invitation**                            | Either partner/company invitation or team invitation.
Has opaque code/token, target/scope, creator, expiration/one-time status and timestamps. Team
invites also carry recipient and optional message.
|
| **Competition**                           | Organizer/organization, title, summary/description,
category/tags, participation mode, team limits, registrations, start/end and registration deadlines,
venue/map, media, agenda, rules/submission policy, contact data, status/visibility gates, criteria,
teams, results, broadcasts and certificates.               |
| **Registration/application**              | Belongs to user + competition; status is at least
registered/approved/rejected/on-hold and determines submission access. May bind a participant to a
team.
|
| **Team**                                  | Competition-scoped name, human share/join code,
capacity, track/category, captain, member list/roles, project data, approval/finalist flags,
attendance and result. A user is expected to have at most one team per competition.
|
| **Submission**                            | Team + competition project title, pitch deck file
and/or URL per competition policy, GitHub repository URL, status/timestamps, potentially staff
messages.
|
| **Evaluation rubric/evaluation**          | Competition criteria: name, description, maximum
score, weight. A jury member has team assignments; an evaluation stores one score/comment per
criterion and can be administratively overridden/audited.
|
| **Attendance/check-in**                   | Competition + participant + supporter/staff actor,
present state, checked-in timestamp, verifier. UI permits check-in toggle, although production audit
policy must be resolved.
|
| **Certificate / result / badge**          | Certificate recipient, competition/team,
type/placement, issue date, public verification code, downloadable asset and revocation/published
status. Result can hide criterion scores while still showing placement/feedback. A public profile
participation badge is derived from a verified participation/result.       |
| **Notification/broadcast/support ticket** | User notification has category, title/message, read
state and optional action/invitation status. Partner broadcast targets ALL, FINALISTS or one TEAM.
Ticket has team, category, subject, threaded TEAM/STAFF messages, unread count and
`PENDING`/`RESPONDED`/`CLOSED` status.                                        |
| **Media / platform admin**                | Media asset metadata (title/alt text, category
AVATAR/COVER, URL/path, dimensions, size); platform settings, feature flags, RBAC grants, email
templates, audit logs and operational health metrics.
|

### Relationships and invariants

**Facts from UI and schemas**

- Team capacity is shown and registration dialogs expect captain/member roles and per-member profile
completion. Competition creation constrains maximum team size to **2–6**.
- Project delivery requires a nonblank title, one PDF pitch deck (maximum **25 MB**), and a valid
HTTPS GitHub repository URL. Existing details call for a 2–4/2–5 member range depending on each
competition, so capacity is competition-specific.
- CV metadata permits only PDF/DOC/DOCX, a nonempty file, and a maximum **5 MB**.
- Certificate verification is public and must distinguish an active valid certificate from a
missing/invalid/revoked one.
- Jury scores are bounded by the criterion maximum. The standalone jury UI uses five 0–10 scores in
0.5 increments and cannot save until all are supplied; the admin matrix clamps 0..criterion.max.
- Admin rubric UI allows max score 1–100 and weight 0–100, but the product needs positive weighted
criteria totalling 100 before activating judging (also enforced by the prototype).
- A supporter-facing attendance payload must never include a finalist collection or per-team
finalist flag. The current prototype deliberately removes it.

**Recommendations required to make UI safe**

- Make user/team/competition scope checks transactional: a captain must be a student and a captain
membership; a student cannot join two teams in one competition; team code joins and accepted
invitations must respect capacity/registration status.
- Freeze roster/project/rubric/assignment mutations according to published competition deadlines and
judging state. The current UI exposes these gates but does not consistently enforce them client-
side.
- Never calculate public leaderboard/certificate eligibility from client totals. Use a server-side
published-result policy.
- Keep audit events for privileged edits, invitation creation/use, attendance changes, score
overrides, certificate generation/revocation/publishing and platform settings.

## Authentication and authorization

### Session response required by the browser

**Fact:** `src/shared/auth/browser-session.ts` validates the stored session and checks JWT claims
against this data:

```json
{
  "accessToken": "jwt",
  "expiresAt": 1780000000,
  "user": {
    "id": "uuid",
    "email": "person@example.com",
    "fullName": "Ad Soyad",
    "role": "STUDENT",
    "universityId": "aztu",
    "avatarUrl": "https://...",
    "isVerified": true
  }
}
```

`expiresAt` is Unix seconds and must agree with JWT `exp`; `sub` must equal `user.id`. JWT claims
may use `userRole`/`user_role`/`accountType`/`account_type`, or metadata. The client normalizes
legacy `PARTNER`, `FOUNDER`, `SPONSOR` to `COMPANY` and `EVENT_ORGANIZER` to `ORGANIZER`.

**Role ambiguity:** Client schema roles are `STUDENT`, `UNIVERSITY`, `COMPANY`, `ACADEMY`,
`ORGANIZER`, `ADMIN`; prototype roles are `STUDENT`, `SUPPORTER`, `JURY`, `COMPANY_ADMIN`,
`SUPER_ADMIN`. The sidebar additionally assumes student/partner/admin audiences. Backend and
frontend owners must adopt a single mapping before wiring guards. Recommended canonical roles are
the prototype roles plus a separate organization type (`UNIVERSITY`/`COMPANY`), not a user-role
substitute.

### Auth endpoints (recommended)

| Method / route                          | Request
| Success data                                                       | Authorization and rules
| Error/loading behavior                                                               |
| --------------------------------------- |
-------------------------------------------------------------- |
------------------------------------------------------------------ | -------------------------------
----------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- |
------------------------------------------------------------------------------------ |
| `POST /api/auth/register/student`       | `{fullName, username, email, universityId, password}`
| Session response above (or explicit verification-pending response) | Public. Username 2–32,
`[A-Za-z0-9_.-]`; full name ≥2; valid email; university required; password ≥8 with uppercase and
digit; uniqueness of email/username. `confirmPassword` is UI-only confirmation. | `422` field
errors, conflict for duplicates; UI needs submitting/inline error state. |
| `POST /api/auth/login`                  | `{email,password}`
| Session response                                                   | Public. Same generic
credential failure for missing account/bad password. Student login currently defaults to demo
values; replace with server response.                                                | `401
INVALID_CREDENTIALS`, rate-limit; client shows form status/error.               |
| `POST /api/auth/login/jury`             | `{juryId,password}` or canonical email/password after
decision | Session response with Jury role                                    | Public. Jury page
requires nonblank ID/password but does not navigate today. Resolve whether `juryId` is a login
alias, referral code, or immutable account code.                                       | `401`;
authorization then permits `/jury-evaluation`.                                |
| `POST /api/auth/login/company`          | `{email,password}`
| Session response with company/partner administrator role           | Public. Company login only
checks nonblank client fields today.
| `401`, account pending/revoked errors.                                               |
| `POST /api/auth/logout`                 | none
| `204` or `{data:{}}`                                               | Authenticated; revoke server
session if applicable. UI must clear its browser session/account marker.
| Safe/idempotent.                                                                     |
| `POST /api/auth/password-reset`         | `{email}`
| accepted acknowledgement                                           | Public; used by Super Admin
“send reset” control.
| Do not expose account existence.                                                     |
| `POST /api/auth/password-reset/confirm` | `{token,newPassword}`
| session or acknowledgement                                         | Public token; enforce server
password policy.
| expired/used/invalid token errors.                                                   |
| `GET /api/me`                           | none
| session `user` plus current status/permissions                     | Authenticated. Allows route
guards to replace current local demo bootstrap.
| `401` clears stale session; `403` should explain pending/revoked account.            |

### Invite/partner/company account flows

| Method / route                                  | Request / response
| Rules implied by UI
|
| ----------------------------------------------- | ------------------------------------------------
----------------------------------------------------------------------------------------------------
-------------------------------------------------- | -----------------------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
---------- |
| `GET /api/partner-invitations/{code}`           | Public; returns valid invitation’s organization
type/name constraints and expiry (not secret credential data).
| `/register/partner` merely checks presence; server must reject invalid/expired/used code.
|
| `POST /api/partner-invitations/{code}/register` |
`{partnerType,organizationName,email,representativeName,representativeRole,websiteUrl,password}` →
`{account:{id,organizationName,partnerType,email,representativeName,verificationStatus,createdAt}}`
| Code is read-only in UI; force invite type if supplied. Fields have ≥2/name validation, URL,
password rules. Account is expected to be pending admin review. One-time use must be atomic.
|
| `POST /api/admin/company-invitations`           | `{companyName,leaderName}` →
`{invitation:{id,companyName,leaderName,code,status,expiresAt},registrationUrl,expiresInSeconds}`
| Super Admin. UI generates and displays a **two-minute**, single-use registration link, supports
copy and regeneration after expiry.
|
| `POST /api/company/register`                    |
`{referralCode,companyName?,fullName/firstName,lastName?,email,password}` → company/admin or pending
account                                                                                           |
Existing company screen requires company name, representative full name, email/password confirmation
and read-only referral code. Prototype requires code, email, first/last name/password but takes
company name from its invitation. Resolve one payload/model. |

## Student competition, team, and submission contract

### Reads

| Method / route                          | Response data needed by UI
| Authorization / state
|
| --------------------------------------- | --------------------------------------------------------
----------------------------------------------------------------------------------------------------
------------------------------- | ------------------------------------------------------------------
------------------------------------------------------------- |
| `GET /api/competitions?scope=available` | Paginated `{items:[CompetitionCard]}` with
`id,title,organizer,shortDescription,status (REGISTERED
| REGISTRATION_OPEN
| EXPIRED or canonical equivalent),category,startDate,location,teamSizeLimit,registeredTeamsCount,is
UserRegistered,isApproved,currentTeam?}`                                         | Student.
Competitions list filters only locally, so server filter/pagination is recommended but not presently
required. Include all item state used to render cards/actions. |
| `GET /api/competitions/{id}`            | Competition detail: list-card fields plus
`description,status(LIVE
| UPCOMING
| ENDED),countdownDeadline,location{venueName,address,googleMapsUrl},agenda[{time,title}],rules[],ev
aluationCriteria[{title,weightPercentage,description}],currentTeam?,submission?` | Student. Return
`404` for unknown ID. Hide data that product visibility does not permit.
|
| `GET /api/student/dashboard`            | Current user’s `profileCompletion`, participant code,
counts/XP/rank, approved/registered competitions, active competition/deadline, team,
certificate/result counts, recent notifications. | Student. The UI currently composes this from
multiple mock files; one aggregate response is the smallest practical replacement. |
| `GET /api/competitions/{id}/team`       | `{team:null
| Team}`; Team has
`id,name,code,capacity,members:[{id,name,role,isCaptain,profileCompletionPercentage,avatarUrl?}]`.
| Registered student. Read-only “view team” must expose share code only to permitted
members/captain.                                                                                |

### Registration and roster writes

| Method / route                                | Request                                 | Success
data             | Authorization, validation, and state changes
|
| --------------------------------------------- | --------------------------------------- |
------------------------ | -------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
-------------------------------------------------------- |
| `POST /api/competitions/{id}/teams`           | `{name,trackCategory?,memberUserIds?}`  |
`{team,registration}`    | Student creates team and becomes captain. Competition registration must
be open, name unique per competition, capacity 2–6 / competition limit. UI’s current create modal
only asks team name; track needs a product decision.        |
| `POST /api/competitions/{id}/teams/join`      | `{code}`                                |
`{team,registration}`    | Student joins matching team code. Code required/nonblank; reject invalid,
closed registration, full team, already registered, or conflicting team membership. The detail
dialog allows code but does not validate it yet.             |
| `POST /api/competitions/{id}/teams/match`     | Optional profile/matching preferences   | `{team
| matchCandidates
| pending}`
| Only if the **team matchmaking feature flag** is on. UI labels automatic matching as skills/role
based but supplies no matching inputs; exact algorithm is unresolved. |
| `POST /api/teams/{teamId}/invites`            | `{recipientUserId,message?}`            |
Invitation/notification  | Captain only. Recipient User ID is required; message max **50 words**.
Team must have capacity and be mutable.
|
| `POST /api/team-invitations/{id}/decision`    | `{decision:"ACCEPTED"                   |
"REJECTED"}`             | updated invitation and, on accept, team/registration
| Invited student only; one decision once. Notification action must become read and record decision.
Acceptance enforces team/competition invariants. |
| `DELETE /api/teams/{teamId}/members/{userId}` | optional confirmation                   | updated
team             | Captain only (except a defined self-leave flow). Current UI allows any displayed
member including possibly captain; backend must prohibit orphaning captain/team or define
transfer/disband behavior.                                 |
| `POST /api/teams/{teamId}/captain`            | `{memberId}`                            | updated
team             | Admin-only administrative correction in current Super Admin team UI. Target must
already be team member.
|
| `POST /api/competitions/{id}/registrations`   | `{teamId}` or `{} ` for individual mode |
Registration status/team | Student. The current modal treats create/join/match as one flow then
completes registration after profile warning. It warns, but does **not** block, members below 100%
profile completion. Decide whether that remains warning-only. |

### Submission and staff contact

| Method / route                                           | Request / response
| Rules and states
|
| -------------------------------------------------------- |
---------------------------------------------------------------- | ---------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------- |
| `POST /api/uploads` (or pre-signed upload init/complete) | Multipart PDF or
`{fileName,contentType,size}` → `{assetId,url}` | Required to upload pitch deck. Only
`application/pdf`/`.pdf`, max **25 MB**. Validate content server-side, ownership and malware/storage
policy. UI displays selection/upload validation, a submit state, and a success toast.
|
| `PUT /api/competitions/{id}/submission`                  |
`{projectTitle,pitchDeckAssetId,githubRepoUrl}` → `Submission`   | Captain/team submission authority
only; route access requires approved registration + team. Title nonblank/max **120**; GitHub URL
must be `https://github.com/{owner}/{repo...}` with at least owner/repo segments. Both pitch deck
and GitHub are mandatory in this particular page. Return 403 `SUBMISSION_RESTRICTED` for redirect
notice. Updates after deadline/jury activation need policy. |
| `POST /api/competitions/{id}/support-tickets`            | `{body}` (and optionally
subject/category) → ticket/message      | Student/team member; current “contact staff” message must
be nonblank and max **100 characters**. The partner support inbox reveals the broader ticket fields.
|

### Competition action/empty/loading/error expectations

- Data pages have route pending/error/not-found boundaries; retain an explicit loading state for
each read and `404` for an unknown competition.
- A fresh user has no team, no registered/approved competitions, no certificates/results, no rank
and receives empty-state CTAs.
- Registered but unapproved users must not submit. `isUserRegistered` and `isApproved` are separate
fields in the code.
- Live / upcoming / ended status controls which action is offered; source currently derives it
inconsistently from mock status/date, so backend must define canonical state and send it.

## Profile, public profile, scoreboards, certificates and verification

### Profile endpoints

| Method / route                        | Request / response
| Validation/access
|
| ------------------------------------- | ----------------------------------------------------------
----------------------------------------------------------------------------------------------------
-------------------------------------------------------- | -----------------------------------------
----------------------------------------------------------------------------------------------------
---------------- |
| `GET /api/me/profile`                 | Base user plus extended profile fields below and CV asset
metadata/URL.
| Authenticated user.
|
| `PUT /api/me/profile`                 | `{fullName,email,universityId,gender?,avatarUrl?,phone?,sp
ecialty?,courseYear?,githubUrl?,linkedinUrl?,portfolioUrl?,cvAssetId?,primaryRole?,skills,experience
Level?,bio?,achievements}` → updated profile/completion. | Authenticated self. Required `fullName`
≥2, valid email, university; `gender` MALE/FEMALE; course `1
| 2        | 3
| 4   | GRADUATE | OTHER`; primary role from `UserExtendedProfileSchema`; experience `BEGINNER |
INTERMEDIATE | ADVANCED`; bio max **300**; social URLs valid or empty; arrays can be empty. Avatar
UI restricts its current asset paths, but API should return media asset URLs. |
| `POST /api/uploads/cv`                | CV file → asset metadata
| Authenticated self. PDF/DOC/DOCX, nonempty, max **5 MB**. The UI currently stores metadata only in
localStorage; backend must persist binary and metadata.    |
| `GET /api/public/profiles/{idOrSlug}` | `{id,fullName,avatarUrl?,universityName,specialty?,primary
Role,completionPercentage,githubUrl?,linkedinUrl?,portfolioUrl?,skills,experienceLevel,teamName?,bio
?,participations[]}`                                     | Public. Do not expose email, phone, CV or
unapproved/private participation. Participation includes
`id,competitionName,year,badgeThemeColor,resultTitle(Qalib | Finalist |
İştirakçı),teamName?,certificateUrl?`. The current mock resolves every ID to the same profile:
backend must return 404 rather than impersonating another profile. |

### Scoreboards and results

| Method / route                          | Needed response
| Rules
|
| --------------------------------------- | --------------------------------------------------------
------------------------------------------------------------- | ------------------------------------
----------------------------------------------------------------------------------------------------
-------------------------------------------------------------- |
| `GET /api/scoreboard?view=TEAMS         | COMMUNITIES&period=WEEK
| MONTH
| ALL_TIME&q=` | `{entries:[{id,name,shortName,affiliation,detail,awards,wins,change,points:{WEEK,MO
NTH,ALL_TIME},isCurrent?}], currentStanding?}` | Student view displays a generic developer/community
points leaderboard, which is **not necessarily competition judging results**. Search/filter is local
today. Return a defined points source and tie/rank policy. |
| `GET /api/competitions/{id}/results/me` | `{competitionTitle,organizer,registeredTeamName,placemen
t,isScoresHidden,criteriaScores?,strengths[],recommendation}` | Student team member. Scores may be
hidden while placement/recommendation remains visible.
|
| `GET /api/competitions/{id}/scoreboard` | sanitized team ranking/results appropriate for
partner/student/public policy                                          | Respect `isScoreboardLive`
/ a canonical publication gate. Hide raw jury identity/comments/private participant data. Partner
scoreboard UI additionally displays track and aggregate criterion scores. |

### Certificates

| Method / route                            | Request / response
| Rules
|
| ----------------------------------------- |
--------------------------------------------------------------------------- | ----------------------
----------------------------------------------------------------------------------------------------
------------------------------------------------ |
| `GET /api/me/certificates?type=&q=`       |
`{items:[{id,competitionTitle,organizer,issueDate,type(WINNER               | PARTICIPANT
| SPECIAL),verificationCode,pdfUrl,status?}]}` | Authenticated user. UI searches id/code/title and
filters ALL/WINNER/PARTICIPANT; returns empty state. Only issued/published, non-revoked certificates
are downloadable. |
| `GET /api/certificates/verify/{codeOrId}` |
`{id,fullName,competitionTitle,type,issueDate,status:"VALID"}` or `404/410` | Public, case-
insensitive trimmed lookup. `/verify/:id` pre-fills/starts lookup; result page needs loading then
valid or “not found”. Do not return private profile fields. |
| `GET /api/certificates/{id}/download`     | PDF redirect/stream or signed URL
| Owner or appropriately authorized administrator. The UI opens/downloads `pdfUrl`; external
LinkedIn URL is built client-side from known certificate details.               |

## Notifications and support

| Method / route                               | Request / response                              |
Access / state behavior
|
| -------------------------------------------- | ----------------------------------------------- | -
----------------------------------------------------------------------------------------------------
----------------------------------- |
| `GET /api/me/notifications?filter=&cursor=`  | `NotificationItem[]` / page: `{id,category:TEAM |
COMPETITION
| SYSTEM,title,message,timestamp,isRead,actionType:NONE | LINK
| INVITE,actionUrl?,teamName?,status?}` | Student. Empty-filter state needs support. Current UI
simulates a stream every 12–15 seconds and persists its own local feed; backend should use
SSE/WebSocket/polling, not create random events. |
| `PATCH /api/me/notifications/{id}`           | `{isRead:true}`                                 |
Owner only; individual and “mark all read” actions.
|
| `POST /api/me/notifications/read-all`        | optional filter                                 |
Owner only.
|
| `GET /api/competitions/{id}/broadcasts`      | broadcast list filtered to caller’s audience    |
Participants receive ALL, their TEAM, and FINALISTS only if finalist. Include
`id,title,body,type(GENERAL                                | URGENT
| SCHEDULE),audience,createdAt,isUnread`.
|
| `POST /api/competitions/{id}/broadcasts`     | `{title,body,type,audience:{kind:ALL            |
FINALISTS
| TEAM,teamId?}}`                                       | Partner admin/staff with broadcast
permission. Title/body required; TEAM requires valid team in same competition. Broadcast creates
participant notifications. |
| `GET /api/competitions/{id}/support-tickets` | tickets with thread messages                    |
Partner staff/admin sees competition tickets; team only sees its own.
|
| `POST /api/support-tickets/{id}/messages`    | `{body}`                                        |
Assigned/authorized staff or owning team member; nonblank. A staff reply moves `PENDING` →
`RESPONDED`, clears counterpart unread count. |
| `PATCH /api/support-tickets/{id}`            | `{status:"CLOSED"}`                             |
Partner staff/admin; current UI supports close, not reopening.
|

## Jury evaluation

The standalone jury screen has six assigned sample teams and a fixed five-criterion 0–10 model,
while the admin screen defines a dynamic per-competition rubric. The dynamic model should prevail
for the backend; keep the client-compatible fields required by the screen.

| Method / route                                                                                 |
Request / response
| Authorization/rules
|
| ---------------------------------------------------------------------------------------------- | -
----------------------------------------------------------------------------------------------------
--------------------------------------------------------------------- | ----------------------------
----------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- |
| `GET /api/jury/competitions/{id}/workspace`                                                    | `
{competition,criteria:[{id,label/title,description,maxScore,weight}],assignedTeams:[{id,name,project
Title/project,track,description,projectUrl/submission}],evaluations}` | Jury member assigned to that
competition. Only assigned teams; project links as needed for evaluation.
|
| `PUT /api/jury/competitions/{id}/teams/{teamId}/evaluation`                                    |
`{scores:[{criterionId,score,comments?}]}` → aggregate/evaluations
| Jury only; judging must be active; assignment required; all rubric criteria expected for a
“saved/complete” UI state; no duplicate criterion rows; score 0..max. Upsert the juror’s own scores
only. |
| `GET /api/admin/competitions/{id}/jury`                                                        |
rubric, jury accounts/assignments, evaluations, completion/average metrics
| Super Admin. Supports admin overview and jury management.
|
| `POST /api/admin/competitions/{id}/criteria` / `PUT .../criteria/{criterionId}` / `DELETE ...` |
`{name,description,maxScore,weight}`
| Super Admin. UI min/max shown: score 1–100; weight 0–100; must guard rubric edits once
judging/score publishing policy locks it.
|
| `PATCH /api/admin/evaluations/{evaluationId}`                                                  |
`{score,comments?,reason?}`
| Super Admin only; represents an administrative override and needs actor/time/audit metadata.
|

## Partner portal

### Competition creation/event management

| Method / route                                   | Request body / output
| Authorization and UI rules
|
| ------------------------------------------------ |
------------------------------------------------------------------------ | -------------------------
----------------------------------------------------------------------------------------------------
------------------------------------------ |
| `POST /api/partner/competitions`                 | Competition with:
`title,summary,description,participationType(TEAM_ONLY | TEAM_AND_INDIVIDUAL),teamLimit,startAt,endA
t,registrationClosesAt,venue,mapUrl,tags,evaluationCriteria?,coverAssetId,contactEmail,contactPhone,
socialUrl,agendaMode(PDF | MANUAL),agendaAssetId?,agendaByDay?,submissionRules{deadline,requireGithu
b,deploymentRequirement(MANDATORY | OPTIONAL | NONE),pitchDeckFormat(FILE | LINK | BOTH)}` →
created/published competition | Partner organizer. UI currently publishes immediately without form
validation; backend must at minimum validate date ordering, team limit 2–6, valid URLs/email, tags,
media format and agenda. Manual agenda has days 1–3 and maximum **10 rows/day**; PDF agenda max **10
MB**. |
| `GET /api/partner/competitions/{id}` / `PUT ...` | Event management fields: title, description,
dates, venue, image, etc.   | Partner scoped to organization/competition. `EventManagementPage` only
edits locally; authenticate and report conflict/validation errors.                               |
| `POST /api/uploads/images`                       | image asset
| Cover/media accepts PNG/JPEG/WEBP in UI; enforce a server storage limit (none is stated by the
frontend).                                                               |
| `POST /api/uploads/agenda`                       | PDF asset
| PDF, max 10 MB.
|

### Participants, attendance, scoreboard and accounts

| Method / route                                                    | Request / response
| Authorization/rules
|
| ----------------------------------------------------------------- | ------------------------------
-------------------------------------------------------------------------------------------------- |
----------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------- |
| `GET /api/partner/competitions/{id}/participants?stage=&q=`       | Candidate/team cards including
participant profile details, application state, team/project/roles, links and profile completion. |
Partner organizer/staff. Pipeline UI distinguishes `requests`, `on_hold`, `approved`, `checked_in`,
`rejected`, and individual/team participation.                                   |
| `PATCH /api/partner/competitions/{id}/participants/{id}`          | `{status}`
| Partner authorized actor. Enforce valid transitions; UI currently permits changing status by
drag/action only locally. Approval must control student submission access.              |
| `GET /api/partner/competitions/{id}/attendance`                   | teams + members +
present/check-in state and counts. Partner response may include `isFinalist`; supporter response may
not.      | Partner/supporter scope.
|
| `PATCH /api/partner/competitions/{id}/attendance/{participantId}` | `{present:boolean}` →
attendance log/current state
| Partner/supporter. Record actor/timestamp. Current toggle supports marking both present and
absent; whether absence deletes/reverses a check-in is unresolved and must be auditable. |
| `GET /api/partner/competitions/{id}/scoreboard`                   | ranking plus tracks, aggregate
scores, finalist state as permitted.                                                              |
Partner organizer. Publication/live policy must be consistently enforced; their internal access can
differ from student/public.                                                      |
| `GET /api/partner/accounts`                                       | branches/accounts:
`id,name,email,role(jury
| support
| volunteer),title,company,hasAccessKey`
| Partner administrator. UI calls staff “support” but creation accepts only jury/support; volunteer
is display-only. |
| `POST /api/partner/accounts`                                      |
`{fullName,email,organization?,role:"jury"
| "support"}`→`{account,temporaryPassword?,referralCode?}`
| Partner admin. Name ≥2, valid email, unique within existing accounts (globally recommended). UI
generates 12-char temporary password and `JURY-######`/`SUPP-######` code then says user must change
password. Deliver secret once through a secure channel; do not persist/read it back. |

## Super Admin portal

All endpoints in this section require Super Admin. The UI has no real guard, so backend
authorization is non-negotiable.

### Organizations, user accounts, and teams

| Method / route                                                         | Contract
| UI behavior / validation
|
| ---------------------------------------------------------------------- | -------------------------
----------------------------------------------------------------------------------------------------
---------- | ---------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
-------- |
| `GET /api/admin/companies?query=&role=`                                | Company hierarchy with
company `{id,name,initials,leader,email,status}` and subaccounts `{id,name,email,role:supporter
| jury,status}`.
| Search by company/person and filter role; supports pending/active/expired in UI. |
| `PATCH /api/admin/companies/{id}`                                      | `{name,leader}`
| Nonblank required.
|
| `POST /api/admin/companies/{id}/revoke-access`                         | updated
company/subaccount status
| UI marks company and all subaccounts expired; record reason/audit.
|
| `GET /api/admin/users?query=&role=&status=`                            | User fields:
`id,fullName,email,university,role(STUDENT
| CAPTAIN
| JURY                                                                             |
COMPANY_ADMIN),status(Aktiv | Bloklanıb),associatedTeam,createdAt` plus detail/activity. |
Search/filter, view, role/status edit, deletion, password-reset dispatch, CSV export. Export can be
client-side from page data or a server report endpoint. |
| `PATCH /api/admin/users/{id}`                                          | `{role,status?}`
| Current UI allows role changes; backend must prevent invalid assignments (e.g. arbitrary captain
role with no team) or make role update transactional.                                              |
| `DELETE /api/admin/users/{id}`                                         | acknowledgement
| UI confirms then deletes. Define retention/cascade and never orphan teams/organizations.
|
| `GET /api/admin/competitions/{id}/teams`                               | teams with `competitionId
,name,projectTitle,description,category,captain,members[{id,name,email,role}],repositoryUrl,isFinali
st,status` | Supports filters/search/detail.
|
| `POST /api/admin/teams` / `PATCH /api/admin/teams/{id}` / `DELETE ...` | Team creation/edit fields
above; member endpoints as in student section.
| Create UI requires team name, competition, project title, description, category, captain
name/email; repo URL is optional text. Admin can add/remove members, transfer captain and toggle
finalist. |
| `PATCH /api/admin/teams/{id}/finalist`                                 | `{isFinalist}`
| Super Admin. Changes must feed permitted results/certificate/attendance data.
|

### Supporters, certificates, media, global system management

| Method / route                                            | Contract
| UI behavior / validation
|
| --------------------------------------------------------- | --------------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------- | --------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
---------------------- |
| `GET/POST/PATCH/DELETE /api/admin/supporters...`          | Supporter
`{id,competitionId,name,email,company,totalCheckIns,status:active
| inactive}`.
| Create/edit requires nonblank name/email/company; status toggle. Scope to selected competition. |
| `GET /api/admin/competitions/{id}/check-in-logs`          |
`{id,participantName,participantEmail,teamName,status:present
| absent,checkedInAt?,verifiedBy?}`
| Super Admin can inspect/toggle from logs. Preserve attendance audit semantics.                  |
| `GET /api/admin/certificates?competitionId=`              | Certificate record
`{id,participantName,email,teamName,type,verifyCode,issueDate,status?}` plus publish state.
| Table, preview, edit type, copy verification URL, revoke and bulk generate.
|
| `POST /api/admin/competitions/{id}/certificates/generate` | optional eligibility selection →
generation summary/items
| Current button bulk-generates. Eligibility/template/source asset is not represented; must be
resolved before implementation.
|
| `PATCH /api/admin/certificates/{id}`                      | `{type}`
| UI types are participant, finalist, first/second/third winner; this differs from student `WINNER
| PARTICIPANT                                                                                     |
SPECIAL`. Normalize mapping. |
| `POST /api/admin/certificates/{id}/revoke`                | acknowledgement/status
| Revoked code must fail public verification and downloads.
|
| `PATCH /api/admin/competitions/{id}/publication`          |
`{isCertificatesPublished?,isRegistrationOpen?,isJuryActive?,isScoreboardLive?}`
| UI/pipeline expects independent gates. Jury activation requires valid rubric weight 100; see
prototype caveats.
|
| `GET/POST/DELETE /api/admin/media`                        | Media assets as domain model. `POST`
multipart: `{file,category,title/alt}`.
| AVATAR/COVER only; client accepts PNG/JPG/WEBP and requires title/alt. Read image dimensions/size.
Copy URL, preview and deletion. UI warns when deleting default fallback assets; backend should
similarly prevent/reassign defaults. |
| `GET/PUT /api/admin/settings`                             | `GlobalSettings` exactly as currently
modeled: branding/contact/domain/language/timezone, maintenance text/flag,
2FA/session/password/login thresholds, whitelist, email/SMS/payment/AI/storage values and
upload/retention limits. | Sensitive secrets must be write-only/redacted in reads; UI has SMS secret
reveal toggle but should not receive raw secret after save. Maintenance change needs confirmation.
|
| `GET/PUT /api/admin/feature-flags`                        | `{id,name,description,enabled}`
| UI flags: plagiarism, public scoreboard, matchmaking, public voting. Flags affect feature
availability; do not use client state as enforcement.
|
| `GET/PUT /api/admin/rbac`                                 | role list + permissions
`{id,label,roles[]}`
| Permission toggle matrix. This UI uses display roles (Super Admin, Organizer, Jury Chair, Mentor,
Supporter) that conflict with auth enums; resolve canonical permissions.
|
| `GET /api/admin/system-health`                            |
`{cpuUsagePercent,ramUsagePercent,redisQueueStatus,databaseLatencyMs,activeSessionsCount}`
| Operational screen supports refresh. Metrics can be approximate but must be truthful.
|
| `GET /api/admin/audit-logs?cursor=`                       |
`{items:[{id,action,executor,ipAddress,timestamp,details}]}`
| Read/detail and CSV export.
|
| `GET/PUT /api/admin/email-templates`                      | `{id,name,subject,trigger,body?}`
| UI lists and edits template subjects/body; triggers include user registration, captain invite,
jury creation, certificate publish.
|

## Existing prototype API

This is **implemented server code but currently not called by the UI**. All listed successes are
`{success:true,data:<below>,error:null}` and failures `{success:false,data:null,error:"CODE"}`. JSON
bodies require `Content-Type: application/json`, are capped at 1 MB, and validation failures are
`422`. Authenticated calls use Bearer JWTs.

| Existing method/route
| Data / authorization
| Frontend alignment and gap
|
| --------------------------------------------------------------------------------------------------
------------------------- | ------------------------------------------------------------------------
----------------------------------------------------------- | --------------------------------------
--------------------------------------------------------------------------- |
| `POST /api/auth/login`
| `{email,password}` → `{accessToken,expiresAt,tokenType:"Bearer",user}`; any stored-user role.
| Browser needs a differently shaped `user` (`fullName` vs prototype first/last names) or adapter.
|
| `POST /api/company/register`
| referral code body or `?ref=`, email, first/last name, strong password → user/company IDs.
| Covers invitation registration but not current company form’s companyName/fullName shape or
partner registration. |
| `GET /api/student/dashboard?competitionId=`
| Student + active/specified competition, team, `projectStatus`.
| Useful core, but insufficient for current dashboard cards/profile/certificates/notifications.
|
| `POST /api/student/team`
| Student: `{competitionId?,name,trackCategory}` → team; uses atomic create-captain RPC.
| Good start; lacks team-code join/invites/registration approval.
|
| `POST /api/student/team/members?competitionId=` and `DELETE
/api/student/team/members/{userId}?competitionId=&confirm=true` | Captain add by exactly one
`{userId
| email}` / remove confirmation.
| Matches part of roster management but missing UI team invite flow and some registration gates. |
| `PUT /api/student/team/project?competitionId=`
| Captain `{projectTitle,projectDescription?,repoUrl?,presentationUrl?}`.
| Does not handle required pitch-deck upload or exact GitHub-only 25 MB UI policy.
|
| `GET /api/student/certificate?competitionId=`
| Student certificate if published.
| Missing list, result details, public verify/download.
|
| `GET/POST /api/supporter/check-in?competitionId=`
| Supporter reads sanitized teams/summary; writes `{competitionId?,userId}` toggling check-in.
| Correctly hides finalists; does not expose partner/admin attendance screens.
|
| `GET /api/jury/assigned-teams?competitionId=`, `GET /api/jury/criteria?competitionId=`, `POST
/api/jury/evaluate`           | Jury assignments/rubric/evaluations; evaluate
`{competitionId?,teamId,scores:[{criteriaId,score,comments?}]}`.                      | Strong
overlap; API response needs an adapter for jury UI naming and full project fields.
|
| `GET/POST /api/company/sub-accounts`
| Company Admin lists/creates Jury or Supporter account; creation accepts optional password and
otherwise returns temporary password. | Covers part of partner Accounts; no volunteer, referral code
output, UI role naming differs.                      |
| `GET /api/admin/overview?competitionId=`, `POST /api/admin/company/invite`
| Super Admin metrics / 2-minute company invite.
| Usable overlap.
|
| `GET /api/admin/teams?competitionId=`, `PATCH /api/admin/teams/{id}/finalist`
| Teams / finalist status.
| Missing admin team CRUD, members/captain transfer.
|
| `GET /api/admin/jury?competitionId=`, `PATCH /api/admin/jury/evaluations/{id}`
| Rubric/progress/evaluations / score override.
| Missing rubric CRUD and assignment management.
|
| `PATCH /api/admin/competitions/{id}/toggles`
| Any of registration/jury/scoreboard/certificates boolean gates.
| The route does enforce rubric total 100 before jury activation; further lifecycle guards are
incomplete.          |

## Errors, loading, empty states and response semantics

### Stable errors needed across routes

Use `401 UNAUTHENTICATED`, `403 FORBIDDEN`, `404 NOT_FOUND`, `409 CONFLICT`, `422 VALIDATION_ERROR`,
and resource-specific codes such as `INVITE_EXPIRED`, `INVITE_ALREADY_USED`, `REGISTRATION_CLOSED`,
`REGISTRATION_NOT_APPROVED`, `TEAM_FULL`, `ALREADY_IN_COMPETITION_TEAM`, `CAPTAIN_REQUIRED`,
`SUBMISSION_DEADLINE_PASSED`, `JURY_NOT_ACTIVE`, `TEAM_NOT_ASSIGNED`, `CERTIFICATE_REVOKED`, and
`CERTIFICATES_NOT_PUBLISHED`.

Each write must return the updated resource (or enough data to refresh the local list) so
optimistic/success UI can stop relying on arbitrary timeouts. Field validation should be addressable
by field; current Zod form messages are Azerbaijani, but backend may return stable keys and UI-
localized messages.

### UI-visible empty states

- no team; no available/filtered competition; no approved competition/submission access;
- no certificate, certificate search no result, no results/evaluation scores;
- no notifications for a filter;
- no media/assets or no admin table search results;
- no finalist matching search;
- no participant matches in a pipeline stage;
- no support tickets/broadcasts for a restricted viewer.

### Async requirements

Route boundaries already display pending/error/not-found UI. Endpoint integration must expose
loading/error/retry states for tables/forms/uploads and prevent duplicate submission while a write
is in progress. The existing notification “live stream” is demonstrative; production should
reconnect gracefully and deduplicate event IDs.

## Product ambiguities and contradictions to resolve before backend implementation

1. **Role model conflict.** Client auth roles, navigation roles, partner account roles and prototype
roles do not match. Decide canonical roles, organization type and permission map first.
2. **Two partner enrollment flows.** `/register/partner` expects pending review with
`UNIVERSITY|COMPANY`; `/company/register` expects referral code and immediately routes to dashboard.
Decide whether these are one flow or separate products, and whether invitation field is `code` or
`ref` (route parses `code`; prototype uses `ref`).
3. **Invitation shape/TTL.** Admin Users/Admin Admins pages independently generate 2-minute links,
prototype uses 2 minutes, but route query names and URL base differ. Keep a server-generated,
single-use expiration and one URL convention.
4. **Competition state.** UI uses `REGISTERED/REGISTRATION_OPEN/EXPIRED`, `LIVE/UPCOMING/ENDED`, and
independent booleans. Create one API state plus explicit publication gates; specify
lifecycle/deadline transitions.
5. **Registration semantics.** UI says create/join/match and warns of incomplete profiles, but does
not collect team name/track in the competition dialog or validate join code. Determine required
application data, whether 100% profile is mandatory, and actual automatic matching rules.
6. **Submission policy mismatch.** Student page mandates PDF+GitHub. Partner competition creation
permits pitch deck file/link/both and optional GitHub. Return a per-competition submission policy
and make client validation follow it.
7. **Scoreboard meaning.** Student `/dashboard/scoreboard` is an XP/community leaderboard, whereas
partner/admin competition scoreboards are judging results. They need separate APIs, authorization
and publication policies.
8. **Certificate vocabulary.** Student uses `WINNER|PARTICIPANT|SPECIAL`; Admin uses
participant/finalist/three winner places; prototype uses participant/finalist/winner. Define
canonical certificate/result types and rendering labels.
9. **Supporter check-in semantics.** Prototype toggles check-in (and can remove a log); UI labels
present/absent. Decide whether uncheck is allowed, preserve historic events, and require
reason/audit.
10. **Admin scope.** Some pages expose global settings, RBAC, health, audits, media, users and
certificate issuance; others model a competition/partner tenant. Define Super Admin vs
Organizer/Partner authority and tenant filtering.
11. **Media/security.** UI stores data URLs/local file paths as mocks. Backend needs stable asset
URLs and upload scanning/access policy, but size limits beyond PDF agenda/CV/submission are not
stated.
12. **Legacy client session.** Browser code supports stale localStorage profiles/demo accounts and
accepts legacy role aliases. This should be a migration-only compatibility path, not backend
authentication authority.

## Implementation sequencing (recommended)

1. Agree canonical identities, roles, organization/competition lifecycle, invitation URL and error
envelope.
2. Implement auth, `GET /api/me`, student profile, competitions/registrations/teams,
uploads/submissions, and route guards.
3. Add jury rubric/assignment/evaluation and attendance with strict tenancy/role controls.
4. Add certificates/results/public verification/public profiles and notification delivery.
5. Implement partner/admin management endpoints only for non-placeholder UI actions, preserving
audit trails and publication controls.
