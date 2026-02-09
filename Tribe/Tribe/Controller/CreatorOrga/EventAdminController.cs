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
    [Route("api/admin/organisation/{organisationId}/events")]
    [Authorize]
    public class EventAdminController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly OrgaDbContext _db;

        public EventAdminController(IEventService eventService, OrgaDbContext db)
        {
            _eventService = eventService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string organisationId)
        {
            var list = await _eventService.GetByOrganizationAsync(organisationId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string organisationId, string id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null || ev.OrganizationId != organisationId) return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string organisationId, [FromBody] Event ev)
        {
            ev.OrganizationId = organisationId;
            var created = await _eventService.CreateAsync(ev);
            return CreatedAtAction(nameof(Get), new { organisationId = organisationId, id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string organisationId, string id, [FromBody] Event ev)
        {
            var existing = await _eventService.GetByIdAsync(id);
            if (existing == null || existing.OrganizationId != organisationId) return NotFound();
            var ok = await _eventService.UpdateAsync(id, ev);
            if (!ok) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string organisationId, string id)
        {
            var existing = await _eventService.GetByIdAsync(id);
            if (existing == null || existing.OrganizationId != organisationId) return NotFound();
            var ok = await _eventService.DeleteAsync(id);
            if (!ok) return BadRequest();
            return NoContent();
        }
    }
}
