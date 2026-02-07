using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tribe.Data;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ShopDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string? GetUserId() => User?.FindFirstValue("profileId");

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsForProduct(string productId)
        {
            var reviews = await _context.ProductReviews.Where(r => r.ProductId == productId).ToListAsync();
            return Ok(reviews);
        }

        [HttpPost("product/{productId}")]
        [Authorize]
        public async Task<IActionResult> PostReview(string productId, [FromBody] ProductReview review)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            review.Id = Guid.NewGuid().ToString();
            review.ProductId = productId;
            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReviewsForProduct), new { productId = productId }, review);
        }
    }
}
