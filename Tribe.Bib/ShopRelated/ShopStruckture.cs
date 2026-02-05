using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tribe.Bib.ShopRelated
{
    public class ShopStruckture
    {

        public static class ProductStatus
        {
            public const string Draft = "draft";
            public const string Active = "active";
            public const string Inactive = "inactive";
            public const string SoldOut = "sold_out";

            public static readonly string[] All = { Draft, Active, Inactive, SoldOut };
        }

        public static class OrderStatus
        {
            public const string Pending = "pending";
            public const string Confirmed = "confirmed";
            public const string Processing = "processing";
            public const string Shipped = "shipped";
            public const string Delivered = "delivered";
            public const string Completed = "completed";
            public const string Cancelled = "cancelled";
            public const string Refunded = "refunded";

            public static readonly string[] All = { Pending, Confirmed, Processing, Shipped, Delivered, Completed, Cancelled, Refunded };
        }

        public static class ProductTypes
        {
            public const string Video = "video";
            public const string Image = "image";
            public const string Physical = "physical";
            public const string Service = "service";
            public const string EventTicket = "event_ticket";

            public static readonly string[] All = { Video, Image, Physical, Service, EventTicket };
        }

        // Base Product Class
        public abstract class ShopProduct
        {
            [Key]
            public string Id { get; set; } = Guid.NewGuid().ToString();

            [Required]
            [MaxLength(200)]
            public string Title { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal Price { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal? OriginalPrice { get; set; }

            public string Status { get; set; } = ProductStatus.Draft;

            // Common Media
            public string? ThumbnailUrl { get; set; }
            public List<string> ImageUrls { get; set; } = new();

            // SEO & Tags
            public List<string> Tags { get; set; } = new();
            // Optional category id reference
            // Optional category id reference
            public string? CategoryId { get; set; }
            public string? SeoTitle { get; set; }
            public string? SeoDescription { get; set; }

            // Creator & Timestamps
            public string CreatorProfileId { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Stats
            public int ViewCount { get; set; } = 0;
            public int PurchaseCount { get; set; } = 0;
            public decimal TotalRevenue { get; set; } = 0;

            // Abstract properties für UI-Darstellung
            public abstract string ProductTypeDisplay { get; }
            public abstract string ProductIcon { get; }
        }

        // Digital Video Product
        public class VideoProduct : ShopProduct
        {
            public string? VideoUrl { get; set; }
            public string? PreviewVideoUrl { get; set; }
            public int DurationSeconds { get; set; }
            public string VideoQuality { get; set; } = "1080p";
            public long FileSizeBytes { get; set; }
            public string VideoFormat { get; set; } = "mp4";
            public bool HasSubtitles { get; set; } = false;
            public List<string> SubtitleLanguages { get; set; } = new();

            // Download settings
            public bool AllowDownload { get; set; } = true;
            public string? DownloadUrl { get; set; }
            public DateTime? DownloadExpiry { get; set; }
            public int MaxDownloads { get; set; } = 3;

            public override string ProductTypeDisplay => "Video";
            public override string ProductIcon => "fas fa-play";
        }

        // Digital Image/Photo Product
        public class ImageProduct : ShopProduct
        {
            public List<string> HighResImageUrls { get; set; } = new();
            public string ImageFormat { get; set; } = "jpg";
            public string Resolution { get; set; } = "1920x1080";
            public long FileSizeBytes { get; set; }
            public bool IsPhotoSet { get; set; } = false;
            public int PhotoCount { get; set; } = 1;

            // Download settings
            public string? DownloadUrl { get; set; }
            public string? ZipDownloadUrl { get; set; }
            public DateTime? DownloadExpiry { get; set; }
            public int MaxDownloads { get; set; } = 5;

            public override string ProductTypeDisplay => "Bilder";
            public override string ProductIcon => "fas fa-image";
        }

        // Physical Product
        public class PhysicalProduct : ShopProduct
        {
            public string? SKU { get; set; }
            public int StockQuantity { get; set; } = 0;
            public bool TrackInventory { get; set; } = true;
            public double? Weight { get; set; } // in grams
            public string? Dimensions { get; set; } // "L x B x H in cm"

            // Shipping
            public decimal ShippingCost { get; set; } = 0;
            public bool FreeShippingOver { get; set; } = false;
            public decimal FreeShippingThreshold { get; set; } = 50;
            public List<string> ShippingCountries { get; set; } = new();
            public int EstimatedDeliveryDays { get; set; } = 7;

            // Product details
            public string? Brand { get; set; }
            public string? Model { get; set; }
            public string? Color { get; set; }
            public string? Size { get; set; }

            public override string ProductTypeDisplay => "Produkt";
            public override string ProductIcon => "fas fa-box";
        }

        // Service Product
        public class ServiceProduct : ShopProduct
        {
            public int DurationMinutes { get; set; } = 60;
            public bool RequiresBooking { get; set; } = true;
            public List<DateTime> AvailableSlots { get; set; } = new();
            public string ServiceType { get; set; } = "consultation"; // consultation, coaching, design, etc.

            // Booking settings
            public int MaxBookingsPerDay { get; set; } = 5;
            public int MinAdvanceBookingHours { get; set; } = 24;
            public int MaxAdvanceBookingDays { get; set; } = 30;
            public bool AllowRescheduling { get; set; } = true;
            public int RescheduleHoursLimit { get; set; } = 24;

            // Meeting details
            public string MeetingType { get; set; } = "online"; // online, in_person, phone
            public string? MeetingUrl { get; set; }
            public string? MeetingLocation { get; set; }
            public string? MeetingInstructions { get; set; }

            public override string ProductTypeDisplay => "Service";
            public override string ProductIcon => "fas fa-handshake";
        }

        // Event Ticket Product
        public class EventTicketProduct : ShopProduct
        {
            [Required]
            public DateTime EventDate { get; set; }
            public DateTime? EventEndDate { get; set; }
            public string EventLocation { get; set; } = string.Empty;
            public string? EventAddress { get; set; }
            public int MaxTickets { get; set; } = 100;
            public int SoldTickets { get; set; } = 0;

            // Ticket details
            public string TicketType { get; set; } = "general"; // general, vip, early_bird
            public bool IncludesFood { get; set; } = false;
            public bool IncludesDrinks { get; set; } = false;
            public List<string> Includes { get; set; } = new();

            // Event details
            public string? EventCategory { get; set; }
            public int EstimatedDurationMinutes { get; set; } = 120;
            public string? DressCode { get; set; }
            public int MinAge { get; set; } = 0;
            public bool RefundableUntil { get; set; } = true;
            public DateTime? RefundDeadline { get; set; }

            public int RemainingTickets => MaxTickets - SoldTickets;
            public bool IsSoldOut => RemainingTickets <= 0;

            public override string ProductTypeDisplay => "Event Ticket";
            public override string ProductIcon => "fas fa-ticket";
        }

        // Shop Category
        public class ShopCategory
        {
            [Key]
            public string Id { get; set; } = Guid.NewGuid().ToString();

            [Required]
            [MaxLength(100)]
            public string Name { get; set; } = string.Empty;
            public string ColorHex { get; set; } = "#1976d2";
            public string CreatorProfileId { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public int ProductCount { get; set; } = 0;
        }

        // Shopping Cart Models
        public class ShopCartItem
        {
            public string ProductId { get; set; } = string.Empty;
            public string ProductTitle { get; set; } = string.Empty;
            public string ProductType { get; set; } = string.Empty;
            public string? ThumbnailUrl { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; } = 1;
            public DateTime AddedAt { get; set; } = DateTime.UtcNow;

            // For services with booking
            public DateTime? SelectedSlot { get; set; }

            // For event tickets
            public string? TicketType { get; set; }

            public decimal TotalPrice => Price * Quantity;
        }

        public class ShopCart
        {
            public List<ShopCartItem> Items { get; set; } = new();
            public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
            public int TotalItems => Items.Sum(i => i.Quantity);

            public void AddItem(ShopProduct product, int quantity = 1, DateTime? selectedSlot = null)
            {
                var existingItem = Items.FirstOrDefault(i => i.ProductId == product.Id);
                if (existingItem != null && product is not ServiceProduct)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    Items.Add(new ShopCartItem
                    {
                        ProductId = product.Id,
                        ProductTitle = product.Title,
                        ProductType = product.GetType().Name,
                        ThumbnailUrl = product.ThumbnailUrl,
                        Price = product.Price,
                        Quantity = quantity,
                        SelectedSlot = selectedSlot
                    });
                }
            }

            public void RemoveItem(string productId)
            {
                Items.RemoveAll(i => i.ProductId == productId);
            }

            public void UpdateQuantity(string productId, int quantity)
            {
                var item = Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    if (quantity <= 0)
                        RemoveItem(productId);
                    else
                        item.Quantity = quantity;
                }
            }

            public void Clear()
            {
                Items.Clear();
            }
        }

        // Order Models
        public class ShopOrder
        {
            [Key]
            public string Id { get; set; } = Guid.NewGuid().ToString();

            [Required]
            public string OrderNumber { get; set; } = string.Empty;
            public string CreatorProfileId { get; set; } = string.Empty;
            public string CustomerUserId { get; set; } = string.Empty;
            public string Status { get; set; } = OrderStatus.Pending;

            [Column(TypeName = "decimal(18,2)")]
            public decimal TotalAmount { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal TaxAmount { get; set; } = 0;

            [Column(TypeName = "decimal(18,2)")]
            public decimal ShippingAmount { get; set; } = 0;

            // Customer Info
            public string CustomerEmail { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string? CustomerPhone { get; set; }

            // Shipping Address (for physical products)
            public string? ShippingAddress { get; set; }
            public string? ShippingCity { get; set; }
            public string? ShippingPostalCode { get; set; }
            public string? ShippingCountry { get; set; }

            // Payment Info
            public string? PaymentMethod { get; set; }
            public string? PaymentTransactionId { get; set; }
            public DateTime? PaidAt { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }

        public class ShopOrderItem
        {
            [Key]
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string OrderId { get; set; } = string.Empty;
            public string ProductId { get; set; } = string.Empty;
            public string ProductTitle { get; set; } = string.Empty;
            public string ProductType { get; set; } = string.Empty;

            [Column(TypeName = "decimal(18,2)")]
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal TotalPrice { get; set; }

            // For services
            public DateTime? BookedSlot { get; set; }

            // For digital products
            public string? DownloadToken { get; set; }
            public DateTime? DownloadExpiry { get; set; }
            public int DownloadCount { get; set; } = 0;
            public int MaxDownloads { get; set; } = 3;
        }

        public class ProductReview
        {
            [Key]
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string ProductId { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public int Rating { get; set; } = 5; // 1-5 stars

            [MaxLength(1000)]
            public string? Comment { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public bool IsVerifiedPurchase { get; set; } = false;
        }
    }
}
