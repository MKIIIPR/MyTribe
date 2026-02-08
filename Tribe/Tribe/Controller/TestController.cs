using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tribe.Data;

namespace Tribe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

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

    /// <summary>
    /// Debug endpoint to check if a TribeUser exists with given ID
    /// </summary>
    [HttpGet("debug/user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugUser(string userId)
    {
        var tribeUser = await _context.TribeUsers.FirstOrDefaultAsync(u => u.Id == userId);

        if (tribeUser == null)
        {
            // Check if maybe the ID is an ApplicationUserId
            var byAppUserId = await _context.TribeUsers.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

            return Ok(new
            {
                found = false,
                searchedId = userId,
                foundByApplicationUserId = byAppUserId != null,
                tribeUserIdIfFoundByAppUserId = byAppUserId?.Id,
                message = byAppUserId != null 
                    ? $"User found by ApplicationUserId! Use TribeUser.Id: {byAppUserId.Id}" 
                    : "No TribeUser found with this ID"
            });
        }

        var creatorProfile = await _context.CreatorProfiles.FindAsync(userId);

        return Ok(new
        {
            found = true,
            tribeUserId = tribeUser.Id,
            displayName = tribeUser.DisplayName,
            isCreator = tribeUser.IsCreator,
            applicationUserId = tribeUser.ApplicationUserId,
            hasCreatorProfile = creatorProfile != null,
            creatorProfileId = creatorProfile?.Id,
            creatorName = creatorProfile?.CreatorName
        });
    }

    /// <summary>
    /// List all TribeUsers (for debugging)
    /// </summary>
    [HttpGet("debug/users")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugListUsers()
    {
        var users = await _context.TribeUsers
            .Select(u => new
            {
                u.Id,
                u.DisplayName,
                u.IsCreator,
                u.ApplicationUserId
            })
            .Take(20)
            .ToListAsync();

        return Ok(new { count = users.Count, users });
    }
}