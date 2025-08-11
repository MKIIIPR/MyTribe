using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tribe.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ISignalRService _signalRService;

    public CustomAuthenticationStateProvider(IAuthService authService, ISignalRService signalRService)
    {
        _authService = authService;
        _signalRService = signalRService;

        // Subscribe to SignalR events
        _signalRService.UserLoggedIn += OnUserLoggedIn;
        _signalRService.UserLoggedOut += OnUserLoggedOut;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = _authService.GetStoredToken();

        if (string.IsNullOrEmpty(token) || _authService.IsTokenExpired(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }

    private void OnUserLoggedIn(string userId, string userName)
    {
        // Refresh auth state when receiving SignalR notification
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private void OnUserLoggedOut(string userId, string userName)
    {
        // Clear auth state when receiving SignalR logout notification
        _authService.RemoveToken();
        NotifyUserLogout();
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}