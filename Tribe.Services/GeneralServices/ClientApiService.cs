using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Tribe.Client.Services
{
    public interface IClientApiService
    {
        Task<List<T>?> GetAllAsync<T>(string endpoint);
        Task<T?> GetByIdAsync<T>(string id);        
        Task<T?> CreateAsync<T>(string endpoint, T data);
        Task<T?> UpdateAsync<T>(string endpoint, string id, T data);
        Task<bool> DeleteAsync(string endpoint, string id);
        Task<List<T>?> GetByParentIdAsync<T>(string endpoint, string parentId);
    }

    public class ClientApiService : IClientApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ClientApiService>? _logger;
        private readonly HttpClient _httpClient;

        public ClientApiService(IApiService apiService, HttpClient httpClient, ILogger<ClientApiService>? logger = null)
        {
            _apiService = apiService;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<T?> GetByIdAsync<T>(string id)
        {
            try
            {
                var entityType = typeof(T).Name.ToLower();
                var response = await _httpClient.GetAsync($"api/genericapi/get/{entityType}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    return JsonSerializer.Deserialize<T>(responseJson, options);
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
                _logger?.LogError(ex, "GetByIdAsync failed for {EntityType} with id {Id}", typeof(T).Name, id);
                return default;
            }
        }

        public async Task<List<T>?> GetAllAsync<T>(string endpoint)
        {
            try
            {
                var entityType = typeof(T).Name.ToLower();
                var response = await _httpClient.GetAsync($"api/genericapi/getall/{entityType}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    return JsonSerializer.Deserialize<List<T>>(responseJson, options);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("GetAllAsync API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetAllAsync failed for {EntityType}", typeof(T).Name);
                return default;
            }
        }

        public async Task<T?> CreateAsync<T>(string endpoint, T data)
        {
            try
            {
                var entityType = typeof(T).Name.ToLower();

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(data, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"api/genericapi/create/{entityType}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(responseJson, options);
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
                _logger?.LogError(ex, "CreateAsync failed for {EntityType}", typeof(T).Name);
                return default;
            }
        }

        public async Task<T?> UpdateAsync<T>(string endpoint, string id, T data)
        {
            try
            {
                var entityType = typeof(T).Name.ToLower();

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(data, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/genericapi/update/{entityType}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(responseJson, options);
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
                _logger?.LogError(ex, "UpdateAsync failed for {EntityType} with id {Id}", typeof(T).Name, id);
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, string id)
        {
            try
            {
                var entityType = endpoint.Split('/').LastOrDefault()?.ToLower() ?? "unknown";
                var response = await _httpClient.DeleteAsync($"api/genericapi/delete/{entityType}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("DeleteAsync API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "DeleteAsync failed for entity {EntityType} with id {Id}", endpoint, id);
                return false;
            }
        }

        public async Task<List<T>?> GetByParentIdAsync<T>(string endpoint, string parentId)
        {
            try
            {
                var entityType = typeof(T).Name.ToLower();
                var response = await _httpClient.GetAsync($"api/genericapi/getbyparent/{entityType}/{parentId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    return JsonSerializer.Deserialize<List<T>>(responseJson, options);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("GetByParentIdAsync API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetByParentIdAsync failed for {EntityType} with parentId {ParentId}", typeof(T).Name, parentId);
                return default;
            }
        }

        }
}
