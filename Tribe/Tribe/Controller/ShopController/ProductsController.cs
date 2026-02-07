using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TribeApp.Repositories;
using Tribe.Controller.ShopController.Validators;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.ShopController
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        private ShopProduct? MapDtoToDomain(Tribe.DTOs.ProductDto dto)
        {
            if (dto == null) return null;

            // map common fields
            ShopProduct product;
            switch (dto.ProductType?.ToLower())
            {
                case "physical":
                    product = new PhysicalProduct
                    {
                        Id = dto.Id ?? Guid.NewGuid().ToString(),
                        Title = dto.Title,
                        Description = dto.Description,
                        Price = dto.Price,
                        CategoryId = dto.CategoryId,
                        ImageUrls = dto.ImageUrls ?? new List<string>(),
                        ThumbnailUrl = dto.ThumbnailUrl,
                        SKU = dto.SKU,
                        StockQuantity = dto.StockQuantity ?? 0,
                        ShippingCost = dto.ShippingCost ?? 0,
                        TrackInventory = dto.TrackInventory ?? true
                    };
                    break;
                case "video":
                    product = new VideoProduct
                    {
                        Id = dto.Id ?? Guid.NewGuid().ToString(),
                        Title = dto.Title,
                        Description = dto.Description,
                        Price = dto.Price,
                        CategoryId = dto.CategoryId,
                        ImageUrls = dto.ImageUrls ?? new List<string>(),
                        ThumbnailUrl = dto.ThumbnailUrl,
                        VideoUrl = dto.VideoUrl
                    };
                    break;
                case "image":
                    product = new ImageProduct
                    {
                        Id = dto.Id ?? Guid.NewGuid().ToString(),
                        Title = dto.Title,
                        Description = dto.Description,
                        Price = dto.Price,
                        CategoryId = dto.CategoryId,
                        ImageUrls = dto.ImageUrls ?? new List<string>(),
                        ThumbnailUrl = dto.ThumbnailUrl,
                        HighResImageUrls = dto.HighResImageUrls ?? new List<string>(),
                        ImageFormat = dto.ImageFormat ?? "jpg"
                    };
                    break;
                case "service":
                    product = new ServiceProduct
                    {
                        Id = dto.Id ?? Guid.NewGuid().ToString(),
                        Title = dto.Title,
                        Description = dto.Description,
                        Price = dto.Price,
                        CategoryId = dto.CategoryId,
                        ImageUrls = dto.ImageUrls ?? new List<string>(),
                        ThumbnailUrl = dto.ThumbnailUrl,
                        DurationMinutes = dto.DurationMinutes ?? 60
                    };
                    break;
                case "event_ticket":
                    product = new EventTicketProduct
                    {
                        Id = dto.Id ?? Guid.NewGuid().ToString(),
                        Title = dto.Title,
                        Description = dto.Description,
                        Price = dto.Price,
                        CategoryId = dto.CategoryId,
                        ImageUrls = dto.ImageUrls ?? new List<string>(),
                        ThumbnailUrl = dto.ThumbnailUrl,
                        EventDate = dto.EventDate ?? DateTime.UtcNow,
                        EventEndDate = dto.EventEndDate,
                        EventLocation = dto.EventLocation ?? string.Empty,
                        MaxTickets = dto.MaxTickets ?? 100
                    };
                    break;
                default:
                    return null;
            }

            return product;
        }

        private string GetUserId()
        {
            return User.FindFirstValue("profileId") ?? string.Empty;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = GetUserId();
            var products = await _productRepository.GetCreatorProducts(userId);
            return Ok(products);
        }

        // GET: api/Products/creator/{creatorId}
        [HttpGet("creator/{creatorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCreatorProducts(string creatorId)
        {
            var products = await _productRepository.GetCreatorProducts(creatorId);
            // Filter only active products for public view
            var activeProducts = products.Where(p => p.Status == ProductStatus.Active).ToList();
            return Ok(activeProducts);
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            // Allow viewing if user is creator or product is public active
            if (product.CreatorProfileId != userId && product.Status != ProductStatus.Active)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Products
        // Erstellt ein neues Produkt über DTO
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Tribe.DTOs.ProductDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Product DTO required" });

            var product = MapDtoToDomain(dto);
            if (product == null) return BadRequest(new { error = "Unsupported product type" });

            var validationErrors = ProductValidator.ValidateProduct(product);
            if (validationErrors.Count > 0)
                return BadRequest(new { errors = validationErrors.Select(e => e.ErrorMessage) });

            product.CreatorProfileId = GetUserId();
            var productId = await _productRepository.CreateProductAsync(product);

            _logger.LogInformation("Product created: {ProductId} by {UserId}", productId, product.CreatorProfileId);
            return CreatedAtAction(nameof(GetProduct), new { id = productId }, product);
        }

        // PUT: api/Products/{id}
        // Aktualisiert ein Produkt über DTO. Der Typ wird automatisch erkannt.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Tribe.DTOs.ProductDto dto)
        {
            if (dto == null)
                return BadRequest(new { error = "Product DTO required" });

            if (!string.IsNullOrEmpty(dto.Id) && dto.Id != id)
            {
                return BadRequest(new { error = "Product ID mismatch" });
            }

            var existing = await _productRepository.GetProductByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            var product = MapDtoToDomain(dto);
            if (product == null)
                return BadRequest(new { error = "Unable to determine product type" });

            product.Id = id;
            product.CreatorProfileId = existing.CreatorProfileId;
            product.CreatedAt = existing.CreatedAt;
            product.ViewCount = existing.ViewCount;
            product.PurchaseCount = existing.PurchaseCount;
            product.TotalRevenue = existing.TotalRevenue;
            product.Status = dto.Status ?? existing.Status;
            product.Tags = dto.Tags != null && dto.Tags.Count > 0 ? dto.Tags : existing.Tags;
            product.ImageUrls = dto.ImageUrls != null && dto.ImageUrls.Count > 0 ? dto.ImageUrls : existing.ImageUrls;
            product.ThumbnailUrl = dto.ThumbnailUrl ?? existing.ThumbnailUrl;
            product.CategoryId = dto.CategoryId ?? existing.CategoryId;
            product.SeoTitle = dto.SeoTitle ?? existing.SeoTitle;
            product.SeoDescription = dto.SeoDescription ?? existing.SeoDescription;

            var validationErrors = ProductValidator.ValidateProduct(product);
            if (validationErrors.Count > 0)
                return BadRequest(new { errors = validationErrors.Select(e => e.ErrorMessage) });

            var userId = GetUserId();
            if (product.CreatorProfileId != userId)
            {
                return Forbid();
            }

            var result = await _productRepository.UpdateProductAsync(product);
            if (!result)
            {
                return NotFound();
            }

            _logger.LogInformation("Product updated: {ProductId}", id);
            return NoContent();
        }
        private ShopProduct? DeserializeToShopProduct(System.Text.Json.JsonElement el)
        {
            var opts = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (el.ValueKind != System.Text.Json.JsonValueKind.Object)
                return null;

            if (el.TryGetProperty("SKU", out _) || el.TryGetProperty("StockQuantity", out _))
            {
                return System.Text.Json.JsonSerializer.Deserialize<PhysicalProduct>(el.GetRawText(), opts);
            }
            if (el.TryGetProperty("VideoUrl", out _))
            {
                return System.Text.Json.JsonSerializer.Deserialize<VideoProduct>(el.GetRawText(), opts);
            }
            if (el.TryGetProperty("HighResImageUrls", out _) || el.TryGetProperty("ImageFormat", out _))
            {
                return System.Text.Json.JsonSerializer.Deserialize<ImageProduct>(el.GetRawText(), opts);
            }
            if (el.TryGetProperty("DurationMinutes", out _))
            {
                return System.Text.Json.JsonSerializer.Deserialize<ServiceProduct>(el.GetRawText(), opts);
            }
            if (el.TryGetProperty("EventDate", out _))
            {
                return System.Text.Json.JsonSerializer.Deserialize<EventTicketProduct>(el.GetRawText(), opts);
            }

            // Fallback: try PhysicalProduct
            return System.Text.Json.JsonSerializer.Deserialize<PhysicalProduct>(el.GetRawText(), opts);
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (product.CreatorProfileId != userId)
            {
                return Forbid();
            }

            var result = await _productRepository.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }

            _logger.LogInformation("Product deleted: {ProductId}", id);
            return NoContent();
        }
    }
}