using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Controller.Services;
using Tribe.Data;

namespace Tribe.Controller
{
    [ApiController]
    [Route("api/CreatorSubscription")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CreatorSubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOwnProfileService _profileService;
        private readonly ILogger<CreatorSubscriptionController> _logger;

        public CreatorSubscriptionController(
            ApplicationDbContext context, 
            IOwnProfileService profileService,
            ILogger<CreatorSubscriptionController> logger)
        {
            _context = context;
            _profileService = profileService;
            _logger = logger;
        }

        /// <summary>
        /// Checkout: Erstellt eine CreatorSubscription und setzt den User als Creator
        /// Unterstützt auch kostenlose Trial-Subscriptions
        /// </summary>
        [HttpPost("checkout")]
        public async Task<ActionResult<CreatorSubscription>> Checkout([FromBody] CreatorSubscription subscription)
        {
            var profileId = GetCurrentProfileId();
            if (string.IsNullOrEmpty(profileId))
            {
                return Unauthorized(new { error = "Nicht authentifiziert" });
            }

            try
            {
                _logger.LogInformation("CreatorSubscription checkout started for profile {ProfileId}, Duration: {Duration}", 
                    profileId, subscription.Duration);

                // Überschreibe die ID aus der Anfrage mit der ProfileId aus den Claims
                subscription.TribeProfileId = profileId;
                subscription.Guid = Guid.NewGuid().ToString();
                subscription.CreatedAt = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;
                subscription.StartDate = DateTime.UtcNow;
                subscription.IsActive = true;

                // Check if this is a FREE TRIAL
                bool isFreeTrial = subscription.Duration == "Trial" || subscription.PaymentStatus == "Trial";

                if (isFreeTrial)
                {
                    // Free Trial - setze Standard-Werte
                    subscription.Duration = "Trial";
                    subscription.PaymentStatus = "Trial";
                    subscription.SubValue = 0;
                    subscription.IsPaid = true; // Free = "bezahlt"
                    subscription.AutoRenew = false;
                    subscription.EndDate = DateTime.UtcNow.AddDays(14); // 14 Tage Trial
                    subscription.NextPaymentDate = subscription.EndDate;

                    // Validiere dass der Plan existiert (falls angegeben)
                    if (!string.IsNullOrEmpty(subscription.CreatorPlanId))
                    {
                        var plan = await _context.CreatorPlans.FindAsync(subscription.CreatorPlanId);
                        if (plan == null)
                        {
                            _logger.LogWarning("CreatorPlan {PlanId} not found for trial", subscription.CreatorPlanId);
                            // Für Trial ist das OK - wir setzen keinen Plan
                            subscription.CreatorPlanId = null;
                        }
                    }

                    _logger.LogInformation("Creating FREE TRIAL subscription for profile {ProfileId}", profileId);
                }
                else
                {
                    // Paid Plan - Validate CreatorPlan exists
                    if (string.IsNullOrEmpty(subscription.CreatorPlanId))
                    {
                        return BadRequest(new { error = "Creator-Plan ID erforderlich" });
                    }

                    var plan = await _context.CreatorPlans.FindAsync(subscription.CreatorPlanId);
                    if (plan == null)
                    {
                        _logger.LogWarning("CreatorPlan {PlanId} not found", subscription.CreatorPlanId);
                        return BadRequest(new { error = "Creator-Plan nicht gefunden" });
                    }

                    // Validate CreatorPlanPricing exists (nur für bezahlte Pläne erforderlich)
                    if (!string.IsNullOrEmpty(subscription.CreatorPlanPricingId))
                    {
                        var pricing = await _context.CreatorPlanPricings.FindAsync(subscription.CreatorPlanPricingId);
                        if (pricing == null)
                        {
                            _logger.LogWarning("CreatorPlanPricing {PricingId} not found", subscription.CreatorPlanPricingId);
                            return BadRequest(new { error = "Preiskonfiguration nicht gefunden" });
                        }

                        // Set subscription value from pricing
                        subscription.SubValue = (double)(subscription.Currency switch
                        {
                            "EUR" => pricing.ValueEuro,
                            "GBP" => pricing.ValueGbPound,
                            _ => pricing.ValueUSD
                        });
                    }
                }

                // Save the subscription
                _context.CreatorSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                // Now update the TribeUser to become a Creator
                var success = await _profileService.CheckOutSubscription(subscription);
                if (!success)
                {
                    _logger.LogError("Failed to set user as creator for profile {ProfileId}", profileId);
                    return StatusCode(500, new { error = "Fehler beim Aktivieren des Creator-Status" });
                }

                _logger.LogInformation("CreatorSubscription checkout completed for profile {ProfileId}, subscription {SubId}, Trial: {IsTrial}", 
                    profileId, subscription.Guid, isFreeTrial);

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CreatorSubscription checkout for profile {ProfileId}", profileId);
                return StatusCode(500, new { error = "Interner Serverfehler", details = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's active subscription
        /// </summary>
        [HttpGet("current")]
        public async Task<ActionResult<CreatorSubscription?>> GetCurrentSubscription()
        {
            var profileId = GetCurrentProfileId();
            if (string.IsNullOrEmpty(profileId))
            {
                return Unauthorized();
            }

            var subscription = await _context.CreatorSubscriptions
                .Include(s => s.CreatorPlan)
                .Include(s => s.CreatorPlanPricing)
                .FirstOrDefaultAsync(s => s.TribeProfileId == profileId && s.IsActive);

            return Ok(subscription);
        }

        /// <summary>
        /// Cancel subscription (soft delete - sets IsActive = false)
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var profileId = GetCurrentProfileId();
            if (string.IsNullOrEmpty(profileId))
            {
                return Unauthorized();
            }

            var subscription = await _context.CreatorSubscriptions
                .FirstOrDefaultAsync(s => s.TribeProfileId == profileId && s.IsActive);

            if (subscription == null)
            {
                return NotFound(new { error = "Keine aktive Subscription gefunden" });
            }

            subscription.IsActive = false;
            subscription.EndDate = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;

            // Note: We don't remove creator status immediately - it stays until subscription period ends
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription cancelled for profile {ProfileId}", profileId);

            return Ok(new { message = "Subscription wurde gekündigt" });
        }

        private string? GetCurrentProfileId()
        {
            return User.FindFirst("profileId")?.Value;
        }
    }
}