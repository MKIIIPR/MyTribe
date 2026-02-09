using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Tribe.Controller.CreatorOrga
{
    [ApiController]
    [Route("api/admin/organisation/{organisationId}/blog")]
    [Authorize]
    public class BlogAdminController : ControllerBase
    {
        // Placeholder implementation until BlogPost domain model and DbSet exist

        [HttpGet]
        public Task<IActionResult> GetAll(string organisationId)
        {
            return Task.FromResult<IActionResult>(Ok(Array.Empty<object>()));
        }

        [HttpGet("{id}")]
        public Task<IActionResult> Get(string organisationId, string id)
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

        [HttpPost]
        public Task<IActionResult> Create(string organisationId)
        {
            return Task.FromResult<IActionResult>(CreatedAtAction(nameof(Get), new { organisationId = organisationId, id = "" }, null));
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(string organisationId, string id)
        {
            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}
