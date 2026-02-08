# Community/Gilden-Organisationssystem - C# Blazor Implementation

## ⚠️ INTEGRATION MIT BESTEHENDER TRIBE-STRUKTUR

**WICHTIG**: Dieses System integriert sich in die bestehende Tribe-Architektur:

### User-Referenzen:
- ✅ **TribeUser.Id** (string) = Hauptkey für alle User-Verlinkungen
- ✅ **CreatorProfile.Id** (string) = Ersteller von Communities
- ✅ **CommunityMember** verbindet TribeUser mit Community
- ✅ **ALLE IDs sind string-basiert** (wie in deiner Struktur)

### Naming Conventions:
- ✅ Folgt deinem Pattern: `Id`, nicht `Guid` oder `EntityId`  
- ✅ `[Key]` mit `public string Id { get; set; } = Guid.NewGuid().ToString();`
- ✅ `[ForeignKey]` Attributes für alle Relationen
- ✅ MaxLength Constraints wie in deinen Shop/Raffle-Models

### DbContext Integration:
```csharp
// In einer neuen context CreatorOrgaContextDb:
public DbSet<Community> Communities { get; set; }
public DbSet<CommunityMember> CommunityMembers { get; set; }
public DbSet<Rank> Ranks { get; set; }
public DbSet<Group> Groups { get; set; }
// ... weitere DbSets
```
### Entity implementierung in "/tribeRelated/CreatorOrga.cs
---

## Projektübersicht
Ein umfassendes Community-Management-System für Spiele mit Gilden-/Clan-Funktionalität, Rangverwaltung, Gruppenorganisation und Craft-Tracking.

## Architektur-Prinzipien
- **KEINE virtuellen Verweise/Navigation Properties** - Verwende stattdessen IDs und explizite Queries
- Entity Framework Core mit expliziten Beziehungen über IDs
- Clean Architecture Pattern (Domain, Application, Infrastructure, Presentation)
- Repository Pattern für Datenzugriff
- CQRS-Light für komplexe Operationen

---

## 1. DOMAIN LAYER - Entitäten

### 1.1 Community (Haupt-Container)
```csharp
public class Community
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string GameName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string GameType { get; set; } = string.Empty; // z.B. "MMO", "FPS", "Strategy"
    
    // === CREATOR CONNECTION ===
    [Required]
    [ForeignKey(nameof(CreatorProfile))]
    public string CreatorProfileId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(500)]
    public string? IconUrl { get; set; }
    
    [MaxLength(500)]
    public string? BannerUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
    public int MaxMembers { get; set; } = 0; // 0 = unlimitiert
    
    [MaxLength(500)]
    public string? DiscordWebhook { get; set; }
    
    [MaxLength(500)]
    public string? Website { get; set; }
    
    // Settings
    public bool AllowMemberInvites { get; set; } = true;
    public bool RequireApproval { get; set; } = false;
    public int MinimumRankToInvite { get; set; } = 0;
    
    // Stats
    public int MemberCount { get; set; } = 0;
    public int GroupCount { get; set; } = 0;
    public int EventCount { get; set; } = 0;
}
```

### 1.2 CommunityMember (User-Community Beziehung)
```csharp
public class CommunityMember
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(TribeUser))]
    public string TribeUserId { get; set; } = string.Empty;
    
    [ForeignKey(nameof(Rank))]
    public string? RankId { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string InGameName { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Notes { get; set; } // Notizen über das Mitglied
    
    public MemberStatus Status { get; set; } = MemberStatus.Active;
    public DateTime? BannedUntil { get; set; }
    
    [MaxLength(500)]
    public string? BanReason { get; set; }
    
    public bool IsFounder { get; set; } = false;
}

public enum MemberStatus
{
    Pending = 0,
    Active = 1,
    Inactive = 2,
    OnLeave = 3,
    Banned = 4
}
```

### 1.3 Rank (Rang-System)
```csharp
public class Rank
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(7)]
    public string ColorHex { get; set; } = "#1976d2";
    
    [MaxLength(500)]
    public string? IconUrl { get; set; }
    
    public int Priority { get; set; } = 0; // Höher = wichtiger, 0 = niedrigster Rang
    public bool IsDefault { get; set; } = false; // Standard-Rang für neue Mitglieder
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Permissions
    public bool CanManageMembers { get; set; } = false;
    public bool CanManageRanks { get; set; } = false;
    public bool CanManageGroups { get; set; } = false;
    public bool CanCreateGroups { get; set; } = false;
    public bool CanInviteMembers { get; set; } = false;
    public bool CanKickMembers { get; set; } = false;
    public bool CanBanMembers { get; set; } = false;
    public bool CanEditCommunity { get; set; } = false;
    public bool CanManageEvents { get; set; } = false;
    public bool CanViewPrivateGroups { get; set; } = false;
    public bool CanManageCrafts { get; set; } = false;
}
```

### 1.4 Group (Gruppen wie Raids, Companies, Büros)
```csharp
public class Group
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public GroupType Type { get; set; } = GroupType.General;
    
    [Required]
    [ForeignKey(nameof(Leader))]
    public string LeaderId { get; set; } = string.Empty; // CommunityMemberId
    
    [ForeignKey(nameof(DeputyLeader))]
    public string? DeputyLeaderId { get; set; } // CommunityMemberId (Vertreter)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int MaxMembers { get; set; } = 0; // 0 = unlimitiert
    public bool IsPrivate { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    [MaxLength(500)]
    public string? IconUrl { get; set; }
    
    [MaxLength(7)]
    public string ColorHex { get; set; } = "#1976d2";
    
    // Raid/Event spezifisch
    public DateTime? ScheduledAt { get; set; }
    public int? RequiredItemLevel { get; set; }
    
    [MaxLength(1000)]
    public string? Requirements { get; set; }
    
    // Stats
    public int MemberCount { get; set; } = 0;
    public int GoalCount { get; set; } = 0;
}

public enum GroupType
{
    General = 0,
    Raid = 1,
    PvP = 2,
    Company = 3,
    Office = 4,
    Department = 5,
    Team = 6,
    Squad = 7,
    Party = 8
}
```

### 1.5 GroupMember (User-Group Beziehung)
```csharp
public class GroupMember
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Group))]
    public string GroupId { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string CommunityMemberId { get; set; } = string.Empty;
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    
    public GroupRole Role { get; set; } = GroupRole.Member;
    
    [MaxLength(100)]
    public string? CustomRole { get; set; } // z.B. "Tank", "Healer", "DPS"
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public enum GroupRole
{
    Member = 0,
    Officer = 1,
    Leader = 2,
    Deputy = 3
}
```

### 1.6 GroupGoal (Gruppenziele für Raids/Events)
```csharp
public class GroupGoal
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Group))]
    public string GroupId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public GoalType Type { get; set; } = GoalType.Custom;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string CreatedByMemberId { get; set; } = string.Empty;
    
    public DateTime? TargetDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
    public int Priority { get; set; } = 1; // 1-5, 5 = höchste Priorität
    
    public int? TargetQuantity { get; set; } // z.B. "10x Boss töten"
    public int CurrentProgress { get; set; } = 0;
    
    [MaxLength(500)]
    public string? Rewards { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public enum GoalType
{
    RaidClear = 0,
    BossKill = 1,
    ItemFarm = 2,
    Achievement = 3,
    LevelUp = 4,
    Territory = 5,
    PvPObjective = 6,
    Custom = 99
}

public enum GoalStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    OnHold = 5
}
```

### 1.7 MemberCraft (Was kann ein User herstellen/machen)
```csharp
public class MemberCraft
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string CommunityMemberId { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(CraftDefinition))]
    public string CraftDefinitionId { get; set; } = string.Empty;
    
    public int Level { get; set; } = 1;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(1000)]
    public string? Notes { get; set; } // z.B. "Kann legendary items craften"
    
    public bool IsFeatured { get; set; } = false; // Auf Profil hervorheben
    public bool IsAvailable { get; set; } = true; // Aktuell verfügbar für Aufträge
}
```

### 1.8 CraftDefinition (Craft-Katalog für Community)
```csharp
public class CraftDefinition
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public CraftCategory Category { get; set; } = CraftCategory.Custom;
    
    [MaxLength(500)]
    public string? IconUrl { get; set; }
    
    [MaxLength(7)]
    public string ColorHex { get; set; } = "#1976d2";
    
    public int MaxLevel { get; set; } = 100; // z.B. 100 für Level-basierte Crafts
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string CreatedByMemberId { get; set; } = string.Empty;
}

public enum CraftCategory
{
    Crafting = 0,
    Gathering = 1,
    Combat = 2,
    Trading = 3,
    Leadership = 4,
    Support = 5,
    Engineering = 6,
    Magic = 7,
    Profession = 8,
    Custom = 99
}
```

### 1.9 CraftRequest (Anfragen: "Wer kann X herstellen?")
```csharp
public class CraftRequest
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(RequestedBy))]
    public string RequestedByMemberId { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(CraftDefinition))]
    public string CraftDefinitionId { get; set; } = string.Empty;
    
    public int? RequiredLevel { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Materials { get; set; } // Was der Requester bereitstellt
    
    [MaxLength(500)]
    public string? Reward { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FulfilledAt { get; set; }
    
    [ForeignKey(nameof(FulfilledBy))]
    public string? FulfilledByMemberId { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Open;
    public RequestPriority Priority { get; set; } = RequestPriority.Normal;
}

public enum RequestStatus
{
    Open = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Expired = 4
}

public enum RequestPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
```

### 1.10 Event (Community-Events)
```csharp
public class CommunityEvent
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [ForeignKey(nameof(Group))]
    public string? GroupId { get; set; } // Optional: Event für spezifische Gruppe
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public EventType Type { get; set; } = EventType.Other;
    
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string OrganizerMemberId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int MaxParticipants { get; set; } = 0; // 0 = unlimitiert
    public int MinParticipants { get; set; } = 0;
    
    [MaxLength(200)]
    public string? Location { get; set; } // In-game Location
    
    [MaxLength(1000)]
    public string? Requirements { get; set; }
    
    [MaxLength(500)]
    public string? VoiceChannel { get; set; } // Discord/TeamSpeak Link
    
    public EventStatus Status { get; set; } = EventStatus.Scheduled;
    public bool IsRecurring { get; set; } = false;
    public RecurrencePattern RecurrencePattern { get; set; } = RecurrencePattern.None;
    
    // Stats
    public int ParticipantCount { get; set; } = 0;
}

public enum EventType
{
    Raid = 0,
    PvP = 1,
    Social = 2,
    Training = 3,
    Meeting = 4,
    Tournament = 5,
    Farm = 6,
    Other = 99
}

public enum EventStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Postponed = 4
}

public enum RecurrencePattern
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    BiWeekly = 3,
    Monthly = 4
}
```

### 1.11 EventParticipant
```csharp
public class EventParticipant
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(CommunityEvent))]
    public string EventId { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string CommunityMemberId { get; set; } = string.Empty;
    
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Interested;
    public DateTime SignedUpAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    
    [MaxLength(100)]
    public string? Role { get; set; } // z.B. "Tank", "DPS"
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public bool IsBackup { get; set; } = false;
}

public enum ParticipantStatus
{
    Interested = 0,
    Confirmed = 1,
    Declined = 2,
    Maybe = 3,
    NoShow = 4,
    Attended = 5
}
```

### 1.12 Announcement (Ankündigungen)
```csharp
public class Announcement
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [ForeignKey(nameof(Group))]
    public string? GroupId { get; set; } // Null = Community-weit
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
    
    public AnnouncementType Type { get; set; } = AnnouncementType.General;
    
    [Required]
    [ForeignKey(nameof(CommunityMember))]
    public string AuthorMemberId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsPinned { get; set; } = false;
    public bool IsImportant { get; set; } = false;
    
    public DateTime? ExpiresAt { get; set; }
    
    [ForeignKey(nameof(Rank))]
    public string? MinimumRankId { get; set; } // Nur für bestimmte Ränge sichtbar
}

public enum AnnouncementType
{
    General = 0,
    Update = 1,
    Event = 2,
    Alert = 3,
    Achievement = 4,
    Welcome = 5
}
```

### 1.13 ActivityLog (Audit Trail)
```csharp
public class ActivityLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Community))]
    public string CommunityId { get; set; } = string.Empty;
    
    [ForeignKey(nameof(Actor))]
    public string? ActorMemberId { get; set; } // Wer hat die Aktion durchgeführt
    
    [ForeignKey(nameof(Target))]
    public string? TargetMemberId { get; set; } // Bei wem wurde es durchgeführt
    
    public ActivityType ActivityType { get; set; }
    
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty; // z.B. "Group", "Rank", "Member"
    
    public string? EntityId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Details { get; set; } // JSON mit zusätzlichen Infos
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
}

public enum ActivityType
{
    MemberJoined = 0,
    MemberLeft = 1,
    MemberKicked = 2,
    MemberBanned = 3,
    MemberPromoted = 4,
    MemberDemoted = 5,
    
    GroupCreated = 10,
    GroupDeleted = 11,
    GroupUpdated = 12,
    MemberJoinedGroup = 13,
    MemberLeftGroup = 14,
    
    RankCreated = 20,
    RankDeleted = 21,
    RankUpdated = 22,
    
    EventCreated = 30,
    EventCancelled = 31,
    EventCompleted = 32,
    
    GoalCreated = 40,
    GoalCompleted = 41,
    
    CraftAdded = 50,
    CraftUpdated = 51,
    
    AnnouncementPosted = 60,
    
    SettingsChanged = 70
}
```csharp
public class MemberCraft
{
    public Guid MemberCraftId { get; set; }
    public Guid CommunityMemberId { get; set; }
    public Guid CraftDefinitionId { get; set; }
    
    public int Level { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public string Notes { get; set; } // z.B. "Kann legendary items craften"
    public bool IsFeatured { get; set; } // Auf Profil hervorheben
    public bool IsAvailable { get; set; } // Aktuell verfügbar für Aufträge
}
```

### 1.8 CraftDefinition (Craft-Katalog für Community)
```csharp
public class CraftDefinition
{
    public Guid CraftDefinitionId { get; set; }
    public Guid CommunityId { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public CraftCategory Category { get; set; }
    
    public string IconUrl { get; set; }
    public string ColorHex { get; set; }
    
    public int MaxLevel { get; set; } // z.B. 100 für Level-basierte Crafts
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByMemberId { get; set; }
}

public enum CraftCategory
{
    Crafting = 0,
    Gathering = 1,
    Combat = 2,
    Trading = 3,
    Leadership = 4,
    Support = 5,
    Engineering = 6,
    Magic = 7,
    Profession = 8,
    Custom = 99
}
```

### 1.9 CraftRequest (Anfragen: "Wer kann X herstellen?")
```csharp
public class CraftRequest
{
    public Guid CraftRequestId { get; set; }
    public Guid CommunityId { get; set; }
    public Guid RequestedByMemberId { get; set; }
    
    public Guid CraftDefinitionId { get; set; }
    public int? RequiredLevel { get; set; }
    
    public string Description { get; set; }
    public string Materials { get; set; } // Was der Requester bereitstellt
    public string Reward { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }
    public Guid? FulfilledByMemberId { get; set; }
    
    public RequestStatus Status { get; set; }
    public RequestPriority Priority { get; set; }
}

public enum RequestStatus
{
    Open = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Expired = 4
}

public enum RequestPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
```

### 1.10 Event (Community-Events)
```csharp
public class CommunityEvent
{
    public Guid EventId { get; set; }
    public Guid CommunityId { get; set; }
    public Guid? GroupId { get; set; } // Optional: Event für spezifische Gruppe
    
    public string Title { get; set; }
    public string Description { get; set; }
    public EventType Type { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
    
    public Guid OrganizerMemberId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    
    public string Location { get; set; } // In-game Location
    public string Requirements { get; set; }
    public string VoiceChannel { get; set; } // Discord/TeamSpeak Link
    
    public EventStatus Status { get; set; }
    public bool IsRecurring { get; set; }
    public RecurrencePattern RecurrencePattern { get; set; }
}

public enum EventType
{
    Raid = 0,
    PvP = 1,
    Social = 2,
    Training = 3,
    Meeting = 4,
    Tournament = 5,
    Farm = 6,
    Other = 99
}

public enum EventStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Postponed = 4
}

public enum RecurrencePattern
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    BiWeekly = 3,
    Monthly = 4
}
```

### 1.11 EventParticipant
```csharp
public class EventParticipant
{
    public Guid EventParticipantId { get; set; }
    public Guid EventId { get; set; }
    public Guid CommunityMemberId { get; set; }
    
    public ParticipantStatus Status { get; set; }
    public DateTime SignedUpAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    
    public string Role { get; set; } // z.B. "Tank", "DPS"
    public string Notes { get; set; }
    public bool IsBackup { get; set; }
}

public enum ParticipantStatus
{
    Interested = 0,
    Confirmed = 1,
    Declined = 2,
    Maybe = 3,
    NoShow = 4,
    Attended = 5
}
```

### 1.12 Announcement (Ankündigungen)
```csharp
public class Announcement
{
    public Guid AnnouncementId { get; set; }
    public Guid CommunityId { get; set; }
    public Guid? GroupId { get; set; } // Null = Community-weit
    
    public string Title { get; set; }
    public string Content { get; set; }
    public AnnouncementType Type { get; set; }
    
    public Guid AuthorMemberId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsPinned { get; set; }
    public bool IsImportant { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    public Guid? MinimumRankId { get; set; } // Nur für bestimmte Ränge sichtbar
}

public enum AnnouncementType
{
    General = 0,
    Update = 1,
    Event = 2,
    Alert = 3,
    Achievement = 4,
    Welcome = 5
}
```

### 1.13 ActivityLog (Audit Trail)
```csharp
public class ActivityLog
{
    public Guid ActivityLogId { get; set; }
    public Guid CommunityId { get; set; }
    
    public Guid? ActorMemberId { get; set; } // Wer hat die Aktion durchgeführt
    public Guid? TargetMemberId { get; set; } // Bei wem wurde es durchgeführt
    
    public ActivityType ActivityType { get; set; }
    public string EntityType { get; set; } // z.B. "Group", "Rank", "Member"
    public Guid? EntityId { get; set; }
    
    public string Description { get; set; }
    public string Details { get; set; } // JSON mit zusätzlichen Infos
    
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
}

public enum ActivityType
{
    MemberJoined = 0,
    MemberLeft = 1,
    MemberKicked = 2,
    MemberBanned = 3,
    MemberPromoted = 4,
    MemberDemoted = 5,
    
    GroupCreated = 10,
    GroupDeleted = 11,
    GroupUpdated = 12,
    MemberJoinedGroup = 13,
    MemberLeftGroup = 14,
    
    RankCreated = 20,
    RankDeleted = 21,
    RankUpdated = 22,
    
    EventCreated = 30,
    EventCancelled = 31,
    EventCompleted = 32,
    
    GoalCreated = 40,
    GoalCompleted = 41,
    
    CraftAdded = 50,
    CraftUpdated = 51,
    
    AnnouncementPosted = 60,
    
    SettingsChanged = 70
}
```

---

## 2. DATABASE CONTEXT (DbContext)

```csharp
public class CommunityDbContext : DbContext
{
    public CommunityDbContext(DbContextOptions<CommunityDbContext> options) 
        : base(options) { }
    
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityMember> CommunityMembers { get; set; }
    public DbSet<Rank> Ranks { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupGoal> GroupGoals { get; set; }
    public DbSet<MemberCraft> MemberCrafts { get; set; }
    public DbSet<CraftDefinition> CraftDefinitions { get; set; }
    public DbSet<CraftRequest> CraftRequests { get; set; }
    public DbSet<CommunityEvent> Events { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Communities
        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.GameName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CreatorProfileId);
            entity.HasIndex(e => new { e.GameName, e.IsActive });
        });
        
        // CommunityMembers
        modelBuilder.Entity<CommunityMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.TribeUserId }).IsUnique();
            entity.HasIndex(e => e.TribeUserId);
            entity.HasIndex(e => new { e.CommunityId, e.Status });
        });
        
        // Ranks
        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.Priority });
            entity.HasIndex(e => new { e.CommunityId, e.IsDefault });
        });
        
        // Groups
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CommunityId);
            entity.HasIndex(e => e.LeaderId);
            entity.HasIndex(e => new { e.CommunityId, e.Type, e.IsActive });
        });
        
        // GroupMembers
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GroupId, e.CommunityMemberId }).IsUnique();
            entity.HasIndex(e => e.CommunityMemberId);
        });
        
        // GroupGoals
        modelBuilder.Entity<GroupGoal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GroupId, e.Status });
            entity.HasIndex(e => e.TargetDate);
        });
        
        // MemberCrafts
        modelBuilder.Entity<MemberCraft>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityMemberId, e.CraftDefinitionId }).IsUnique();
            entity.HasIndex(e => e.CraftDefinitionId);
            entity.HasIndex(e => new { e.CommunityMemberId, e.IsAvailable });
        });
        
        // CraftDefinitions
        modelBuilder.Entity<CraftDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.Category });
            entity.HasIndex(e => new { e.CommunityId, e.IsActive });
        });
        
        // CraftRequests
        modelBuilder.Entity<CraftRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.Status });
            entity.HasIndex(e => e.CraftDefinitionId);
            entity.HasIndex(e => e.RequestedByMemberId);
        });
        
        // Events
        modelBuilder.Entity<CommunityEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.StartTime });
            entity.HasIndex(e => new { e.CommunityId, e.Status });
            entity.HasIndex(e => e.GroupId);
        });
        
        // EventParticipants
        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.EventId, e.CommunityMemberId }).IsUnique();
            entity.HasIndex(e => new { e.EventId, e.Status });
        });
        
        // Announcements
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.CreatedAt });
            entity.HasIndex(e => new { e.GroupId, e.CreatedAt });
            entity.HasIndex(e => new { e.CommunityId, e.IsPinned });
        });
        
        // ActivityLogs
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommunityId, e.Timestamp });
            entity.HasIndex(e => new { e.ActorMemberId, e.Timestamp });
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
        });
    }
}
```

```csharp
public class CommunityDbContext : DbContext
{
    public CommunityDbContext(DbContextOptions<CommunityDbContext> options) 
        : base(options) { }
    
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityMember> CommunityMembers { get; set; }
    public DbSet<Rank> Ranks { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupGoal> GroupGoals { get; set; }
    public DbSet<MemberCraft> MemberCrafts { get; set; }
    public DbSet<CraftDefinition> CraftDefinitions { get; set; }
    public DbSet<CraftRequest> CraftRequests { get; set; }
    public DbSet<CommunityEvent> Events { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Communities
        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasKey(e => e.CommunityId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.GameName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CreatorUserId);
            entity.HasIndex(e => new { e.GameName, e.IsActive });
        });
        
        // CommunityMembers
        modelBuilder.Entity<CommunityMember>(entity =>
        {
            entity.HasKey(e => e.CommunityMemberId);
            entity.HasIndex(e => new { e.CommunityId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.CommunityId, e.Status });
        });
        
        // Ranks
        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.RankId);
            entity.HasIndex(e => new { e.CommunityId, e.Priority });
            entity.HasIndex(e => new { e.CommunityId, e.IsDefault });
        });
        
        // Groups
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId);
            entity.HasIndex(e => e.CommunityId);
            entity.HasIndex(e => e.LeaderId);
            entity.HasIndex(e => new { e.CommunityId, e.Type, e.IsActive });
        });
        
        // GroupMembers
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.GroupMemberId);
            entity.HasIndex(e => new { e.GroupId, e.CommunityMemberId }).IsUnique();
            entity.HasIndex(e => e.CommunityMemberId);
        });
        
        // GroupGoals
        modelBuilder.Entity<GroupGoal>(entity =>
        {
            entity.HasKey(e => e.GroupGoalId);
            entity.HasIndex(e => new { e.GroupId, e.Status });
            entity.HasIndex(e => e.TargetDate);
        });
        
        // MemberCrafts
        modelBuilder.Entity<MemberCraft>(entity =>
        {
            entity.HasKey(e => e.MemberCraftId);
            entity.HasIndex(e => new { e.CommunityMemberId, e.CraftDefinitionId }).IsUnique();
            entity.HasIndex(e => e.CraftDefinitionId);
            entity.HasIndex(e => new { e.CommunityMemberId, e.IsAvailable });
        });
        
        // CraftDefinitions
        modelBuilder.Entity<CraftDefinition>(entity =>
        {
            entity.HasKey(e => e.CraftDefinitionId);
            entity.HasIndex(e => new { e.CommunityId, e.Category });
            entity.HasIndex(e => new { e.CommunityId, e.IsActive });
        });
        
        // CraftRequests
        modelBuilder.Entity<CraftRequest>(entity =>
        {
            entity.HasKey(e => e.CraftRequestId);
            entity.HasIndex(e => new { e.CommunityId, e.Status });
            entity.HasIndex(e => e.CraftDefinitionId);
            entity.HasIndex(e => e.RequestedByMemberId);
        });
        
        // Events
        modelBuilder.Entity<CommunityEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.HasIndex(e => new { e.CommunityId, e.StartTime });
            entity.HasIndex(e => new { e.CommunityId, e.Status });
            entity.HasIndex(e => e.GroupId);
        });
        
        // EventParticipants
        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => e.EventParticipantId);
            entity.HasIndex(e => new { e.EventId, e.CommunityMemberId }).IsUnique();
            entity.HasIndex(e => new { e.EventId, e.Status });
        });
        
        // Announcements
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId);
            entity.HasIndex(e => new { e.CommunityId, e.CreatedAt });
            entity.HasIndex(e => new { e.GroupId, e.CreatedAt });
            entity.HasIndex(e => new { e.CommunityId, e.IsPinned });
        });
        
        // ActivityLogs
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.ActivityLogId);
            entity.HasIndex(e => new { e.CommunityId, e.Timestamp });
            entity.HasIndex(e => new { e.ActorMemberId, e.Timestamp });
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
        });
    }
}
```

---

## 3. REPOSITORY INTERFACES

### 3.1 Base Repository
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}
```

### 3.2 Spezifische Repositories
```csharp
public interface ICommunityRepository : IRepository<Community>
{
    Task<IEnumerable<Community>> GetByCreatorAsync(Guid userId);
    Task<IEnumerable<Community>> GetByGameAsync(string gameName);
    Task<Community> GetByIdWithDetailsAsync(Guid communityId);
    Task<int> GetMemberCountAsync(Guid communityId);
    Task<bool> IsUserMemberAsync(Guid communityId, Guid userId);
}

public interface ICommunityMemberRepository : IRepository<CommunityMember>
{
    Task<CommunityMember> GetByUserAndCommunityAsync(Guid userId, Guid communityId);
    Task<IEnumerable<CommunityMember>> GetByCommunityAsync(Guid communityId);
    Task<IEnumerable<CommunityMember>> GetByUserAsync(Guid userId);
    Task<IEnumerable<CommunityMember>> GetByRankAsync(Guid rankId);
    Task<IEnumerable<CommunityMember>> GetActiveMembers(Guid communityId);
    Task<bool> HasPermissionAsync(Guid memberId, string permissionName);
}

public interface IRankRepository : IRepository<Rank>
{
    Task<IEnumerable<Rank>> GetByCommunityAsync(Guid communityId);
    Task<Rank> GetDefaultRankAsync(Guid communityId);
    Task<Rank> GetHighestPriorityRankAsync(Guid communityId);
    Task<int> GetMemberCountByRankAsync(Guid rankId);
}

public interface IGroupRepository : IRepository<Group>
{
    Task<IEnumerable<Group>> GetByCommunityAsync(Guid communityId);
    Task<IEnumerable<Group>> GetByTypeAsync(Guid communityId, GroupType type);
    Task<IEnumerable<Group>> GetByLeaderAsync(Guid leaderId);
    Task<Group> GetWithMembersAsync(Guid groupId);
    Task<int> GetMemberCountAsync(Guid groupId);
    Task<bool> IsMemberOfGroupAsync(Guid groupId, Guid memberId);
}

public interface IGroupMemberRepository : IRepository<GroupMember>
{
    Task<IEnumerable<GroupMember>> GetByGroupAsync(Guid groupId);
    Task<IEnumerable<GroupMember>> GetByMemberAsync(Guid communityMemberId);
    Task<GroupMember> GetByGroupAndMemberAsync(Guid groupId, Guid communityMemberId);
    Task<IEnumerable<GroupMember>> GetActiveGroupMembersAsync(Guid groupId);
}

public interface IGroupGoalRepository : IRepository<GroupGoal>
{
    Task<IEnumerable<GroupGoal>> GetByGroupAsync(Guid groupId);
    Task<IEnumerable<GroupGoal>> GetByStatusAsync(Guid groupId, GoalStatus status);
    Task<IEnumerable<GroupGoal>> GetUpcomingGoalsAsync(Guid groupId);
    Task<IEnumerable<GroupGoal>> GetActiveGoalsAsync(Guid communityId);
}

public interface IMemberCraftRepository : IRepository<MemberCraft>
{
    Task<IEnumerable<MemberCraft>> GetByMemberAsync(Guid communityMemberId);
    Task<IEnumerable<MemberCraft>> GetByCraftDefinitionAsync(Guid craftDefinitionId);
    Task<IEnumerable<MemberCraft>> GetAvailableCraftersAsync(Guid craftDefinitionId, int? minLevel = null);
    Task<IEnumerable<MemberCraft>> GetMembersByCraftCategoryAsync(Guid communityId, CraftCategory category);
}

public interface ICraftDefinitionRepository : IRepository<CraftDefinition>
{
    Task<IEnumerable<CraftDefinition>> GetByCommunityAsync(Guid communityId);
    Task<IEnumerable<CraftDefinition>> GetByCategoryAsync(Guid communityId, CraftCategory category);
    Task<IEnumerable<CraftDefinition>> GetActiveCraftsAsync(Guid communityId);
}

public interface ICraftRequestRepository : IRepository<CraftRequest>
{
    Task<IEnumerable<CraftRequest>> GetOpenRequestsAsync(Guid communityId);
    Task<IEnumerable<CraftRequest>> GetByRequesterAsync(Guid memberId);
    Task<IEnumerable<CraftRequest>> GetByFulfillerAsync(Guid memberId);
    Task<IEnumerable<CraftRequest>> GetByCraftAsync(Guid craftDefinitionId);
    Task<IEnumerable<CraftRequest>> GetByStatusAsync(Guid communityId, RequestStatus status);
}

public interface IEventRepository : IRepository<CommunityEvent>
{
    Task<IEnumerable<CommunityEvent>> GetByCommunityAsync(Guid communityId);
    Task<IEnumerable<CommunityEvent>> GetUpcomingEventsAsync(Guid communityId);
    Task<IEnumerable<CommunityEvent>> GetByGroupAsync(Guid groupId);
    Task<IEnumerable<CommunityEvent>> GetByOrganizerAsync(Guid organizerMemberId);
    Task<CommunityEvent> GetWithParticipantsAsync(Guid eventId);
}

public interface IEventParticipantRepository : IRepository<EventParticipant>
{
    Task<IEnumerable<EventParticipant>> GetByEventAsync(Guid eventId);
    Task<IEnumerable<EventParticipant>> GetByMemberAsync(Guid communityMemberId);
    Task<EventParticipant> GetByEventAndMemberAsync(Guid eventId, Guid communityMemberId);
    Task<int> GetConfirmedCountAsync(Guid eventId);
}

public interface IAnnouncementRepository : IRepository<Announcement>
{
    Task<IEnumerable<Announcement>> GetByCommunityAsync(Guid communityId);
    Task<IEnumerable<Announcement>> GetByGroupAsync(Guid groupId);
    Task<IEnumerable<Announcement>> GetPinnedAnnouncementsAsync(Guid communityId);
    Task<IEnumerable<Announcement>> GetRecentAnnouncementsAsync(Guid communityId, int count = 10);
}

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetByCommunityAsync(Guid communityId, int pageSize = 50);
    Task<IEnumerable<ActivityLog>> GetByActorAsync(Guid actorMemberId);
    Task<IEnumerable<ActivityLog>> GetByTypeAsync(Guid communityId, ActivityType activityType);
    Task<IEnumerable<ActivityLog>> GetByEntityAsync(string entityType, Guid entityId);
    Task LogActivityAsync(ActivityLog log);
}
```

---

## 4. SERVICE LAYER (Business Logic)

### 4.1 ICommunityService
```csharp
public interface ICommunityService
{
    Task<Community> CreateCommunityAsync(Guid creatorUserId, string name, string gameName, string description);
    Task<Community> UpdateCommunityAsync(Guid communityId, Guid updaterUserId, Community updates);
    Task<bool> DeleteCommunityAsync(Guid communityId, Guid userId);
    Task<Community> GetCommunityDetailsAsync(Guid communityId);
    Task<IEnumerable<Community>> GetUserCommunitiesAsync(Guid userId);
    Task<IEnumerable<Community>> SearchCommunitiesAsync(string gameName, string searchTerm);
    
    // Member Management
    Task<CommunityMember> JoinCommunityAsync(Guid communityId, Guid userId, string inGameName);
    Task<bool> LeaveCommunityAsync(Guid communityId, Guid userId);
    Task<bool> KickMemberAsync(Guid communityId, Guid kickerUserId, Guid targetMemberId, string reason);
    Task<bool> BanMemberAsync(Guid communityId, Guid bannerUserId, Guid targetMemberId, string reason, DateTime? until);
    Task<bool> UnbanMemberAsync(Guid communityId, Guid userId, Guid targetMemberId);
    
    // Permissions
    Task<bool> UserHasPermissionAsync(Guid communityId, Guid userId, string permission);
    Task<Rank> GetMemberRankAsync(Guid communityMemberId);
}
```

### 4.2 IRankService
```csharp
public interface IRankService
{
    Task<Rank> CreateRankAsync(Guid communityId, Guid creatorMemberId, Rank rank);
    Task<Rank> UpdateRankAsync(Guid rankId, Guid updaterMemberId, Rank updates);
    Task<bool> DeleteRankAsync(Guid rankId, Guid deleterMemberId);
    Task<IEnumerable<Rank>> GetCommunityRanksAsync(Guid communityId);
    Task<bool> AssignRankToMemberAsync(Guid communityMemberId, Guid rankId, Guid assignerMemberId);
    Task<bool> CanManageRank(Guid actorMemberId, Guid targetRankId);
}
```

### 4.3 IGroupService
```csharp
public interface IGroupService
{
    Task<Group> CreateGroupAsync(Guid communityId, Guid creatorMemberId, Group group);
    Task<Group> UpdateGroupAsync(Guid groupId, Guid updaterMemberId, Group updates);
    Task<bool> DeleteGroupAsync(Guid groupId, Guid deleterMemberId);
    Task<IEnumerable<Group>> GetCommunityGroupsAsync(Guid communityId, GroupType? type = null);
    Task<Group> GetGroupWithMembersAsync(Guid groupId);
    
    // Member Management
    Task<bool> AddMemberToGroupAsync(Guid groupId, Guid communityMemberId, Guid adderMemberId);
    Task<bool> RemoveMemberFromGroupAsync(Guid groupId, Guid communityMemberId, Guid removerMemberId);
    Task<bool> SetGroupLeaderAsync(Guid groupId, Guid newLeaderId, Guid setterMemberId);
    Task<bool> SetDeputyLeaderAsync(Guid groupId, Guid deputyId, Guid setterMemberId);
    Task<IEnumerable<GroupMember>> GetGroupMembersAsync(Guid groupId);
    
    // Permissions
    Task<bool> CanManageGroup(Guid actorMemberId, Guid groupId);
    Task<bool> IsGroupLeader(Guid memberId, Guid groupId);
}
```

### 4.4 IGroupGoalService
```csharp
public interface IGroupGoalService
{
    Task<GroupGoal> CreateGoalAsync(Guid groupId, Guid creatorMemberId, GroupGoal goal);
    Task<GroupGoal> UpdateGoalAsync(Guid goalId, Guid updaterMemberId, GroupGoal updates);
    Task<bool> DeleteGoalAsync(Guid goalId, Guid deleterMemberId);
    Task<IEnumerable<GroupGoal>> GetGroupGoalsAsync(Guid groupId, GoalStatus? status = null);
    Task<bool> UpdateGoalProgressAsync(Guid goalId, int newProgress, Guid updaterMemberId);
    Task<bool> CompleteGoalAsync(Guid goalId, Guid completerMemberId);
    Task<IEnumerable<GroupGoal>> GetUpcomingGoalsAsync(Guid communityId);
}
```

### 4.5 ICraftService
```csharp
public interface ICraftService
{
    // Craft Definitions
    Task<CraftDefinition> CreateCraftDefinitionAsync(Guid communityId, Guid creatorMemberId, CraftDefinition craft);
    Task<CraftDefinition> UpdateCraftDefinitionAsync(Guid craftId, Guid updaterMemberId, CraftDefinition updates);
    Task<bool> DeleteCraftDefinitionAsync(Guid craftId, Guid deleterMemberId);
    Task<IEnumerable<CraftDefinition>> GetCommunityCraftsAsync(Guid communityId);
    
    // Member Crafts
    Task<MemberCraft> AddCraftToMemberAsync(Guid communityMemberId, Guid craftDefinitionId, int level, string notes);
    Task<bool> UpdateMemberCraftAsync(Guid memberCraftId, int newLevel, bool isAvailable);
    Task<bool> RemoveMemberCraftAsync(Guid memberCraftId);
    Task<IEnumerable<MemberCraft>> GetMemberCraftsAsync(Guid communityMemberId);
    
    // Craft Requests
    Task<CraftRequest> CreateCraftRequestAsync(Guid communityId, Guid requesterMemberId, CraftRequest request);
    Task<bool> FulfillCraftRequestAsync(Guid requestId, Guid fulfillerMemberId);
    Task<bool> CancelCraftRequestAsync(Guid requestId, Guid cancelerMemberId);
    Task<IEnumerable<CraftRequest>> GetOpenRequestsAsync(Guid communityId);
    Task<IEnumerable<MemberCraft>> FindAvailableCraftersAsync(Guid craftDefinitionId, int? minLevel = null);
}
```

### 4.6 IEventService
```csharp
public interface IEventService
{
    Task<CommunityEvent> CreateEventAsync(Guid communityId, Guid organizerMemberId, CommunityEvent eventData);
    Task<CommunityEvent> UpdateEventAsync(Guid eventId, Guid updaterMemberId, CommunityEvent updates);
    Task<bool> CancelEventAsync(Guid eventId, Guid cancelerMemberId);
    Task<IEnumerable<CommunityEvent>> GetUpcomingEventsAsync(Guid communityId);
    Task<CommunityEvent> GetEventWithParticipantsAsync(Guid eventId);
    
    // Participants
    Task<bool> SignUpForEventAsync(Guid eventId, Guid communityMemberId, string role);
    Task<bool> UpdateParticipantStatusAsync(Guid eventId, Guid communityMemberId, ParticipantStatus status);
    Task<bool> RemoveParticipantAsync(Guid eventId, Guid communityMemberId, Guid removerMemberId);
    Task<IEnumerable<EventParticipant>> GetEventParticipantsAsync(Guid eventId);
    Task<bool> IsEventFullAsync(Guid eventId);
}
```

### 4.7 IAnnouncementService
```csharp
public interface IAnnouncementService
{
    Task<Announcement> CreateAnnouncementAsync(Guid communityId, Guid authorMemberId, Announcement announcement);
    Task<Announcement> UpdateAnnouncementAsync(Guid announcementId, Guid updaterMemberId, Announcement updates);
    Task<bool> DeleteAnnouncementAsync(Guid announcementId, Guid deleterMemberId);
    Task<IEnumerable<Announcement>> GetCommunityAnnouncementsAsync(Guid communityId);
    Task<IEnumerable<Announcement>> GetGroupAnnouncementsAsync(Guid groupId);
    Task<bool> PinAnnouncementAsync(Guid announcementId, Guid pinnerMemberId);
    Task<bool> UnpinAnnouncementAsync(Guid announcementId, Guid unpinnerMemberId);
}
```

---

## 5. DTOs (Data Transfer Objects)

```csharp
// Request DTOs
public class CreateCommunityRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string GameName { get; set; }
    public string GameType { get; set; }
    public int MaxMembers { get; set; }
}

public class CreateGroupRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public GroupType Type { get; set; }
    public int MaxMembers { get; set; }
    public bool IsPrivate { get; set; }
}

public class CreateRankRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ColorHex { get; set; }
    public int Priority { get; set; }
    public RankPermissions Permissions { get; set; }
}

public class RankPermissions
{
    public bool CanManageMembers { get; set; }
    public bool CanManageRanks { get; set; }
    public bool CanManageGroups { get; set; }
    // ... weitere Permissions
}

public class CreateCraftDefinitionRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public CraftCategory Category { get; set; }
    public int MaxLevel { get; set; }
}

public class AddMemberCraftRequest
{
    public Guid CraftDefinitionId { get; set; }
    public int Level { get; set; }
    public string Notes { get; set; }
}

public class CreateCraftRequestRequest
{
    public Guid CraftDefinitionId { get; set; }
    public int? RequiredLevel { get; set; }
    public string Description { get; set; }
    public string Materials { get; set; }
    public string Reward { get; set; }
}

public class CreateGoalRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public GoalType Type { get; set; }
    public DateTime? TargetDate { get; set; }
    public int Priority { get; set; }
}

public class CreateEventRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxParticipants { get; set; }
}

// Response DTOs
public class CommunityDetailDto
{
    public Guid CommunityId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string GameName { get; set; }
    public int MemberCount { get; set; }
    public int GroupCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUserMember { get; set; }
    public RankDto UserRank { get; set; }
}

public class MemberDto
{
    public Guid CommunityMemberId { get; set; }
    public Guid UserId { get; set; }
    public string InGameName { get; set; }
    public RankDto Rank { get; set; }
    public DateTime JoinedAt { get; set; }
    public MemberStatus Status { get; set; }
    public List<CraftSummaryDto> Crafts { get; set; }
}

public class RankDto
{
    public Guid RankId { get; set; }
    public string Name { get; set; }
    public string ColorHex { get; set; }
    public int Priority { get; set; }
}

public class GroupDto
{
    public Guid GroupId { get; set; }
    public string Name { get; set; }
    public GroupType Type { get; set; }
    public MemberDto Leader { get; set; }
    public MemberDto Deputy { get; set; }
    public int MemberCount { get; set; }
    public int MaxMembers { get; set; }
}

public class GroupDetailDto : GroupDto
{
    public List<GroupMemberDto> Members { get; set; }
    public List<GroupGoalDto> ActiveGoals { get; set; }
}

public class GroupGoalDto
{
    public Guid GroupGoalId { get; set; }
    public string Title { get; set; }
    public GoalType Type { get; set; }
    public GoalStatus Status { get; set; }
    public DateTime? TargetDate { get; set; }
    public int? CurrentProgress { get; set; }
    public int? TargetQuantity { get; set; }
}

public class CraftSummaryDto
{
    public Guid CraftDefinitionId { get; set; }
    public string Name { get; set; }
    public CraftCategory Category { get; set; }
    public int Level { get; set; }
    public bool IsAvailable { get; set; }
}

public class CraftRequestDto
{
    public Guid CraftRequestId { get; set; }
    public CraftDefinition Craft { get; set; }
    public MemberDto Requester { get; set; }
    public string Description { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EventDto
{
    public Guid EventId { get; set; }
    public string Title { get; set; }
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public MemberDto Organizer { get; set; }
    public int ParticipantCount { get; set; }
    public int MaxParticipants { get; set; }
    public EventStatus Status { get; set; }
}
```

---

## 7. SIGNALR REAL-TIME COMMUNICATION

### 7.1 CommunityHub (SignalR Hub)
```csharp
public class CommunityHub : Hub
{
    private readonly ICommunityService _communityService;
    private readonly ILogger<CommunityHub> _logger;
    
    public CommunityHub(ICommunityService communityService, ILogger<CommunityHub> logger)
    {
        _communityService = communityService;
        _logger = logger;
    }
    
    // === CONNECTION MANAGEMENT ===
    public async Task JoinCommunityChannel(string communityId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"community_{communityId}");
        _logger.LogInformation($"User {Context.ConnectionId} joined community channel: {communityId}");
    }
    
    public async Task LeaveCommunityChannel(string communityId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"community_{communityId}");
        _logger.LogInformation($"User {Context.ConnectionId} left community channel: {communityId}");
    }
    
    public async Task JoinGroupChannel(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{groupId}");
    }
    
    public async Task LeaveGroupChannel(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group_{groupId}");
    }
    
    // === REAL-TIME NOTIFICATIONS ===
    public async Task NotifyMemberJoined(string communityId, CommunityMemberDto member)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("MemberJoined", member);
    }
    
    public async Task NotifyMemberLeft(string communityId, string memberId, string reason)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("MemberLeft", memberId, reason);
    }
    
    public async Task NotifyRankChanged(string communityId, string memberId, RankDto newRank)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("RankChanged", memberId, newRank);
    }
    
    public async Task NotifyGroupCreated(string communityId, GroupDto group)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("GroupCreated", group);
    }
    
    public async Task NotifyGroupUpdated(string communityId, GroupDto group)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("GroupUpdated", group);
    }
    
    public async Task NotifyGroupDeleted(string communityId, string groupId)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("GroupDeleted", groupId);
    }
    
    public async Task NotifyEventCreated(string communityId, EventDto evt)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("EventCreated", evt);
    }
    
    public async Task NotifyEventUpdated(string communityId, EventDto evt)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("EventUpdated", evt);
    }
    
    public async Task NotifyEventCancelled(string communityId, string eventId, string reason)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("EventCancelled", eventId, reason);
    }
    
    public async Task NotifyEventParticipantJoined(string communityId, string eventId, EventParticipantDto participant)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("EventParticipantJoined", eventId, participant);
    }
    
    public async Task NotifyGoalCreated(string groupId, GroupGoalDto goal)
    {
        await Clients.Group($"group_{groupId}")
            .SendAsync("GoalCreated", goal);
    }
    
    public async Task NotifyGoalProgressUpdated(string groupId, string goalId, int newProgress, int targetProgress)
    {
        await Clients.Group($"group_{groupId}")
            .SendAsync("GoalProgressUpdated", goalId, newProgress, targetProgress);
    }
    
    public async Task NotifyGoalCompleted(string groupId, GroupGoalDto goal)
    {
        await Clients.Group($"group_{groupId}")
            .SendAsync("GoalCompleted", goal);
    }
    
    public async Task NotifyCraftRequestCreated(string communityId, CraftRequestDto request)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("CraftRequestCreated", request);
    }
    
    public async Task NotifyCraftRequestFulfilled(string communityId, string requestId, string fulfillerName)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("CraftRequestFulfilled", requestId, fulfillerName);
    }
    
    public async Task NotifyAnnouncementPosted(string communityId, AnnouncementDto announcement)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("AnnouncementPosted", announcement);
    }
    
    public async Task NotifyAnnouncementPinned(string communityId, string announcementId)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("AnnouncementPinned", announcementId);
    }
    
    // === TYPING INDICATORS (Optional für Chat) ===
    public async Task UserTyping(string communityId, string userName)
    {
        await Clients.OthersInGroup($"community_{communityId}")
            .SendAsync("UserTyping", userName);
    }
    
    // === ONLINE STATUS ===
    public async Task UpdateOnlineStatus(string communityId, string memberId, bool isOnline)
    {
        await Clients.Group($"community_{communityId}")
            .SendAsync("OnlineStatusChanged", memberId, isOnline);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"User {Context.ConnectionId} disconnected");
        await base.OnDisconnectedAsync(exception);
    }
}
```

### 7.2 SignalR Service Integration
```csharp
public interface ICommunitySignalRService
{
    Task BroadcastMemberJoinedAsync(string communityId, CommunityMemberDto member);
    Task BroadcastMemberLeftAsync(string communityId, string memberId, string reason);
    Task BroadcastRankChangedAsync(string communityId, string memberId, RankDto newRank);
    Task BroadcastGroupCreatedAsync(string communityId, GroupDto group);
    Task BroadcastGroupUpdatedAsync(string communityId, GroupDto group);
    Task BroadcastEventCreatedAsync(string communityId, EventDto evt);
    Task BroadcastEventUpdatedAsync(string communityId, EventDto evt);
    Task BroadcastGoalProgressAsync(string groupId, string goalId, int progress, int target);
    Task BroadcastAnnouncementAsync(string communityId, AnnouncementDto announcement);
}

public class CommunitySignalRService : ICommunitySignalRService
{
    private readonly IHubContext<CommunityHub> _hubContext;
    
    public CommunitySignalRService(IHubContext<CommunityHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task BroadcastMemberJoinedAsync(string communityId, CommunityMemberDto member)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("MemberJoined", member);
    }
    
    public async Task BroadcastMemberLeftAsync(string communityId, string memberId, string reason)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("MemberLeft", memberId, reason);
    }
    
    public async Task BroadcastRankChangedAsync(string communityId, string memberId, RankDto newRank)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("RankChanged", memberId, newRank);
    }
    
    public async Task BroadcastGroupCreatedAsync(string communityId, GroupDto group)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("GroupCreated", group);
    }
    
    public async Task BroadcastGroupUpdatedAsync(string communityId, GroupDto group)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("GroupUpdated", group);
    }
    
    public async Task BroadcastEventCreatedAsync(string communityId, EventDto evt)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("EventCreated", evt);
    }
    
    public async Task BroadcastEventUpdatedAsync(string communityId, EventDto evt)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("EventUpdated", evt);
    }
    
    public async Task BroadcastGoalProgressAsync(string groupId, string goalId, int progress, int target)
    {
        await _hubContext.Clients.Group($"group_{groupId}")
            .SendAsync("GoalProgressUpdated", goalId, progress, target);
    }
    
    public async Task BroadcastAnnouncementAsync(string communityId, AnnouncementDto announcement)
    {
        await _hubContext.Clients.Group($"community_{communityId}")
            .SendAsync("AnnouncementPosted", announcement);
    }
}
```

### 7.3 Program.cs Configuration
```csharp
// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<ICommunitySignalRService, CommunitySignalRService>();

// ... nach app.Build()

// Map SignalR Hub
app.MapHub<CommunityHub>("/hubs/community");
```

### 7.4 Services Integration (Beispiel: EventService)
```csharp
public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ICommunitySignalRService _signalRService;
    
    public EventService(
        IEventRepository eventRepository,
        ICommunitySignalRService signalRService)
    {
        _eventRepository = eventRepository;
        _signalRService = signalRService;
    }
    
    public async Task<CommunityEvent> CreateEventAsync(string communityId, string organizerMemberId, CommunityEvent eventData)
    {
        // Validierung & Business Logic
        var evt = await _eventRepository.AddAsync(eventData);
        
        // SignalR Broadcast
        var eventDto = MapToDto(evt);
        await _signalRService.BroadcastEventCreatedAsync(communityId, eventDto);
        
        return evt;
    }
    
    public async Task<bool> UpdateEventAsync(string eventId, string updaterMemberId, CommunityEvent updates)
    {
        var evt = await _eventRepository.UpdateAsync(updates);
        
        var eventDto = MapToDto(evt);
        await _signalRService.BroadcastEventUpdatedAsync(evt.CommunityId, eventDto);
        
        return true;
    }
}
```

---

## 8. MUDBLAZOR COMPONENTS

### 8.1 Community-Übersicht mit SignalR
```razor
@page "/communities"
@inject ICommunityService CommunityService

<h3>Meine Communities</h3>

<button @onclick="ShowCreateModal">Neue Community erstellen</button>

@if (communities != null)
{
    <div class="community-grid">
        @foreach (var community in communities)
        {
            <CommunityCard Community="community" OnClick="() => NavigateToCommunity(community.CommunityId)" />
        }
    </div>
}

@code {
    private List<CommunityDetailDto> communities;
    
    protected override async Task OnInitializedAsync()
    {
        communities = await CommunityService.GetUserCommunitiesAsync(CurrentUserId);
    }
}
```

### 6.2 Gruppen-Management
```razor
@page "/community/{CommunityId:guid}/groups"
@inject IGroupService GroupService

<h3>Gruppen</h3>

<div class="group-filters">
    <button @onclick="() => FilterByType(GroupType.Raid)">Raids</button>
    <button @onclick="() => FilterByType(GroupType.PvP)">PvP</button>
    <button @onclick="() => FilterByType(GroupType.Company)">Companies</button>
</div>

@if (CanCreateGroups)
{
    <button @onclick="ShowCreateGroupModal">Neue Gruppe erstellen</button>
}

<div class="groups-list">
    @foreach (var group in filteredGroups)
    {
        <GroupCard Group="group" OnClick="() => ViewGroupDetails(group.GroupId)" />
    }
</div>
```

### 6.3 Craft-Browser
```razor
@page "/community/{CommunityId:guid}/crafts"
@inject ICraftService CraftService

<h3>Crafts & Herstellung</h3>

<div class="craft-search">
    <input @bind="searchTerm" placeholder="Craft suchen..." />
    <select @bind="selectedCategory">
        <option value="">Alle Kategorien</option>
        @foreach (var category in Enum.GetValues<CraftCategory>())
        {
            <option value="@category">@category</option>
        }
    </select>
</div>

<h4>Wer kann was?</h4>
<div class="craft-list">
    @foreach (var craft in crafts)
    {
        <div class="craft-item">
            <h5>@craft.Name</h5>
            <button @onclick="() => FindCrafters(craft.CraftDefinitionId)">
                Crafter finden
            </button>
        </div>
    }
</div>

@if (showCrafterModal)
{
    <CrafterModal Craft="selectedCraft" Crafters="availableCrafters" OnClose="CloseCrafterModal" />
}
```

### 6.4 Event-Kalender
```razor
@page "/community/{CommunityId:guid}/events"
@inject IEventService EventService

<h3>Events</h3>

<button @onclick="ShowCreateEventModal">Event erstellen</button>

<div class="event-calendar">
    @foreach (var evt in upcomingEvents)
    {
        <div class="event-card">
            <h4>@evt.Title</h4>
            <p>@evt.StartTime.ToString("dd.MM.yyyy HH:mm")</p>
            <p>@evt.ParticipantCount / @evt.MaxParticipants Teilnehmer</p>
            <button @onclick="() => SignUpForEvent(evt.EventId)">Anmelden</button>
        </div>
    }
</div>
```

---

### 8.1 Community-Übersicht mit SignalR
```razor
@page "/communities"
@inject ICommunityService CommunityService
@inject NavigationManager Navigation
@inject ISnackbar Snackbar
@inject HubConnection HubConnection
@implements IAsyncDisposable

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Meine Communities</MudText>
    
    <MudButton Variant="Variant.Filled" 
               Color="Color.Primary" 
               StartIcon="@Icons.Material.Filled.Add"
               OnClick="ShowCreateDialog">
        Neue Community erstellen
    </MudButton>
    
    @if (communities == null)
    {
        <MudProgressCircular Color="Color.Primary" Indeterminate="true" />
    }
    else if (communities.Count == 0)
    {
        <MudAlert Severity="Severity.Info" Class="mt-4">
            Du bist noch in keiner Community. Erstelle eine oder tritt einer bei!
        </MudAlert>
    }
    else
    {
        <MudGrid Class="mt-4">
            @foreach (var community in communities)
            {
                <MudItem xs="12" sm="6" md="4">
                    <MudCard>
                        @if (!string.IsNullOrEmpty(community.BannerUrl))
                        {
                            <MudCardMedia Image="@community.BannerUrl" Height="200" />
                        }
                        <MudCardContent>
                            <MudText Typo="Typo.h6">@community.Name</MudText>
                            <MudText Typo="Typo.body2" Color="Color.Secondary">
                                @community.GameName
                            </MudText>
                            <MudText Typo="Typo.body2" Class="mt-2">
                                @community.MemberCount Mitglieder
                            </MudText>
                        </MudCardContent>
                        <MudCardActions>
                            <MudButton Variant="Variant.Text" 
                                       Color="Color.Primary"
                                       OnClick="() => NavigateToCommunity(community.Id)">
                                Öffnen
                            </MudButton>
                        </MudCardActions>
                    </MudCard>
                </MudItem>
            }
        </MudGrid>
    }
</MudContainer>

<MudDialog @bind-IsVisible="showCreateDialog">
    <DialogContent>
        <MudTextField @bind-Value="newCommunity.Name" 
                      Label="Community Name" 
                      Required="true" />
        <MudTextField @bind-Value="newCommunity.GameName" 
                      Label="Spiel Name" 
                      Required="true" 
                      Class="mt-3" />
        <MudTextField @bind-Value="newCommunity.Description" 
                      Label="Beschreibung" 
                      Lines="3" 
                      Class="mt-3" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="() => showCreateDialog = false">Abbrechen</MudButton>
        <MudButton Color="Color.Primary" OnClick="CreateCommunity">Erstellen</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private List<CommunityDetailDto>? communities;
    private bool showCreateDialog;
    private CreateCommunityRequest newCommunity = new();
    private HubConnection? hubConnection;
    
    protected override async Task OnInitializedAsync()
    {
        communities = await CommunityService.GetUserCommunitiesAsync(CurrentUserId);
        await InitializeSignalR();
    }
    
    private async Task InitializeSignalR()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/hubs/community"))
            .WithAutomaticReconnect()
            .Build();
        
        hubConnection.On<CommunityDetailDto>("CommunityUpdated", (community) =>
        {
            var existing = communities?.FirstOrDefault(c => c.Id == community.Id);
            if (existing != null)
            {
                var index = communities!.IndexOf(existing);
                communities[index] = community;
                InvokeAsync(StateHasChanged);
            }
        });
        
        await hubConnection.StartAsync();
    }
    
    private void ShowCreateDialog() => showCreateDialog = true;
    
    private async Task CreateCommunity()
    {
        var community = await CommunityService.CreateCommunityAsync(
            CurrentUserId, 
            newCommunity.Name, 
            newCommunity.GameName, 
            newCommunity.Description);
        
        communities?.Add(MapToDetailDto(community));
        showCreateDialog = false;
        Snackbar.Add("Community erfolgreich erstellt!", Severity.Success);
        StateHasChanged();
    }
    
    private void NavigateToCommunity(string communityId)
    {
        Navigation.NavigateTo($"/community/{communityId}");
    }
    
    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
```

### 8.2 Gruppen-Management mit Real-Time Updates
```razor
@page "/community/{CommunityId}/groups"
@inject IGroupService GroupService
@inject ICommunitySignalRService SignalRService
@inject ISnackbar Snackbar
@implements IAsyncDisposable

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Gruppen</MudText>
    
    <MudChipSet>
        <MudChip Color="Color.Primary" OnClick="() => FilterByType(null)">Alle</MudChip>
        <MudChip OnClick="() => FilterByType(GroupType.Raid)">Raids</MudChip>
        <MudChip OnClick="() => FilterByType(GroupType.PvP)">PvP</MudChip>
        <MudChip OnClick="() => FilterByType(GroupType.Company)">Companies</MudChip>
    </MudChipSet>
    
    @if (CanCreateGroups)
    {
        <MudButton Variant="Variant.Filled" 
                   Color="Color.Primary" 
                   StartIcon="@Icons.Material.Filled.Add"
                   OnClick="ShowCreateGroupDialog"
                   Class="mt-3">
            Neue Gruppe erstellen
        </MudButton>
    }
    
    <MudGrid Class="mt-4">
        @foreach (var group in filteredGroups)
        {
            <MudItem xs="12" md="6" lg="4">
                <MudCard>
                    <MudCardHeader>
                        <CardHeaderContent>
                            <MudText Typo="Typo.h6">@group.Name</MudText>
                            <MudChip Size="Size.Small" Color="Color.Info">@group.Type</MudChip>
                        </CardHeaderContent>
                        <CardHeaderActions>
                            @if (group.IsPrivate)
                            {
                                <MudIcon Icon="@Icons.Material.Filled.Lock" Size="Size.Small" />
                            }
                        </CardHeaderActions>
                    </MudCardHeader>
                    <MudCardContent>
                        <MudText Typo="Typo.body2">@group.Description</MudText>
                        <MudDivider Class="my-2" />
                        <MudText Typo="Typo.body2">
                            <MudIcon Icon="@Icons.Material.Filled.Person" Size="Size.Small" />
                            @group.MemberCount / @(group.MaxMembers == 0 ? "∞" : group.MaxMembers.ToString())
                        </MudText>
                        <MudText Typo="Typo.body2" Class="mt-1">
                            Leader: @group.Leader?.InGameName
                        </MudText>
                    </MudCardContent>
                    <MudCardActions>
                        <MudButton Variant="Variant.Text" 
                                   Color="Color.Primary"
                                   OnClick="() => ViewGroupDetails(group.Id)">
                            Details
                        </MudButton>
                    </MudCardActions>
                </MudCard>
            </MudItem>
        }
    </MudGrid>
</MudContainer>

@code {
    [Parameter] public string CommunityId { get; set; } = string.Empty;
    
    private List<GroupDto> groups = new();
    private List<GroupDto> filteredGroups = new();
    private GroupType? selectedType;
    private HubConnection? hubConnection;
    private bool CanCreateGroups => true; // Aus Permissions laden
    
    protected override async Task OnInitializedAsync()
    {
        groups = (await GroupService.GetCommunityGroupsAsync(CommunityId)).ToList();
        filteredGroups = groups;
        
        await InitializeSignalR();
    }
    
    private async Task InitializeSignalR()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/community"))
            .WithAutomaticReconnect()
            .Build();
        
        hubConnection.On<GroupDto>("GroupCreated", (group) =>
        {
            groups.Add(group);
            FilterByType(selectedType);
            InvokeAsync(StateHasChanged);
            Snackbar.Add($"Neue Gruppe '{group.Name}' wurde erstellt!", Severity.Info);
        });
        
        hubConnection.On<GroupDto>("GroupUpdated", (group) =>
        {
            var existing = groups.FirstOrDefault(g => g.Id == group.Id);
            if (existing != null)
            {
                var index = groups.IndexOf(existing);
                groups[index] = group;
                FilterByType(selectedType);
                InvokeAsync(StateHasChanged);
            }
        });
        
        hubConnection.On<string>("GroupDeleted", (groupId) =>
        {
            groups.RemoveAll(g => g.Id == groupId);
            FilterByType(selectedType);
            InvokeAsync(StateHasChanged);
            Snackbar.Add("Gruppe wurde gelöscht", Severity.Warning);
        });
        
        await hubConnection.StartAsync();
        await hubConnection.InvokeAsync("JoinCommunityChannel", CommunityId);
    }
    
    private void FilterByType(GroupType? type)
    {
        selectedType = type;
        filteredGroups = type.HasValue 
            ? groups.Where(g => g.Type == type.Value).ToList()
            : groups;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.InvokeAsync("LeaveCommunityChannel", CommunityId);
            await hubConnection.DisposeAsync();
        }
    }
}
```

### 8.3 Event-Kalender mit Live-Updates
```razor
@page "/community/{CommunityId}/events"
@inject IEventService EventService
@inject ISnackbar Snackbar
@implements IAsyncDisposable

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Events</MudText>
    
    <MudButton Variant="Variant.Filled" 
               Color="Color.Primary" 
               StartIcon="@Icons.Material.Filled.Event"
               OnClick="ShowCreateEventDialog">
        Event erstellen
    </MudButton>
    
    <MudTimeline TimelineOrientation="TimelineOrientation.Vertical" Class="mt-4">
        @foreach (var evt in upcomingEvents.OrderBy(e => e.StartTime))
        {
            <MudTimelineItem Color="GetEventColor(evt.Type)" Size="Size.Large">
                <ItemOpposite>
                    <MudText Typo="Typo.body2">
                        @evt.StartTime.ToString("dd.MM.yyyy HH:mm")
                    </MudText>
                </ItemOpposite>
                <ItemContent>
                    <MudCard>
                        <MudCardHeader>
                            <CardHeaderContent>
                                <MudText Typo="Typo.h6">@evt.Title</MudText>
                                <MudChip Size="Size.Small" Color="GetEventColor(evt.Type)">
                                    @evt.Type
                                </MudChip>
                            </CardHeaderContent>
                        </MudCardHeader>
                        <MudCardContent>
                            <MudText Typo="Typo.body2">@evt.Description</MudText>
                            <MudDivider Class="my-2" />
                            <MudText Typo="Typo.body2">
                                <MudIcon Icon="@Icons.Material.Filled.People" Size="Size.Small" />
                                @evt.ParticipantCount / @evt.MaxParticipants Teilnehmer
                            </MudText>
                            <MudText Typo="Typo.body2" Class="mt-1">
                                Organisiert von: @evt.Organizer?.InGameName
                            </MudText>
                            @if (!string.IsNullOrEmpty(evt.Location))
                            {
                                <MudText Typo="Typo.body2" Class="mt-1">
                                    <MudIcon Icon="@Icons.Material.Filled.LocationOn" Size="Size.Small" />
                                    @evt.Location
                                </MudText>
                            }
                        </MudCardContent>
                        <MudCardActions>
                            <MudButton Variant="Variant.Text" 
                                       Color="Color.Primary"
                                       OnClick="() => SignUpForEvent(evt.Id)"
                                       Disabled="IsEventFull(evt)">
                                @(IsSignedUp(evt.Id) ? "Angemeldet" : "Anmelden")
                            </MudButton>
                            <MudButton Variant="Variant.Text" 
                                       OnClick="() => ViewEventDetails(evt.Id)">
                                Details
                            </MudButton>
                        </MudCardActions>
                    </MudCard>
                </ItemContent>
            </MudTimelineItem>
        }
    </MudTimeline>
</MudContainer>

@code {
    [Parameter] public string CommunityId { get; set; } = string.Empty;
    
    private List<EventDto> upcomingEvents = new();
    private HashSet<string> signedUpEvents = new();
    private HubConnection? hubConnection;
    
    protected override async Task OnInitializedAsync()
    {
        upcomingEvents = (await EventService.GetUpcomingEventsAsync(CommunityId)).ToList();
        await InitializeSignalR();
    }
    
    private async Task InitializeSignalR()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/community"))
            .WithAutomaticReconnect()
            .Build();
        
        hubConnection.On<EventDto>("EventCreated", (evt) =>
        {
            upcomingEvents.Add(evt);
            InvokeAsync(StateHasChanged);
            Snackbar.Add($"Neues Event: {evt.Title}", Severity.Info);
        });
        
        hubConnection.On<EventDto>("EventUpdated", (evt) =>
        {
            var existing = upcomingEvents.FirstOrDefault(e => e.Id == evt.Id);
            if (existing != null)
            {
                var index = upcomingEvents.IndexOf(existing);
                upcomingEvents[index] = evt;
                InvokeAsync(StateHasChanged);
            }
        });
        
        hubConnection.On<string, EventParticipantDto>("EventParticipantJoined", 
            (eventId, participant) =>
        {
            var evt = upcomingEvents.FirstOrDefault(e => e.Id == eventId);
            if (evt != null)
            {
                evt.ParticipantCount++;
                InvokeAsync(StateHasChanged);
            }
        });
        
        await hubConnection.StartAsync();
        await hubConnection.InvokeAsync("JoinCommunityChannel", CommunityId);
    }
    
    private async Task SignUpForEvent(string eventId)
    {
        await EventService.SignUpForEventAsync(eventId, CurrentMemberId, null);
        signedUpEvents.Add(eventId);
        Snackbar.Add("Erfolgreich angemeldet!", Severity.Success);
    }
    
    private Color GetEventColor(EventType type) => type switch
    {
        EventType.Raid => Color.Error,
        EventType.PvP => Color.Warning,
        EventType.Social => Color.Success,
        EventType.Training => Color.Info,
        _ => Color.Default
    };
    
    private bool IsEventFull(EventDto evt) => 
        evt.MaxParticipants > 0 && evt.ParticipantCount >= evt.MaxParticipants;
    
    private bool IsSignedUp(string eventId) => signedUpEvents.Contains(eventId);
    
    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.InvokeAsync("LeaveCommunityChannel", CommunityId);
            await hubConnection.DisposeAsync();
        }
    }
}
```

### 8.4 Craft-Browser mit Live-Requests
```razor
@page "/community/{CommunityId}/crafts"
@inject ICraftService CraftService
@inject ISnackbar Snackbar
@implements IAsyncDisposable

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Crafts & Herstellung</MudText>
    
    <MudGrid>
        <MudItem xs="12" md="8">
            <MudTextField @bind-Value="searchTerm" 
                          Label="Craft suchen" 
                          Variant="Variant.Outlined"
                          Adornment="Adornment.Start"
                          AdornmentIcon="@Icons.Material.Filled.Search" />
        </MudItem>
        <MudItem xs="12" md="4">
            <MudSelect @bind-Value="selectedCategory" 
                       Label="Kategorie" 
                       Variant="Variant.Outlined">
                <MudSelectItem Value="@((CraftCategory?)null)">Alle Kategorien</MudSelectItem>
                @foreach (var category in Enum.GetValues<CraftCategory>())
                {
                    <MudSelectItem Value="@category">@category</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
    </MudGrid>
    
    <MudTabs Elevation="2" Rounded="true" Class="mt-4">
        <MudTabPanel Text="Verfügbare Crafts" Icon="@Icons.Material.Filled.Build">
            <MudGrid Class="mt-3">
                @foreach (var craft in filteredCrafts)
                {
                    <MudItem xs="12" sm="6" md="4">
                        <MudCard>
                            <MudCardContent>
                                <MudText Typo="Typo.h6">@craft.Name</MudText>
                                <MudChip Size="Size.Small" Color="Color.Primary">
                                    @craft.Category
                                </MudChip>
                                <MudText Typo="Typo.body2" Class="mt-2">
                                    @craft.Description
                                </MudText>
                            </MudCardContent>
                            <MudCardActions>
                                <MudButton Variant="Variant.Text" 
                                           Color="Color.Primary"
                                           StartIcon="@Icons.Material.Filled.Search"
                                           OnClick="() => FindCrafters(craft.Id)">
                                    Crafter finden
                                </MudButton>
                            </MudCardActions>
                        </MudCard>
                    </MudItem>
                }
            </MudGrid>
        </MudTabPanel>
        
        <MudTabPanel Text="Offene Anfragen" Icon="@Icons.Material.Filled.Help" 
                     BadgeData="@openRequests.Count" BadgeColor="Color.Error">
            <MudList Clickable="true">
                @foreach (var request in openRequests)
                {
                    <MudListItem>
                        <MudGrid>
                            <MudItem xs="8">
                                <MudText Typo="Typo.body1" Color="Color.Primary">
                                    @request.Craft.Name
                                </MudText>
                                <MudText Typo="Typo.body2">
                                    @request.Description
                                </MudText>
                                <MudText Typo="Typo.caption">
                                    Angefragt von: @request.Requester.InGameName
                                </MudText>
                            </MudItem>
                            <MudItem xs="4" Class="d-flex align-center justify-end">
                                <MudChip Color="GetPriorityColor(request.Priority)">
                                    @request.Priority
                                </MudChip>
                                <MudButton Variant="Variant.Filled" 
                                           Color="Color.Success" 
                                           Size="Size.Small"
                                           OnClick="() => FulfillRequest(request.Id)"
                                           Class="ml-2">
                                    Annehmen
                                </MudButton>
                            </MudItem>
                        </MudGrid>
                    </MudListItem>
                    <MudDivider />
                }
            </MudList>
        </MudTabPanel>
    </MudTabs>
</MudContainer>

<MudDialog @bind-IsVisible="showCraftersDialog">
    <DialogContent>
        <MudText Typo="Typo.h6" Class="mb-3">
            Verfügbare Crafter für @selectedCraft?.Name
        </MudText>
        @if (availableCrafters.Any())
        {
            <MudList>
                @foreach (var crafter in availableCrafters)
                {
                    <MudListItem>
                        <MudGrid>
                            <MudItem xs="8">
                                <MudText Typo="Typo.body1">@crafter.Member.InGameName</MudText>
                                <MudText Typo="Typo.body2">Level @crafter.Level</MudText>
                                @if (!string.IsNullOrEmpty(crafter.Notes))
                                {
                                    <MudText Typo="Typo.caption">@crafter.Notes</MudText>
                                }
                            </MudItem>
                            <MudItem xs="4" Class="d-flex align-center justify-end">
                                @if (crafter.IsAvailable)
                                {
                                    <MudChip Color="Color.Success" Size="Size.Small">Verfügbar</MudChip>
                                }
                                else
                                {
                                    <MudChip Color="Color.Default" Size="Size.Small">Beschäftigt</MudChip>
                                }
                            </MudItem>
                        </MudGrid>
                    </MudListItem>
                }
            </MudList>
        }
        else
        {
            <MudAlert Severity="Severity.Warning">
                Aktuell keine Crafter verfügbar
            </MudAlert>
        }
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="() => showCraftersDialog = false">Schließen</MudButton>
    </DialogActions>
</MudDialog>

@code {
    [Parameter] public string CommunityId { get; set; } = string.Empty;
    
    private List<CraftDefinition> crafts = new();
    private List<CraftDefinition> filteredCrafts = new();
    private List<CraftRequestDto> openRequests = new();
    private List<MemberCraftDto> availableCrafters = new();
    
    private string searchTerm = string.Empty;
    private CraftCategory? selectedCategory;
    private CraftDefinition? selectedCraft;
    private bool showCraftersDialog;
    
    private HubConnection? hubConnection;
    
    protected override async Task OnInitializedAsync()
    {
        crafts = (await CraftService.GetCommunityCraftsAsync(CommunityId)).ToList();
        filteredCrafts = crafts;
        openRequests = (await CraftService.GetOpenRequestsAsync(CommunityId)).ToList();
        
        await InitializeSignalR();
    }
    
    private async Task InitializeSignalR()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/community"))
            .WithAutomaticReconnect()
            .Build();
        
        hubConnection.On<CraftRequestDto>("CraftRequestCreated", (request) =>
        {
            openRequests.Add(request);
            InvokeAsync(StateHasChanged);
            Snackbar.Add($"Neue Craft-Anfrage: {request.Craft.Name}", Severity.Info);
        });
        
        hubConnection.On<string, string>("CraftRequestFulfilled", (requestId, fulfillerName) =>
        {
            openRequests.RemoveAll(r => r.Id == requestId);
            InvokeAsync(StateHasChanged);
            Snackbar.Add($"{fulfillerName} hat eine Anfrage angenommen!", Severity.Success);
        });
        
        await hubConnection.StartAsync();
        await hubConnection.InvokeAsync("JoinCommunityChannel", CommunityId);
    }
    
    private async Task FindCrafters(string craftDefinitionId)
    {
        selectedCraft = crafts.FirstOrDefault(c => c.Id == craftDefinitionId);
        availableCrafters = (await CraftService.FindAvailableCraftersAsync(craftDefinitionId))
            .ToList();
        showCraftersDialog = true;
    }
    
    private Color GetPriorityColor(RequestPriority priority) => priority switch
    {
        RequestPriority.Urgent => Color.Error,
        RequestPriority.High => Color.Warning,
        RequestPriority.Normal => Color.Info,
        _ => Color.Default
    };
    
    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.InvokeAsync("LeaveCommunityChannel", CommunityId);
            await hubConnection.DisposeAsync();
        }
    }
}
```

---

## 9. IMPLEMENTATION ANWEISUNGEN FÜR COPILOT

## 9. IMPLEMENTATION ANWEISUNGEN FÜR COPILOT

### Phase 1: Datenbank-Setup
1. Erstelle alle Entity-Klassen im `Domain/Entities` Ordner
2. Implementiere den `CommunityDbContext` mit allen DbSets und Konfigurationen
3. Füge DbSets zu deinem bestehenden `ApplicationDbContext` hinzu
4. Erstelle Migration: `Add-Migration AddCommunitySystem`
5. Aktualisiere Datenbank: `Update-Database`

### Phase 2: Repository-Layer
1. Implementiere das generische `Repository<T>` als Basisklasse
2. Erstelle alle spezifischen Repository-Implementierungen
3. Füge Dependency Injection in `Program.cs` hinzu:
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

### Phase 3: Service-Layer
1. Implementiere alle Service-Interfaces
2. Füge Business-Logik für Permissions hinzu
3. Implementiere Activity-Logging in allen kritischen Operationen
4. **WICHTIG**: Integriere SignalR-Broadcasts in alle Services:
```csharp
public class CommunityService : ICommunityService
{
    private readonly ICommunityRepository _repository;
    private readonly ICommunitySignalRService _signalR;
    
    public CommunityService(
        ICommunityRepository repository,
        ICommunitySignalRService signalR)
    {
        _repository = repository;
        _signalR = signalR;
    }
    
    public async Task<CommunityMember> JoinCommunityAsync(string communityId, string userId)
    {
        var member = await _repository.AddMemberAsync(communityId, userId);
        
        // SignalR Broadcast
        await _signalR.BroadcastMemberJoinedAsync(communityId, MapToDto(member));
        
        return member;
    }
}
```
5. Füge Services zur DI hinzu

### Phase 4: SignalR Setup
1. Erstelle `CommunityHub.cs` im `Hubs` Ordner
2. Implementiere `ICommunitySignalRService` und `CommunitySignalRService`
3. Füge SignalR in `Program.cs` hinzu:
```csharp
// SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<ICommunitySignalRService, CommunitySignalRService>();

// Nach app.Build()
app.MapHub<CommunityHub>("/hubs/community");
```

### Phase 5: MudBlazor Components
1. Installiere MudBlazor (falls nicht vorhanden):
```bash
dotnet add package MudBlazor
```
2. Konfiguriere MudBlazor in `Program.cs`:
```csharp
builder.Services.AddMudServices();
```
3. Füge MudBlazor zu `_Imports.razor` hinzu:
```razor
@using MudBlazor
```
4. Erstelle Shared Components:
   - `CommunityCard.razor`
   - `GroupCard.razor`
   - `MemberCard.razor`
   - `EventCard.razor`
   - `CraftRequestCard.razor`
5. Implementiere Pages für jede Hauptfunktion (siehe Beispiele oben)
6. Implementiere SignalR-Integration in jedem Component:
```csharp
private HubConnection? hubConnection;

protected override async Task OnInitializedAsync()
{
    await InitializeSignalR();
}

private async Task InitializeSignalR()
{
    hubConnection = new HubConnectionBuilder()
        .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/community"))
        .WithAutomaticReconnect()
        .Build();
    
    // Event Handlers registrieren
    hubConnection.On<DataDto>("EventName", HandleEvent);
    
    await hubConnection.StartAsync();
    await hubConnection.InvokeAsync("JoinCommunityChannel", CommunityId);
}

public async ValueTask DisposeAsync()
{
    if (hubConnection != null)
    {
        await hubConnection.InvokeAsync("LeaveCommunityChannel", CommunityId);
        await hubConnection.DisposeAsync();
    }
}
```

### Phase 6: API-Endpoints (Optional für externe Zugriffe)
1. Erstelle Controller für jede Hauptentität
2. Implementiere DTOs für Request/Response
3. Füge AutoMapper-Profile hinzu für Entity↔DTO Mapping
4. Implementiere Validierung mit FluentValidation
5. Füge Authorization hinzu mit `[Authorize]` Attributes

### Phase 7: Testing & Optimization
1. Teste SignalR-Verbindungen
2. Teste Real-Time Updates in mehreren Browser-Tabs
3. Implementiere Reconnection-Logic
4. Füge Error-Handling für SignalR hinzu
5. Performance-Testing mit mehreren gleichzeitigen Usern

### Phase 8: Zusätzliche Features
1. Implementiere Benachrichtigungssystem (Toast-Notifications mit MudBlazor)
2. Füge Discord-Integration hinzu (Webhooks)
3. Implementiere Export-Funktionen (CSV, Excel)
4. Füge Statistiken und Dashboards hinzu
5. Implementiere Mobile-Responsive Design (MudBlazor ist bereits responsive)

---

## 10. MUDBLAZOR STYLING CONVENTIONS
1. Erstelle alle Entity-Klassen im `Domain/Entities` Ordner
2. Implementiere den `CommunityDbContext` mit allen DbSets und Konfigurationen
3. Erstelle Migration: `Add-Migration InitialCreate`
4. Aktualisiere Datenbank: `Update-Database`

### Phase 2: Repository-Layer
1. Implementiere das generische `Repository<T>` als Basisklasse
2. Erstelle alle spezifischen Repository-Implementierungen
3. Füge Dependency Injection in `Program.cs` hinzu:
```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
// ... weitere Repositories
```

### Phase 3: Service-Layer
1. Implementiere alle Service-Interfaces
2. Füge Business-Logik für Permissions hinzu
3. Implementiere Activity-Logging in allen kritischen Operationen
4. Füge Services zur DI hinzu

### Phase 4: API-Endpoints (wenn benötigt)
1. Erstelle Controller für jede Hauptentität
2. Implementiere DTOs für Request/Response
3. Füge AutoMapper-Profile hinzu für Entity↔DTO Mapping
4. Implementiere Validierung mit FluentValidation

### Phase 5: Blazor Components
1. Erstelle Shared Components (CommunityCard, GroupCard, MemberCard, etc.)
2. Implementiere Pages für jede Hauptfunktion
3. Füge State Management hinzu (z.B. mit Fluxor oder einfachem Service)
4. Implementiere Authorization mit `AuthorizeView`

### Phase 6: Zusätzliche Features
1. Implementiere Benachrichtigungssystem (SignalR)
2. Füge Discord-Integration hinzu (Webhooks)
3. Implementiere Export-Funktionen (CSV, Excel)
4. Füge Statistiken und Dashboards hinzu

---

## 10. MUDBLAZOR STYLING CONVENTIONS

### Standard-Komponenten verwenden:
```razor
<!-- Cards für Container -->
<MudCard Elevation="2">
    <MudCardHeader>...</MudCardHeader>
    <MudCardContent>...</MudCardContent>
    <MudCardActions>...</MudCardActions>
</MudCard>

<!-- Dialogs für Popups -->
<MudDialog @bind-IsVisible="showDialog">
    <DialogContent>...</DialogContent>
    <DialogActions>...</DialogActions>
</MudDialog>

<!-- Tables für Listen -->
<MudTable Items="@items" Hover="true" Breakpoint="Breakpoint.Sm">
    <HeaderContent>
        <MudTh>Name</MudTh>
        <MudTh>Actions</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.Name</MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Edit" />
        </MudTd>
    </RowTemplate>
</MudTable>

<!-- Snackbar für Notifications -->
@inject ISnackbar Snackbar
Snackbar.Add("Erfolgreich gespeichert!", Severity.Success);
```

### Farb-Schema:
- **Primary**: Haupt-Aktionen (Erstellen, Speichern)
- **Secondary**: Sekundäre Aktionen
- **Success**: Erfolgreiche Operationen (Grün)
- **Error**: Fehler und Löschungen (Rot)
- **Warning**: Warnungen (Orange)
- **Info**: Informationen (Blau)

### Spacing & Layout:
```razor
<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-4 mb-4">
    <MudGrid Spacing="3">
        <MudItem xs="12" sm="6" md="4">...</MudItem>
    </MudGrid>
</MudContainer>
```

---

## 11. SIGNALR BEST PRACTICES MIT CASCADING VALUE

### ✅ BESSERER ANSATZ: CascadingValue für Community-weite HubConnection

Statt in jedem Component eine HubConnection zu erstellen, nutzen wir **CascadingValue** um die Connection community-weit zu teilen:

### 11.1 CommunityConnectionProvider (Wrapper Component)
```csharp
// Components/Community/CommunityConnectionProvider.razor
@implements IAsyncDisposable

<CascadingValue Value="this">
    @ChildContent
</CascadingValue>

@code {
    [Parameter] public string CommunityId { get; set; } = string.Empty;
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILogger<CommunityConnectionProvider> Logger { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    
    public HubConnection? HubConnection { get; private set; }
    public bool IsConnected => HubConnection?.State == HubConnectionState.Connected;
    
    // Events die Child-Components abonnieren können
    public event Action<CommunityMemberDto>? OnMemberJoined;
    public event Action<string, string>? OnMemberLeft;
    public event Action<string, RankDto>? OnRankChanged;
    public event Action<GroupDto>? OnGroupCreated;
    public event Action<GroupDto>? OnGroupUpdated;
    public event Action<string>? OnGroupDeleted;
    public event Action<EventDto>? OnEventCreated;
    public event Action<EventDto>? OnEventUpdated;
    public event Action<string, EventParticipantDto>? OnEventParticipantJoined;
    public event Action<GroupGoalDto>? OnGoalCreated;
    public event Action<string, int, int>? OnGoalProgressUpdated;
    public event Action<GroupGoalDto>? OnGoalCompleted;
    public event Action<CraftRequestDto>? OnCraftRequestCreated;
    public event Action<string, string>? OnCraftRequestFulfilled;
    public event Action<AnnouncementDto>? OnAnnouncementPosted;
    public event Action<string, bool>? OnOnlineStatusChanged;
    
    protected override async Task OnInitializedAsync()
    {
        await InitializeSignalR();
    }
    
    protected override async Task OnParametersSetAsync()
    {
        // Wenn CommunityId sich ändert, Channel wechseln
        if (HubConnection?.State == HubConnectionState.Connected)
        {
            await SwitchCommunityChannel(CommunityId);
        }
    }
    
    private async Task InitializeSignalR()
    {
        try
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/community"))
                .WithAutomaticReconnect(new[] { 
                    TimeSpan.Zero, 
                    TimeSpan.FromSeconds(2), 
                    TimeSpan.FromSeconds(10), 
                    TimeSpan.FromSeconds(30) 
                })
                .Build();
            
            // Event Handler registrieren
            RegisterEventHandlers();
            
            // Connection Events
            HubConnection.Reconnecting += OnReconnecting;
            HubConnection.Reconnected += OnReconnected;
            HubConnection.Closed += OnClosed;
            
            await HubConnection.StartAsync();
            await HubConnection.InvokeAsync("JoinCommunityChannel", CommunityId);
            
            Logger.LogInformation($"SignalR connected to community {CommunityId}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize SignalR");
            Snackbar.Add("Verbindung zum Server fehlgeschlagen", Severity.Error);
        }
    }
    
    private void RegisterEventHandlers()
    {
        if (HubConnection == null) return;
        
        // Member Events
        HubConnection.On<CommunityMemberDto>("MemberJoined", member =>
        {
            OnMemberJoined?.Invoke(member);
            Snackbar.Add($"{member.InGameName} ist beigetreten", Severity.Info);
        });
        
        HubConnection.On<string, string>("MemberLeft", (memberId, reason) =>
        {
            OnMemberLeft?.Invoke(memberId, reason);
        });
        
        HubConnection.On<string, RankDto>("RankChanged", (memberId, rank) =>
        {
            OnRankChanged?.Invoke(memberId, rank);
        });
        
        // Group Events
        HubConnection.On<GroupDto>("GroupCreated", group =>
        {
            OnGroupCreated?.Invoke(group);
            Snackbar.Add($"Neue Gruppe: {group.Name}", Severity.Info);
        });
        
        HubConnection.On<GroupDto>("GroupUpdated", group =>
        {
            OnGroupUpdated?.Invoke(group);
        });
        
        HubConnection.On<string>("GroupDeleted", groupId =>
        {
            OnGroupDeleted?.Invoke(groupId);
            Snackbar.Add("Gruppe wurde gelöscht", Severity.Warning);
        });
        
        // Event Events
        HubConnection.On<EventDto>("EventCreated", evt =>
        {
            OnEventCreated?.Invoke(evt);
            Snackbar.Add($"Neues Event: {evt.Title}", Severity.Info);
        });
        
        HubConnection.On<EventDto>("EventUpdated", evt =>
        {
            OnEventUpdated?.Invoke(evt);
        });
        
        HubConnection.On<string, EventParticipantDto>("EventParticipantJoined", 
            (eventId, participant) =>
        {
            OnEventParticipantJoined?.Invoke(eventId, participant);
        });
        
        // Goal Events
        HubConnection.On<GroupGoalDto>("GoalCreated", goal =>
        {
            OnGoalCreated?.Invoke(goal);
        });
        
        HubConnection.On<string, int, int>("GoalProgressUpdated", 
            (goalId, progress, target) =>
        {
            OnGoalProgressUpdated?.Invoke(goalId, progress, target);
        });
        
        HubConnection.On<GroupGoalDto>("GoalCompleted", goal =>
        {
            OnGoalCompleted?.Invoke(goal);
            Snackbar.Add($"Ziel erreicht: {goal.Title}", Severity.Success);
        });
        
        // Craft Events
        HubConnection.On<CraftRequestDto>("CraftRequestCreated", request =>
        {
            OnCraftRequestCreated?.Invoke(request);
            Snackbar.Add($"Neue Anfrage: {request.Craft.Name}", Severity.Info);
        });
        
        HubConnection.On<string, string>("CraftRequestFulfilled", 
            (requestId, fulfillerName) =>
        {
            OnCraftRequestFulfilled?.Invoke(requestId, fulfillerName);
        });
        
        // Announcement Events
        HubConnection.On<AnnouncementDto>("AnnouncementPosted", announcement =>
        {
            OnAnnouncementPosted?.Invoke(announcement);
            if (announcement.IsImportant)
            {
                Snackbar.Add(announcement.Title, Severity.Warning);
            }
        });
        
        // Online Status
        HubConnection.On<string, bool>("OnlineStatusChanged", (memberId, isOnline) =>
        {
            OnOnlineStatusChanged?.Invoke(memberId, isOnline);
        });
    }
    
    private async Task SwitchCommunityChannel(string newCommunityId)
    {
        if (HubConnection == null || string.IsNullOrEmpty(CommunityId)) return;
        
        try
        {
            // Leave old channel
            await HubConnection.InvokeAsync("LeaveCommunityChannel", CommunityId);
            
            // Join new channel
            await HubConnection.InvokeAsync("JoinCommunityChannel", newCommunityId);
            
            Logger.LogInformation($"Switched from {CommunityId} to {newCommunityId}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to switch community channel");
        }
    }
    
    // Public methods für Child-Components
    public async Task JoinGroupChannel(string groupId)
    {
        if (HubConnection?.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("JoinGroupChannel", groupId);
        }
    }
    
    public async Task LeaveGroupChannel(string groupId)
    {
        if (HubConnection?.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("LeaveGroupChannel", groupId);
        }
    }
    
    public async Task SendTypingIndicator(string userName)
    {
        if (HubConnection?.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("UserTyping", CommunityId, userName);
        }
    }
    
    // Connection Events
    private Task OnReconnecting(Exception? error)
    {
        Snackbar.Add("Verbindung unterbrochen, versuche neu zu verbinden...", 
            Severity.Warning);
        return Task.CompletedTask;
    }
    
    private async Task OnReconnected(string? connectionId)
    {
        Snackbar.Add("Verbindung wiederhergestellt!", Severity.Success);
        
        // Rejoin community channel nach Reconnect
        if (!string.IsNullOrEmpty(CommunityId))
        {
            await HubConnection!.InvokeAsync("JoinCommunityChannel", CommunityId);
        }
    }
    
    private Task OnClosed(Exception? error)
    {
        Snackbar.Add("Verbindung verloren. Bitte Seite neu laden.", Severity.Error);
        return Task.CompletedTask;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (HubConnection != null)
        {
            try
            {
                if (HubConnection.State == HubConnectionState.Connected)
                {
                    await HubConnection.InvokeAsync("LeaveCommunityChannel", CommunityId);
                }
                
                await HubConnection.DisposeAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error disposing SignalR connection");
            }
        }
    }
}
```

### 11.2 Community Layout mit Provider
```razor
// Pages/Community/CommunityLayout.razor
@page "/community/{CommunityId}"
@layout MainLayout

<CommunityConnectionProvider CommunityId="@CommunityId">
    <MudContainer MaxWidth="MaxWidth.ExtraLarge">
        <MudTabs Elevation="2" Rounded="true" ApplyEffectsToContainer="true">
            <MudTabPanel Text="Übersicht" Icon="@Icons.Material.Filled.Dashboard">
                <CommunityDashboard CommunityId="@CommunityId" />
            </MudTabPanel>
            
            <MudTabPanel Text="Mitglieder" Icon="@Icons.Material.Filled.People">
                <CommunityMembers CommunityId="@CommunityId" />
            </MudTabPanel>
            
            <MudTabPanel Text="Gruppen" Icon="@Icons.Material.Filled.Groups">
                <CommunityGroups CommunityId="@CommunityId" />
            </MudTabPanel>
            
            <MudTabPanel Text="Events" Icon="@Icons.Material.Filled.Event">
                <CommunityEvents CommunityId="@CommunityId" />
            </MudTabPanel>
            
            <MudTabPanel Text="Crafts" Icon="@Icons.Material.Filled.Build">
                <CommunityCrafts CommunityId="@CommunityId" />
            </MudTabPanel>
            
            <MudTabPanel Text="Ankündigungen" Icon="@Icons.Material.Filled.Announcement">
                <CommunityAnnouncements CommunityId="@CommunityId" />
            </MudTabPanel>
        </MudTabs>
    </MudContainer>
</CommunityConnectionProvider>

@code {
    [Parameter] public string CommunityId { get; set; } = string.Empty;
}
```

### 11.3 Child Component Beispiel (vereinfacht!)
```razor
// Components/Community/CommunityGroups.razor
@inject IGroupService GroupService

<MudText Typo="Typo.h4" Class="mb-4">Gruppen</MudText>

<MudButton Variant="Variant.Filled" 
           Color="Color.Primary" 
           OnClick="ShowCreateDialog">
    Neue Gruppe erstellen
</MudButton>

<MudGrid Class="mt-4">
    @foreach (var group in groups)
    {
        <MudItem xs="12" sm="6" md="4">
            <MudCard>
                <MudCardContent>
                    <MudText Typo="Typo.h6">@group.Name</MudText>
                    <MudText Typo="Typo.body2">@group.Type</MudText>
                </MudCardContent>
            </MudCard>
        </MudItem>
    }
</MudGrid>

@code {
    [Parameter] public string CommunityId { get; set; } = string.Empty;
    
    // ✅ Cascading Value empfangen
    [CascadingParameter] 
    public CommunityConnectionProvider ConnectionProvider { get; set; } = null!;
    
    private List<GroupDto> groups = new();
    
    protected override async Task OnInitializedAsync()
    {
        groups = (await GroupService.GetCommunityGroupsAsync(CommunityId)).ToList();
        
        // ✅ Events vom Provider abonnieren (statt eigene HubConnection)
        ConnectionProvider.OnGroupCreated += HandleGroupCreated;
        ConnectionProvider.OnGroupUpdated += HandleGroupUpdated;
        ConnectionProvider.OnGroupDeleted += HandleGroupDeleted;
    }
    
    // ✅ Event Handler
    private void HandleGroupCreated(GroupDto group)
    {
        groups.Add(group);
        InvokeAsync(StateHasChanged);
    }
    
    private void HandleGroupUpdated(GroupDto group)
    {
        var existing = groups.FirstOrDefault(g => g.Id == group.Id);
        if (existing != null)
        {
            var index = groups.IndexOf(existing);
            groups[index] = group;
            InvokeAsync(StateHasChanged);
        }
    }
    
    private void HandleGroupDeleted(string groupId)
    {
        groups.RemoveAll(g => g.Id == groupId);
        InvokeAsync(StateHasChanged);
    }
    
    // ✅ Cleanup
    public void Dispose()
    {
        ConnectionProvider.OnGroupCreated -= HandleGroupCreated;
        ConnectionProvider.OnGroupUpdated -= HandleGroupUpdated;
        ConnectionProvider.OnGroupDeleted -= HandleGroupDeleted;
    }
}
```

### 11.4 Vorteile dieses Ansatzes:

✅ **Eine einzige HubConnection** pro Community
✅ **Automatisches Reconnect-Handling** zentral
✅ **Weniger Code** in Child-Components
✅ **Bessere Performance** (weniger Connections)
✅ **Einfacheres State-Management**
✅ **Snackbar-Notifications** zentral verwaltet
✅ **Channel-Switching** beim Wechseln zwischen Communities
✅ **Kein IAsyncDisposable** in jedem Component

### 11.5 Event-Spezifische Channels (Optional)
```csharp
// Für spezielle Components die nur bestimmte Updates brauchen
public class GroupDetailComponent : ComponentBase, IDisposable
{
    [Parameter] public string GroupId { get; set; } = string.Empty;
    [CascadingParameter] public CommunityConnectionProvider Provider { get; set; } = null!;
    
    protected override async Task OnInitializedAsync()
    {
        // Join group-specific channel
        await Provider.JoinGroupChannel(GroupId);
        
        // Subscribe zu relevanten Events
        Provider.OnGoalCreated += HandleGoalCreated;
        Provider.OnGoalProgressUpdated += HandleGoalProgress;
    }
    
    public void Dispose()
    {
        // Leave channel & unsubscribe
        _ = Provider.LeaveGroupChannel(GroupId);
        Provider.OnGoalCreated -= HandleGoalCreated;
        Provider.OnGoalProgressUpdated -= HandleGoalProgress;
    }
}
```

---

## 12. PERFORMANCE-ÜBERLEGUNGEN
```csharp
// Automatische Reconnection
hubConnection = new HubConnectionBuilder()
    .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/community"))
    .WithAutomaticReconnect(new[] { 
        TimeSpan.Zero, 
        TimeSpan.FromSeconds(2), 
        TimeSpan.FromSeconds(10), 
        TimeSpan.FromSeconds(30) 
    })
    .Build();

// Reconnection Events
hubConnection.Reconnecting += error =>
{
    Snackbar.Add("Verbindung unterbrochen, versuche neu zu verbinden...", Severity.Warning);
    return Task.CompletedTask;
};

hubConnection.Reconnected += connectionId =>
{
    Snackbar.Add("Verbindung wiederhergestellt!", Severity.Success);
    return Task.CompletedTask;
};

hubConnection.Closed += error =>
{
    Snackbar.Add("Verbindung verloren. Bitte Seite neu laden.", Severity.Error);
    return Task.CompletedTask;
};
```

### Error Handling:
```csharp
try
{
    await hubConnection.StartAsync();
    await hubConnection.InvokeAsync("JoinCommunityChannel", CommunityId);
}
catch (Exception ex)
{
    _logger.LogError(ex, "SignalR connection failed");
    Snackbar.Add("Verbindung fehlgeschlagen", Severity.Error);
}
```

### Optimistic UI Updates:
```csharp
// Sofort UI updaten, dann Server-Call
private async Task CreateGroup()
{
    var tempId = Guid.NewGuid().ToString();
    var group = new GroupDto { Id = tempId, Name = newGroupName };
    
    // UI sofort updaten
    groups.Add(group);
    StateHasChanged();
    
    try
    {
        // Server Call
        var created = await GroupService.CreateGroupAsync(CommunityId, group);
        
        // Ersetze temp mit echten Daten
        var index = groups.FindIndex(g => g.Id == tempId);
        groups[index] = created;
    }
    catch
    {
        // Rollback bei Fehler
        groups.RemoveAll(g => g.Id == tempId);
        Snackbar.Add("Fehler beim Erstellen", Severity.Error);
    }
    
    StateHasChanged();
}
```

### Throttling für häufige Updates:
```csharp
private DateTime _lastUpdate = DateTime.MinValue;
private const int UPDATE_THROTTLE_MS = 500;

hubConnection.On<int, int>("GoalProgressUpdated", (goalId, progress) =>
{
    var now = DateTime.UtcNow;
    if ((now - _lastUpdate).TotalMilliseconds < UPDATE_THROTTLE_MS)
        return; // Skip zu häufige Updates
    
    _lastUpdate = now;
    UpdateGoalProgress(goalId, progress);
    InvokeAsync(StateHasChanged);
});
```

---

## 12. PERFORMANCE-ÜBERLEGUNGEN

### Indizierung
- Alle Foreign Keys sind indiziert
- Composite Indexes für häufige Abfragen (CommunityId + Status, etc.)
- Covering Indexes für Read-Heavy Queries

### Caching-Strategie
```csharp
// Verwende IMemoryCache für häufig abgefragte Daten:
- Community-Stammdaten (5 Min TTL)
- Rank-Definitionen (10 Min TTL)
- Craft-Definitionen (15 Min TTL)
- Member-Permissions (2 Min TTL)
```

### Pagination
```csharp
public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize)
{
    var total = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<T>(items, total, page, pageSize);
}
```

### Eager Loading nur wenn nötig
```csharp
// VERMEIDEN: 
var groups = await _context.Groups
    .Include(g => g.Members)
    .Include(g => g.Goals)
    .ToListAsync();

// BESSER: Separate Queries oder Select/Projection
var groups = await _context.Groups
    .Select(g => new GroupDto 
    { 
        /* nur benötigte Felder */ 
    })
    .ToListAsync();
```

---

## 9. SICHERHEIT

### Authorization
```csharp
// Implementiere Custom Authorization Handler
public class CommunityPermissionHandler : AuthorizationHandler<CommunityPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CommunityPermissionRequirement requirement)
    {
        var userId = context.User.GetUserId();
        var communityId = GetCommunityIdFromContext(context);
        
        var hasPermission = await _communityService
            .UserHasPermissionAsync(communityId, userId, requirement.Permission);
        
        if (hasPermission)
            context.Succeed(requirement);
    }
}
```

### Input-Validierung
```csharp
public class CreateCommunityValidator : AbstractValidator<CreateCommunityRequest>
{
    public CreateCommunityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-zA-Z0-9 -]+$");
        
        RuleFor(x => x.Description)
            .MaximumLength(2000);
        
        RuleFor(x => x.MaxMembers)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10000);
    }
}
```

---

## 10. TESTING-HINWEISE

### Unit Tests
```csharp
public class CommunityServiceTests
{
    [Fact]
    public async Task CreateCommunity_ValidData_ReturnsNewCommunity()
    {
        // Arrange
        var mockRepo = new Mock<ICommunityRepository>();
        var service = new CommunityService(mockRepo.Object);
        
        // Act
        var result = await service.CreateCommunityAsync(
            userId, "Test Community", "Test Game", "Description");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Community", result.Name);
    }
}
```

### Integration Tests
```csharp
public class CommunityIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetCommunity_ExistingId_ReturnsOk()
    {
        // Test mit echter Datenbank und API
    }
}
```

---

## 11. ERWEITERUNGSMÖGLICHKEITEN

### Zukünftige Features
1. **Achievements-System**: Achievements für Communities und Mitglieder
2. **Punkte-System**: Contribution Points für Aktivitäten
3. **Shop/Economy**: Virtuelles Wirtschaftssystem
4. **Allianzen**: Beziehungen zwischen Communities
5. **Turniere**: Tournament-Bracket-System
6. **Chat-System**: Integrierter Chat
7. **Voice-Integration**: Discord/TeamSpeak-Integration
8. **Mobile App**: Xamarin/MAUI App
9. **Analytics**: Detaillierte Statistiken und Reports
10. **API für Drittanbieter**: Public API für externe Tools

---

## 12. DEPLOYMENT-HINWEISE

### Datenbank-Migrations-Strategie
```bash
# Development
dotnet ef migrations add MigrationName
dotnet ef database update

# Production
dotnet ef migrations script --idempotent > migration.sql
# SQL manuell auf Produktions-DB ausführen
```

### App-Settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=CommunitySystem;..."
  },
  "Community": {
    "MaxCommunitiesPerUser": 10,
    "MaxGroupsPerCommunity": 50,
    "MaxMembersDefault": 500,
    "EnableDiscordIntegration": true
  }
}
```

---

## ZUSAMMENFASSUNG

Dieses System bietet:
✅ Vollständiges Community-Management
✅ Flexibles Rang- und Permissions-System
✅ Gruppen mit Führungsstrukturen
✅ Ziel-Tracking für Raids/Events
✅ Craft-System mit Request-Mechanismus ("Wer kann was herstellen?")
✅ Event-Management mit Teilnehmerlisten
✅ Activity-Logging für Audit Trail
✅ Skalierbare Architektur ohne virtuelle Navigation Properties
✅ Klare Trennung zwischen Domain, Services und Präsentation
✅ Bereit für Blazor WebAssembly oder Server

**Nächste Schritte:**
1. Projekt-Struktur erstellen
2. Entities implementieren
3. DbContext konfigurieren
4. Repositories implementieren
5. Services implementieren
6. Blazor UI erstellen

Viel Erfolg bei der Implementierung! 🚀