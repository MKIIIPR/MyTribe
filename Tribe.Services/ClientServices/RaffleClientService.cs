using Tribe.Client.Services;
using Tribe.Bib.Models.TribeRelated;
using Microsoft.Extensions.Logging;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface IRaffleClientService
    {
        Task<List<Raffle>?> GetCreatorRafflesAsync();
        Task<List<Raffle>?> GetCreatorRafflesByCreatorIdAsync(string creatorId);
        Task<Raffle?> GetRaffleByIdAsync(string id);
        Task<Raffle?> GetProductRaffleAsync(string productId);
        Task<string?> CreateRaffleAsync(Raffle raffle);
        Task<bool> UpdateRaffleAsync(string id, Raffle raffle);
        Task<bool> DeleteRaffleAsync(string id);
        Task<bool> BindRaffleToProductAsync(string raffleId, string productId);
        Task<bool> AlreadyExistsAsync(string id);
    }

    public class RaffleClientService : IRaffleClientService
    {
        private readonly IApiService _api;
        private readonly ILogger<RaffleClientService>? _logger;
        private const string Base = "api/shop/raffles";

        public RaffleClientService(IApiService api, ILogger<RaffleClientService>? logger = null)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<List<Raffle>?> GetCreatorRafflesAsync()
        {
            try
            {
                return await _api.GetAsync<List<Raffle>>($"{Base}/creator/all");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetCreatorRafflesAsync failed");
                return null;
            }
        }

        public async Task<List<Raffle>?> GetCreatorRafflesByCreatorIdAsync(string creatorId)
        {
            if (string.IsNullOrEmpty(creatorId))
            {
                return new List<Raffle>();
            }

            try
            {
                return await _api.GetAsync<List<Raffle>>($"{Base}/creator/{creatorId}/public");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "GetCreatorRafflesAsync(creatorId) failed");
                return null;
            }
        }

        public async Task<Raffle?> GetRaffleByIdAsync(string id)
        {
            try
            {
                return await _api.GetAsync<Raffle>($"{Base}/{id}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetRaffleByIdAsync failed");
                return null;
            }
        }

        public async Task<string?> CreateRaffleAsync(Raffle raffle)
        {
            try
            {
                var res = await _api.PostAsync<Raffle, object>($"{Base}/create", raffle);
                // controller returns { raffleId = id, message = ... }
                if (res == null) return null;
                // attempt to read raffleId property via serialization
                var json = System.Text.Json.JsonSerializer.Serialize(res);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("raffleId", out var idEl))
                    return idEl.GetString();
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "CreateRaffleAsync failed");
                return null;
            }
        }

        public async Task<bool> UpdateRaffleAsync(string id, Raffle raffle)
        {
            try
            {
                return await _api.PutAsync($"{Base}/{id}", raffle);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "UpdateRaffleAsync failed");
                return false;
            }
        }

        public async Task<bool> DeleteRaffleAsync(string id)
        {
            try
            {
                return await _api.DeleteAsync($"{Base}/{id}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "DeleteRaffleAsync failed");
                return false;
            }
        }

        public async Task<bool> BindRaffleToProductAsync(string raffleId, string productId)
        {
            try
            {
                var endpoint = $"{Base}/bind/{raffleId}/product/{productId}";
                // POST with empty body
                var res = await _api.PostAsync<object, object>(endpoint, new { });
                return res != null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "BindRaffleToProductAsync failed");
                return false;
            }
        }

        public async Task<Raffle?> GetProductRaffleAsync(string productId)
        {
            try
            {
                return await _api.GetAsync<Raffle>($"{Base}/product/{productId}");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "GetProductRaffleAsync failed");
                return null;
            }
        }

        public async Task<bool> AlreadyExistsAsync(string id)
        {
            try
            {
                var res = await _api.GetAsync<bool>($"{Base}/alreadyexist/{id}");
                return res;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "AlreadyExistsAsync failed");
                return false;
            }
        }
        
    }
}
