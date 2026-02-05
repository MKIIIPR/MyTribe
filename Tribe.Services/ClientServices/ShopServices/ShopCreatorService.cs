using Tribe.Client.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tribe.Bib.ShopRelated;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface IShopCreatorService
    {
        // Products
        Task<List<ShopProduct>> GetMyProductsAsync();
        Task<ShopProduct?> GetProductByIdAsync(string id);
        Task<ShopProduct?> CreateProductAsync<T>(T product) where T : ShopProduct;
        Task<bool> UpdateProductAsync<T>(T product) where T : ShopProduct;
        Task<bool> DeleteProductAsync(string productId);

        // Categories
        Task<List<ShopCategory>> GetMyCategoriesAsync();
        Task<ShopCategory?> GetCategoryByIdAsync(string id);
        Task<ShopCategory?> CreateCategoryAsync(ShopCategory category);
        Task<bool> UpdateCategoryAsync(ShopCategory category);
        Task<bool> DeleteCategoryAsync(string id);

        // Orders
        Task<List<ShopOrder>?> GetCreatorOrdersAsync();
        Task<ShopOrder?> GetOrderByIdAsync(string id);
        Task<bool> UpdateOrderStatusAsync(string id, string newStatus);

        // Raffles
        Task<List<dynamic>?> GetMyRafflesAsync();
        Task<dynamic?> CreateRaffleAsync(object raffle);
        Task<bool> UpdateRaffleAsync(string id, object raffle);
        Task<bool> DeleteRaffleAsync(string id);
    }

    public class ShopCreatorService : IShopCreatorService
    {
        private readonly IApiService _api;
        private const string ProductsEndpoint = "api/Products";
        private const string CategoriesEndpoint = "api/shop/categories";
        private const string OrdersEndpoint = "api/shop/orders";
        private const string RafflesEndpoint = "api/shop/raffles";

        public ShopCreatorService(IApiService api)
        {
            _api = api;
        }

        // Products
        public async Task<List<ShopProduct>> GetMyProductsAsync()
        {
            var res = await _api.GetAsync<List<ShopProduct>>(ProductsEndpoint);
            return res ?? new List<ShopProduct>();
        }

        public Task<ShopProduct?> GetProductByIdAsync(string id) => _api.GetAsync<ShopProduct>($"{ProductsEndpoint}/{id}");

        public Task<ShopProduct?> CreateProductAsync<T>(T product) where T : ShopProduct
            => _api.PostAsync<T, ShopProduct>(ProductsEndpoint, product);

        public Task<bool> UpdateProductAsync<T>(T product) where T : ShopProduct
            => _api.PutAsync($"{ProductsEndpoint}/{product.Id}", product);

        public Task<bool> DeleteProductAsync(string productId) => _api.DeleteAsync($"{ProductsEndpoint}/{productId}");

        // Categories
        public async Task<List<ShopCategory>> GetMyCategoriesAsync()
        {
            var res = await _api.GetAsync<List<ShopCategory>>(CategoriesEndpoint);
            return res ?? new List<ShopCategory>();
        }

        public Task<ShopCategory?> GetCategoryByIdAsync(string id) => _api.GetAsync<ShopCategory>($"{CategoriesEndpoint}/{id}");

        public Task<ShopCategory?> CreateCategoryAsync(ShopCategory category)
            => _api.PostAsync<ShopCategory, ShopCategory>(CategoriesEndpoint, category);

        public Task<bool> UpdateCategoryAsync(ShopCategory category)
            => _api.PutAsync($"{CategoriesEndpoint}/{category.Id}", category);

        public Task<bool> DeleteCategoryAsync(string id) => _api.DeleteAsync($"{CategoriesEndpoint}/{id}");

        // Orders
        public Task<List<ShopOrder>?> GetCreatorOrdersAsync() => _api.GetAsync<List<ShopOrder>>($"{OrdersEndpoint}/creator");

        public Task<ShopOrder?> GetOrderByIdAsync(string id) => _api.GetAsync<ShopOrder>($"{OrdersEndpoint}/{id}");

        public Task<bool> UpdateOrderStatusAsync(string id, string newStatus) => _api.PutAsync($"{OrdersEndpoint}/{id}/status", newStatus);

        // Raffles (dynamic because model may live elsewhere)
        public Task<List<dynamic>?> GetMyRafflesAsync() => _api.GetAsync<List<dynamic>>($"{RafflesEndpoint}/creator/all");
        public Task<dynamic?> CreateRaffleAsync(object raffle) => _api.PostAsync<object, dynamic>($"{RafflesEndpoint}/create", raffle);
        public Task<bool> UpdateRaffleAsync(string id, object raffle) => _api.PutAsync($"{RafflesEndpoint}/{id}", raffle);
        public Task<bool> DeleteRaffleAsync(string id) => _api.DeleteAsync($"{RafflesEndpoint}/{id}");
    }
}
