using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Tribe.Data;
using Tribe.Bib.Models.TribeRelated;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/creators")]
    public class CreatorProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreatorProfileController> _logger;

        public CreatorProfileController(ApplicationDbContext context, ILogger<CreatorProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{creatorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCreatorProfile(string creatorId)
        {
            if (string.IsNullOrEmpty(creatorId))
            {
                return BadRequest(new { error = "CreatorId is required" });
            }

            _logger.LogInformation("Public creator profile requested: {CreatorId}", creatorId);

            var normalizedCreatorId = creatorId.ToLowerInvariant();

            var profile = await _context.CreatorProfiles
                .Include(cp => cp.TribeUser)
                .Include(cp => cp.CreatorTokens)
                .Include(cp => cp.AffiliatePartners)
                .Include(cp => cp.Placements)
                .FirstOrDefaultAsync(cp => cp.Id == creatorId
                    || (cp.TribeUser.DisplayName != null && cp.TribeUser.DisplayName.ToLower() == normalizedCreatorId)
                    || (cp.CreatorName != null && cp.CreatorName.ToLower() == normalizedCreatorId));

            if (profile == null)
            {
                _logger.LogWarning("Creator profile not found for {CreatorId}", creatorId);
                return NotFound(new { error = "Creator profile not found" });
            }

            _logger.LogInformation("Returning creator profile {CreatorId}=Id:{ProfileId}", creatorId, profile.Id);
            return Ok(profile);
        }

    }
}
