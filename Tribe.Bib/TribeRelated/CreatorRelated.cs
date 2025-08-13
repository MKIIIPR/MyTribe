using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tribe.Bib.Models.TribeRelated;

// === CREATOR DEFINITION ===
public class CreatorDefinition
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Verbindung zum Profil
    [Required]
    public string CreatorProfileId { get; set; }
    [ForeignKey(nameof(CreatorProfileId))]
    public CreatorProfile CreatorProfile { get; set; }

    // Basisdaten
    [Required, MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Mission { get; set; }

    [MaxLength(500)]
    public string? TargetAudience { get; set; }

    // Kategorien
    public string ContentCategories { get; set; } = ""; // JSON
    public string? PrimaryCategory { get; set; }

    // Erfahrung
    public DateTime CreatorSince { get; set; } = DateTime.UtcNow;
    public string? ExperienceLevel { get; set; } = CreatorConstants.ExperienceLevel.Beginner;
    public int? YearsOfExperience { get; set; }

    // Veröffentlichungsplan
    public string? ContentSchedule { get; set; }
    public string? Timezone { get; set; } = "UTC";
    public bool HasRegularSchedule { get; set; } = false;

    // Kollaborationspräferenzen
    public bool OpenForCollaborations { get; set; } = false;
    public string? CollaborationTypes { get; set; } // JSON
    public string? CollaborationNote { get; set; }

    // Monetarisierung
    public bool WantsToSellProducts { get; set; } = false;
    public bool AcceptsDonations { get; set; } = false;
    public bool OffersSubscriptions { get; set; } = false;

    // Business-Info
    public string? BusinessEmail { get; set; }
    public string? BusinessPhone { get; set; }
    public bool IsBusinessEntity { get; set; } = false;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<CreatorSkill> Skills { get; set; } = new List<CreatorSkill>();
    public ICollection<CreatorEquipment> Equipment { get; set; } = new List<CreatorEquipment>();
    public CreatorShop? Shop { get; set; }
}

// === SKILLS ===
public class CreatorSkill
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CreatorDefinitionId { get; set; }
    [ForeignKey(nameof(CreatorDefinitionId))]
    public CreatorDefinition CreatorDefinition { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    public string Category { get; set; } // "Technical", "Creative", "Business"
    [Range(1, 10)]
    public int Level { get; set; } = 1;
    public string? Description { get; set; }
    public bool IsPrimary { get; set; } = false;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

// === EQUIPMENT ===
public class CreatorEquipment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CreatorDefinitionId { get; set; }
    [ForeignKey(nameof(CreatorDefinitionId))]
    public CreatorDefinition CreatorDefinition { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    public string Type { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Description { get; set; }

    public bool IsPrimary { get; set; } = false;
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

// === SHOP ===
public class CreatorShop
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CreatorDefinitionId { get; set; }
    [ForeignKey(nameof(CreatorDefinitionId))]
    public CreatorDefinition CreatorDefinition { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    public string? BannerUrl { get; set; }
    public string? LogoUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;
    public DateTime? LaunchedAt { get; set; }

    // Einstellungen
    public string DefaultCurrency { get; set; } = "EUR";
    public string? Timezone { get; set; } = "UTC";
    public string? Language { get; set; } = "de";

    // Zahlungen
    public bool AcceptsPayPal { get; set; } = false;
    public bool AcceptsStripe { get; set; } = false;
    public bool AcceptsCrypto { get; set; } = false;
    public bool AcceptsTokens { get; set; } = true;

    // Versand
    public bool OffersShipping { get; set; } = false;
    public bool OffersDigitalOnly { get; set; } = true;
    public string? ShippingNote { get; set; }

    // Statistik
    public int TotalProducts { get; set; } = 0;
    public int TotalSales { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ShopProduct> Products { get; set; } = new List<ShopProduct>();
    public ICollection<ShopOrder> Orders { get; set; } = new List<ShopOrder>();
}

// === PRODUKTE ===
public class ShopProduct
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CreatorShopId { get; set; }
    [ForeignKey(nameof(CreatorShopId))]
    public CreatorShop Shop { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public string Type { get; set; } // Physical, Digital, Service
    public string Category { get; set; }

    // Medien
    public string? MainImageUrl { get; set; }
    public string? Images { get; set; } // JSON
    public string? PreviewVideoUrl { get; set; }

    // Preise
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? OriginalPrice { get; set; }
    public string Currency { get; set; } = "EUR";

    // Token Preis
    public int? TokenPrice { get; set; }
    public string? AcceptedTokenId { get; set; }
    [ForeignKey(nameof(AcceptedTokenId))]
    public CreatorToken? AcceptedToken { get; set; }

    // Status
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;
    public bool IsFeatured { get; set; } = false;

    // Lager
    public int? StockQuantity { get; set; }
    public bool HasUnlimitedStock { get; set; } = true;
    public bool TrackInventory { get; set; } = false;

    // Digital
    public string? DownloadUrl { get; set; }
    public string? AccessUrl { get; set; }
    public string? ProductData { get; set; }

    // Stats
    public int TotalSales { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public decimal AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LaunchedAt { get; set; }

    // Navigation
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    public ICollection<ShopOrderItem> OrderItems { get; set; } = new List<ShopOrderItem>();
}

// === BESTELLUNGEN ===
public class ShopOrder
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CreatorShopId { get; set; }
    [ForeignKey(nameof(CreatorShopId))]
    public CreatorShop Shop { get; set; }

    // Kunde
    public string? CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public TribeProfile? Customer { get; set; }
    public string CustomerEmail { get; set; }
    public string? CustomerName { get; set; }

    // Bestellung
    public string OrderNumber { get; set; } = "";
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // Preise
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubtotalAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0;
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingAmount { get; set; } = 0;
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "EUR";

    // Token-Zahlung
    public int? TokensSpent { get; set; }
    public string? UsedTokenId { get; set; }
    [ForeignKey(nameof(UsedTokenId))]
    public CreatorToken? UsedToken { get; set; }

    // Status
    public string OrderStatus { get; set; } = ShopConstants.OrderStatus.Pending;
    public string PaymentStatus { get; set; } = ShopConstants.PaymentStatus.Pending;
    public string FulfillmentStatus { get; set; } = ShopConstants.FulfillmentStatus.Pending;

    // Zahlung
    public string? PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Versand
    public string? ShippingAddress { get; set; }
    public string? ShippingMethod { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }

    // Notizen
    public string? CustomerNotes { get; set; }
    public string? CreatorNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ShopOrderItem> OrderItems { get; set; } = new List<ShopOrderItem>();
}

// === BESTELL-ARTIKEL ===
public class ShopOrderItem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ShopOrderId { get; set; }
    [ForeignKey(nameof(ShopOrderId))]
    public ShopOrder Order { get; set; }

    [Required]
    public string ShopProductId { get; set; }
    [ForeignKey(nameof(ShopProductId))]
    public ShopProduct Product { get; set; }

    public string ProductName { get; set; }
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "EUR";

    // Token
    public int? TokensPerUnit { get; set; }
    public int? TotalTokens { get; set; }

    // Digital
    public string? DownloadUrl { get; set; }
    public string? AccessKey { get; set; }
    public DateTime? AccessExpiresAt { get; set; }
    public bool IsDelivered { get; set; } = false;
    public DateTime? DeliveredAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// === REVIEWS ===
public class ProductReview
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ShopProductId { get; set; }
    [ForeignKey(nameof(ShopProductId))]
    public ShopProduct Product { get; set; }

    [Required]
    public string ReviewerId { get; set; }
    [ForeignKey(nameof(ReviewerId))]
    public TribeProfile Reviewer { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? ReviewText { get; set; }
    public string? ReviewTitle { get; set; }

    public bool IsApproved { get; set; } = true;
    public bool IsPublic { get; set; } = true;
    public DateTime? ApprovedAt { get; set; }

    public int HelpfulVotes { get; set; } = 0;
    public int TotalVotes { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}


// 9. CONSTANTS FÜR CREATOR & SHOP
public static class CreatorConstants
{
    public static class ContentCategories
    {
        public const string Gaming = "Gaming";
        public const string Fitness = "Fitness";
        public const string Cooking = "Cooking";
        public const string Technology = "Technology";
        public const string Education = "Education";
        public const string Entertainment = "Entertainment";
        public const string Music = "Music";
        public const string Art = "Art";
        public const string Fashion = "Fashion";
        public const string Travel = "Travel";
        public const string Business = "Business";
        public const string Lifestyle = "Lifestyle";
        public const string Sports = "Sports";
        public const string News = "News";
        public const string Comedy = "Comedy";
        public const string Beauty = "Beauty";
        public const string DIY = "DIY";
        public const string Parenting = "Parenting";
        public const string Spirituality = "Spirituality";
        public const string Other = "Other";
    }

    public static class ExperienceLevel
    {
        public const string Beginner = "Beginner";
        public const string Intermediate = "Intermediate";
        public const string Advanced = "Advanced";
        public const string Expert = "Expert";
        public const string Professional = "Professional";
    }

    public static class CollaborationTypes
    {
        public const string CrossPromotion = "CrossPromotion";
        public const string JointContent = "JointContent";
        public const string GuestAppearance = "GuestAppearance";
        public const string Sponsorship = "Sponsorship";
        public const string ProductReview = "ProductReview";
        public const string Challenge = "Challenge";
        public const string Event = "Event";
    }

    public static class ProductCategories
    {
        public const string Merch = "Merch";
        public const string Tutorial = "Tutorial";
        public const string Course = "Course";
        public const string Ebook = "Ebook";
        public const string Video = "Video";
        public const string Podcast = "Podcast";
        public const string Preset = "Preset";
        public const string Template = "Template";
        public const string Coaching = "Coaching";
        public const string Consultation = "Consultation";
        public const string Membership = "Membership";
        public const string Software = "Software";
        public const string Music = "Music";
        public const string Photo = "Photo";
        public const string Other = "Other";
    }

    public static class ProductTypes
    {
        public const string Physical = "Physical";
        public const string Digital = "Digital";
        public const string Service = "Service";
        public const string Subscription = "Subscription";
    }
}

public static class ShopConstants
{
    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
        public const string PartiallyRefunded = "PartiallyRefunded";
    }

    public static class FulfillmentStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Fulfilled = "Fulfilled";
        public const string PartiallyFulfilled = "PartiallyFulfilled";
        public const string Cancelled = "Cancelled";
    }
}
