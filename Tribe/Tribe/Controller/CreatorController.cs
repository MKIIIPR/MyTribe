using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tribe.Data;

namespace YourProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreatorController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreatorController> _logger;

    public CreatorController(ApplicationDbContext context, ILogger<CreatorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Alle CreatorPlans abrufen
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreatorPlan>>> GetCreatorPlans()
    {
        try
        {
            var plans = await _context.CreatorPlans
                .Include(p => p.PricingList)
                .ToListAsync();

            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der CreatorPlans");
            return StatusCode(500, "Ein Fehler ist aufgetreten");
        }
    }

    /// <summary>
    /// CreatorPlan anhand der GUID abrufen
    /// </summary>
    [HttpGet("{guid}")]
    public async Task<ActionResult<CreatorPlan>> GetCreatorPlan(string guid)
    {
        try
        {
            var plan = await _context.CreatorPlans
                .Include(p => p.PricingList)
                .FirstOrDefaultAsync(p => p.Guid == guid);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen des CreatorPlans mit GUID {Guid}", guid);
            return StatusCode(500, "Ein Fehler ist aufgetreten");
        }
    }

    /// <summary>
    /// Neuen CreatorPlan erstellen
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreatorPlan>> CreateCreatorPlan(CreatorPlan creatorPlan)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Neue GUID generieren falls nicht vorhanden oder leer
            if (string.IsNullOrEmpty(creatorPlan.Guid))
            {
                creatorPlan.Guid = Guid.NewGuid().ToString();
            }

            // GUIDs für Preise generieren
            if (creatorPlan.PricingList != null)
            {
                foreach (var pricing in creatorPlan.PricingList)
                {
                    if (string.IsNullOrEmpty(pricing.Guid))
                    {
                        pricing.Guid = Guid.NewGuid().ToString();
                    }
                    pricing.CreatorPlanGuid = creatorPlan.Guid;
                }
            }

            _context.CreatorPlans.Add(creatorPlan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCreatorPlan), new { guid = creatorPlan.Guid }, creatorPlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Erstellen des CreatorPlans");
            return StatusCode(500, "Ein Fehler ist aufgetreten");
        }
    }

    /// <summary>
    /// CreatorPlan aktualisieren
    /// </summary>
    [HttpPut("{guid}")]
    public async Task<IActionResult> UpdateCreatorPlan(string guid, CreatorPlan creatorPlan)
    {
        try
        {
            if (guid != creatorPlan.Guid)
            {
                return BadRequest("GUID stimmt nicht überein");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPlan = await _context.CreatorPlans
                .Include(p => p.PricingList)
                .FirstOrDefaultAsync(p => p.Guid == guid);

            if (existingPlan == null)
            {
                return NotFound();
            }

            // Plan-Eigenschaften aktualisieren
            existingPlan.Name = creatorPlan.Name;
            existingPlan.TokenMenge = creatorPlan.TokenMenge;
            existingPlan.FeaturesJson = creatorPlan.FeaturesJson;
            existingPlan.Aktiv = creatorPlan.Aktiv;

            // Preise aktualisieren
            if (creatorPlan.PricingList != null)
            {
                // Alte Preise entfernen
                _context.CreatorPlanPricings.RemoveRange(existingPlan.PricingList ?? new List<CreatorPlanPricing>());

                // Neue Preise hinzufügen
                foreach (var pricing in creatorPlan.PricingList)
                {
                    if (string.IsNullOrEmpty(pricing.Guid))
                    {
                        pricing.Guid = Guid.NewGuid().ToString();
                    }
                    pricing.CreatorPlanGuid = guid;
                    _context.CreatorPlanPricings.Add(pricing);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Aktualisieren des CreatorPlans mit GUID {Guid}", guid);
            return StatusCode(500, "Ein Fehler ist aufgetreten");
        }
    }

    /// <summary>
    /// CreatorPlan löschen
    /// </summary>
    [HttpDelete("{guid}")]
    public async Task<IActionResult> DeleteCreatorPlan(string guid)
    {
        try
        {
            var plan = await _context.CreatorPlans
                .Include(p => p.PricingList)
                .FirstOrDefaultAsync(p => p.Guid == guid);

            if (plan == null)
            {
                return NotFound();
            }

            // Zuerst die Preise löschen
            if (plan.PricingList != null && plan.PricingList.Any())
            {
                _context.CreatorPlanPricings.RemoveRange(plan.PricingList);
            }

            // Dann den Plan löschen
            _context.CreatorPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Löschen des CreatorPlans mit GUID {Guid}", guid);
            return StatusCode(500, "Ein Fehler ist aufgetreten");
        }
    }

    /// <summary>
    /// Prüfen ob CreatorPlan existiert
    /// </summary>
    [HttpHead("{guid}")]
    public async Task<IActionResult> CreatorPlanExists(string guid)
    {
        try
        {
            var exists = await _context.CreatorPlans.AnyAsync(p => p.Guid == guid);
            return exists ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Prüfen der Existenz des CreatorPlans mit GUID {Guid}", guid);
            return StatusCode(500, "Ein Fehler ist aufgetreten");
        }
    }
}
