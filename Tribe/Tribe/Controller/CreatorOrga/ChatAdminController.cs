using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Tribe.Controller.CreatorOrga
{
    [ApiController]
    [Route("api/admin/organisation/{organisationId}/chats")]
    [Authorize]
    public class ChatAdminController : ControllerBase
    {
        private readonly Data.OrgaDbContext _db;

        public ChatAdminController(Data.OrgaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string organisationId)
        {
            var chats = await _db.OrganizationChats
                .AsNoTracking()
                .Where(c => c.OrganizationId == organisationId)
                .ToListAsync();
            return Ok(chats);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string organisationId, string id)
        {
            var chat = await _db.OrganizationChats
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrganizationId == organisationId && c.Id == id);
            if (chat == null) return NotFound();
            return Ok(chat);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string organisationId, [FromBody] Tribe.Bib.Models.CommunityManagement.OrganizationChat chat)
        {
            chat.OrganizationId = organisationId;
            _db.OrganizationChats.Add(chat);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { organisationId, id = chat.Id }, chat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string organisationId, string id, [FromBody] Tribe.Bib.Models.CommunityManagement.OrganizationChat chat)
        {
            var existing = await _db.OrganizationChats.FirstOrDefaultAsync(c => c.OrganizationId == organisationId && c.Id == id);
            if (existing == null) return NotFound();
            existing.Name = chat.Name;
            existing.Description = chat.Description;
            existing.ChatType = chat.ChatType;
            existing.IsPublic = chat.IsPublic;
            existing.IsAnnouncementOnly = chat.IsAnnouncementOnly;
            existing.AllowFiles = chat.AllowFiles;
            existing.AllowImages = chat.AllowImages;
            existing.AllowedRoleIdsJson = chat.AllowedRoleIdsJson;
            existing.MessageRetentionDays = chat.MessageRetentionDays;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string organisationId, string id)
        {
            var chat = await _db.OrganizationChats.FirstOrDefaultAsync(c => c.OrganizationId == organisationId && c.Id == id);
            if (chat == null) return NotFound();
            _db.OrganizationChats.Remove(chat);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
