using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tribe.Controller.Services;
using Tribe.Data;
using Tribe.Bib.Models.TribeRelated;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/raffles")]
    [Authorize]
    public class RaffleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRaffleService _raffleService;
        private readonly ILogger<RaffleController> _logger;

        public RaffleController(ApplicationDbContext context, IRaffleService raffleService, ILogger<RaffleController> logger)
        {
            _context = context;
            _raffleService = raffleService;
            _logger = logger;
        }

        // Gibt die ApplicationUser.Id zurück (aus AspNetUsers)
        private string GetApplicationUserId()
        {
            // Debug: Alle Claims loggen
            _logger.LogInformation("=== DEBUG: User Claims ===");
            _logger.LogInformation("User.Identity.IsAuthenticated: {IsAuth}", User.Identity?.IsAuthenticated);
            _logger.LogInformation("User.Identity.AuthenticationType: {AuthType}", User.Identity?.AuthenticationType);

            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            _logger.LogInformation("=== END DEBUG ===");

            // Versuche verschiedene Claim-Namen
            var userId = User.FindFirstValue("nameid")
                         ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("sub")
                         ?? User.FindFirstValue("id")
                         ?? string.Empty;

            _logger.LogInformation("Resolved UserId: {UserId}", userId);
            return userId;
        }

        // Ermittelt die CreatorProfile.Id (TribeUser.Id) anhand der ApplicationUserId
        private async Task<string?> GetCreatorProfileIdAsync()
        {
            var appUserId = GetApplicationUserId();
            if (string.IsNullOrEmpty(appUserId)) return null;

            var creatorProfile = await _context.TribeUsers
                .FirstOrDefaultAsync(p => p.Id == appUserId);

            return creatorProfile?.Id;
        }

        /// <summary>
        /// Create a new raffle (Creator only)
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateRaffle([FromBody] Raffle raffle)
        {
            try
            {
                if (raffle == null)
                    return BadRequest(new { error = "Raffle data required" });

                // Ermittle die CreatorProfile.Id (nicht ApplicationUser.Id!)
                var creatorProfileId = await GetCreatorProfileIdAsync();
                if (string.IsNullOrEmpty(creatorProfileId))
                    return BadRequest(new { error = "No creator profile found for current user" });

                raffle.CreatorProfileId = creatorProfileId;

                // Defaults setzen falls nicht vorhanden
                if (string.IsNullOrEmpty(raffle.RaffleType))
                    raffle.RaffleType = "Standard";

                if (raffle.StartDate == default || raffle.StartDate == null) 
                    raffle.StartDate = DateTime.UtcNow;

                var raffleId = await _raffleService.CreateRaffleAsync(raffle);
                _logger.LogInformation("Raffle created: {RaffleId} by CreatorProfile {CreatorProfileId}", raffleId, creatorProfileId);

                return CreatedAtAction(nameof(GetRaffle), new { id = raffleId }, new { raffleId = raffleId, message = "Raffle created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating raffle");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get raffle by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRaffle(string id)
        {
            var raffle = await _raffleService.GetRaffleByIdAsync(id);
            if (raffle == null)
                return NotFound(new { error = "Raffle not found" });

            return Ok(raffle);
        }

        /// <summary>
        /// Get all raffles for current creator
        /// </summary>
        [HttpGet("creator/all")]
        public async Task<IActionResult> GetCreatorRaffles()
        {
            var creatorProfileId = await GetCreatorProfileIdAsync();
            if (string.IsNullOrEmpty(creatorProfileId))
                return Ok(new List<Raffle>()); // Kein Creator-Profil -> leere Liste

            var raffles = await _raffleService.GetCreatorRafflesAsync(creatorProfileId);
            return Ok(raffles);
        }
        /// <summary>
        /// Check whether a raffle with given id exists and belongs to current user
        /// Returns true if raffle exists and CreatorProfileId == current user id
        /// </summary>
        [HttpGet("alreadyexist/{id}")]
        public async Task<IActionResult> AlreadyExists(string id)
        {
            var raffle = await _raffleService.GetRaffleByIdAsync(id);
            if (raffle == null) return Ok(false);

            var creatorProfileId = await GetCreatorProfileIdAsync();
            return Ok(raffle.CreatorProfileId == creatorProfileId);
        }

        /// <summary>
        /// Update a raffle (Creator only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRaffle(string id, [FromBody] Raffle raffle)
        {
            var existingRaffle = await _raffleService.GetRaffleByIdAsync(id);
            if (existingRaffle == null)
                return NotFound();

            var creatorProfileId = await GetCreatorProfileIdAsync();
            if (existingRaffle.CreatorProfileId != creatorProfileId)
                return Forbid();

            raffle.Id = id;
            raffle.CreatorProfileId = creatorProfileId!;
            var success = await _raffleService.UpdateRaffleAsync(raffle);

            if (!success)
                return BadRequest(new { error = "Failed to update raffle" });

            _logger.LogInformation("Raffle updated: {RaffleId}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a raffle (Creator only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRaffle(string id)
        {
            var raffle = await _raffleService.GetRaffleByIdAsync(id);
            if (raffle == null)
                return NotFound();

            var creatorProfileId = await GetCreatorProfileIdAsync();
            if (raffle.CreatorProfileId != creatorProfileId)
                return Forbid();

            var success = await _raffleService.DeleteRaffleAsync(id);
            if (!success)
                return BadRequest(new { error = "Failed to delete raffle" });

            _logger.LogInformation("Raffle deleted: {RaffleId}", id);
            return NoContent();
        }

        /// <summary>
        /// Bind a raffle to a product (Creator only, 1 product = 1 raffle)
        /// </summary>
        [HttpPost("bind/{raffleId}/product/{productId}")]
        public async Task<IActionResult> BindRaffleToProduct(string raffleId, string productId)
        {
            // Validate raffle exists and belongs to creator
            var raffle = await _raffleService.GetRaffleByIdAsync(raffleId);
            if (raffle == null)
                return NotFound(new { error = "Raffle not found" });

            var creatorProfileId = await GetCreatorProfileIdAsync();
            if (raffle.CreatorProfileId != creatorProfileId)
                return Forbid();

            var success = await _raffleService.BindRaffleToProductAsync(productId, raffleId);
            if (!success)
                return BadRequest(new { error = "Failed to bind raffle to product" });

            _logger.LogInformation("Raffle {RaffleId} bound to Product {ProductId}", raffleId, productId);
            return Ok(new { message = "Raffle bound to product successfully" });
        }

        /// <summary>
        /// Unbind raffle from product (Creator only)
        /// </summary>
        [HttpDelete("unbind/product/{productId}")]
        public async Task<IActionResult> UnbindRaffleFromProduct(string productId)
        {
            var success = await _raffleService.UnbindRaffleFromProductAsync(productId);
            if (!success)
                return BadRequest(new { error = "Failed to unbind raffle from product" });

            _logger.LogInformation("Raffle unbound from Product {ProductId}", productId);
            return Ok(new { message = "Raffle unbound from product successfully" });
        }

        /// <summary>
        /// Get raffle bound to a product
        /// </summary>
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductRaffle(string productId)
        {
            var raffle = await _raffleService.GetProductRaffleAsync(productId);
            if (raffle == null)
                return NotFound(new { message = "No raffle bound to this product" });

            return Ok(raffle);
        }

        /// <summary>
        /// Add entry to raffle (Customer, called after purchase)
        /// </summary>
        [HttpPost("{raffleId}/entry")]
        public async Task<IActionResult> AddRaffleEntry(string raffleId, [FromBody] AddEntryRequest request)
        {
            if (request == null || request.Quantity <= 0)
                return BadRequest(new { error = "Invalid quantity" });

            var creatorProfileId = await GetCreatorProfileIdAsync();
            var success = await _raffleService.AddRaffleEntryAsync(raffleId, creatorProfileId, request.Quantity);

            if (!success)
                return BadRequest(new { error = "Failed to add raffle entry" });

            _logger.LogInformation("Raffle entry added: {RaffleId} x {Quantity} for User {UserId}", raffleId, request.Quantity, creatorProfileId);
            return Ok(new { message = $"Added {request.Quantity} entry(ies) to raffle" });
        }

        /// <summary>
        /// Get raffle entry count
        /// </summary>
        [HttpGet("{raffleId}/entries")]
        public async Task<IActionResult> GetRaffleEntries(string raffleId)
        {
            var count = await _raffleService.GetRaffleEntriesAsync(raffleId);
            return Ok(new { raffleId = raffleId, entryCount = count });
        }

        public class AddEntryRequest
        {
            public int Quantity { get; set; } = 1;
        }
    }
}
