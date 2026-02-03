// ===== AUTH CONTROLLER - VEREINHEITLICHT =====
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Services.ServerServices;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuthNotificationService _authNotificationService;
        private readonly ILogger<AuthController> _logger;
        private readonly IHostEnvironment _environment;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            IAuthNotificationService authNotificationService,
            ILogger<AuthController> logger,
            IHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _authNotificationService = authNotificationService;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            _logger.LogInformation("LOGIN_ATTEMPT: Email={Email}, IP={ClientIp}", request.Email, clientIp);

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("LOGIN_FAILED: Email={Email}, Reason=UserNotFound, IP={ClientIp}",
                        request.Email, clientIp);
                    return BadRequest(new { message = "Invalid credentials" });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("LOGIN_FAILED: Email={Email}, UserId={UserId}, Reason=InvalidPassword, IP={ClientIp}",
                        request.Email, user.Id, clientIp);
                    return BadRequest(new { message = "Invalid credentials" });
                }

                var token = _jwtTokenService.GenerateToken(user);

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Cookie setzen - EINZIGE Token-Quelle
                SetAuthCookie(token);

                // SignalR notification
                await _authNotificationService.NotifyUserLoggedInAsync(user.Id, user.UserName!);

                _logger.LogInformation("LOGIN_SUCCESS: Email={Email}, UserId={UserId}, IP={ClientIp}",
                    request.Email, user.Id, clientIp);

                return Ok(new LoginResponse
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LOGIN_ERROR: Email={Email}, IP={ClientIp}", request.Email, clientIp);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("LOGOUT_ATTEMPT: UserId={UserId}, UserName={UserName}, IP={ClientIp}",
                userId, userName, clientIp);

            try
            {
                // Cookie löschen
                RemoveAuthCookie();

                // SignalR notification
                if (userId != null && userName != null)
                {
                    await _authNotificationService.NotifyUserLoggedOutAsync(userId, userName);
                }

                await _signInManager.SignOutAsync();

                _logger.LogInformation("LOGOUT_SUCCESS: UserId={UserId}, UserName={UserName}, IP={ClientIp}",
                    userId, userName, clientIp);

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LOGOUT_ERROR: UserId={UserId}, UserName={UserName}, IP={ClientIp}",
                    userId, userName, clientIp);
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _logger.LogDebug("USER_INFO_REQUEST: UserId={UserId}", userId);

            try
            {
                if (userId == null)
                {
                    _logger.LogWarning("USER_INFO_FAILED: Reason=NoUserId");
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("USER_INFO_FAILED: UserId={UserId}, Reason=UserNotFound", userId);
                    return NotFound();
                }

                _logger.LogDebug("USER_INFO_SUCCESS: UserId={UserId}, UserName={UserName}", userId, user.UserName);

                return Ok(new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "USER_INFO_ERROR: UserId={UserId}", userId);
                return StatusCode(500, new { message = "An error occurred retrieving user information" });
            }
        }

        /// <summary>
        /// Setzt das JWT-Token als Cookie
        /// </summary>
        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                // In Development: HttpOnly=false für Debugging via JS Console
                // In Production: HttpOnly=true für XSS-Schutz
                HttpOnly = !_environment.IsDevelopment(),
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(24),
                Path = "/"
            };

            HttpContext.Response.Cookies.Append("jwt_token", token, cookieOptions);
            
            _logger.LogDebug("AUTH_COOKIE_SET: HttpOnly={HttpOnly}, Secure={Secure}, Expires={Expires}",
                cookieOptions.HttpOnly, cookieOptions.Secure, cookieOptions.Expires);
        }

        /// <summary>
        /// Entfernt das JWT-Token Cookie
        /// </summary>
        private void RemoveAuthCookie()
        {
            HttpContext.Response.Cookies.Delete("jwt_token", new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Strict
            });

            _logger.LogDebug("AUTH_COOKIE_REMOVED");
        }
    }
}