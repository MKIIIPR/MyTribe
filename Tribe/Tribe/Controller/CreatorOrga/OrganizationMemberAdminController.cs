using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Tribe.Bib.Models.CommunityManagement;

namespace Tribe.Controller.CreatorOrga
{
    [ApiController]
    [Route("api/admin/organisation/{organisationId}/members")]
    [Authorize]
    public class OrganizationMemberAdminController : ControllerBase
    {
        private readonly Data.OrgaDbContext _db;

        public OrganizationMemberAdminController(Data.OrgaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string organisationId)
        {
            var members = await _db.OrganizationMembers
                .AsNoTracking()
                .Where(m => m.OrganizationId == organisationId)
                .ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string organisationId, string id)
        {
            var member = await _db.OrganizationMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.OrganizationId == organisationId && m.Id == id);
            if (member == null) return NotFound();
            return Ok(member);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string organisationId, [FromBody] OrganizationMember member)
        {
            member.OrganizationId = organisationId;
            _db.OrganizationMembers.Add(member);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { organisationId, id = member.Id }, member);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string organisationId, string id, [FromBody] OrganizationMember member)
        {
            var existing = await _db.OrganizationMembers.FirstOrDefaultAsync(m => m.OrganizationId == organisationId && m.Id == id);
            if (existing == null) return NotFound();
            existing.UserId = member.UserId;
            existing.RoleId = member.RoleId;
            existing.Status = member.Status;
            existing.Notes = member.Notes;
            existing.JoinedAt = member.JoinedAt;
            existing.LastActive = member.LastActive;
            existing.InactiveSince = member.InactiveSince;
            existing.LeaveDate = member.LeaveDate;
            existing.AttendanceCount = member.AttendanceCount;
            existing.EventOrganizedCount = member.EventOrganizedCount;
            existing.ContributionScore = member.ContributionScore;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string organisationId, string id)
        {
            var member = await _db.OrganizationMembers.FirstOrDefaultAsync(m => m.OrganizationId == organisationId && m.Id == id);
            if (member == null) return NotFound();
            _db.OrganizationMembers.Remove(member);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}   