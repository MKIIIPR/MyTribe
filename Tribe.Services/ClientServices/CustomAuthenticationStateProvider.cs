
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tribe.Services.ClientServices.SimpleAuth;

namespace Tribe.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ISignalRService _signalRService;

    public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, ISignalRService signalRService)
    {
        _jsRuntime = jsRuntime;
        _signalRService = signalRService;

        // Subscribe to SignalR events
        _signalRService.UserLoggedIn += OnUserLoggedIn;
        _signalRService.UserLoggedOut += OnUserLoggedOut;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Try to get token from localStorage first
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            // If not found, try cookie
            if (string.IsNullOrEmpty(token))
            {
                token = await GetTokenFromCookie();
            }

            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                Console.WriteLine("No valid token found - user not authenticated");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            Console.WriteLine($"User authenticated: {user.Identity?.Name}");
            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        Console.WriteLine($"Notifying user authentication: {user.Identity?.Name}");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        Console.WriteLine("Notifying user logout");
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
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
        Console.WriteLine($"SignalR Event: User {userName} logged in");
        // Refresh auth state when receiving SignalR notification
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private void OnUserLoggedOut(string userId, string userName)
    {
        Console.WriteLine($"SignalR Event: User {userName} logged out");
        // Clear tokens and auth state
        _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        NotifyUserLogout();
    }

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.ValidTo < DateTime.UtcNow.AddMinutes(-1); // 1 minute buffer
        }
        catch
        {
            return true;
        }
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}
