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
    [Route("api/admin/organisation/{organisationId}/recruitment")]
    [Authorize]
    public class RecruitmentAdminController : ControllerBase
    {
        private readonly IRecruitmentService _recruitmentService;
        private readonly OrgaDbContext _db;

        public RecruitmentAdminController(IRecruitmentService recruitmentService, OrgaDbContext db)
        {
            _recruitmentService = recruitmentService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(string organisationId)
        {
            var list = await _recruitmentService.GetPostsByOrganizationAsync(organisationId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(string organisationId, string id)
        {
            var post = await _recruitmentService.GetPostByIdAsync(id);
            if (post == null || post.OrganizationId != organisationId) return NotFound();
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string organisationId, [FromBody] RecruitmentPost post)
        {
            post.OrganizationId = organisationId;
            var created = await _recruitmentService.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPost), new { organisationId = organisationId, id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string organisationId, string id)
        {
            var existing = await _recruitmentService.GetPostByIdAsync(id);
            if (existing == null || existing.OrganizationId != organisationId) return NotFound();
            var ok = await _recruitmentService.DeletePostAsync(id);
            if (!ok) return BadRequest();
            return NoContent();
        }

        [HttpGet("{postId}/applications")]
        public async Task<IActionResult> GetApplications(string organisationId, string postId)
        {
            var apps = await _recruitmentService.GetApplicationsForPostAsync(postId);
            return Ok(apps);
        }

        [HttpPost("{postId}/applications")]
        public async Task<IActionResult> Apply(string organisationId, string postId, [FromBody] MemberApplication application)
        {
            application.RecruitmentPostId = postId;
            var created = await _recruitmentService.ApplyAsync(application);
            return CreatedAtAction(nameof(GetApplications), new { organisationId = organisationId, postId = postId }, created);
        }

        [HttpPost("applications/{applicationId}/review")]
        public async Task<IActionResult> Review(string organisationId, string applicationId, [FromBody] ReviewDto dto)
        {
            var ok = await _recruitmentService.ReviewApplicationAsync(applicationId, dto.ReviewerMemberId, dto.Status, dto.Notes);
            if (!ok) return BadRequest();
            return NoContent();
        }

        public class ReviewDto
        {
            public string ReviewerMemberId { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }
    }
}
