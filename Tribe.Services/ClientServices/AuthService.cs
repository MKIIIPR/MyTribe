using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task<bool> LogoutAsync();
        Task<UserInfo?> GetCurrentUserAsync();
        Task<string?> GetStoredTokenAsync();
        Task StoreTokenAsync(string token);
        Task RemoveTokenAsync();
        bool IsTokenExpired(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TOKEN_KEY = "authToken";
        private const string COOKIE_KEY = "jwt_token";

        public AuthService(IApiService apiService, IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor)
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
                await StoreTokenAsync(response.Token);
                _apiService.SetAuthToken(response.Token);
            }

            return response;
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var token = await GetStoredTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _apiService.SetAuthToken(token);
                    await _apiService.PostAsync<object, object>("api/auth/logout", new { });
                }

                await RemoveTokenAsync();
                _apiService.RemoveAuthToken();
                return true;
            }
            catch
            {
                await RemoveTokenAsync();
                _apiService.RemoveAuthToken();
                return false;
            }
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            var token = await GetStoredTokenAsync();
            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                return null;
            }

            _apiService.SetAuthToken(token);
            return await _apiService.GetAsync<UserInfo>("api/auth/user");
        }

        public async Task<string?> GetStoredTokenAsync()
        {
            try
            {
                // Zuerst versuchen, Token aus localStorage zu holen
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);

                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }

                // Falls localStorage nicht verfügbar oder leer, aus Cookie lesen
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Request.Cookies.TryGetValue(COOKIE_KEY, out var cookieToken) == true)
                {
                    // Token auch in localStorage speichern für zukünftige Verwendung
                    if (!string.IsNullOrEmpty(cookieToken))
                    {
                        try
                        {
                            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, cookieToken);
                        }
                        catch
                        {
                            // localStorage nicht verfügbar, aber Cookie funktioniert
                        }
                    }
                    return cookieToken;
                }

                return null;
            }
            catch
            {
                // Fallback: Nur Cookie verwenden
                try
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext?.Request.Cookies.TryGetValue(COOKIE_KEY, out var cookieToken) == true)
                    {
                        return cookieToken;
                    }
                }
                catch
                {
                    // Alles fehlgeschlagen
                }
                return null;
            }
        }

        public async Task StoreTokenAsync(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
            }
            catch
            {
                // localStorage nicht verfügbar - Token ist hoffentlich im Cookie gespeichert
            }
        }

        public async Task RemoveTokenAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            }
            catch
            {
                // localStorage nicht verfügbar
            }

            // Cookie auch entfernen
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Response.Cookies.Delete(COOKIE_KEY);
                }
            }
            catch
            {
                // Cookie konnte nicht entfernt werden
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