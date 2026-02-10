using System.Threading.Tasks;
using Tribe.Client.Services;
using Tribe.Bib.Models.TribeRelated;

namespace Tribe.Services.ClientServices.ShopServices
{
    /// <summary>
    /// DTO für öffentliche Creator-Daten (Landing Page)
    /// </summary>
    public class CreatorProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? CreatorName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? Bio { get; set; }
        public bool VerifiedCreator { get; set; }
        public int FollowerCount { get; set; }
        public int TotalRaffles { get; set; }
        public int TotalTokensDistributed { get; set; }

        // Social Links
        public string? PatreonUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? DiscordUrl { get; set; }

        // Collaboration
        public bool AcceptingCollaborations { get; set; }
        public string? CollaborationInfo { get; set; }
    }

    public interface ICreatorProfileClientService
    {
        Task<CreatorProfileDto?> GetCreatorProfileAsync(string creatorId);
        Task<CreatorProfileDto?> GetCreatorByNameAsync(string creatorName);
        Task<bool> UpdateCreatorProfileAsync(UpdateCreatorProfileRequest request);
        Task<bool> IsCreatorNameAvailableAsync(string creatorName);
    }

    /// <summary>
    /// Request DTO for updating creator profile
    /// </summary>
    public class UpdateCreatorProfileRequest
    {
        public string? CreatorName { get; set; }
        public string? BannerUrl { get; set; }
        public string? ImageTemplateUrl { get; set; }
        public bool? AcceptingCollaborations { get; set; }
        public string? CollaborationInfo { get; set; }
        public string? PatreonUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? DiscordUrl { get; set; }
    }

    public class CreatorProfileClientService : ICreatorProfileClientService
    {
        private readonly IApiService _api;
        private const string BaseEndpoint = "api/shop/creators";

        public CreatorProfileClientService(IApiService api)
        {
            _api = api;
        }

        public async Task<CreatorProfileDto?> GetCreatorProfileAsync(string creatorId)
        {
            if (string.IsNullOrEmpty(creatorId))
            {
                Console.WriteLine("[CreatorProfileClientService] creatorId is null or empty");
                return null;
            }

            var endpoint = $"{BaseEndpoint}/{creatorId}";
            Console.WriteLine($"[CreatorProfileClientService] Calling GET {endpoint}");

            var result = await _api.GetAsync<CreatorProfileDto>(endpoint);

            Console.WriteLine($"[CreatorProfileClientService] Result: {(result != null ? $"Id={result.Id}" : "NULL")}");
            return result;
        }

        public Task<CreatorProfileDto?> GetCreatorByNameAsync(string creatorName)
        {
            if (string.IsNullOrEmpty(creatorName))
            {
                return Task.FromResult<CreatorProfileDto?>(null);
            }

            return _api.GetAsync<CreatorProfileDto>($"{BaseEndpoint}/name/{creatorName}");
        }

        public async Task<bool> UpdateCreatorProfileAsync(UpdateCreatorProfileRequest request)
        {
            try
            {
                return await _api.PutAsync($"{BaseEndpoint}/profile", request);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsCreatorNameAvailableAsync(string creatorName)
        {
            if (string.IsNullOrWhiteSpace(creatorName) || creatorName.Length < 3)
            {
                return false;
            }

            try
            {
                var result = await _api.GetAsync<NameAvailabilityResponse>($"{BaseEndpoint}/name-available/{creatorName}");
                return result?.Available ?? false;
            }
            catch
            {
                return false;
            }
        }

        private class NameAvailabilityResponse
        {
            public bool Available { get; set; }
        }
    }
}
