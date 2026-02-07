using System.Threading.Tasks;
using Tribe.Client.Services;
using Tribe.Bib.Models.TribeRelated;

namespace Tribe.Services.ClientServices.ShopServices
{
    public interface ICreatorProfileClientService
    {
        Task<CreatorProfile?> GetCreatorProfileAsync(string creatorId);
    }

    public class CreatorProfileClientService : ICreatorProfileClientService
    {
        private readonly IApiService _api;
        private const string BaseEndpoint = "api/shop/creators";

        public CreatorProfileClientService(IApiService api)
        {
            _api = api;
        }

        public Task<CreatorProfile?> GetCreatorProfileAsync(string creatorId)
        {
            if (string.IsNullOrEmpty(creatorId))
            {
                return Task.FromResult<CreatorProfile?>(null);
            }

            return _api.GetAsync<CreatorProfile>($"{BaseEndpoint}/{creatorId}");
        }
    }
}
