using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Controller;
using Tribe.Controller.Services;

namespace Tribe.Controller
{
    [ApiController]
    [Route("api/own")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OwnProfileController : ControllerBase
    {
        private readonly IOwnProfileService _profileService;
        private readonly ILogger<OwnProfileController> _logger;

        public OwnProfileController(IOwnProfileService profileService, ILogger<OwnProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        [HttpGet("state")]
        public async Task<ActionResult<TribeUser>> GetOwnProfile()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetOwnProfile(userId);

            if (profile == null)
            {
                _logger.LogInformation("Profile not found for user {UserId}", userId);
                return NotFound(new { Message = "Profil nicht gefunden. Vielleicht erstellen?" });
            }

            return Ok(profile);
        }

        [HttpPost("application")]
        public async Task<ActionResult<TribeUser>> CreatorApplication([FromBody] CreatorSubscription sub)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // ⚠️ Überschreibe die ID aus der Anfrage mit der UserID aus den Claims
            sub.TribeProfileId = userId;

            var success = await _profileService.CheckOutSubscription(sub);
            if (!success)
            {
                return BadRequest(new { Message = "Creator-Anwendung konnte nicht verarbeitet werden." });
            }

            var profile = await _profileService.GetOwnProfile(userId);
            if (profile == null)
            {
                return NotFound(new { Message = "Profil nicht gefunden." });
            }

            return Ok(profile);
        }

        [HttpPost("state")]
        public async Task<ActionResult<TribeUser>> CreateOwnProfile([FromBody] TribeUser profile)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            profile.Id = userId;

            try
            {
                var createdProfile = await _profileService.CreateAsync(profile);
                _logger.LogInformation("Profile created for user {UserId}", userId);
                return CreatedAtAction(nameof(GetOwnProfile), createdProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Profils für User {UserId}", userId);
                return StatusCode(500, new { Message = "Interner Serverfehler" });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<TribeUser>> UpdateOwnProfile([FromBody] TribeUser profile)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingProfile = await _profileService.GetOwnProfile(userId);
            if (existingProfile == null)
            {
                return NotFound(new { Message = "Profil existiert nicht. Bitte erstellen." });
            }

            if (profile.Id != userId)
            {
                _logger.LogWarning("User {ProfileId} versucht, fremdes Profil {TargetProfileId} zu ändern", userId, profile.Id);
                return Forbid();
            }

            try
            {
                var updatedProfile = await _profileService.UpdateAsync(profile);
                _logger.LogInformation("Profile updated for user {UserId}", userId);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Profils für User {UserId}", userId);
                return StatusCode(500, new { Message = "Interner Serverfehler" });
            }
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue("profileId");
        }
    }
}