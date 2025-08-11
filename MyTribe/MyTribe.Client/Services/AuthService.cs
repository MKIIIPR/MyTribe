using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyTribe.Client.Services;

public interface IAuthService
{
    Task<bool> LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUserNameAsync();
    Task<ClaimsPrincipal> GetUserAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(
        HttpClient httpClient,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            // 1. Server-seitigen Logout durchführen
            var response = await _httpClient.PostAsync("Account/Logout", null);

            if (response.IsSuccessStatusCode)
            {
                // 2. Client-seitige Authentication State zurücksetzen
                await InvalidateAuthenticationStateAsync();

                // 3. Zur Login-Seite navigieren mit forceLoad für vollständigen Reload
                _navigationManager.NavigateTo("/Account/Login", forceLoad: true);

                return true;
            }
            else
            {
                // Fallback: auch bei Server-Fehler Client-State clearen
                await InvalidateAuthenticationStateAsync();
                _navigationManager.NavigateTo("/Account/Login", forceLoad: true);
                return false;
            }
        }
        catch (Exception ex)
        {
            // Bei Netzwerkfehlern trotzdem Client clearen
            Console.WriteLine($"Logout error: {ex.Message}");
            await InvalidateAuthenticationStateAsync();
            _navigationManager.NavigateTo("/Account/Login", forceLoad: true);
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User?.Identity?.IsAuthenticated ?? false;
    }

    public async Task<string?> GetUserNameAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User?.Identity?.Name;
    }

    public async Task<ClaimsPrincipal> GetUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User ?? new ClaimsPrincipal();
    }

    private async Task InvalidateAuthenticationStateAsync()
    {
        // AuthenticationStateProvider benachrichtigen über State-Änderung
        if (_authenticationStateProvider is IDisposable disposable)
        {
            // Trigger einer Aktualisierung des Authentication States
            var _ = _authenticationStateProvider.GetAuthenticationStateAsync();
        }
    }
}