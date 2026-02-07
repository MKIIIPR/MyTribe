using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tribe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult GetPublicData()
    {
        return Ok(new { message = "This is public data", timestamp = DateTime.UtcNow });
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtectedData()
    {
        var userName = User.Identity?.Name ?? "Unknown";
        return Ok(new
        {
            message = $"Hello {userName}, this is protected data!",
            timestamp = DateTime.UtcNow,
            userId = User.FindFirst("profileId")?.Value
        });
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdminData()
    {
        return Ok(new { message = "This is admin-only data", timestamp = DateTime.UtcNow });
    }
}