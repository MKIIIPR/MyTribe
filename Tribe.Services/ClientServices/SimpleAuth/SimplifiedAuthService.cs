// ===== 1. SIMPLIFIED AUTH SERVICE MIT LOGGING =====
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
        event Action<UserInfo?> OnUserLoggedIn;
        event Action OnUserLoggedOut;

    }

    public class SimplifiedAuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<SimplifiedAuthService> _logger;
        private const string COOKIE_KEY = "jwt_token";
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


        public async Task<string?> GetTokenAsync()
        {
            _logger.LogDebug("Attempting to retrieve token from cookie");

            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("eval",
                    "document.cookie.split('; ').find(row => row.startsWith('jwt_token='))?.split('=')[1]");

                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("Token successfully retrieved from cookie, length: {TokenLength}", token.Length);

                    if (IsTokenExpired(token))
                    {
                        _logger.LogWarning("Retrieved token is expired");
                        return null;
                    }
                }
                else
                {
                    _logger.LogDebug("No token found in cookie");
                }

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving token from cookie");
                return null;
            }
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            _logger.LogDebug("Retrieving current user information");

            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("No valid token available for user info retrieval");
                    return null;
                }

                _apiService.SetAuthToken(token);
                var userInfo = await _apiService.GetAsync<UserInfo>("api/auth/user");

                if (userInfo != null)
                {
                    _logger.LogDebug("Successfully retrieved user info for: {UserName}", userInfo.UserName);
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve user info despite valid token");
                }

                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user information");
                return null;
            }
        }
 

        private async Task TrackLoginEvent(string? userId, string email, string status)
        {
            try
            {
                _logger.LogInformation("LOGIN_ANALYTICS: UserId={UserId}, Email={Email}, Status={Status}, Timestamp={Timestamp}",
                    userId ?? "Unknown", email, status, DateTime.UtcNow);

                // Hier könntest du auch an Analytics-Service senden
                // await _analyticsService.TrackEventAsync("UserLogin", new { userId, email, status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track login event");
            }
        }

        private async Task TrackLogoutEvent()
        {
            try
            {
                _logger.LogInformation("LOGOUT_ANALYTICS: Timestamp={Timestamp}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track logout event");
            }
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                var isExpired = jsonToken.ValidTo < DateTime.UtcNow.AddMinutes(-1);

                if (isExpired)
                {
                    _logger.LogWarning("Token expired at {ExpiryTime}", jsonToken.ValidTo);
                }

                return isExpired;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token expiration");
                return true;
            }
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            _logger.LogInformation("Starting login attempt for user: {Email}", email);

            try
            {
                var loginRequest = new LoginRequest { Email = email, Password = password };
                var response = await _apiService.PostAsync<LoginRequest, LoginResponse>("api/auth/login", loginRequest);

                if (response != null)
                {
                    _logger.LogInformation("Login successful for user: {Email}, UserId: {UserId}", email, response.UserId);

                    var userInfo = await GetCurrentUserAsync(); // Lade UserInfo nach Login
                    OnUserLoggedIn?.Invoke(userInfo); // 🔔 Event auslösen
                    await TrackLoginEvent(response.UserId, email, "Success");
                }
                else
                {
                    _logger.LogWarning("Login failed for user: {Email} - Invalid credentials", email);
                    await TrackLoginEvent(null, email, "Failed");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login exception for user: {Email}", email);
                await TrackLoginEvent(null, email, "Error");
                throw;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            _logger.LogInformation("Starting logout process");

            try
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _apiService.SetAuthToken(token);
                    await _apiService.PostAsync<object, object>("api/auth/logout", new { });
                    _logger.LogInformation("Logout API call successful");
                }

                _apiService.RemoveAuthToken();
                await TrackLogoutEvent();

                OnUserLoggedOut?.Invoke(); // 🔔 Event auslösen

                _logger.LogInformation("Logout process completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                return false;
            }
        }
    }
}

