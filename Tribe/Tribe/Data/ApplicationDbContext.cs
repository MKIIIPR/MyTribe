using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Tribe.Bib.Models.TribeRelated;

namespace Tribe.Data
{
    // Die Basis-Klasse 'ApplicationDbContext' bleibt unverändert.
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // === DbSet-Eigenschaften bleiben unverändert ===
        public DbSet<TribeProfile> TribeProfiles { get; set; }
        public DbSet<CreatorProfile> CreatorProfiles { get; set; }
        public DbSet<ProfileFollow> ProfileFollows { get; set; }
        public DbSet<CreatorPlan> CreatorPlans { get; set; }
        public DbSet<CreatorPlanPricing> CreatorPlanPricings{ get; set; }
        public DbSet<CreatorToken> CreatorTokens { get; set; }
        public DbSet<ProfileTokenHolding> ProfileTokenHoldings { get; set; }

        public DbSet<Raffle> Raffles { get; set; }
        public DbSet<RaffleTokenRequirement> RaffleTokenRequirements { get; set; }
        public DbSet<RaffleEntry> RaffleEntries { get; set; }
        public DbSet<RaffleWinner> RaffleWinners { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            CreatorPlanSeeder.SeedData(builder);

            // === IDENTITY TABLES UMBENENNEN ===
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            #region CreatorPlan Configuration
            builder.Entity<CreatorSubscription>()
            .HasOne(cs => cs.TribeProfile)
            .WithOne(tp => tp.ActiveCreatorSubscription)
            .HasForeignKey<CreatorSubscription>(cs => cs.TribeProfileId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CreatorSubscription>()
                .HasOne(cs => cs.CreatorPlan)
                .WithMany(cp => cp.Subscriptions)
                .HasForeignKey(cs => cs.CreatorPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CreatorSubscription>()
                .HasOne(cs => cs.CreatorPlanPricing)
                .WithMany()
                .HasForeignKey(cs => cs.CreatorPlanPricingId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            // === PROFILE KONFIGURATION ===
            builder.Entity<TribeProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Bio).HasMaxLength(1000);
                entity.HasIndex(e => e.ApplicationUserId).IsUnique();
                entity.HasIndex(e => e.DisplayName);

                
            });

            builder.Entity<CreatorProfile>(entity =>
            {
                
                entity.Property(e => e.CreatorName).HasMaxLength(100);
                entity.HasIndex(e => e.CreatorName).IsUnique()
                      .HasFilter("[CreatorName] IS NOT NULL");
            });

            // === FOLLOW SYSTEM ===
            builder.Entity<ProfileFollow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.FollowerId, e.CreatorProfileId }).IsUnique();

                entity.HasOne(e => e.Follower)
                      .WithMany()
                      .HasForeignKey(e => e.FollowerId)
                      .OnDelete(DeleteBehavior.Restrict); // Verhindert das Löschen eines Profils, wenn es Follows hat.

                entity.HasOne(e => e.Followed)
                      .WithMany()
                      .HasForeignKey(e => e.CreatorProfileId)
                      .OnDelete(DeleteBehavior.Restrict); // Verhindert das Löschen eines Creator-Profils, wenn es Follows hat.

                entity.ToTable(t => t.HasCheckConstraint("CK_ProfileFollow_NotSelf",
                    "[FollowerId] <> [CreatorProfileId]"));
            });

            // === TOKEN SYSTEM ===
            builder.Entity<CreatorToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TokenName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TokenSymbol).HasMaxLength(10);
                entity.HasIndex(e => new { e.CreatorProfileId, e.TokenName }).IsUnique();

                entity.HasOne(e => e.Creator)
                      .WithMany(p => p.CreatorTokens)
                      .HasForeignKey(e => e.CreatorProfileId)
                      .OnDelete(DeleteBehavior.Cascade); // Löscht alle Tokens des Creators.
            });

            builder.Entity<ProfileTokenHolding>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ProfileId, e.CreatorTokenId }).IsUnique();

                entity.HasOne(e => e.Profile)
                      .WithMany()
                      .HasForeignKey(e => e.ProfileId)
                      .OnDelete(DeleteBehavior.NoAction); // Verhindert das Löschen eines Profils, das Tokens hält.

                entity.HasOne(e => e.Token)
                      .WithMany()
                      .HasForeignKey(e => e.CreatorTokenId)
                      .OnDelete(DeleteBehavior.NoAction); // Verhindert das Löschen eines Tokens, das gehalten wird.

                entity.ToTable(t => t.HasCheckConstraint("CK_TokenHolding_PositiveAmount",
                    "[Amount] >= 0"));
            });

            // === RAFFLE SYSTEM ===
            builder.Entity<Raffle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RaffleType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PrizeValue).HasPrecision(18, 2);

                entity.HasOne(e => e.Creator)
                      .WithMany(p => p.Raffles)
                      .HasForeignKey(e => e.CreatorProfileId)
                      .OnDelete(DeleteBehavior.Cascade); // Löscht alle Raffles, wenn der Creator gelöscht wird.

                entity.ToTable(t => t.HasCheckConstraint("CK_Raffle_ValidDateRange",
                    "[EndDate] > [StartDate]"));
            });

            builder.Entity<RaffleTokenRequirement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequirementType).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => new { e.RaffleId, e.CreatorTokenId }).IsUnique();

                entity.HasOne(e => e.Raffle)
                      .WithMany()
                      .HasForeignKey(e => e.RaffleId)
                      .OnDelete(DeleteBehavior.Restrict); // Verhindert das Löschen eines Raffles, wenn noch Anforderungen existieren.

                entity.HasOne(e => e.CreatorToken)
                      .WithMany()
                      .HasForeignKey(e => e.CreatorTokenId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict); // Verhindert das Löschen eines Tokens, wenn es für ein Raffle benötigt wird.

                entity.ToTable(t => t.HasCheckConstraint("CK_RaffleTokenReq_PositiveAmount",
                    "[RequiredAmount] >= 0"));
            });

            builder.Entity<RaffleEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntryType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Stage).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.Raffle)
                      .WithMany()
                      .HasForeignKey(e => e.RaffleId)
                      .OnDelete(DeleteBehavior.NoAction); // **WICHTIGE KORREKTUR:** Verhindert die automatische Löschung der Einträge, wenn das Raffle gelöscht wird. Dies behebt den "multiple cascade paths" Fehler.

                entity.HasOne(e => e.Profile)
                      .WithMany()
                      .HasForeignKey(e => e.ProfileId)
                      .OnDelete(DeleteBehavior.NoAction); // Verhindert das Löschen eines Profils, wenn es Einträge hat.

                entity.HasOne(e => e.UsedToken)
                      .WithMany()
                      .HasForeignKey(e => e.UsedTokenId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_RaffleEntry_PositiveTokens", "[TokensSpent] >= 0");
                    t.HasCheckConstraint("CK_RaffleEntry_PositiveCount", "[EntryCount] > 0");
                });
            });

            builder.Entity<RaffleWinner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Stage).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PrizeValue).HasPrecision(18, 2);

                entity.HasOne(e => e.Entry)
                      .WithOne()
                      .HasForeignKey<RaffleWinner>(e => e.EntryId)
                      .OnDelete(DeleteBehavior.NoAction); // **KORRIGIERT:** Verhindert das Löschen eines Eintrags, der einen Gewinner hat.

                entity.HasOne(e => e.Raffle)
                      .WithMany()
                      .HasForeignKey(e => e.RaffleId)
                      .OnDelete(DeleteBehavior.NoAction); // **KORRIGIERT:** Verhindert das Löschen eines Raffles, wenn bereits ein Gewinner gezogen wurde.

                entity.HasOne(e => e.Profile)
                      .WithMany()
                      .HasForeignKey(e => e.ProfileId)
                      .OnDelete(DeleteBehavior.NoAction); // Verhindert das Löschen eines Profils, das als Gewinner gezogen wurde.

                entity.ToTable(t => t.HasCheckConstraint("CK_RaffleWinner_PositivePosition",
                    "[Position] > 0"));
            });
        }
    }
}