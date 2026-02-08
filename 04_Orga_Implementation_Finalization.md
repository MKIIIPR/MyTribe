# 🏁 Phase 4: API, Security & Finalization

## 🎯 Objective
Expose the logic via Controllers (if applicable), secure the application, enforce permissions, and perform final integration testing.

## 🛡️ Step 1: Security & Authorization
**Location:** `Tribe.Server` (or Shared Services)

1.  **Authorization Handlers:**
    *   Create `CommunityPermissionRequirement`.
    *   Implement `CommunityPermissionHandler` to check `CommunityMember` permissions (e.g., `CanManageGroups`, `IsAdmin`).
    *   Register policies in `Program.cs`.

2.  **Controller Security:**
    *   Apply `[Authorize]` to all Controllers.
    *   Apply `[Authorize(Policy = "CanManageGroups")]` to sensitive endpoints.

## 🌐 Step 2: API Controllers
**Location:** `Tribe.Server/Controllers/Community`

*If you are using Hosted Blazor WebAssembly, you need these endpoints. If Server-Side Blazor, Services are injected directly (skip to Step 3).*

Create Controllers mapping to Services:
*   `CommunitiesController` (CRUD, Join/Leave)
*   `GroupsController` (CRUD, Members)
*   `EventsController` (CRUD, Signups)
*   `CraftsController` (Definitions, Requests)

**Important:** Ensure all write operations call the Service layer, which triggers the SignalR broadcasts. Do not broadcast from Controllers directly.

## 🧪 Step 3: Testing & Validation

1.  **Unit Tests:**
    *   Test `CommunityService` logic (especially SignalR invocation).
    *   Test Permission checks (Rank logic).

2.  **Integration Tests:**
    *   Test the database flow (Entity Framework).
    *   Test SignalR connectivity.

3.  **User Acceptance Testing (UAT):**
    *   **Scenario 1:** User A creates a Group. User B (in same community) sees it appear instantly.
    *   **Scenario 2:** User A posts a Craft Request. User B sees the notification snackbar.
    *   **Scenario 3:** Permissions - a Member without `CanKick` tries to kick someone (UI should hide button, API should return 403).

## 🚀 Step 4: Final Polish

1.  **Validation:**
    *   Add `FluentValidation` for all Request DTOs (e.g., Name length, max members).
2.  **Error Handling:**
    *   Global Exception Handler for friendly error messages.
    *   SignalR connection loss handling (Retry policies in UI).
3.  **UI Polish:**
    *   Loading skeletons (`MudSkeleton`) while fetching data.
    *   Empty states (e.g., "No events scheduled").

## 📋 Final Deployment Checklist
- [ ] Database Migrations applied (`Update-Database`)?
- [ ] SignalR Hub mapped in `Program.cs`?
- [ ] Services registered in DI?
- [ ] Authorization policies defined?
- [ ] Client configured with correct API Base URL?
