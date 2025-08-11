using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task<bool> LogoutAsync();
        Task<UserInfo?> GetCurrentUserAsync();
        string? GetStoredToken();
        void StoreToken(string token);
        void RemoveToken();
        bool IsTokenExpired(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private const string TOKEN_KEY = "authToken";

        public AuthService(IApiService apiService, IJSRuntime jsRuntime)
        {
            _apiService = apiService;
            _jsRuntime = jsRuntime;
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var loginRequest = new LoginRequest { Email = email, Password = password };
            var response = await _apiService.PostAsync<LoginRequest, LoginResponse>("api/auth/login", loginRequest);

            if (response != null)
            {
                StoreToken(response.Token);
                _apiService.SetAuthToken(response.Token);
            }

            return response;
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var token = GetStoredToken();
                if (!string.IsNullOrEmpty(token))
                {
                    _apiService.SetAuthToken(token);
                    await _apiService.PostAsync<object, object>("api/auth/logout", new { });
                }

                RemoveToken();
                _apiService.RemoveAuthToken();
                return true;
            }
            catch
            {
                RemoveToken();
                _apiService.RemoveAuthToken();
                return false;
            }
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            var token = GetStoredToken();
            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                return null;
            }

            _apiService.SetAuthToken(token);
            return await _apiService.GetAsync<UserInfo>("api/auth/user");
        }

        public string? GetStoredToken()
        {
            try
            {
                return _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        public void StoreToken(string token)
        {
            _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
        }

        public void RemoveToken()
        {
            _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}
