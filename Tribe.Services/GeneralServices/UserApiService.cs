using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Tribe.Bib.Models.TribeRelated;

namespace Tribe.Client.Services
{


    public interface IUserApiService
    {
        Task<TribeUser> CreateProfileAsync(TribeUser profile);
        Task<TribeUser?> GetProfileAsync();
        Task<TribeUser?> UpdateAsync(TribeUser data);
    }
    public class UserApiService : IUserApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserApiService>? _logger;
        private readonly HttpClient _httpClient;

        public UserApiService(IApiService apiService, HttpClient httpClient, ILogger<UserApiService>? logger = null)
        {
            _apiService = apiService;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<TribeUser?> GetProfileAsync()
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/own/state");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    return JsonSerializer.Deserialize<TribeUser>(responseJson, options);
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
        public async Task<TribeUser> CreateProfileAsync(TribeUser profile)
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
                    return JsonSerializer.Deserialize<TribeUser>(responseJson, options);
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
                _logger?.LogError(ex, "CreateAsync failed for {EntityType}", typeof(TribeUser).Name);
                return default;
            }
        }
        public async Task<TribeUser?> UpdateAsync(TribeUser data)
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
                    return JsonSerializer.Deserialize<TribeUser>(responseJson, options);
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
                _logger?.LogError(ex, "UpdateAsync failed for {EntityType} with id {Id}", typeof(TribeUser).Name, data.Id);
                return default;
            }
        }
            }
        }
