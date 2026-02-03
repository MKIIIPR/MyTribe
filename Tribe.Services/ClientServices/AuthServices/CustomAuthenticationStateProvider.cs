using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tribe.Services.ClientServices.SimpleAuth;

namespace Tribe.Client.Services;

/// <summary>
/// Alternative AuthenticationStateProvider - verwendet nur Cookies (kein localStorage)
/// Wird normalerweise nicht benötigt, da CookieAuthenticationStateProvider verwendet wird
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ISignalRService _signalRService;

    public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, ISignalRService signalRService)
    {
        _jsRuntime = jsRuntime;
        _signalRService = signalRService;

        _signalRService.UserLoggedIn += OnUserLoggedIn;
        _signalRService.UserLoggedOut += OnUserLoggedOut;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // NUR Cookie verwenden - KEIN localStorage
            var token = await GetTokenFromCookie();

            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                return CreateAnonymousState();
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            Console.WriteLine($"[Auth] User authenticated: {user.Identity?.Name}");
            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] Error: {ex.Message}");
            return CreateAnonymousState();
        }
    }

    /// <summary>
    /// Benachrichtigt die UI über erfolgreiche Authentifizierung
    /// </summary>
    public void NotifyUserAuthentication(string token)
    {
        try
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            Console.WriteLine($"[Auth] Notifying authentication: {user.Identity?.Name}");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] NotifyUserAuthentication error: {ex.Message}");
        }
    }

    /// <summary>
    /// Benachrichtigt die UI über Logout
    /// </summary>
    public void NotifyUserLogout()
    {
        Console.WriteLine("[Auth] Notifying logout");
        NotifyAuthenticationStateChanged(Task.FromResult(CreateAnonymousState()));
    }

    private async Task<string?> GetTokenFromCookie()
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

    private void OnUserLoggedIn(string userId, string userName)
    {
        Console.WriteLine($"[SignalR] User logged in: {userName}");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private void OnUserLoggedOut(string userId, string userName)
    {
        Console.WriteLine($"[SignalR] User logged out: {userName}");
        NotifyUserLogout();
    }

    private static AuthenticationState CreateAnonymousState()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static bool IsTokenExpired(string token)
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

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
        catch
        {
            return Enumerable.Empty<Claim>();
        }
    }
}
