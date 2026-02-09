using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;

namespace Tribe.Data
{
    public class OrgaDbContext : DbContext
    {
        public OrgaDbContext(DbContextOptions<OrgaDbContext> options) : base(options) { }

        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<OrganizationTerminology> OrganizationTerminologies { get; set; } = null!;
        public DbSet<OrganizationRole> OrganizationRoles { get; set; } = null!;
        public DbSet<OrganizationMember> OrganizationMembers { get; set; } = null!;
        public DbSet<GameProfile> GameProfiles { get; set; } = null!;

        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupMembership> GroupMemberships { get; set; } = null!;
        public DbSet<GroupOfficer> GroupOfficers { get; set; } = null!;

        public DbSet<OrganizationChat> OrganizationChats { get; set; } = null!;
        public DbSet<GroupChat> GroupChats { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<MessageReaction> MessageReactions { get; set; } = null!;
        public DbSet<MessageReply> MessageReplies { get; set; } = null!;

        public DbSet<BlogPost> BlogPosts { get; set; } = null!;
        public DbSet<BlogComment> BlogComments { get; set; } = null!;
        public DbSet<BlogLike> BlogLikes { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipation> EventParticipations { get; set; } = null!;
        public DbSet<EventComment> EventComments { get; set; } = null!;

        public DbSet<OrganizationAsset> OrganizationAssets { get; set; } = null!;
        public DbSet<MemberAsset> MemberAssets { get; set; } = null!;

        public DbSet<RecruitmentPost> RecruitmentPosts { get; set; } = null!;
        public DbSet<MemberApplication> MemberApplications { get; set; } = null!;

        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Basic keys and indexes
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CreatorUserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });


            modelBuilder.Entity<OrganizationRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.OrganizationId, e.Priority });
                entity.HasOne(r => r.Organization)
                      .WithMany(o => o.Roles)
                      .HasForeignKey(r => r.OrganizationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrganizationMember>()
                .HasOne(m => m.Organization)
                .WithMany(o => o.Members)
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.LeaderId);
                entity.HasOne(g => g.Organization)
                      .WithMany(o => o.Groups)
                      .HasForeignKey(g => g.OrganizationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GroupOfficer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.GroupId, e.MemberId });
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrganizationId);
                // Reactions and replies need explicit configuration because MessageReply references ChatMessage twice
                entity.HasMany(e => e.Reactions)
                      .WithOne(r => r.Message)
                      .HasForeignKey(r => r.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Replies)
                      .WithOne(r => r.ParentMessage)
                      .HasForeignKey(r => r.ParentMessageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessageReaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MessageId);
                entity.HasOne(e => e.Message)
                      .WithMany(m => m.Reactions)
                      .HasForeignKey(e => e.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MessageReply>(entity =>
            {
                entity.HasKey(e => e.Id);
                // ParentMessage -> collection of replies
                entity.HasOne(e => e.ParentMessage)
                      .WithMany(m => m.Replies)
                      .HasForeignKey(e => e.ParentMessageId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ReplyMessage is the message that is a reply; no collection navigation on ChatMessage
                entity.HasOne(e => e.ReplyMessage)
                      .WithMany()
                      .HasForeignKey(e => e.ReplyMessageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GameProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.GameKey).IsUnique();
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.OrganizationId, e.StartTime });
            });

            // leave other relationships to default conventions for now
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.MemberId);
                entity.HasOne(a => a.Organization)
                      .WithMany()
                      .HasForeignKey(a => a.OrganizationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

