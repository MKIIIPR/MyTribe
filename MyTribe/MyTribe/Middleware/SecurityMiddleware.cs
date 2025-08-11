using Microsoft.AspNetCore.Identity;
using MyTribe.Data;
using System.Security.Claims;

namespace MyTribe.Middleware;

public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityMiddleware> _logger;

    public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        // Session-Validierung für authentifizierte Benutzer
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await ValidateSessionSecurity(context, signInManager);
        }

        await _next(context);
    }

    private async Task ValidateSessionSecurity(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        try
        {
            var user = context.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                await ForceLogout(context, signInManager, "Invalid user ID");
                return;
            }

            // IP-Adresse validieren (optional - kann bei dynamischen IPs problematisch sein)
            var currentIp = GetClientIpAddress(context);
            var storedIp = user.FindFirst("ip_address")?.Value;

            // User-Agent validieren
            var currentUserAgent = context.Request.Headers["User-Agent"].ToString();
            var storedUserAgent = user.FindFirst("user_agent")?.Value;

            // Verdächtige Aktivitäten prüfen
            if (IsSessionCompromised(currentIp, storedIp, currentUserAgent, storedUserAgent))
            {
                _logger.LogWarning($"Suspicious activity detected for user {userId}. Forcing logout.");
                await ForceLogout(context, signInManager, "Suspicious activity detected");
                return;
            }

            // Session-Timeout prüfen
            var lastActivity = user.FindFirst("last_activity")?.Value;
            if (!string.IsNullOrEmpty(lastActivity) &&
                DateTime.TryParse(lastActivity, out var lastActivityTime))
            {
                if (DateTime.UtcNow.Subtract(lastActivityTime).TotalHours > 24)
                {
                    await ForceLogout(context, signInManager, "Session expired");
                    return;
                }
            }

            // Last Activity aktualisieren
            await UpdateLastActivity(context, signInManager);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during session validation");
            // Bei Validierungsfehlern sicherheitshalber ausloggen
            await ForceLogout(context, signInManager, "Session validation error");
        }
    }

    private bool IsSessionCompromised(string? currentIp, string? storedIp,
                                    string currentUserAgent, string? storedUserAgent)
    {
        // User-Agent stark verändert
        if (!string.IsNullOrEmpty(storedUserAgent) &&
            !currentUserAgent.Contains(storedUserAgent.Split(' ')[0]))
        {
            return true;
        }

        // Weitere Validierungslogik hier...
        return false;
    }

    private async Task ForceLogout(HttpContext context, SignInManager<ApplicationUser> signInManager, string reason)
    {
        _logger.LogWarning($"Forcing logout: {reason}");

        await signInManager.SignOutAsync();

        // Alle Cookies löschen
        foreach (var cookie in context.Request.Cookies)
        {
            context.Response.Cookies.Delete(cookie.Key);
        }

        // Redirect zur Login-Seite
        context.Response.Redirect("/Account/Login?reason=security");
    }

    private async Task UpdateLastActivity(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        // Implementierung um Last Activity zu aktualisieren
        // Dies würde normalerweise die Claims aktualisieren oder in der Datenbank speichern
    }

    private string GetClientIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ??
               context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
               context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
               "Unknown";
    }
}