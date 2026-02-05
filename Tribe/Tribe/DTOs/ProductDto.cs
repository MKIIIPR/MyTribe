using System;
using System.Collections.Generic;

namespace Tribe.DTOs
{
    public class ProductDto
    {
        public string ProductType { get; set; } = "physical";
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string? ThumbnailUrl { get; set; }

        // Physical specific
        public string? SKU { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? ShippingCost { get; set; }
        public bool? TrackInventory { get; set; }

        // Video specific
        public string? VideoUrl { get; set; }

        // Image specific
        public List<string>? HighResImageUrls { get; set; }
        public string? ImageFormat { get; set; }

        // Service specific
        public int? DurationMinutes { get; set; }

        // Event ticket specific
        public DateTime? EventDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string? EventLocation { get; set; }
        public int? MaxTickets { get; set; }
    }
}
