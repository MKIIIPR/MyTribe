using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http.Headers;
using System.Net.Http.Json; // New using statement for JSON extensions
using System.Text.Json;

namespace Tribe.Client.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);

        // Added for UPDATE (PUT)
        Task<bool> PutAsync<TRequest>(string endpoint, TRequest data);

        // Added for DELETE (DELETE)
        Task<bool> DeleteAsync(string endpoint);

        void SetAuthToken(string token);
        void RemoveAuthToken();
    }


    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService>? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger ?? NullLogger<ApiService>.Instance;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void RemoveAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("GET {Endpoint} returned {StatusCode}: {Content}", endpoint, response.StatusCode, content);
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAsync failed for {Endpoint}", endpoint);
                return default;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("POST {Endpoint} returned {StatusCode}: {Content}", endpoint, response.StatusCode, content);
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PostAsync failed for {Endpoint}", endpoint);
                return default;
            }
        }

        /// <summary>
        /// Sends a PUT request with JSON data to the specified endpoint.
        /// </summary>
        /// <typeparam name="TRequest">The type of the object to send.</typeparam>
        /// <param name="endpoint">The API endpoint URL.</param>
        /// <param name="data">The object to serialize and send.</param>
        /// <returns>A boolean indicating success or failure.</returns>
        public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("PUT {Endpoint} returned {StatusCode}: {Content}", endpoint, response.StatusCode, content);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PutAsync failed for {Endpoint}", endpoint);
                return false;
            }
        }

        /// <summary>
        /// Sends a DELETE request to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The API endpoint URL.</param>
        /// <returns>A boolean indicating success or failure.</returns>
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("DELETE {Endpoint} returned {StatusCode}: {Content}", endpoint, response.StatusCode, content);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed for {Endpoint}", endpoint);
                return false;
            }
        }
    }
}