# DevJourney Playwright Testing Report

**Date/Time of Test:** 2026-08-18
**Environment tested:** Production (`https://devjourney-az.onrender.com`)

I have successfully run an end-to-end user test on the production environment using the Playwright MCP server. I navigated the site, registered a brand new user, logged in, and verified the dashboard.

Below are the detailed issues encountered during the test, separated by Frontend and Backend/Data issues.

---

## 🛑 Frontend Issues

### 1. Registration Page - Corrupted University Dropdown Data
**Severity:** Medium
**Location:** `/register` -> `Universitet` dropdown.
**Issue:** When selecting a university during registration, the dropdown contains corrupted or test data alongside the real universities. 
**Observed Values:**
- `string`
- `XP󡡆ìäƒ򲫟ÔaM°񄼯獞”ÿ2`
- `񍰻`
**Recommendation:** The frontend should either filter out invalid data, or the backend database (`UniversityProfiles` table) needs to be cleaned of these corrupted entries.

### 2. Login Page - Broken Demo Accounts
**Severity:** High
**Location:** `/login` -> "Demo hesabları" buttons.
**Issue:** Clicking the demo account button `demo.active@devjourney.az` auto-fills the email and sets the password to `Demo1234`. However, clicking "Giriş et" returns an **"Invalid email or password"** error. 
**Cause:** The frontend has hardcoded demo buttons, but those specific demo users either do not exist in the production database, or their passwords do not match `Demo1234` (the backend seeder typically uses `Password123!`).
**Recommendation:** Ensure the backend seeder explicitly creates these exact demo accounts with the `Demo1234` password, or remove the buttons from the production build.

---

## 🛑 Backend & Deployment Issues

### 3. Student 35 Missing Certificates and Competitions (Your Reported Bug)
**Severity:** High
**Location:** `/dashboard` and `/dashboard/profile`
**Issue:** You mentioned that existing students (like Student 35) still see "0 certificates" and "0 active contests".
**Cause:** 
The reason this is happening on the live site is that **the backend code we just wrote has not been deployed to Render.** 
Locally, we completely updated `StudentProfileDto.cs`, completely rewrote `GetStudentProfileQueryHandler.cs` to fetch this data, and overhauled `MockDataSeeder.cs` to attach certificates to every user. However, `devjourney-az.onrender.com` is still running the old version of the backend code where `GetStudentProfileQueryHandler` simply drops certificates and competitions and returns empty values. 
**Recommendation:** You must push the latest local commits to your GitHub repository and trigger a new deployment on Render for the backend. Once the Render server restarts with the new code, the `MockDataSeeder` will run automatically, and the API will successfully return the data to the frontend!

---

### ✅ What Worked perfectly:
* The website loads and the landing page UI looks great.
* The complete registration flow works perfectly for normal users (I successfully registered as `johndoe777`).
* Redirection from Registration -> Login works.
* The Dashboard UI successfully loads and dynamically shows the correct 0 metrics for a brand new user.
