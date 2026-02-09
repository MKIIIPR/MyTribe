namespace Tribe.Bib.Models.CommunityManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Tribe.Bib.Models.TribeRelated;

    // ============================================
    // ENUMS (anstelle von statischen Konstanten)
    // ============================================

    // ORGANIZATION TYPES
    public enum OrganizationType
    {
        Guild = 0,
        Clan = 1,
        Company = 2,
        Alliance = 3,
        Community = 4,
        Organization = 5
    }

    // MEMBER STATUSES
    public enum MemberStatus
    {
        Active = 0,
        Inactive = 1,
        Trial = 2,
        OnLeave = 3,
        Banned = 4,
        Pending = 5
    }

    // GROUP TYPES
    public enum GroupType
    {
        Raid = 0,
        PvP = 1,
        Crafting = 2,
        Social = 3,
        Training = 4,
        EventTeam = 5,
        Leadership = 6,
        Custom = 99
    }

    // EVENT TYPES
    public enum EventType
    {
        Raid = 0,
        PvP = 1,
        Meeting = 2,
        Training = 3,
        Social = 4,
        Tournament = 5,
        Custom = 99
    }

    // EVENT STATUS
    public enum EventStatus
    {
        Draft = 0,
        Scheduled = 1,
        Ongoing = 2,
        Completed = 3,
        Cancelled = 4
    }

    // PARTICIPATION STATUS
    public enum ParticipationStatus
    {
        Accepted = 0,
        Maybe = 1,
        Declined = 2,
        Waitlist = 3,
        NoResponse = 4
    }

    // CHAT TYPES
    public enum ChatType
    {
        Organization = 0,
        Group = 1,
        Direct = 2,
        Announcement = 3
    }

    // MESSAGE TYPES
    public enum MessageType
    {
        Text = 0,
        Image = 1,
        File = 2,
        Link = 3,
        Announcement = 4,
        System = 5
    }

    // BLOG POST TYPES
    public enum BlogPostType
    {
        News = 0,
        Guide = 1,
        Strategy = 2,
        Lore = 3,
        Announcement = 4,
        Discussion = 5,
        Recruitment = 6
    }

    // BLOG POST STATUS
    public enum BlogPostStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }

    // ASSET CATEGORIES
    public enum AssetCategory
    {
        Ship = 0,
        Vehicle = 1,
        CharacterClass = 2,
        Profession = 3,
        CraftingSkill = 4,
        Equipment = 5,
        Resource = 6,
        Custom = 99
    }

    // APPLICATION STATUS
    public enum ApplicationStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Withdrawn = 3
    }

    // ============================================
    // BASE ENTITY
    // ============================================
    public abstract class EntityBase
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

    // ============================================
    // ORGANIZATION CORE
    // ============================================
    public class Organization : EntityBase
    {
        // BASIC INFO
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Tag { get; set; } // Short tag like [TAG]

        // VISUALS
        [MaxLength(500)]
        public string? BannerUrl { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        // TYPE & SETTINGS
        [Required]
        public OrganizationType OrganizationType { get; set; } = OrganizationType.Guild;

        public string? SettingsJson { get; set; } // Custom settings per organization

        // CREATOR/MANAGEMENT
        [Required]
        public string CreatorUserId { get; set; } // References TribeUser.Id

        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = true; // Can be discovered by others
        public bool AcceptingApplications { get; set; } = true;

        // match PK length of GameProfile.Id (nvarchar(450)) to avoid FK size mismatch
        [MaxLength(450)]
        public string? GameProfileId { get; set; }

        public virtual GameProfile? GameProfile { get; set; }

        // STATISTICS
        public int MemberCount { get; set; } = 0;
        public int GroupCount { get; set; } = 0;
        public int EventCount { get; set; } = 0;

        // NAVIGATION PROPERTIES
        public virtual ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
        public virtual ICollection<OrganizationRole> Roles { get; set; } = new List<OrganizationRole>();
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<OrganizationChat> Chats { get; set; } = new List<OrganizationChat>();
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public virtual ICollection<OrganizationAsset> Assets { get; set; } = new List<OrganizationAsset>();
        public virtual ICollection<OrganizationTerminology> Terminologies { get; set; } = new List<OrganizationTerminology>();
    }

    // ============================================
    // CUSTOM TERMINOLOGY (for custom role/group names)
    // ============================================
    public class OrganizationTerminology : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty; // e.g., "GuildMaster", "Officer", "Member"

        [Required]
        [MaxLength(100)]
        public string Value { get; set; } = string.Empty; // Custom display name

        [MaxLength(500)]
        public string? Description { get; set; }

        public virtual Organization Organization { get; set; } = null!;
    }

    // ============================================
    // ROLES & PERMISSIONS
    // ============================================
    public class OrganizationRole : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // HIERARCHY
        public int Priority { get; set; } = 0; // Higher = more power
        public bool IsDefaultRole { get; set; } = false; // Assigned to new members
        public bool IsOfficerRole { get; set; } = false; // Special officer designation

        // PERMISSIONS
        public string? PermissionsJson { get; set; } // JSON array of permission strings

        // NAVIGATION
        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
        public virtual ICollection<GroupOfficer> GroupOfficers { get; set; } = new List<GroupOfficer>();
    }

    // ============================================
    // MEMBERSHIP
    // ============================================
    public class OrganizationMember : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string UserId { get; set; } // References TribeUser.Id

        [Required]
        public string RoleId { get; set; } // References OrganizationRole.Id

        // STATUS & INFO
        [Required]
        public MemberStatus Status { get; set; } = MemberStatus.Active;

        [MaxLength(500)]
        public string? Notes { get; set; } // Officer notes about member

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActive { get; set; }
        public DateTime? InactiveSince { get; set; }
        public DateTime? LeaveDate { get; set; }

        // CONTRIBUTION TRACKING
        public int AttendanceCount { get; set; } = 0;
        public int EventOrganizedCount { get; set; } = 0;
        public int ContributionScore { get; set; } = 0;

        // NAVIGATION
        public virtual Organization Organization { get; set; } = null!;
        public virtual OrganizationRole Role { get; set; } = null!;
        public virtual ICollection<MemberAsset> Assets { get; set; } = new List<MemberAsset>();
        public virtual ICollection<EventParticipation> EventParticipations { get; set; } = new List<EventParticipation>();
        public virtual ICollection<GroupMembership> GroupMemberships { get; set; } = new List<GroupMembership>();
    }

    // ============================================
    // GROUPS (sub-divisions within organization)
    // ============================================
    public class Group : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public GroupType GroupType { get; set; } = GroupType.Custom;

        [Required]
        public string LeaderId { get; set; } = string.Empty;

        public string? DeputyLeaderId { get; set; }

        // VISUALS
        [MaxLength(500)]
        public string? BannerUrl { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        // SETTINGS
        public bool IsPublic { get; set; } = false; // Visible to all org members
        public bool AcceptingMembers { get; set; } = true;
        public bool RequiresApproval { get; set; } = true;

        // STATISTICS
        public int MemberCount { get; set; } = 0;

        // NAVIGATION
        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<GroupMembership> Members { get; set; } = new List<GroupMembership>();
        public virtual ICollection<GroupOfficer> Officers { get; set; } = new List<GroupOfficer>();
        public virtual ICollection<GroupChat> Chats { get; set; } = new List<GroupChat>();
    }

    // GROUP MEMBERSHIP
    public class GroupMembership : EntityBase
    {
        [Required]
        public string GroupId { get; set; }

        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        public virtual Group Group { get; set; } = null!;
        public virtual OrganizationMember Member { get; set; } = null!;
    }

    // GROUP OFFICERS (multiple officers per group possible)
    public class GroupOfficer : EntityBase
    {
        [Required]
        public string GroupId { get; set; }

        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        [MaxLength(200)]
        public string? Title { get; set; } // Custom title like "Lead Strategist"

        public DateTime AppointedAt { get; set; } = DateTime.UtcNow;

        public virtual Group Group { get; set; } = null!;
        public virtual OrganizationMember Member { get; set; } = null!;
        public virtual OrganizationRole Role { get; set; } = null!; // For permission checking
    }

    // ============================================
    // CHAT SYSTEM
    // ============================================
    // ORGANIZATION CHAT
    public class OrganizationChat : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public ChatType ChatType { get; set; } = ChatType.Organization;

        // SETTINGS
        public bool IsPublic { get; set; } = true; // All members can join
        public bool IsAnnouncementOnly { get; set; } = false; // Only officers can post
        public bool AllowFiles { get; set; } = true;
        public bool AllowImages { get; set; } = true;

        // MODERATION
        public string? AllowedRoleIdsJson { get; set; } // JSON array of role IDs that can access
        public int MessageRetentionDays { get; set; } = 90;

        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    // GROUP CHAT
    public class GroupChat : EntityBase
    {
        [Required]
        public string GroupId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // SETTINGS
        public bool IsPublic { get; set; } = true; // All group members can access
        public bool AllowFiles { get; set; } = true;
        public bool AllowImages { get; set; } = true;

        public virtual Group Group { get; set; } = null!;
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    // CHAT MESSAGES
    public class ChatMessage : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        public string? OrganizationChatId { get; set; } // Null if group chat
        public string? GroupChatId { get; set; } // Null if org chat

        [Required]
        public string SenderMemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public MessageType MessageType { get; set; } = MessageType.Text;

        // FOR FILES/IMAGES
        [MaxLength(500)]
        public string? AttachmentUrl { get; set; }

        [MaxLength(200)]
        public string? FileName { get; set; }

        // MODERATION
        public bool IsPinned { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string? DeletedByMemberId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public int LikeCount { get; set; } = 0;

        public virtual Organization Organization { get; set; } = null!;
        public virtual OrganizationChat? OrganizationChat { get; set; }
        public virtual GroupChat? GroupChat { get; set; }
        public virtual OrganizationMember Sender { get; set; } = null!;
        public virtual ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
        public virtual ICollection<MessageReply> Replies { get; set; } = new List<MessageReply>();
    }

    // MESSAGE REACTIONS
    public class MessageReaction : EntityBase
    {
        [Required]
        public string MessageId { get; set; }

        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(50)]
        public string Emoji { get; set; } = string.Empty;

        public DateTime ReactedAt { get; set; } = DateTime.UtcNow;

        public virtual ChatMessage Message { get; set; } = null!;
        public virtual OrganizationMember Member { get; set; } = null!;
    }

    // MESSAGE REPLIES
    public class MessageReply : EntityBase
    {
        [Required]
        public string ParentMessageId { get; set; }

        [Required]
        public string ReplyMessageId { get; set; } // References ChatMessage.Id

        public virtual ChatMessage ParentMessage { get; set; } = null!;
        public virtual ChatMessage ReplyMessage { get; set; } = null!;
    }

    // ============================================
    // BLOG SYSTEM
    // ============================================
    public class BlogPost : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string AuthorMemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty; // HTML/Markdown content

        [Required]
        public BlogPostType PostType { get; set; } = BlogPostType.News;

        [Required]
        public BlogPostStatus Status { get; set; } = BlogPostStatus.Draft;

        // TARGETING
        public string? TargetGroupId { get; set; } // Null = org-wide
        public string? TargetRoleId { get; set; } // Null = all roles

        // VISUALS
        [MaxLength(500)]
        public string? FeaturedImageUrl { get; set; }

        [MaxLength(500)]
        public string? Summary { get; set; }

        // ENGAGEMENT
        public int ViewCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        public DateTime PublishedAt { get; set; }
        public DateTime? LastEditedAt { get; set; }

        public virtual Organization Organization { get; set; } = null!;
        public virtual OrganizationMember Author { get; set; } = null!;
        public virtual Group? TargetGroup { get; set; }
        public virtual ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();
        public virtual ICollection<BlogLike> Likes { get; set; } = new List<BlogLike>();
    }

    // BLOG COMMENTS
    public class BlogComment : EntityBase
    {
        [Required]
        public string BlogPostId { get; set; }

        [Required]
        public string AuthorMemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public string? ParentCommentId { get; set; } // For nested replies

        public bool IsDeleted { get; set; } = false;

        public virtual BlogPost BlogPost { get; set; } = null!;
        public virtual OrganizationMember Author { get; set; } = null!;
        public virtual BlogComment? ParentComment { get; set; }
        public virtual ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();
    }

    // BLOG LIKES
    public class BlogLike : EntityBase
    {
        [Required]
        public string BlogPostId { get; set; }

        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;

        public virtual BlogPost BlogPost { get; set; } = null!;
        public virtual OrganizationMember Member { get; set; } = null!;
    }

    // ============================================
    // EVENT SYSTEM
    // ============================================
    public class Event : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string OrganizerMemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public EventType EventType { get; set; } = EventType.Custom;

        // SCHEDULING
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        // TARGETING
        public string? TargetGroupId { get; set; } // Null = org-wide
        public string? RequiredRoleId { get; set; } // Null = no requirement

        // CAPACITY & ROSTER
        public int? MaxParticipants { get; set; } // Null = unlimited
        public int CurrentParticipantCount { get; set; } = 0;

        // REQUIREMENTS
        public string? RequirementsJson { get; set; } // Custom requirements (assets, roles, etc.)

        // STATUS
        [Required]
        public EventStatus Status { get; set; } = EventStatus.Scheduled;

        // REPEATING EVENTS
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; } // e.g., "FREQ=WEEKLY;BYDAY=MO,WE"

        // COMMUNICATION
        public string? DiscordInviteUrl { get; set; }
        public string? VoiceChannelInfo { get; set; }

        // STATISTICS
        public int ViewCount { get; set; } = 0;

        public virtual Organization Organization { get; set; } = null!;
        public virtual OrganizationMember Organizer { get; set; } = null!;
        public virtual Group? TargetGroup { get; set; }
        public virtual ICollection<EventParticipation> Participations { get; set; } = new List<EventParticipation>();
        public virtual ICollection<EventComment> Comments { get; set; } = new List<EventComment>();
    }

    // EVENT PARTICIPATION
    public class EventParticipation : EntityBase
    {
        [Required]
        public string EventId { get; set; }

        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        [Required]
        public string Status { get; set; } = Constants.ApplicationStatus.Pending    ;

        [MaxLength(500)]
        public string? Comment { get; set; } // Optional comment when responding

        public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastReminderSentAt { get; set; }

        // FOR ATTENDANCE TRACKING
        public bool Attended { get; set; } = false;
        public DateTime? AttendedAt { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual OrganizationMember Member { get; set; } = null!;
    }

    // EVENT COMMENTS
    public class EventComment : EntityBase
    {
        [Required]
        public string EventId { get; set; }

        [Required]
        public string AuthorMemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;

        public virtual Event Event { get; set; } = null!;
        public virtual OrganizationMember Author { get; set; } = null!;
    }

    // ============================================
    // ASSET MANAGEMENT SYSTEM (Generic)
    // ============================================
    // ORGANIZATION ASSETS (shared resources)
    public class OrganizationAsset : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public AssetCategory Category { get; set; } = AssetCategory.Custom;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // QUANTITY & AVAILABILITY
        public int Quantity { get; set; } = 1;
        public int AvailableQuantity { get; set; } = 1;

        // SPECIFICATIONS (JSON for flexible attributes)
        public string? SpecificationsJson { get; set; } // e.g., {"size": "Large", "capacity": 100}

        // LOCATION/USAGE
        [MaxLength(500)]
        public string? Location { get; set; }
        public string? Notes { get; set; }

        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<MemberAsset> AssignedToMembers { get; set; } = new List<MemberAsset>();
    }

    // MEMBER ASSETS (what each member owns/provides)
    public class MemberAsset : EntityBase
    {
        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public AssetCategory Category { get; set; } = AssetCategory.Custom;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // FOR SHIPS/VEHICLES
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? SizeClass { get; set; } // e.g., "Small", "Medium", "Large"

        // FOR SKILLS/PROFESSIONS
        public int? SkillLevel { get; set; } // 1-100 or null
        public string? Specialization { get; set; }

        // AVAILABILITY
        public bool IsAvailable { get; set; } = true;
        public string? AvailabilityNotes { get; set; } // e.g., "Available weekends only"

        // SPECIFICATIONS (JSON for game-specific attributes)
        public string? AttributesJson { get; set; }
        // Example for Star Citizen ship:
        // {"shipType": "Herald", "quantumSpeed": 0.25, "weapons": ["S2 Ballistic Gatling"]}
        // Example for WoW profession:
        // {"profession": "Blacksmithing", "specialization": "Weaponsmith", "recipes": 150}

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastVerifiedAt { get; set; }

        public virtual OrganizationMember Member { get; set; } = null!;
    }

    // ============================================
    // RECRUITMENT & APPLICATIONS
    // ============================================
    public class RecruitmentPost : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string AuthorMemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // TARGETING
        public string? TargetRole { get; set; } // e.g., "Tank", "Healer", "Pilot"
        public string? RequiredExperience { get; set; }
        public string? TimezonePreference { get; set; }

        // REQUIREMENTS
        public string? RequirementsJson { get; set; }

        [Required]
        public BlogPostStatus Status { get; set; } = BlogPostStatus.Published;

        public DateTime ExpiresAt { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public int ApplicationCount { get; set; } = 0;

        public virtual Organization Organization { get; set; } = null!;
        public virtual OrganizationMember Author { get; set; } = null!;
        public virtual ICollection<MemberApplication> Applications { get; set; } = new List<MemberApplication>();
    }

    public class MemberApplication : EntityBase
    {
        [Required]
        public string RecruitmentPostId { get; set; }

        [Required]
        public string ApplicantUserId { get; set; } // References TribeUser.Id (not yet a member)

        [Required]
        [MaxLength(100)]
        public string ApplicantName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = Constants.ApplicationStatus.Pending;

        // SCREENING QUESTIONS & ANSWERS
        public string? ScreeningAnswersJson { get; set; }

        // REVIEW
        public string? ReviewerMemberId { get; set; } // References OrganizationMember.Id
        public string? ReviewNotes { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public virtual RecruitmentPost RecruitmentPost { get; set; } = null!;
        public virtual OrganizationMember? Reviewer { get; set; }
    }

    // ============================================
    // NOTIFICATIONS
    // ============================================
    public class Notification : EntityBase
    {
        [Required]
        public string UserId { get; set; } // References TribeUser.Id

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "Info"; // Info, Warning, Event, Message, etc.

        // LINKS
        public string? LinkUrl { get; set; }
        public string? LinkType { get; set; } // Event, Message, BlogPost, etc.
        public string? LinkId { get; set; } // ID of the linked item

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }

    // ============================================
    // AUDIT LOG (for tracking changes)
    // ============================================
    public class AuditLog : EntityBase
    {
        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string MemberId { get; set; } // References OrganizationMember.Id

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // e.g., "MemberJoined", "RoleChanged"

        [Required]
        public string EntityType { get; set; } = string.Empty; // e.g., "Member", "Event", "Role"

        public string? EntityId { get; set; }
        public string? DetailsJson { get; set; } // Additional details about the action

        public DateTime ActionTime { get; set; } = DateTime.UtcNow;

        public virtual Organization Organization { get; set; } = null!;
        public virtual OrganizationMember Member { get; set; } = null!;
    }
}