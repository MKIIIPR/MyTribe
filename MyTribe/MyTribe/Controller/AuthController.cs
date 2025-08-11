using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTribe.Services;
using System.ComponentModel.DataAnnotations;

namespace MyTribe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            // JWT Token im HttpOnly Cookie setzen (für Server-side Blazor)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Nur über HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("jwt", result.AccessToken!, cookieOptions);

            // Refresh Token im HttpOnly Cookie
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", result.RefreshToken!, refreshCookieOptions);

            _logger.LogInformation($"User {request.Email} logged in successfully");

            // Für WebAssembly: Token auch im Response Body
            return Ok(new
            {
                success = true,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiresIn = result.ExpiresIn,
                user = result.User
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request.Email, request.Password, request.UserName);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation($"User {request.Email} registered successfully");

            // Nach erfolgreicher Registrierung automatisch einloggen
            return Ok(new
            {
                success = true,
                message = "Registration successful",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiresIn = result.ExpiresIn,
                user = result.User
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Refresh Token aus Cookie oder Request Body
            var refreshToken = request.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token required" });
            }

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            // Neue Tokens in Cookies setzen
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("jwt", result.AccessToken!, cookieOptions);

            return Ok(new
            {
                success = true,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiresIn = result.ExpiresIn
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh error");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }

            // Alle Auth-Cookies löschen
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("refreshToken");

            _logger.LogInformation($"User {User.Identity?.Name} logged out");

            return Ok(new { success = true, message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var user = new
            {
                id = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,
                userName = User.Identity?.Name,
                email = User.FindFirst("email")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            };

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get current user error");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

// DTOs
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string? RefreshToken { get; set; }
}