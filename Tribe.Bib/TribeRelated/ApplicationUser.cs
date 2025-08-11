﻿namespace Tribe.Bib.Models.TribeRelated
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Identity;
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   
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

    }

    // 2. TribeProfile - �FFENTLICHE Anzeige-Daten
    public class TribeProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // === �FFENTLICHE BASIC INFO ===
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } // �ffentlicher Anzeigename

        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }

        // === PROFILE STATUS ===
        public string ProfileType { get; set; } = Constants.ProfileTypes.Regular;
        public bool IsCreator { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // === USER CONNECTION ===
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser User { get; set; }
    }

    // 3. CreatorProfile - �FFENTLICHE Creator-Daten
    public class CreatorProfile : TribeProfile
    {
        public CreatorProfile()
        {
            ProfileType = Constants.ProfileTypes.Regular;
            IsCreator = true;
        }

        // === �FFENTLICHE CREATOR INFO ===
        [MaxLength(100)]
        public string? CreatorName { get; set; } // Eindeutiger Creator-Name

        public string? ImageTemplateUrl { get; set; }
        public string? BannerUrl { get; set; }

        // === �FFENTLICHE STATS ===
        public int FollowerCount { get; set; } = 0;
        public int TotalRaffles { get; set; } = 0;
        public int TotalTokensDistributed { get; set; } = 0;

        // === �FFENTLICHE SOCIAL LINKS ===
        public string? PatreonUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? DiscordUrl { get; set; }
        public ICollection<AffiliatePartner> PartnerUrl { get; set; }  // Enum f�r Partner-Links

        // === �FFENTLICHE CREATOR SETTINGS ===
        public bool AcceptingCollaborations { get; set; } = false;
        public string? CollaborationInfo { get; set; }
        public bool VerifiedCreator { get; set; } = false;
        public DateTime? VerifiedAt { get; set; }

        // Navigation properties for related data
        public ICollection<CreatorToken> CreatorTokens { get; set; } = new List<CreatorToken>();
        public ICollection<Raffle> Raffles { get; set; } = new List<Raffle>();
    }
    public class AffiliatePartner
    {
        public string Id { get; set; }
        public string PartneName { get; set; }
        public string PartnerUrl { get; set; }
        public string PartnerLogoUrl { get; set; }

    }
    // 4. FOLLOW SYSTEM
    public class ProfileFollow
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FollowerId { get; set; }
        [ForeignKey(nameof(FollowerId))]
        public TribeProfile Follower { get; set; }

        public string CreatorProfileId { get; set; }
        [ForeignKey(nameof(CreatorProfileId))]
        public CreatorProfile Followed { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }

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

    // 6. TOKEN HOLDINGS
    public class ProfileTokenHolding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public TribeProfile Profile { get; set; }

        public string CreatorTokenId { get; set; }
        [ForeignKey(nameof(CreatorTokenId))]
        public CreatorToken Token { get; set; }

        public int Amount { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // 7. RAFFLE SYSTEM - Erweitert f�r verschiedene Verlosungstypen
    public class Raffle
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // === RAFFLE CONFIG ===
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxEntries { get; set; } = 1000;
        public int CurrentEntries { get; set; } = 0;

        // === VERLOSUNGSYSTEM ===
        public string RaffleType { get; set; } = Constants.RaffleTypes.Standard; // Standard, Multiple, TwoStage
        public string? RaffleConfig { get; set; } // JSON Config f�r komplexe Systeme

        // === PRIZE INFO ===
        public string PrizeDescription { get; set; }
        public decimal PrizeValue { get; set; } = 0;
        public int PrizeCount { get; set; } = 1; // Anzahl Gewinner f�r Multiple-Raffles

        // === STATUS ===
        public string Status { get; set; } = Constants.RaffleStatus.Active;
        public DateTime? DrawnAt { get; set; }

        // === CREATOR CONNECTION ===
        public string CreatorProfileId { get; set; }
        [ForeignKey(nameof(CreatorProfileId))]
        public CreatorProfile Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
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
        public TribeProfile Profile { get; set; }

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
        public TribeProfile Profile { get; set; }

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