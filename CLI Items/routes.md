# Application Routes

This project uses TanStack Start file-based routing. Route files live in `src/routes`, while `src/routeTree.gen.ts` is generated automatically and must not be edited manually.

The tables below describe the routes currently registered in the generated route tree. `:id` means that the URL segment is dynamic; the corresponding route filename uses `$id`.

## Route status

- **Functional** — renders an implemented page or workflow.
- **Layout** — wraps child routes and renders them through `<Outlet />`.
- **Redirect** — immediately sends the visitor to another route.
- **Guarded** — checks route-specific input or access before rendering.
- **Placeholder** — renders a temporary section page for functionality that is not implemented yet.
- **Developer tool** — intended for development and testing rather than normal product navigation.

## Public and authentication routes

| URL                 | Route file                         | Status         | What it does                                                                                                                                                       |
| ------------------- | ---------------------------------- | -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `/`                 | `src/routes/index.tsx`             | Functional     | Renders the public DevJourney landing page.                                                                                                                        |
| `/login`            | `src/routes/login.tsx`             | Functional     | Renders the main account login page. Accepts a validated internal `?from=/path` return URL.                                                                        |
| `/register`         | `src/routes/register.tsx`          | Functional     | Renders the main user registration flow.                                                                                                                           |
| `/register/partner` | `src/routes/register_.partner.tsx` | Guarded        | Handles partner invitation registration. Requires `?code=...`; without a code it redirects to `/register`. It can also accept `type=UNIVERSITY` or `type=COMPANY`. |
| `/company/login`    | `src/routes/company/login.tsx`     | Functional     | Renders the dedicated company account login page.                                                                                                                  |
| `/company/register` | `src/routes/company/register.tsx`  | Functional     | Renders company registration and accepts an optional validated `?code=...` referral code.                                                                          |
| `/verify`           | `src/routes/verify.tsx`            | Functional     | Opens the certificate verification form.                                                                                                                           |
| `/verify/:id`       | `src/routes/verify.$id.tsx`        | Functional     | Opens certificate verification with the certificate ID already selected.                                                                                           |
| `/u/:id`            | `src/routes/u.$id.tsx`             | Functional     | Renders a public user profile with skills, social links, and verified competition badges.                                                                          |
| `/test-certificate` | `src/routes/test-certificate.tsx`  | Developer tool | Opens the client-only certificate laboratory used to test certificate rendering and generation. SSR is disabled for this route.                                    |

## Jury and supporter routes

| URL                   | Route file                          | Status     | What it does                                                                    |
| --------------------- | ----------------------------------- | ---------- | ------------------------------------------------------------------------------- |
| `/jury-evaluation`    | `src/routes/jury-evaluation.tsx`    | Functional | Provides the jury-facing competition evaluation and score submission workspace. |
| `/supporter/check-in` | `src/routes/supporter/check-in.tsx` | Functional | Provides the supporter-facing attendance and event check-in portal.             |

## Student dashboard routes

| URL                                  | Route file                                           | Status     | What it does                                                                                                                                                                                                     |
| ------------------------------------ | ---------------------------------------------------- | ---------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `/dashboard`                         | `src/routes/dashboard.tsx`                           | Layout     | Wraps all student pages in `DashboardLayout` and supplies the student portal metadata and route fallbacks.                                                                                                       |
| `/dashboard/`                        | `src/routes/dashboard/index.tsx`                     | Functional | Renders the student dashboard overview/home page. In normal navigation this is the dashboard landing page.                                                                                                       |
| `/dashboard/competitions`            | `src/routes/dashboard/competitions.tsx`              | Functional | Lists competitions available to the current student account.                                                                                                                                                     |
| `/dashboard/competitions/:id`        | `src/routes/dashboard/competitions_.$id.tsx`         | Guarded    | Loads a specific competition and renders its details and team workspace. Unknown competition IDs produce the route-level not-found page.                                                                         |
| `/dashboard/competitions/:id/submit` | `src/routes/dashboard/competitions_.$id_.submit.tsx` | Guarded    | Opens the project submission workflow for Pitch Deck and GitHub repository delivery. Invalid IDs return not-found; users without submission access are redirected to the competition details page with a notice. |
| `/dashboard/scoreboard`              | `src/routes/dashboard/scoreboard.tsx`                | Functional | Displays the student-facing competition leaderboard.                                                                                                                                                             |
| `/dashboard/certificates`            | `src/routes/dashboard/certificates.tsx`              | Functional | Lists certificates earned by the student and provides certificate-related actions.                                                                                                                               |
| `/dashboard/notifications`           | `src/routes/dashboard/notifications.tsx`             | Functional | Opens the student's notification center.                                                                                                                                                                         |
| `/dashboard/profile`                 | `src/routes/dashboard/profile.tsx`                   | Functional | Allows the student to review and manage personal profile information.                                                                                                                                            |

## Partner portal routes

| URL                                   | Route file                                          | Status      | What it does                                                                                                         |
| ------------------------------------- | --------------------------------------------------- | ----------- | -------------------------------------------------------------------------------------------------------------------- |
| `/partner`                            | `src/routes/partner.tsx`                            | Layout      | Provides a route namespace for partner pages. It only renders an `<Outlet />` and has no standalone screen.          |
| `/partner/dashboard`                  | `src/routes/partner/dashboard.tsx`                  | Layout      | Wraps partner dashboard pages in `PartnerDashboardLayout` and supplies partner portal metadata and route fallbacks.  |
| `/partner/dashboard/`                 | `src/routes/partner/dashboard/index.tsx`            | Functional  | Renders the partner dashboard overview/home page.                                                                    |
| `/partner/dashboard/competitions/new` | `src/routes/partner/dashboard/competitions/new.tsx` | Functional  | Provides the form for creating a new competition.                                                                    |
| `/partner/dashboard/event-management` | `src/routes/partner/dashboard/event-management.tsx` | Functional  | Manages competition configuration and event operations.                                                              |
| `/partner/dashboard/participants`     | `src/routes/partner/dashboard/participants.tsx`     | Functional  | Manages registered participants and participant-related operations.                                                  |
| `/partner/dashboard/check-in`         | `src/routes/partner/dashboard/check-in.tsx`         | Functional  | Manages venue attendance and competition team rosters.                                                               |
| `/partner/dashboard/scoreboard`       | `src/routes/partner/dashboard/scoreboard.tsx`       | Functional  | Displays the partner-facing competition leaderboard.                                                                 |
| `/partner/dashboard/accounts`         | `src/routes/partner/dashboard/accounts.tsx`         | Functional  | Manages partner profiles, subaccounts, and access accounts.                                                          |
| `/partner/dashboard/notifications`    | `src/routes/partner/dashboard/notifications.tsx`    | Functional  | Opens partner notifications and support information.                                                                 |
| `/partner/dashboard/achievements`     | `src/routes/partner/dashboard/achievements.tsx`     | Placeholder | Shows a temporary achievements section; competition results and participant achievement management are planned here. |

## Super Admin routes

| URL                   | Route file                          | Status            | What it does                                                                                                        |
| --------------------- | ----------------------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------- |
| `/admin`              | `src/routes/admin.tsx`              | Redirect + Layout | Redirects the exact `/admin` URL to `/admin/dashboard`. It also wraps all Admin child pages in `AdminLayout`.       |
| `/admin/dashboard`    | `src/routes/admin/dashboard.tsx`    | Functional        | Renders the Super Admin overview and platform-level summary.                                                        |
| `/admin/users`        | `src/routes/admin/users.tsx`        | Functional        | Manages partner companies, subaccounts, contacts, roles, and account statuses.                                      |
| `/admin/teams`        | `src/routes/admin/teams.tsx`        | Functional        | Manages event-scoped teams and projects, finalists, categories, captains, members, repository links, and documents. |
| `/admin/jury`         | `src/routes/admin/jury.tsx`         | Functional        | Manages event-scoped evaluation criteria, weight totals, assigned jury members, progress, and score overrides.      |
| `/admin/check-in`     | `src/routes/admin/check-in.tsx`     | Placeholder       | Shows a temporary Super Admin section for supporter access, venue attendance, and event check-in monitoring.        |
| `/admin/certificates` | `src/routes/admin/certificates.tsx` | Placeholder       | Shows a temporary Super Admin section for certificate templates, issuance batches, verification, and revocation.    |
| `/admin/settings`     | `src/routes/admin/settings.tsx`     | Placeholder       | Shows a temporary Super Admin section for registration windows, policies, limits, and system controls.              |

## Root route and shared behavior

`src/routes/__root.tsx` is not a separate URL. It wraps every route and is responsible for:

- the application HTML shell, global stylesheet, scripts, and Azerbaijani `lang="az"` setting;
- the React Query client and Redux store route context;
- default metadata, favicon, and theme initialization;
- global loading, not-found, and error fallbacks;
- rendering child routes through `<Outlet />`;
- scrolling the page to the top when the pathname or search parameters change.

Most leaf routes also provide their own loading, error, and not-found components. Full-page public/authentication routes use `FullPageRoutePending`; dashboard routes generally use `ContentRoutePending` inside their existing layout.

## Access-control note

The `/dashboard`, `/partner/dashboard`, and `/admin` prefixes describe the intended audience and UI layout. Their route files currently do not contain a general authentication or role guard. The explicit route-level guards that exist today are:

1. `/register/partner` requires an invitation code.
2. Competition detail routes reject unknown competition IDs.
3. Competition submission checks whether the current account may submit and redirects when access is restricted.

If authentication is enforced elsewhere, such as server middleware or API authorization, it should be documented separately from this client route inventory.

## File naming details

- `$id` in a filename creates a dynamic URL segment such as `:id`.
- A trailing underscore changes TanStack route nesting without adding an underscore to the public URL. For example, `register_.partner.tsx` becomes `/register/partner`.
- `index.tsx` is the index page for its parent route.
- `routeTree.gen.ts` is generated from these files; add or change routes in `src/routes` instead of editing the generated file.
