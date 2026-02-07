using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tribe.Client.Services;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface IProductClientService
    {
        void SetAuthToken(string token);
        void RemoveAuthToken();
        Task<List<ShopProduct>> GetMyProductsAsync();
        Task<List<ShopProduct>> GetCreatorProductsAsync(string creatorId);
        Task<ShopProduct?> GetProductByIdAsync(string productId);
        Task<ShopProduct?> CreateProductAsync<T>(T product) where T : ShopProduct;
        Task<bool> UpdateProductAsync(string productId, ProductDto dto);
        Task<bool> DeleteProductAsync(string productId);
    }

    public class ProductClientService : IProductClientService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ProductClientService> _logger;
        private const string ProductsEndpoint = "api/Products";

        public ProductClientService(IApiService apiService, ILogger<ProductClientService>? logger = null)
        {
            _apiService = apiService;
            _logger = logger ?? NullLogger<ProductClientService>.Instance;
        }

        public void SetAuthToken(string token)
        {
            _apiService.SetAuthToken(token);
        }

        public void RemoveAuthToken()
        {
            _apiService.RemoveAuthToken();
        }

        public async Task<List<ShopProduct>> GetMyProductsAsync()
        {
            // Read raw JSON elements and map to concrete ShopProduct implementations
            try
            {
                var elements = await _apiService.GetAsync<List<System.Text.Json.JsonElement>>(ProductsEndpoint);
                var list = new List<ShopProduct>();
                if (elements == null)
                {
                    _logger.LogWarning("GetMyProductsAsync: API returned null elements for {Endpoint}", ProductsEndpoint);
                    // fallback: try direct typed deserialization
                    var direct = await _apiService.GetAsync<List<ShopProduct>>(ProductsEndpoint);
                    if (direct != null)
                    {
                        return direct;
                    }
                    return list;
                }

                var opts = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                foreach (var el in elements)
                {
                    try
                    {
                        ShopProduct? p = null;
                        if (el.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            if (el.TryGetProperty("SKU", out _) || el.TryGetProperty("StockQuantity", out _))
                                p = System.Text.Json.JsonSerializer.Deserialize<PhysicalProduct>(el.GetRawText(), opts);
                            else if (el.TryGetProperty("VideoUrl", out _))
                                p = System.Text.Json.JsonSerializer.Deserialize<VideoProduct>(el.GetRawText(), opts);
                            else if (el.TryGetProperty("HighResImageUrls", out _) || el.TryGetProperty("ImageFormat", out _))
                                p = System.Text.Json.JsonSerializer.Deserialize<ImageProduct>(el.GetRawText(), opts);
                            else if (el.TryGetProperty("DurationMinutes", out _))
                                p = System.Text.Json.JsonSerializer.Deserialize<ServiceProduct>(el.GetRawText(), opts);
                            else if (el.TryGetProperty("EventDate", out _))
                                p = System.Text.Json.JsonSerializer.Deserialize<EventTicketProduct>(el.GetRawText(), opts);
                            else
                                p = System.Text.Json.JsonSerializer.Deserialize<PhysicalProduct>(el.GetRawText(), opts);
                        }

                        if (p != null) list.Add(p);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize product element");
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyProductsAsync failed");
                return new List<ShopProduct>();
            }
        }

        public async Task<ShopProduct?> GetProductByIdAsync(string productId)
        {
            var el = await _apiService.GetAsync<System.Text.Json.JsonElement?>($"{ProductsEndpoint}/{productId}");
            if (el == null || el.Value.ValueKind != System.Text.Json.JsonValueKind.Object) return null;
            var opts = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var je = el.Value;
            try
            {
                if (je.TryGetProperty("SKU", out _) || je.TryGetProperty("StockQuantity", out _))
                    return System.Text.Json.JsonSerializer.Deserialize<PhysicalProduct>(je.GetRawText(), opts);
                if (je.TryGetProperty("VideoUrl", out _))
                    return System.Text.Json.JsonSerializer.Deserialize<VideoProduct>(je.GetRawText(), opts);
                if (je.TryGetProperty("HighResImageUrls", out _) || je.TryGetProperty("ImageFormat", out _))
                    return System.Text.Json.JsonSerializer.Deserialize<ImageProduct>(je.GetRawText(), opts);
                if (je.TryGetProperty("DurationMinutes", out _))
                    return System.Text.Json.JsonSerializer.Deserialize<ServiceProduct>(je.GetRawText(), opts);
                if (je.TryGetProperty("EventDate", out _))
                    return System.Text.Json.JsonSerializer.Deserialize<EventTicketProduct>(je.GetRawText(), opts);

                return System.Text.Json.JsonSerializer.Deserialize<PhysicalProduct>(je.GetRawText(), opts);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ShopProduct?> CreateProductAsync<T>(T product) where T : ShopProduct
        {
            return await _apiService.PostAsync<T, ShopProduct>(ProductsEndpoint, product);
        }

        public async Task<bool> UpdateProductAsync(string productId, ProductDto dto)
        {
            if (string.IsNullOrEmpty(productId))
            {
                _logger.LogWarning("UpdateProductAsync called without productId");
                return false;
            }

            return await _apiService.PutAsync($"{ProductsEndpoint}/{productId}", dto);
        }

        public async Task<List<ShopProduct>> GetCreatorProductsAsync(string creatorId)
        {
            if (string.IsNullOrEmpty(creatorId))
            {
                return new List<ShopProduct>();
            }

            var result = await _apiService.GetAsync<List<ShopProduct>>($"{ProductsEndpoint}/creator/{creatorId}");
            return result ?? new List<ShopProduct>();
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            return await _apiService.DeleteAsync($"{ProductsEndpoint}/{productId}");
        }
    }
}