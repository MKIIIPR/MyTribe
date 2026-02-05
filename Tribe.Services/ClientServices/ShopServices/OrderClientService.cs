using Tribe.Client.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface IOrderClientService
    {
        void SetAuthToken(string token);
        void RemoveAuthToken();
        Task<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>?> GetCreatorOrdersAsync();
        Task<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>?> GetMyOrdersAsync();
        Task<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder?> GetOrderByIdAsync(string id);
        Task<bool> UpdateOrderStatusAsync(string id, string newStatus);
    }

    public class OrderClientService : IOrderClientService
    {
        private readonly IApiService _apiService;
        private const string OrdersEndpoint = "api/shop/orders";

        public OrderClientService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public void SetAuthToken(string token) => _apiService.SetAuthToken(token);
        public void RemoveAuthToken() => _apiService.RemoveAuthToken();

        public async Task<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>?> GetCreatorOrdersAsync()
        {
            return await _apiService.GetAsync<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>>($"{OrdersEndpoint}/creator");
        }

        public async Task<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>?> GetMyOrdersAsync()
        {
            return await _apiService.GetAsync<List<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>>($"{OrdersEndpoint}/my");
        }

        public async Task<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder?> GetOrderByIdAsync(string id)
        {
            return await _apiService.GetAsync<Tribe.Bib.ShopRelated.ShopStruckture.ShopOrder>($"{OrdersEndpoint}/{id}");
        }

        public async Task<bool> UpdateOrderStatusAsync(string id, string newStatus)
        {
            return await _apiService.PutAsync($"{OrdersEndpoint}/{id}/status", newStatus);
        }
    }
}
