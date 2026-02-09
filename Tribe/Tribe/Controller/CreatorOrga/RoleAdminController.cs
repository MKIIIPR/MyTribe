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
    [Route("api/admin/organisation/{organisationId}/roles")]
    [Authorize]
    public class RoleAdminController : ControllerBase
    {
        private readonly IOrganizationRoleService _roleService;
        private readonly OrgaDbContext _db;

        public RoleAdminController(IOrganizationRoleService roleService, OrgaDbContext db)
        {
            _roleService = roleService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string organisationId)
        {
            var list = await _roleService.GetByOrganizationAsync(organisationId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string organisationId, [FromBody] OrganizationRole role)
        {
            role.OrganizationId = organisationId;
            var created = await _roleService.CreateAsync(role);
            return CreatedAtAction(nameof(GetAll), new { organisationId = organisationId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string organisationId, string id, [FromBody] OrganizationRole role)
        {
            var ok = await _roleService.UpdateAsync(id, role);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string organisationId, string id)
        {
            var existing = await _db.OrganizationRoles.FindAsync(id);
            if (existing == null || existing.OrganizationId != organisationId) return NotFound();
            var ok = await _roleService.DeleteAsync(id);
            if (!ok) return BadRequest();
            return NoContent();
        }
    }
}
