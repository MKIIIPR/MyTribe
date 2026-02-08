// ===== 2. AUTHENTICATION STATE PROVIDER MIT LOGGING =====
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tribe.Services.ClientServices.SimpleAuth
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ISignalRService _signalRService;
        private readonly ILogger<CookieAuthenticationStateProvider> _logger;
        private AuthenticationState? _cachedState;
        private string? _cachedToken;

        public CookieAuthenticationStateProvider(
            IJSRuntime jsRuntime,
            ISignalRService signalRService,
            ILogger<CookieAuthenticationStateProvider> logger)
        {
            _jsRuntime = jsRuntime;
            _signalRService = signalRService;
            _logger = logger;

            // SignalR Events mit Logging
            _signalRService.UserLoggedIn += OnUserLoggedIn;
            _signalRService.UserLoggedOut += OnUserLoggedOut;

            _logger.LogInformation("CookieAuthenticationStateProvider initialized");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await GetCookieTokenAsync();

                // Wenn Token gleich wie gecachtes Token, cached state zurückgeben
                if (_cachedState != null && _cachedToken == token && !string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("Returning cached authentication state (same token)");
                    return _cachedState;
                }

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("No token found - returning anonymous state");
                    return CacheAndReturn(CreateAnonymousState(), null);
                }

                if (IsTokenExpired(token))
                {
                    _logger.LogWarning("Token is expired - returning anonymous state");
                    return CacheAndReturn(CreateAnonymousState(), null);
                }

                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var authenticatedState = new AuthenticationState(new ClaimsPrincipal(identity));

                var userName = identity.Name ?? "Unknown";
                _logger.LogDebug("User authenticated: {UserName}", userName);

                return CacheAndReturn(authenticatedState, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting authentication state");
                return CacheAndReturn(CreateAnonymousState(), null);
            }
        }

        private AuthenticationState CacheAndReturn(AuthenticationState state, string? token)
        {
            _cachedState = state;
            _cachedToken = token;
            return state;
        }

        private async Task<string?> GetCookieTokenAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("eval",
                    "document.cookie.split('; ').find(row => row.startsWith('jwt_token='))?.split('=')[1]");

                _logger.LogDebug("Token retrieval from cookie: {HasToken}", !string.IsNullOrEmpty(token));
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve token from cookie");
                return null;
            }
        }

        private void OnUserLoggedIn(string userId, string userName)
        {
            _logger.LogInformation("SIGNALR_EVENT: UserLoggedIn - UserId={UserId}, UserName={UserName}", userId, userName);

            try
            {
                // Clear cache to force fresh state
                _cachedState = null;
                _cachedToken = null;
                RefreshAuthState();
                _logger.LogDebug("Auth state refreshed after login event");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing auth state after login event");
            }
        }

        private void OnUserLoggedOut(string userId, string userName)
        {
            _logger.LogInformation("SIGNALR_EVENT: UserLoggedOut - UserId={UserId}, UserName={UserName}", userId, userName);

            try
            {
                // Clear cache
                _cachedState = null;
                _cachedToken = null;

                // Cookie löschen
                _jsRuntime.InvokeVoidAsync("eval",
                    "document.cookie = 'jwt_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Strict'");

                RefreshAuthState();
                _logger.LogInformation("Cookie cleared and auth state refreshed after logout event");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling logout event");
            }
        }

        private void RefreshAuthState()
        {
            _logger.LogDebug("Refreshing authentication state");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        /// <summary>
        /// Öffentliche Methode zum Aktualisieren des AuthState nach Login/Logout
        /// </summary>
        public Task NotifyAuthStateChangedAsync()
        {
            _logger.LogInformation("NotifyAuthStateChangedAsync called - refreshing auth state");
            _cachedState = null;
            _cachedToken = null;
            RefreshAuthState();
            return Task.CompletedTask;
        }

        private AuthenticationState CreateAnonymousState()
            => new(new ClaimsPrincipal(new ClaimsIdentity()));

        private bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                var isExpired = jsonToken.ValidTo < DateTime.UtcNow.AddMinutes(-1);

                _logger.LogDebug("Token expiration check: Expired={IsExpired}, ExpiresAt={ExpiryTime}",
                    isExpired, jsonToken.ValidTo);

                return isExpired;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token expiration");
                return true;
            }
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);

                _logger.LogDebug("Parsed {ClaimCount} claims from JWT", token.Claims.Count());
                return token.Claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JWT claims");
                return Enumerable.Empty<Claim>();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _signalRService.UserLoggedIn -= OnUserLoggedIn;
                _signalRService.UserLoggedOut -= OnUserLoggedOut;
                _logger.LogInformation("CookieAuthenticationStateProvider disposed");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}