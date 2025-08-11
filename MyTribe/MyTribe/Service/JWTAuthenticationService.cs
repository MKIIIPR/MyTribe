using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyTribe.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MyTribe.Services;

public interface IJwtAuthenticationService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<AuthResult> RegisterAsync(string email, string password, string userName);
}

public class JwtAuthenticationService : IJwtAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtAuthenticationService> _logger;

    public JwtAuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ILogger<JwtAuthenticationService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResult { Success = false, Message = "Invalid credentials" };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return new AuthResult { Success = false, Message = "Invalid credentials" };
            }

            var tokens = await GenerateTokensAsync(user);
            await UpdateRefreshTokenAsync(user, tokens.RefreshToken);

            _logger.LogInformation($"User {email} logged in successfully");

            return new AuthResult
            {
                Success = true,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresIn = 3600, // 1 Stunde
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Login failed for {email}");
            return new AuthResult { Success = false, Message = "Login failed" };
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string userName)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true // Für Demo - in Produktion sollte Email bestätigt werden
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Automatisch einloggen nach Registrierung
            return await LoginAsync(email, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Registration failed for {email}");
            return new AuthResult { Success = false, Message = "Registration failed" };
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return new AuthResult { Success = false, Message = "Invalid refresh token" };
            }

            var tokens = await GenerateTokensAsync(user);
            await UpdateRefreshTokenAsync(user, tokens.RefreshToken);

            return new AuthResult
            {
                Success = true,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresIn = 3600
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh token failed");
            return new AuthResult { Success = false, Message = "Refresh failed" };
        }
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token revocation failed");
            return false;
        }
    }

    private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user)
    {
        // Access Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("jti", Guid.NewGuid().ToString())
        };

        // Rollen hinzufügen
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Refresh Token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return (accessToken, refreshToken);
    }

    private async Task UpdateRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Refresh Token gültig für 7 Tage
        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
}

// DTOs
public class AuthResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public UserInfo? User { get; set; }
}

public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
}