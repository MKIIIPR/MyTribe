using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;
using Microsoft.EntityFrameworkCore;
using Tribe.Controller.CreatorOrga.Services;

namespace Tribe.Controller.CreatorOrga
{
    [ApiController]
    [Route("api/admin/organisation")]
    [Authorize]
    public class OrganisationAdminController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly OrgaDbContext _db;

        public OrganisationAdminController(IOrganisationService organisationService, OrgaDbContext db)
        {
            _organisationService = organisationService;
            _db = db;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyOrganisations()
        {
            var profileId = User.FindFirst("profileId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (profileId == null) return Forbid();

            var list = await _organisationService.GetByCreatorAsync(profileId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Organization org)
        {
            var profileId = User.FindFirst("profileId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (profileId == null) return Forbid();

            org.CreatorUserId = profileId;
            if (!string.IsNullOrEmpty(org.GameProfileId))
            {
                var gameProfile = await _db.GameProfiles.FindAsync(org.GameProfileId);
                if (gameProfile is null)
                {
                    ModelState.AddModelError("GameProfileId", "The specified GameProfile does not exist.");
                    return BadRequest(ModelState);
                }
            }
            var created = await _organisationService.CreateAsync(org);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var org = await _organisationService.GetByIdAsync(id);
            if (org == null) return NotFound();
            return Ok(org);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var profileId = User.FindFirst("profileId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (profileId == null) return Forbid();

            var existing = await _db.Organizations.FindAsync(id);
            if (existing == null) return NotFound();
            if (existing.CreatorUserId != profileId) return Forbid();

            var ok = await _organisationService.DeleteAsync(id);
            if (!ok) return BadRequest();
            return NoContent();
        }
    }
}
