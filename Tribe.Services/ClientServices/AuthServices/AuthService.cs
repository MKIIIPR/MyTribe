// ===== DEPRECATED - NICHT MEHR VERWENDEN =====
// Diese Klasse wurde durch SimplifiedAuthService ersetzt
// IHttpContextAccessor funktioniert nicht in Blazor WebAssembly
// 
// Verwende stattdessen: Tribe.Services.ClientServices.SimpleAuth.SimplifiedAuthService

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using Tribe.Services.ClientServices.SimpleAuth;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Client.Services
{
    /// <summary>
    /// DEPRECATED: Verwende SimplifiedAuthService stattdessen.
    /// Diese Klasse nutzt IHttpContextAccessor, was in Blazor WASM nicht funktioniert.
    /// </summary>
    [Obsolete("Verwende SimplifiedAuthService stattdessen")]
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private const string COOKIE_KEY = "jwt_token";
        
        public event Action<UserInfo?>? OnUserLoggedIn;
        public event Action? OnUserLoggedOut;

        public AuthService(IApiService apiService, IJSRuntime jsRuntime, IHttpContextAccessor? httpContextAccessor = null)
        {
            _apiService = apiService;
            _jsRuntime = jsRuntime;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var loginRequest = new LoginRequest { Email = email, Password = password };
            var response = await _apiService.PostAsync<LoginRequest, LoginResponse>("api/auth/login", loginRequest);

            if (response != null)
            {
                _apiService.SetAuthToken(response.Token);
                OnUserLoggedIn?.Invoke(null);
            }

            return response;
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _apiService.SetAuthToken(token);
                    await _apiService.PostAsync<object, object>("api/auth/logout", new { });
                }

                _apiService.RemoveAuthToken();
                OnUserLoggedOut?.Invoke();
                return true;
            }
            catch
            {
                _apiService.RemoveAuthToken();
                OnUserLoggedOut?.Invoke();
                return false;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("eval",
                    "document.cookie.split('; ').find(row => row.startsWith('jwt_token='))?.split('=')[1]");
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
                {
                    return null;
                }

                _apiService.SetAuthToken(token);
                return await _apiService.GetAsync<UserInfo>("api/auth/user");
            }
            catch
            {
                return null;
            }
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