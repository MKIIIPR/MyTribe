using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tribe.Data;
using Tribe.Services.ClientServices.ShopServices;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/creators")]
    public class CreatorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreatorsController> _logger;

        public CreatorsController(ApplicationDbContext context, ILogger<CreatorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get public creator profile by ID
        /// </summary>
        [HttpGet("{creatorId}")]
        public async Task<IActionResult> GetCreatorProfile(string creatorId)
        {
            try
            {
                _logger.LogInformation("GetCreatorProfile called with creatorId: {CreatorId}", creatorId);

                // First try to find TribeUser with IsCreator = true
                var tribeUser = await _context.TribeUsers
                    .FirstOrDefaultAsync(u => u.Id == creatorId && u.IsCreator);

                // If not found, check if user exists but is not a creator
                if (tribeUser == null)
                {
                    var anyUser = await _context.TribeUsers.FirstOrDefaultAsync(u => u.Id == creatorId);
                    if (anyUser != null)
                    {
                        _logger.LogWarning("User {CreatorId} exists but IsCreator = {IsCreator}", creatorId, anyUser.IsCreator);

                        // If user exists but not marked as creator, still return profile (for own view)
                        var profile = await _context.CreatorProfiles.FindAsync(creatorId);

                        return Ok(new CreatorProfileDto
                        {
                            Id = anyUser.Id,
                            DisplayName = anyUser.DisplayName,
                            AvatarUrl = anyUser.AvatarUrl,
                            Bio = anyUser.Bio,
                            CreatorName = profile?.CreatorName,
                            BannerUrl = profile?.BannerUrl,
                            VerifiedCreator = profile?.VerifiedCreator ?? false,
                            FollowerCount = profile?.FollowerCount ?? 0,
                            TotalRaffles = profile?.TotalRaffles ?? 0,
                            TotalTokensDistributed = profile?.TotalTokensDistributed ?? 0,
                            PatreonUrl = profile?.PatreonUrl,
                            YouTubeUrl = profile?.YouTubeUrl,
                            TwitchUrl = profile?.TwitchUrl,
                            TwitterUrl = profile?.TwitterUrl,
                            InstagramUrl = profile?.InstagramUrl,
                            TikTokUrl = profile?.TikTokUrl,
                            DiscordUrl = profile?.DiscordUrl,
                            AcceptingCollaborations = profile?.AcceptingCollaborations ?? false,
                            CollaborationInfo = profile?.CollaborationInfo
                        });
                    }

                    _logger.LogWarning("No user found with creatorId: {CreatorId}", creatorId);
                    return NotFound(new { error = "Creator not found", searchedId = creatorId });
                }

                var creatorProfile = await _context.CreatorProfiles.FindAsync(creatorId);

                var dto = new CreatorProfileDto
                {
                    Id = tribeUser.Id,
                    DisplayName = tribeUser.DisplayName,
                    AvatarUrl = tribeUser.AvatarUrl,
                    Bio = tribeUser.Bio,
                    CreatorName = creatorProfile?.CreatorName,
                    BannerUrl = creatorProfile?.BannerUrl,
                    VerifiedCreator = creatorProfile?.VerifiedCreator ?? false,
                    FollowerCount = creatorProfile?.FollowerCount ?? 0,
                    TotalRaffles = creatorProfile?.TotalRaffles ?? 0,
                    TotalTokensDistributed = creatorProfile?.TotalTokensDistributed ?? 0,
                    PatreonUrl = creatorProfile?.PatreonUrl,
                    YouTubeUrl = creatorProfile?.YouTubeUrl,
                    TwitchUrl = creatorProfile?.TwitchUrl,
                    TwitterUrl = creatorProfile?.TwitterUrl,
                    InstagramUrl = creatorProfile?.InstagramUrl,
                    TikTokUrl = creatorProfile?.TikTokUrl,
                    DiscordUrl = creatorProfile?.DiscordUrl,
                    AcceptingCollaborations = creatorProfile?.AcceptingCollaborations ?? false,
                    CollaborationInfo = creatorProfile?.CollaborationInfo
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching creator profile {CreatorId}", creatorId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get public creator profile by unique CreatorName
        /// </summary>
        [HttpGet("name/{creatorName}")]
        public async Task<IActionResult> GetCreatorByName(string creatorName)
        {
            try
            {
                var creatorProfile = await _context.CreatorProfiles
                    .Include(cp => cp.TribeUser)
                    .FirstOrDefaultAsync(cp => cp.CreatorName == creatorName);

                if (creatorProfile == null || creatorProfile.TribeUser == null)
                {
                    return NotFound(new { error = "Creator not found" });
                }

                var dto = new CreatorProfileDto
                {
                    Id = creatorProfile.Id,
                    DisplayName = creatorProfile.TribeUser.DisplayName,
                    AvatarUrl = creatorProfile.TribeUser.AvatarUrl,
                    Bio = creatorProfile.TribeUser.Bio,
                    CreatorName = creatorProfile.CreatorName,
                    BannerUrl = creatorProfile.BannerUrl,
                    VerifiedCreator = creatorProfile.VerifiedCreator,
                    FollowerCount = creatorProfile.FollowerCount,
                    TotalRaffles = creatorProfile.TotalRaffles,
                    TotalTokensDistributed = creatorProfile.TotalTokensDistributed,
                    PatreonUrl = creatorProfile.PatreonUrl,
                    YouTubeUrl = creatorProfile.YouTubeUrl,
                    TwitchUrl = creatorProfile.TwitchUrl,
                    TwitterUrl = creatorProfile.TwitterUrl,
                    InstagramUrl = creatorProfile.InstagramUrl,
                    TikTokUrl = creatorProfile.TikTokUrl,
                    DiscordUrl = creatorProfile.DiscordUrl,
                    AcceptingCollaborations = creatorProfile.AcceptingCollaborations,
                    CollaborationInfo = creatorProfile.CollaborationInfo
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching creator by name {CreatorName}", creatorName);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Search creators by name (for discovery)
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCreators([FromQuery] string q, [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                {
                    return Ok(new List<CreatorProfileDto>());
                }

                var creators = await _context.TribeUsers
                    .Where(u => u.IsCreator && u.DisplayName.Contains(q))
                    .Take(limit)
                    .ToListAsync();

                var creatorIds = creators.Select(c => c.Id).ToList();
                var profiles = await _context.CreatorProfiles
                    .Where(cp => creatorIds.Contains(cp.Id))
                    .ToDictionaryAsync(cp => cp.Id);

                var results = creators.Select(u =>
                {
                    profiles.TryGetValue(u.Id, out var cp);
                    return new CreatorProfileDto
                    {
                        Id = u.Id,
                        DisplayName = u.DisplayName,
                        AvatarUrl = u.AvatarUrl,
                        Bio = u.Bio,
                        CreatorName = cp?.CreatorName,
                        VerifiedCreator = cp?.VerifiedCreator ?? false,
                        FollowerCount = cp?.FollowerCount ?? 0
                    };
                }).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching creators with query {Query}", q);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get featured/popular creators
        /// </summary>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedCreators([FromQuery] int limit = 6)
        {
            try
            {
                var featuredProfiles = await _context.CreatorProfiles
                    .Include(cp => cp.TribeUser)
                    .Where(cp => cp.TribeUser.IsCreator)
                    .OrderByDescending(cp => cp.FollowerCount)
                    .Take(limit)
                    .ToListAsync();

                var results = featuredProfiles.Select(cp => new CreatorProfileDto
                {
                    Id = cp.Id,
                    DisplayName = cp.TribeUser?.DisplayName ?? "",
                    AvatarUrl = cp.TribeUser?.AvatarUrl,
                    Bio = cp.TribeUser?.Bio,
                    CreatorName = cp.CreatorName,
                    BannerUrl = cp.BannerUrl,
                    VerifiedCreator = cp.VerifiedCreator,
                    FollowerCount = cp.FollowerCount,
                    TotalRaffles = cp.TotalRaffles
                }).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching featured creators");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update own creator profile (requires authentication)
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateCreatorProfile([FromBody] UpdateCreatorProfileDto dto)
        {
            try
            {
                // Get current user's profile ID from claims
                var profileId = User.FindFirst("profileId")?.Value;
                if (string.IsNullOrEmpty(profileId))
                {
                    return Unauthorized(new { error = "Nicht authentifiziert" });
                }

                var creatorProfile = await _context.CreatorProfiles.FindAsync(profileId);
                if (creatorProfile == null)
                {
                    return NotFound(new { error = "CreatorProfile nicht gefunden" });
                }

                // Prüfe ob der neue CreatorName bereits vergeben ist (falls geändert)
                if (!string.IsNullOrEmpty(dto.CreatorName) && dto.CreatorName != creatorProfile.CreatorName)
                {
                    var nameExists = await _context.CreatorProfiles
                        .AnyAsync(cp => cp.CreatorName == dto.CreatorName && cp.Id != profileId);

                    if (nameExists)
                    {
                        return BadRequest(new { error = "Dieser Creator-Name ist bereits vergeben" });
                    }

                    creatorProfile.CreatorName = dto.CreatorName;
                }

                // Update other fields
                if (dto.BannerUrl != null) creatorProfile.BannerUrl = dto.BannerUrl;
                if (dto.ImageTemplateUrl != null) creatorProfile.ImageTemplateUrl = dto.ImageTemplateUrl;
                if (dto.AcceptingCollaborations.HasValue) creatorProfile.AcceptingCollaborations = dto.AcceptingCollaborations.Value;
                if (dto.CollaborationInfo != null) creatorProfile.CollaborationInfo = dto.CollaborationInfo;

                // Social Links
                if (dto.PatreonUrl != null) creatorProfile.PatreonUrl = dto.PatreonUrl;
                if (dto.YouTubeUrl != null) creatorProfile.YouTubeUrl = dto.YouTubeUrl;
                if (dto.TwitchUrl != null) creatorProfile.TwitchUrl = dto.TwitchUrl;
                if (dto.TwitterUrl != null) creatorProfile.TwitterUrl = dto.TwitterUrl;
                if (dto.InstagramUrl != null) creatorProfile.InstagramUrl = dto.InstagramUrl;
                if (dto.TikTokUrl != null) creatorProfile.TikTokUrl = dto.TikTokUrl;
                if (dto.DiscordUrl != null) creatorProfile.DiscordUrl = dto.DiscordUrl;

                await _context.SaveChangesAsync();

                _logger.LogInformation("CreatorProfile updated for {ProfileId}", profileId);

                return Ok(new { message = "Profil erfolgreich aktualisiert" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating creator profile");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Check if a creator name is available
        /// </summary>
        [HttpGet("name-available/{creatorName}")]
        public async Task<IActionResult> CheckCreatorNameAvailable(string creatorName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(creatorName) || creatorName.Length < 3)
                {
                    return BadRequest(new { available = false, error = "Name muss mindestens 3 Zeichen lang sein" });
                }

                var exists = await _context.CreatorProfiles.AnyAsync(cp => cp.CreatorName == creatorName);
                return Ok(new { available = !exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking creator name availability");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    /// <summary>
    /// DTO for updating creator profile
    /// </summary>
    public class UpdateCreatorProfileDto
    {
        public string? CreatorName { get; set; }
        public string? BannerUrl { get; set; }
        public string? ImageTemplateUrl { get; set; }
        public bool? AcceptingCollaborations { get; set; }
        public string? CollaborationInfo { get; set; }
        public string? PatreonUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? DiscordUrl { get; set; }
    }
}