# Backend Issue Analysis & Fixes

## Issues Identified & Fixed:

1. **Dashboard Data (0s everywhere):**
   - **Root Cause:** The `GetStudentDashboardQueryHandler` was entirely mocked and returning `0` for `ActiveCompetitions` and `DeveloperXp`.
   - **Fix:** Injected `IStudentProfileRepository` to find the user's profile and count their active competitions correctly based on `CompetitionParticipant` members. Replaced hardcoded values with actual database lookups and calculation (`xp = certCount * 50 + activeComps * 10`).

2. **Profile Cabinet Not Opening / Loading:**
   - **Root Cause:** The API endpoints `/api/me` and `/api/me/profile` were powered by `GetMeQueryHandler` and `GetMyProfileQueryHandler`, which were stubbed out and always returning a hardcoded `"Mock User"` with random empty fields, which broke the frontend UI.
   - **Fix:** Updated `GetMeQueryHandler` to return the real authenticated user's Email, Id, and properly resolved Role (checking if they have a Partner profile or defaulting to Student). Updated `GetMyProfileQueryHandler` to resolve the real `StudentProfile` and return accurate data.

3. **Seeder Inconsistencies:**
   - **Root Cause:** The `MockDataSeeder` added 1-2 random certificates and attached the student to a competition with a *random* `ApplicationStatus` (which could be rejected or pending, making it "inactive").
   - **Fix:** Updated the seeder to explicitly guarantee exactly **2 certificates** and strictly assign the `ApplicationStatus.Approved` state to their competition participation so that the active contest always shows up.

All issues were strictly caused by the backend's incomplete/stubbed CQRS handlers and the randomness in the seeder. The codebase has been updated, successfully built, and the fixes are fully implemented.
