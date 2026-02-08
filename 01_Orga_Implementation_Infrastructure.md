# 🏗️ Phase 1: Infrastructure & Data Access

## 🎯 Objective
Establish the database schema, entity models, and the repository layer required for the Community Organization system.

## 📝 Step 1: Define Domain Entities
**File:** `Tribe.Bib/TribeRelated/CreatorOrga.cs`

Update the content to include all defined enums and classes:
1.  **Enums:** `MemberStatus`, `GroupType`, `GroupRole`, `GoalType`, `GoalStatus`, `CraftCategory`, `RequestStatus`, `RequestPriority`, `EventType`, `EventStatus`, `RecurrencePattern`, `ParticipantStatus`, `AnnouncementType`, `ActivityType`.
2.  **Entities:**
    *   `Community`
    *   `CommunityMember`
    *   `Rank`
    *   `Group`
    *   `GroupMember`
    *   `GroupGoal`
    *   `MemberCraft`
    *   `CraftDefinition`
    *   `CraftRequest`
    *   `CommunityEvent`
    *   `EventParticipant`
    *   `Announcement`
    *   `ActivityLog`
for details CreatorOrgaImplementierung.md

> **Note:** Ensure all Models have proper `[Key]`, `[ForeignKey]`, and `[MaxLength]` attributes as specified in the main documentation.


## 🗄️ Step 2: Database Context
**File:** `Tribe.Data/Context/CommunityDbContext.cs` 

1.  Create `CommunityDbContext`(new DBcontext)
2.  Add `DbSet<T>` for all entities listed above.
3.  Implement `OnModelCreating` to configure indexes and composite keys (e.g., `GroupMember` unique constraints).
4.  **Migration:**
    *   Run `Add-Migration AddCreatorOrga`
    *   Run `Update-Database`

## ⚙️ Step 3: Repository Pattern
**Location:** `Tribe.Services/Repositories`

### 3.1 Interfaces
Create `IRepository<T>` (Base) and specific interfaces:
*   `ICommunityRepository`
*   `ICommunityMemberRepository`
*   `IRankRepository`
*   `IGroupRepository`
*   `IGroupMemberRepository`
*   `IGroupGoalRepository`
*   `IMemberCraftRepository`
*   `ICraftDefinitionRepository`
*   `ICraftRequestRepository`
*   `IEventRepository`
*   `IEventParticipantRepository`
*   `IAnnouncementRepository`
*   `IActivityLogRepository`

### 3.2 Implementations
Create `Repository<T>` and specific implementations relying on `CommunityDbContext`:
*   `CommunityRepository` implementation
*   ... and all others listed above.

## 🔌 Step 4: Dependency Injection
**File:** `Program.cs`

Register the repositories:
```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
builder.Services.AddScoped<IRankRepository, RankRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
builder.Services.AddScoped<IGroupGoalRepository, GroupGoalRepository>();
builder.Services.AddScoped<IMemberCraftRepository, MemberCraftRepository>();
builder.Services.AddScoped<ICraftDefinitionRepository, CraftDefinitionRepository>();
builder.Services.AddScoped<ICraftRequestRepository, CraftRequestRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
```
