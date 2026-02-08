namespace Tribe.Bib.Models.TribeRelated
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Identity;
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Tribe.Bib.CommunicationModels.ComModels;

    // 1. ApplicationUser - PRIVATE Daten (Backend only)
    public class ApplicationUser : IdentityUser
    {
        // === PRIVATE PERS�NLICHE DATEN ===
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // === PRIVATE ADRESSE ===
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? StateOrRegion { get; set; }
        public string? CountryCode { get; set; }

        // === PRIVATE BUSINESS INFO ===
        public string? CompanyName { get; set; }
        public string? LegalForm { get; set; }
        public string? TaxNumber { get; set; }
        public string? VATId { get; set; }
        public bool IsVATPayer { get; set; } = false;

        // === PRIVATE PAYMENT TOKENS (verschl�sselt!) ===
        public string? StripeAccountId { get; set; }
        public string? ShopifyToken { get; set; }
        public string? PaypalPayerId { get; set; }

        // === PRIVATE DATENSCHUTZ ===
        public bool TermsAccepted { get; set; }
        public DateTime? TermsAcceptedAt { get; set; }
        public bool PrivacyPolicyAccepted { get; set; }
        public DateTime? PrivacyPolicyAcceptedAt { get; set; }

        // === SYSTEM DATEN ===
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public BillingAddress? BillingAddress { get; set; } = new BillingAddress();

    }

    // 2. TribeProfile - �FFENTLICHE Anzeige-Daten
    // 1. TribeProfile - Basis-Profil
    public class TribeUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // === ÖFFENTLICHE BASIC INFO ===
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }

        // === PROFILE STATUS ===
        public string ProfileType { get; set; } = Constants.ProfileTypes.Regular;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // === USER CONNECTION ===
        [ForeignKey(nameof(ApplicationUserId))]
        public string ApplicationUserId { get; set; } = string.Empty;

        public bool IsCreator { get; set; } = false;

        // Navigation Properties
        public CreatorSubscription? ActiveCreatorSubscription { get; set; }
        public CreatorProfile? CreatorProfile { get; set; }
    }

    // 2. CreatorPlan - Die verfügbaren Pläne
    public class CreatorPlan
    {
        [Key]
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
        public int TokenMenge { get; set; }
        public string FeaturesJson { get; set; } = "{}";
        public bool CanUploadDigitalContent { get; set; } = true;
        public bool HaveShopItems { get; set; } = true;
        public bool CanCreateEvents { get; set; } = true;
        public bool CanCreateRaffles { get; set; } = true;
        public bool CanUseWindowsApp { get; set; } = true;
        public bool Aktiv { get; set; }

        // Navigation Property
        public virtual ICollection<CreatorPlanPricing> PricingList { get; set; } = new List<CreatorPlanPricing>();
        public virtual ICollection<CreatorSubscription> Subscriptions { get; set; } = new List<CreatorSubscription>();
    }

    // 3. CreatorPlanPricing - Preise für die Pläne
    public class CreatorPlanPricing
    {
        [Key]
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();

        [ForeignKey(nameof(CreatorPlan))]
        public string CreatorPlanGuid { get; set; } = string.Empty;

        public string Duration { get; set; } = "Monthly"; // Monthly, Annual

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValueUSD { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValueEuro { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValueGbPound { get; set; }

        // Navigation Property
        public virtual CreatorPlan CreatorPlan { get; set; } = null!;
    }

    // 4. CreatorSubscription - Die Zwischentabelle/Subscription-Tabelle
    public class CreatorSubscription
    {
        [Key]
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();

        // Verweis auf TribeProfile
        [ForeignKey(nameof(TribeProfile))]
        public string TribeProfileId { get; set; } = string.Empty;

        // Verweis auf CreatorPlan (nullable für Trial)
        [ForeignKey(nameof(CreatorPlan))]
        public string? CreatorPlanId { get; set; }

        // Verweis auf das spezifische Pricing (nullable für Trial)
        [ForeignKey(nameof(CreatorPlanPricing))]
        public string? CreatorPlanPricingId { get; set; }
        public string Currency { get; set; } = "USD"; // USD, EUR, GBP
        [Column(TypeName = "decimal(18, 2)")]
        public double SubValue { get; set; }
        // Subscription Details
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } // null = aktiv
        public string Duration { get; set; } = "Monthly"; // Monthly, Annual, Trial
        public bool IsActive { get; set; } = true;
        public bool IsPaid { get; set; } = false;

        // Payment Details
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed, Cancelled, Trial
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }

        // Subscription Management
        public bool AutoRenew { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties (nullable für Trial-Subscriptions)
        public virtual TribeUser? TribeProfile { get; set; }
        public virtual CreatorPlan? CreatorPlan { get; set; }
        public virtual CreatorPlanPricing? CreatorPlanPricing { get; set; }

        public BillingAddress BillingAddress { get; set; } = new();
        public PaymentInfo PaymentInfo { get; set; } = new();

        // Helper property
        public bool IsTrial => Duration == "Trial" || PaymentStatus == "Trial";
    }

    // 5. CreatorProfile - Erweiterte Creator-Daten
    public class CreatorProfile
    {
        [Key]
        [ForeignKey(nameof(TribeUser))]
        public string Id { get; set; } = string.Empty;

        public TribeUser TribeUser { get; set; } = null!;

        [NotMapped]
        public string? DisplayName => TribeUser?.DisplayName;

        [NotMapped]
        public string? AvatarUrl => TribeUser?.AvatarUrl;

        [NotMapped]
        public string? Bio => TribeUser?.Bio;

        [NotMapped]
        public string ProfileType => TribeUser?.ProfileType ?? Constants.ProfileTypes.Regular;

        [NotMapped]
        public bool IsCreator => TribeUser?.IsCreator ?? false;

        // === ÖFFENTLICHE CREATOR INFO ===
        [MaxLength(100)]
        public string? CreatorName { get; set; }
        public string? ImageTemplateUrl { get; set; }
        public string? BannerUrl { get; set; }

        // === ÖFFENTLICHE STATS ===
        public int FollowerCount { get; set; } = 0;
        public int TotalRaffles { get; set; } = 0;
        public int TotalTokensDistributed { get; set; } = 0;

        // === SOCIAL LINKS ===
        public string? PatreonUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? DiscordUrl { get; set; }

        // === CREATOR SETTINGS ===
        public bool AcceptingCollaborations { get; set; } = false;
        public string? CollaborationInfo { get; set; }
        public bool VerifiedCreator { get; set; } = false;
        public DateTime? VerifiedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<CreatorToken> CreatorTokens { get; set; } = new List<CreatorToken>();
        public virtual ICollection<AffiliatePartner> AffiliatePartners { get; set; } = new List<AffiliatePartner>();
        public virtual ICollection<CreatorPlacement> Placements { get; set; } = new List<CreatorPlacement>();
    }

    // 6. DbContext Configuration
  
    // 5. CREATOR TOKENS
    public class CreatorToken
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(50)]
        public string TokenName { get; set; }

        [MaxLength(10)]
        public string TokenSymbol { get; set; }

        public string? TokenImageUrl { get; set; }
        public string? Description { get; set; }

        public int TotalSupply { get; set; }
        public int CirculatingSupply { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // === CREATOR CONNECTION ===
        public string CreatorProfileId { get; set; }
        [ForeignKey(nameof(CreatorProfileId))]
        public CreatorProfile Creator { get; set; }
    }
    public class CreatorPlacement
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? PlacementName { get; set; } // z.B. "Featured", "Sponsored", etc.
        public string? PlacementUrl { get; set; } // URL zur Platzierung
        public string? PlacementLogoUrl { get; set; } // Logo der Platzierung
        public string? WebHookUrl { get; set; } // Webhook URL f�r Benachrichtigungen    
        // === CREATOR CONNECTION ===
        public string? CreatorProfileId { get; set; }
        [ForeignKey(nameof(CreatorProfileId))]
        public CreatorProfile Creator { get; set; }
    }
    public class AffiliatePartner
    {
        public string Id { get; set; }
        public string PartneName { get; set; }
        public string PartnerUrl { get; set; }
        public string PartnerLogoUrl { get; set; }
        [Required]
        public string CreatorProfileId { get; set; }

        [ForeignKey(nameof(CreatorProfileId))]
        public CreatorProfile Creator { get; set; }

    }
   
    public class ProfileFollow
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FollowerId { get; set; }
        [ForeignKey(nameof(FollowerId))]
        public TribeUser Follower { get; set; }

        public string CreatorProfileId { get; set; }
        [ForeignKey(nameof(CreatorProfileId))]
        public CreatorProfile Followed { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
    // 7. RAFFLE SYSTEM - Erweitert für verschiedene Verlosungstypen
    public class Raffle
    {
        [Key]
        public string Id { get; set; } = string.Empty; // Wird erst beim Speichern gesetzt

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? PrizeName { get; set; }
        public string? EntryRequirement { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // === RAFFLE CONFIG ===
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MaxEntries { get; set; } = 1000;
        public int CurrentEntries { get; set; } = 0;

        // === VERLOSUNGSYSTEM ===
        public string RaffleType { get; set; } = Constants.RaffleTypes.Standard;
        public string? RaffleConfig { get; set; }

        // === PRIZE INFO ===
        public string PrizeDescription { get; set; } = string.Empty;
        public decimal PrizeValue { get; set; } = 0;
        public int PrizeCount { get; set; } = 1;

        // === STATUS ===
        public string Status { get; set; } = Constants.RaffleStatus.Active;
        public DateTime? DrawnAt { get; set; }

        // === CREATOR CONNECTION (nur ID, keine Navigation für API-Kommunikation) ===
        // [Required] entfernt — wird serverseitig aus JWT gesetzt, nicht vom Client gesendet
        public string CreatorProfileId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
  
   

    // 6. TOKEN HOLDINGS
    public class ProfileTokenHolding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public TribeUser Profile { get; set; }

        public string CreatorTokenId { get; set; }
        [ForeignKey(nameof(CreatorTokenId))]
        public CreatorToken Token { get; set; }

        public int Amount { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }



    // 8. RAFFLE TOKEN REQUIREMENTS - Multiple Tokens pro Raffle m�glich
    public class RaffleTokenRequirement
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string RaffleId { get; set; }
        [ForeignKey(nameof(RaffleId))]
        public Raffle Raffle { get; set; }

        public string? CreatorTokenId { get; set; } // NULL = Follower-Only Entry
        [ForeignKey(nameof(CreatorTokenId))]
        public CreatorToken CreatorToken { get; set; }

        public int RequiredAmount { get; set; } = 0; // 0 = Free Entry
        public string RequirementType { get; set; } = Constants.RequirementTypes.Token; // Token, Follow, Both
        public bool IsOptional { get; set; } = false; // F�r Alternative Entry-Methoden

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // 9. RAFFLE ENTRIES - Erweitert f�r verschiedene Entry-Typen
    public class RaffleEntry
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string RaffleId { get; set; }
        [ForeignKey(nameof(RaffleId))]
        public Raffle Raffle { get; set; }

        public string ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public TribeUser Profile { get; set; }

        // === ENTRY DETAILS ===
        public string EntryType { get; set; } = Constants.EntryTypes.Token; // Token, Follow, Free
        public int TokensSpent { get; set; } = 0;
        public string? UsedTokenId { get; set; } // Welcher Token verwendet wurde
        [ForeignKey(nameof(UsedTokenId))]
        public CreatorToken UsedToken { get; set; }

        // === MEHRSTUFIGE VERLOSUNGEN ===
        public string Stage { get; set; } = Constants.RaffleStages.Primary; // Primary, Secondary, Final
        public bool QualifiedForNextStage { get; set; } = false;

        // === MULTIPLE ENTRIES ===
        public int EntryCount { get; set; } = 1; // Anzahl Lose/Tickets
        public string EntryNumbers { get; set; } = ""; // JSON Array der Losnummern

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // 10. RAFFLE WINNERS - F�r Multiple Winner Support
    public class RaffleWinner
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string RaffleId { get; set; }
        [ForeignKey(nameof(RaffleId))]
        public Raffle Raffle { get; set; }

        public string ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public TribeUser Profile { get; set; }

        public string EntryId { get; set; }
        [ForeignKey(nameof(EntryId))]
        public RaffleEntry Entry { get; set; }

        // === WINNER DETAILS ===
        public int Position { get; set; } = 1; // 1st Place, 2nd Place, etc.
        public string Stage { get; set; } = Constants.RaffleStages.Primary; // Welche Stufe gewonnen
        public string WinningNumber { get; set; } = ""; // Gewinnende Losnummer

        // === PRIZE INFO ===
        public string PrizeDescription { get; set; }
        public decimal PrizeValue { get; set; } = 0;
        public bool PrizeClaimed { get; set; } = false;
        public DateTime? PrizeClaimedAt { get; set; }

        public DateTime DrawnAt { get; set; } = DateTime.UtcNow;
    }

    // 11. STATIC STRING CONSTANTS statt Enums
  

}