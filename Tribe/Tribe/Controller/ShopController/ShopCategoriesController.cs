using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tribe.Data;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/categories")]
    [Authorize]
    public class ShopCategoriesController : ControllerBase
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<ShopCategoriesController> _logger;

        public ShopCategoriesController(ShopDbContext context, ILogger<ShopCategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetMyCategories()
        {
            var userId = GetUserId();
            var categories = await _context.ShopCategories
                .Where(c => c.CreatorProfileId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(string id)
        {
            var cat = await _context.ShopCategories.FindAsync(id);
            if (cat == null) return NotFound();
            var userId = GetUserId();
            if (cat.CreatorProfileId != userId) return Forbid();
            return Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] ShopCategory category)
        {
            var userId = GetUserId();
            category.Id = Guid.NewGuid().ToString();
            category.CreatorProfileId = userId;
            category.CreatedAt = DateTime.UtcNow;
            _context.ShopCategories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] ShopCategory category)
        {
            if (id != category.Id) return BadRequest();
            var existing = await _context.ShopCategories.FindAsync(id);
            if (existing == null) return NotFound();
            var userId = GetUserId();
            if (existing.CreatorProfileId != userId) return Forbid();

            existing.Name = category.Name;
            existing.ColorHex = category.ColorHex;

            _context.ShopCategories.Update(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var existing = await _context.ShopCategories.FindAsync(id);
            if (existing == null) return NotFound();
            var userId = GetUserId();
            if (existing.CreatorProfileId != userId) return Forbid();

            _context.ShopCategories.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
