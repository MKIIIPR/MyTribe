using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;
using Microsoft.EntityFrameworkCore;
using Tribe.Controller.CreatorOrga.Services;

namespace Tribe.Controller.CreatorOrga
{
    [ApiController]
    [Route("api/admin/organisation/{organisationId}/assets")]
    [Authorize]
    public class AssetAdminController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly OrgaDbContext _db;

        public AssetAdminController(IAssetService assetService, OrgaDbContext db)
        {
            _assetService = assetService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string organisationId)
        {
            var list = await _assetService.GetOrganizationAssetsAsync(organisationId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string organisationId, string id)
        {
            var asset = await _assetService.GetOrganizationAssetByIdAsync(id);
            if (asset == null || asset.OrganizationId != organisationId) return NotFound();
            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string organisationId, [FromBody] OrganizationAsset asset)
        {
            asset.OrganizationId = organisationId;
            var created = await _assetService.CreateOrganizationAssetAsync(asset);
            return CreatedAtAction(nameof(Get), new { organisationId = organisationId, id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string organisationId, string id, [FromBody] OrganizationAsset asset)
        {
            var existing = await _assetService.GetOrganizationAssetByIdAsync(id);
            if (existing == null || existing.OrganizationId != organisationId) return NotFound();
            var ok = await _assetService.UpdateOrganizationAssetAsync(id, asset);
            if (!ok) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string organisationId, string id)
        {
            var existing = await _assetService.GetOrganizationAssetByIdAsync(id);
            if (existing == null || existing.OrganizationId != organisationId) return NotFound();
            var ok = await _assetService.DeleteOrganizationAssetAsync(id);
            if (!ok) return BadRequest();
            return NoContent();
        }
    }
}
