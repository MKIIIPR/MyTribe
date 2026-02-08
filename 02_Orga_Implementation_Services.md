# 🧠 Phase 2: Service Layer & Business Logic

## 🎯 Objective
Implement the business logic, Data Transfer Objects (DTOs), and Real-Time Communication (SignalR) infrastructure.

## 📦 Step 1: Data Transfer Objects (DTOs)
**Location:** `Tribe.Bib/DTOs` (or `Tribe.Shared/DTOs` if separated)

Create the following classes to decouple the API/UI from Domain Entities:

1.  **Request DTOs:**
    *   `CreateCommunityRequest`
    *   `CreateGroupRequest`
    *   `CreateRankRequest`
    *   `RankPermissions` (Helper class)
    *   `CreateCraftDefinitionRequest`
    *   `AddMemberCraftRequest`
    *   `CreateCraftRequestRequest`
    *   `CreateGoalRequest`
    *   `CreateEventRequest`

2.  **Response DTOs:**
    *   `CommunityDetailDto`
    *   `MemberDto`
    *   `RankDto`
    *   `GroupDto`
    *   `GroupDetailDto`
    *   `GroupGoalDto`
    *   `CraftSummaryDto`
    *   `CraftRequestDto`
    *   `EventDto`
    *   `EventParticipantDto` (implied by Event logic)
    *   `AnnouncementDto`

> **Reference:** See Section 5 "DTOs" in `CreatorOrgaImplementierung.md` for property definitions.

## 📡 Step 2: SignalR Infrastructure
**Location:** `Tribe.Server/Hubs` (Server Project)

1.  **Hub Implementation:**
    *   Create `CommunityHub : Hub`.
    *   Implement Connection Management (`JoinCommunityChannel`, `LeaveCommunityChannel`, etc.).
    *   Define Client methods (`NotifyMemberJoined`, `NotifyGroupCreated`, etc.).

2.  **SignalR Service Wrapper:**
    *   **Interface:** `ICommunitySignalRService` (in `Tribe.Services/Interfaces`).
    *   **Implementation:** `CommunitySignalRService` (in `Tribe.Services/SignalR`).
    *   **Logic:** Inject `IHubContext<CommunityHub>` and implement broadcasting methods to decouple Services from the Hub directly.

3.  **Registration:**
    *   In `Program.cs`: `builder.Services.AddSignalR();`
    *   Map Hub: `app.MapHub<CommunityHub>("/hubs/community");`

## 🧠 Step 3: Service Interfaces
**Location:** `Tribe.Services/Interfaces`

Define the contracts for Business Logic:
*   `ICommunityService`
*   `IRankService`
*   `IGroupService`
*   `IGroupGoalService`
*   `ICraftService`
*   `IEventService`
*   `IAnnouncementService`

> **Reference:** See Section 4 "SERVICE LAYER" in `CreatorOrgaImplementierung.md`.

## ⚙️ Step 4: Service Implementations
**Location:** `Tribe.Services/Implementations`

Implement the logic using Repositories (Phase 1) and SignalR Service (Step 2).

1.  **CommunityService:**
    *   Handle Creation (Link to Creator).
    *   Handle Joining (Add Member, Broadcast via SignalR).
    *   Permissions checks.
2.  **RankService:**
    *   Manage Ranks and Permissions.
3.  **GroupService:**
    *   Group CRUD and Member management.
4.  **CraftService:**
    *   Combine Definitions, MemberCrafts, and Requests logic.
5.  **EventService:**
    *   Event lifecycle and Participant management.
6.  **AnnouncementService:**
    *   Broadcasting announcements.

**Key Requirement:** Every state-changing method (Create, Update, Join, etc.) must:
1.  Persist data via Repository.
2.  Log activity via `IActivityLogRepository`.
3.  Broadcast updates via `ICommunitySignalRService`.

## 🔌 Step 5: Dependency Injection
**File:** `Program.cs`

Register the services:
```csharp
// SignalR Service
builder.Services.AddScoped<ICommunitySignalRService, CommunitySignalRService>();

// Business Services
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupGoalService, GroupGoalService>();
builder.Services.AddScoped<ICraftService, CraftService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
```
