using Tribe.Client.Services;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface IProductClientService
    {
        void SetAuthToken(string token);
        void RemoveAuthToken();
        Task<List<ShopProduct>> GetMyProductsAsync();
        Task<ShopProduct?> GetProductByIdAsync(string productId);
        Task<ShopProduct?> CreateProductAsync<T>(T product) where T : ShopProduct;
        Task<bool> UpdateProductAsync<T>(T product) where T : ShopProduct;
        Task<bool> DeleteProductAsync(string productId);
    }

    public class ProductClientService : IProductClientService
    {
        private readonly IApiService _apiService;
        private const string ProductsEndpoint = "api/Products";

        public ProductClientService(IApiService apiService)
        {
            _apiService = apiService;
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
            var products = await _apiService.GetAsync<List<ShopProduct>>(ProductsEndpoint);
            return products ?? new List<ShopProduct>();
        }

        public async Task<ShopProduct?> GetProductByIdAsync(string productId)
        {
            return await _apiService.GetAsync<ShopProduct>($"{ProductsEndpoint}/{productId}");
        }

        public async Task<ShopProduct?> CreateProductAsync<T>(T product) where T : ShopProduct
        {
            return await _apiService.PostAsync<T, ShopProduct>(ProductsEndpoint, product);
        }

        public async Task<bool> UpdateProductAsync<T>(T product) where T : ShopProduct
        {
            return await _apiService.PutAsync($"{ProductsEndpoint}/{product.Id}", product);
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            return await _apiService.DeleteAsync($"{ProductsEndpoint}/{productId}");
        }
    }
}