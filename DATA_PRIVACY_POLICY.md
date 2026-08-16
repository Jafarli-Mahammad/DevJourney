# Data Privacy Policy & Documentation

## Overview
This document outlines the personal data collected by the DevJourney API, how it is processed, protected, and how long it is retained.

## Data Collected
- **Account Information**: User names, email addresses, password hashes (Argon2/Identity default).
- **Profile Data**: First names, last names, phone numbers, bios, GitHub links, LinkedIn links, and CV/Resume files.
- **Educational/Professional Data**: University affiliations, professions, specific roles, skills, and languages.
- **System Telemetry**: IP addresses (temporarily for rate limiting), trace correlations.

## Data Exposure & DTOs
We enforce strict DTO projections to prevent data leakage:
- **Public Profiles**: Internal application IDs (`ApplicationUserId`), emails, and phone numbers are excluded from public responses (e.g., `GetAllStudentProfiles` and `GetPublicProfile`).
- Only explicitly whitelisted fields are exposed over REST. 

## Data Retention & Deletion
- **Soft Delete Strategy**: Entities implement `IAuditableEntity` and `DeletedAt`. When a user or entity is deleted, it is removed from query views via EF Core Global Query Filters (`IsDeleted == null`).
- **Data Purging**: A background worker (`DataRetentionWorker`) runs every 24 hours to automatically purge records that have been soft-deleted for more than 30 days.

## SQL Injection Protection
All database queries interact via Entity Framework Core's parameterized `IQueryable` mechanisms or safely mapped Dapper queries (`CommandDefinition` parameters). We strictly forbid dynamic SQL string concatenations.
