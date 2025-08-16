using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using Tribe.Services.ClientServices.SimpleAuth;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Client.Services
{

    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TOKEN_KEY = "authToken";
        private const string COOKIE_KEY = "jwt_token";
        public event Action<UserInfo?>? OnUserLoggedIn;
        public event Action? OnUserLoggedOut;
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

                // Token aus localStorage entfernen
                await RemoveTokenAsync();
                _apiService.RemoveAuthToken();

                // WICHTIG: Zur Logout-Seite navigieren, die den Cookie serverseitig entfernt
                var navigationManager = _httpContextAccessor.HttpContext?.RequestServices.GetService<NavigationManager>();
                navigationManager?.NavigateTo("/Account/Logout", forceLoad: true);

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
            // PRIMÄR: Cookie verwenden (funktioniert immer in Blazor Server)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request.Cookies.TryGetValue(COOKIE_KEY, out var cookieToken) == true)
            {
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    // Versuchen, auch in localStorage zu speichern (falls JS verfügbar)
                    await TryStoreInLocalStorageAsync(cookieToken);
                    return cookieToken;
                }
            }

            // SEKUNDÄR: localStorage versuchen (falls verfügbar)
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }
            }
            catch
            {
                // localStorage nicht verfügbar - das ist OK, Cookie funktioniert
            }

            return null;
        }

        public async Task StoreTokenAsync(string token)
        {
            // Nur localStorage versuchen - Cookie wird vom Server gesetzt
            await TryStoreInLocalStorageAsync(token);
        }

        private async Task TryStoreInLocalStorageAsync(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
            }
            catch
            {
                // localStorage nicht verfügbar - das ist OK, Cookie funktioniert
            }
        }

        public async Task RemoveTokenAsync()
        {
            // localStorage versuchen zu leeren
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            }
            catch
            {
                // Nicht kritisch
            }

            // Cookie via JavaScript entfernen (funktioniert besser als Response.Cookies.Delete)
            try
            {
                await _jsRuntime.InvokeVoidAsync("eval",
                    $"document.cookie = '{COOKIE_KEY}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Strict'");
            }
            catch
            {
                // JavaScript Cookie-Entfernung fehlgeschlagen
                // Versuche als Fallback Response.Cookies (funktioniert nur in bestimmten Kontexten)
                try
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        httpContext.Response.Cookies.Delete(COOKIE_KEY, new CookieOptions
                        {
                            Path = "/",
                            SameSite = SameSiteMode.Strict
                        });
                    }
                }
                catch
                {
                    // Auch das hat nicht funktioniert - nicht kritisch
                }
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