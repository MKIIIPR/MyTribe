using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Tribe.Bib.Models.TribeRelated;

namespace Tribe.Client.Services
{


    public interface IUserApiService
    {
        Task<TribeProfile> CreateProfileAsync(TribeProfile profile);
        Task<TribeProfile?> GetProfileAsync();
        Task<TribeProfile?> UpdateAsync(TribeProfile data);
    }
    public class UserApiService : IUserApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ClientApiService>? _logger;
        private readonly HttpClient _httpClient;

        public UserApiService(IApiService apiService, ILogger<ClientApiService>? logger = null, HttpClient? httpClient = null)
        {
            _apiService = apiService;
            _logger = logger;
            _httpClient = httpClient ?? GetHttpClientFromApiService();
        }

        public async Task<TribeProfile?> GetProfileAsync()
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/own/state");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    return JsonSerializer.Deserialize<TribeProfile>(responseJson, options);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("GetByIdAsync API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetByIdAsync failed for {EntityType} with id {Id}");
                return default;
            }
        }



        public async Task<TribeProfile> CreateProfileAsync(TribeProfile profile)
        {
            try
            {


                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(profile, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"api/own/state", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TribeProfile>(responseJson, options);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("CreateAsync API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "CreateAsync failed for {EntityType}", typeof(TribeProfile).Name);
                return default;
            }
        }

        public async Task<TribeProfile?> UpdateAsync(TribeProfile data)
        {
            try
            {

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(data, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/own/update", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TribeProfile>(responseJson, options);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("UpdateAsync API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "UpdateAsync failed for {EntityType} with id {Id}", typeof(TribeProfile).Name, data.Id);
                return default;
            }
        }


        private HttpClient GetHttpClientFromApiService()
        {
            var field = _apiService.GetType().GetField("_httpClient",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                _logger?.LogWarning("HttpClient field not found in IApiService");
                throw new InvalidOperationException("HttpClient field not found in IApiService");
            }
            return (HttpClient)field.GetValue(_apiService)!;
        }
    }
}
