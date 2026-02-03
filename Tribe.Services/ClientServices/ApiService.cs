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

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (Exception)
            {
                // General exception handling for network or serialization errors.
                return default;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                // Using PostAsJsonAsync for simpler and cleaner code
                var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (Exception)
            {
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}