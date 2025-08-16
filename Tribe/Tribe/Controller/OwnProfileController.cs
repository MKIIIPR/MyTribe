using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Controller;
using Tribe.Controller.Services; // Annahme: Dein Profil-Service

namespace Tribe.Controller
{
    [ApiController]
    [Route("api/own")]
    [Authorize] // 🔐 Nur authentifizierte Benutzer
    public class OwnProfileController : ControllerBase
    {
        private readonly IOwnProfileService _profileService;
        private readonly ILogger<OwnProfileController> _logger;

        public OwnProfileController(IOwnProfileService profileService, ILogger<OwnProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        /// <summary>
        /// Holt das eigene TribeProfile
        /// GET: /api/own/state
        /// </summary>
        [HttpGet("state")]
        public async Task<ActionResult<TribeProfile>> GetOwnProfile()
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

        /// <summary>
        /// Erstellt ein neues TribeProfile (für den aktuellen Benutzer)
        /// POST: /api/own/state
        /// </summary>
        [HttpPost("state")]
        public async Task<ActionResult<TribeProfile>> CreateOwnProfile([FromBody] TribeProfile profile)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Sicherstellen, dass das Profil dem aktuellen User zugeordnet wird
            profile.Id = userId; // oder eine andere Profil-ID, aber User-Zuordnung klar

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

        /// <summary>
        /// Aktualisiert das eigene TribeProfile
        /// PUT: /api/own/update
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<TribeProfile>> UpdateOwnProfile([FromBody] TribeProfile profile)
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

            // 🔒 Sicherstellen: Nur das Profil des aktuellen Users wird geändert
            if (profile.ApplicationUserId != userId)
            {
                _logger.LogWarning("User {UserId} versucht, Profil mit ID {ProfileId} zu ändern", userId, profile.Id);
                return Forbid(); // 🔥 Verboten!
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

        // Hilfsmethode: Extrahiere die User-ID aus den Claims
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub"); // fallback für JWT
        }
    }
}