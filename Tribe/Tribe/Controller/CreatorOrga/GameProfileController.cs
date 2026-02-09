using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tribe.Controller.CreatorOrga.Services;
using Tribe.Bib.Models.CommunityManagement;

namespace Tribe.Controller.CreatorOrga
{
    [ApiController]
    [Route("api/admin/gameprofiles")]
    [Authorize]
    public class GameProfileController : ControllerBase
    {
        private readonly IGameProfileService _service;

        public GameProfileController(IGameProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GameProfile profile)
        {
            var created = await _service.CreateAsync(profile);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] GameProfile profile)
        {
            var success = await _service.UpdateAsync(id, profile);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
