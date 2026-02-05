using System;
using System.Collections.Generic;

namespace Tribe.Services.ClientServices.ShopServices
{
    public class ProductDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string? ThumbnailUrl { get; set; }
        public List<string> Tags { get; set; } = new();
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
    }

    public class PhysicalProductDto : ProductDto
    {
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackInventory { get; set; } = true;
        public decimal ShippingCost { get; set; }
        public bool FreeShippingOver { get; set; }
        public decimal FreeShippingThreshold { get; set; }
    }

    public class VideoProductDto : ProductDto
    {
        public string? VideoUrl { get; set; }
        public int DurationSeconds { get; set; }
        public string VideoQuality { get; set; } = "1080p";
    }

    public class ImageProductDto : ProductDto
    {
        public List<string> HighResImageUrls { get; set; } = new();
        public string ImageFormat { get; set; } = "jpg";
    }

    public class ServiceProductDto : ProductDto
    {
        public int DurationMinutes { get; set; } = 60;
        public bool RequiresBooking { get; set; } = true;
    }

    public class EventTicketProductDto : ProductDto
    {
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public DateTime? EventEndDate { get; set; }
        public string EventLocation { get; set; } = string.Empty;
        public int MaxTickets { get; set; } = 100;
    }
}
