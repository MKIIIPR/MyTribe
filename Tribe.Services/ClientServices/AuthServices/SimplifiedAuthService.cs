// ===== SIMPLIFIED AUTH SERVICE - COOKIE-ONLY =====
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tribe.Client.Services;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Services.ClientServices.SimpleAuth
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task<bool> LogoutAsync();
        Task<string?> GetTokenAsync();
        Task<UserInfo?> GetCurrentUserAsync();
        bool IsTokenExpired(string token);
        event Action<UserInfo?> OnUserLoggedIn;
        event Action OnUserLoggedOut;
    }

    public class SimplifiedAuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<SimplifiedAuthService> _logger;
        
        public event Action<UserInfo?>? OnUserLoggedIn;
        public event Action? OnUserLoggedOut;

        public SimplifiedAuthService(
            IApiService apiService,
            IJSRuntime jsRuntime,
            ILogger<SimplifiedAuthService> logger)
        {
            _apiService = apiService;
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        /// <summary>
        /// Liest das JWT-Token aus dem Cookie
        /// </summary>
        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("eval",
                    "document.cookie.split('; ').find(row => row.startsWith('jwt_token='))?.split('=')[1]");

                if (!string.IsNullOrEmpty(token))
                {
                    if (IsTokenExpired(token))
                    {
                        _logger.LogWarning("Token is expired");
                        return null;
                    }
                    _logger.LogDebug("Token retrieved from cookie");
                }

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving token from cookie");
                return null;
            }
        }

        /// <summary>
        /// Holt die aktuellen User-Informationen vom Server
        /// </summary>
        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                _apiService.SetAuthToken(token);
                return await _apiService.GetAsync<UserInfo>("api/auth/user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user");
                return null;
            }
        }

        /// <summary>
        /// Login - Cookie wird vom Server gesetzt
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            _logger.LogInformation("Login attempt for: {Email}", email);

            try
            {
                var loginRequest = new LoginRequest { Email = email, Password = password };
                var response = await _apiService.PostAsync<LoginRequest, LoginResponse>("api/auth/login", loginRequest);

                if (response != null)
                {
                    _logger.LogInformation("Login successful for: {Email}", email);
                    
                    // Token für API-Aufrufe setzen
                    _apiService.SetAuthToken(response.Token);
                    
                    // User-Info laden und Event auslösen
                    var userInfo = await GetCurrentUserAsync();
                    OnUserLoggedIn?.Invoke(userInfo);
                    
                    await TrackLoginEvent(response.UserId, email, "Success");
                }
                else
                {
                    _logger.LogWarning("Login failed for: {Email}", email);
                    await TrackLoginEvent(null, email, "Failed");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for: {Email}", email);
                await TrackLoginEvent(null, email, "Error");
                throw;
            }
        }

        /// <summary>
        /// Logout - Cookie wird vom Server gelöscht
        /// </summary>
        public async Task<bool> LogoutAsync()
        {
            _logger.LogInformation("Logout initiated");

            try
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _apiService.SetAuthToken(token);
                    await _apiService.PostAsync<object, object>("api/auth/logout", new { });
                }

                // Client-seitig Cookie löschen als Backup
                await ClearCookieAsync();
                
                _apiService.RemoveAuthToken();
                OnUserLoggedOut?.Invoke();

                _logger.LogInformation("Logout completed");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                
                // Trotzdem versuchen aufzuräumen
                await ClearCookieAsync();
                _apiService.RemoveAuthToken();
                OnUserLoggedOut?.Invoke();
                
                return false;
            }
        }

        /// <summary>
        /// Prüft ob das Token abgelaufen ist
        /// </summary>
        public bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                return jsonToken.ValidTo < DateTime.UtcNow.AddMinutes(-1);
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Löscht das Cookie client-seitig
        /// </summary>
        private async Task ClearCookieAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("eval",
                    "document.cookie = 'jwt_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Strict'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cookie");
            }
        }

        private Task TrackLoginEvent(string? userId, string email, string status)
        {
            _logger.LogInformation("LOGIN_ANALYTICS: UserId={UserId}, Email={Email}, Status={Status}, Timestamp={Timestamp}",
                userId ?? "Unknown", email, status, DateTime.UtcNow);
            return Task.CompletedTask;
        }
    }
}

