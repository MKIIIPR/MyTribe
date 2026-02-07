using Tribe.Client.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tribe.Bib.ShopRelated;
using Tribe.Bib.Models.TribeRelated;
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
        Task<List<Raffle>?> GetMyRafflesAsync();
        Task<Raffle?> CreateRaffleAsync(Raffle raffle);
        Task<bool> UpdateRaffleAsync(string id, Raffle raffle);
        Task<bool> DeleteRaffleAsync(string id);
    }

    public class ShopCreatorService : IShopCreatorService
    {
        private readonly IApiService _api;
        private readonly IRaffleClientService _raffleService;
        private const string ProductsEndpoint = "api/Products";
        private const string CategoriesEndpoint = "api/shop/categories";
        private const string OrdersEndpoint = "api/shop/orders";

        public ShopCreatorService(IApiService api, IRaffleClientService raffleService)
        {
            _api = api;
            _raffleService = raffleService;
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

        // Raffles — delegiert an IRaffleClientService
        public Task<List<Raffle>?> GetMyRafflesAsync() => _raffleService.GetCreatorRafflesAsync();
        public async Task<Raffle?> CreateRaffleAsync(Raffle raffle)
        {
            var id = await _raffleService.CreateRaffleAsync(raffle);
            if (id == null) return null;
            raffle.Id = id;
            return raffle;
        }
        public Task<bool> UpdateRaffleAsync(string id, Raffle raffle) => _raffleService.UpdateRaffleAsync(id, raffle);
        public Task<bool> DeleteRaffleAsync(string id) => _raffleService.DeleteRaffleAsync(id);
    }
}
