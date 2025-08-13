using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Data;

namespace Tribe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GenericApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GenericApiController> _logger;

        public GenericApiController(ApplicationDbContext context, ILogger<GenericApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private DbSet<T> GetDbSet<T>() where T : class
        {
            var property = _context.GetType().GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(DbSet<T>));

            if (property == null)
                throw new InvalidOperationException($"DbSet<{typeof(T).Name}> not found in context");

            return (DbSet<T>)property.GetValue(_context)!;
        }
        [HttpPut("update/creatorprofile/{id}")]
        public async Task<ActionResult<CreatorProfile>> UpdateCreatorProfileAsync(string id, [FromBody] CreatorProfile profile)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var existingProfile = await _context.CreatorProfiles
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingProfile == null)
                    return NotFound($"CreatorProfile with ID {id} not found");

                // Validate ownership
                if (existingProfile.Id != userId)
                    return Forbid("You can only update your own profile");

                // Manual property update - keine Navigation Properties!
                existingProfile.CreatorName = profile.CreatorName;
                existingProfile.ImageTemplateUrl = profile.ImageTemplateUrl;
                existingProfile.BannerUrl = profile.BannerUrl;
                existingProfile.PatreonUrl = profile.PatreonUrl;
                existingProfile.YouTubeUrl = profile.YouTubeUrl;
                existingProfile.TwitchUrl = profile.TwitchUrl;
                existingProfile.TwitterUrl = profile.TwitterUrl;
                existingProfile.InstagramUrl = profile.InstagramUrl;
                existingProfile.TikTokUrl = profile.TikTokUrl;
                existingProfile.DiscordUrl = profile.DiscordUrl;
                existingProfile.AcceptingCollaborations = profile.AcceptingCollaborations;
                existingProfile.CollaborationInfo = profile.CollaborationInfo;

                await _context.SaveChangesAsync();

                return Ok(existingProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CreatorProfile with ID: {Id}. Error: {Message}", id, ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task<bool> ValidateOwnership<T>(T entity, string userId) where T : class
        {
            // Für CreatorProfile - prüfe UserId
            if (typeof(T) == typeof(CreatorProfile))
            {
                var profile = entity as CreatorProfile;
                return profile?.Id == userId;
            }

            // Für Unterklassen - prüfe über CreatorProfileId
            var creatorProfileIdProperty = typeof(T).GetProperty("CreatorProfileId");
            if (creatorProfileIdProperty != null)
            {
                var creatorProfileId = creatorProfileIdProperty.GetValue(entity)?.ToString();
                if (!string.IsNullOrEmpty(creatorProfileId))
                {
                    var profile = await _context.CreatorProfiles
                        .FirstOrDefaultAsync(p => p.Id == creatorProfileId);
                    return profile?.Id == userId;
                }
            }

            return false;
        }

        // GET: api/genericapi/get/{entityType}/{id}
        [HttpGet("get/{entityType}/{id}")]
        public async Task<ActionResult<object>> GetByIdAsync(string entityType, string id)
        {
            try
            {
                var type = GetEntityType(entityType);
                if (type == null)
                    return BadRequest($"Unknown entity type: {entityType}");

                var method = typeof(GenericApiController)
                    .GetMethod(nameof(GetByIdGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(type);

                var result = await (Task<ActionResult<object>>)method.Invoke(this, new object[] { id })!;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity by ID: {EntityType} - {Id}", entityType, id);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ActionResult<object>> GetByIdGeneric<T>(string id) where T : class
        {
            var dbSet = GetDbSet<T>();
            var query = dbSet.AsQueryable();

            // Include related data für CreatorProfile
            if (typeof(T) == typeof(CreatorProfile))
            {
                query = query
                    .Include("CreatorTokens")
                    .Include("Partner")
                    .Include("Placements")
                    .Include("Raffles");
            }

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);

            if (entity == null)
                return NotFound($"{typeof(T).Name} with ID {id} not found");

            return Ok(entity);
        }

        // GET: api/genericapi/getall/{entityType}
        [HttpGet("getall/{entityType}")]
        public async Task<ActionResult<object>> GetAllAsync(string entityType)
        {
            try
            {
                var type = GetEntityType(entityType);
                if (type == null)
                    return BadRequest($"Unknown entity type: {entityType}");

                var method = typeof(GenericApiController)
                    .GetMethod(nameof(GetAllGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(type);

                var result = await (Task<ActionResult<object>>)method.Invoke(this, Array.Empty<object>())!;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities: {EntityType}", entityType);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ActionResult<object>> GetAllGeneric<T>() where T : class
        {
            var dbSet = GetDbSet<T>();
            var query = dbSet.AsQueryable();

            // Include related data für CreatorProfile
            if (typeof(T) == typeof(CreatorProfile))
            {
                query = query
                    .Include("CreatorTokens")
                    .Include("Partner")
                    .Include("Placements")
                    .Include("Raffles");
            }

            var entities = await query.ToListAsync();
            return Ok(entities);
        }

        // GET: api/genericapi/getbyparent/{entityType}/{parentId}
        [HttpGet("getbyparent/{entityType}/{parentId}")]
        public async Task<ActionResult<object>> GetByParentIdAsync(string entityType, string parentId)
        {
            try
            {
                var type = GetEntityType(entityType);
                if (type == null)
                    return BadRequest($"Unknown entity type: {entityType}");

                var method = typeof(GenericApiController)
                    .GetMethod(nameof(GetByParentIdGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(type);

                var result = await (Task<ActionResult<object>>)method.Invoke(this, new object[] { parentId })!;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entities by parent ID: {EntityType} - {ParentId}", entityType, parentId);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ActionResult<object>> GetByParentIdGeneric<T>(string parentId) where T : class
        {
            var dbSet = GetDbSet<T>();
            var entities = await dbSet
                .Where(e => EF.Property<string>(e, "CreatorProfileId") == parentId)
                .ToListAsync();

            return Ok(entities);
        }

        // POST: api/genericapi/create/{entityType}
        [HttpPost("create/{entityType}")]
        public async Task<ActionResult<object>> CreateAsync(string entityType, [FromBody] object data)
        {
            try
            {
                var type = GetEntityType(entityType);
                if (type == null)
                    return BadRequest($"Unknown entity type: {entityType}");

                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var entity = System.Text.Json.JsonSerializer.Deserialize(json, type);

                if (entity == null)
                    return BadRequest("Invalid entity data");

                var method = typeof(GenericApiController)
                    .GetMethod(nameof(CreateGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(type);

                var result = await (Task<ActionResult<object>>)method.Invoke(this, new object[] { entity })!;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating entity: {EntityType}", entityType);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ActionResult<object>> CreateGeneric<T>(T entity) where T : class
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Set ID für neue Entität
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
                idProperty.SetValue(entity, Guid.NewGuid().ToString());

            // Set UserId für CreatorProfile
            if (typeof(T) == typeof(CreatorProfile))
            {
                var userIdProperty = typeof(T).GetProperty("UserId");
                if (userIdProperty != null)
                    userIdProperty.SetValue(entity, userId);

                var createdAtProperty = typeof(T).GetProperty("CreatedAt");
                if (createdAtProperty != null)
                    createdAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            // Validate ownership für Unterklassen
            if (typeof(T) != typeof(CreatorProfile))
            {
                var isValid = await ValidateOwnership(entity, userId);
                if (!isValid)
                    return Forbid("You can only create entities for your own profile");
            }

            var dbSet = GetDbSet<T>();
            dbSet.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync),
                new { entityType = typeof(T).Name.ToLower(), id = idProperty?.GetValue(entity) },
                entity);
        }

        // PUT: api/genericapi/update/{entityType}/{id}
        [HttpPut("update/{entityType}/{id}")]
        public async Task<ActionResult<object>> UpdateAsync(string entityType, string id, [FromBody] object data)
        {
            try
            {
                var type = GetEntityType(entityType);
                if (type == null)
                    return BadRequest($"Unknown entity type: {entityType}");

                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var entity = System.Text.Json.JsonSerializer.Deserialize(json, type);

                if (entity == null)
                    return BadRequest("Invalid entity data");

                var method = typeof(GenericApiController)
                    .GetMethod(nameof(UpdateGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(type);

                var result = await (Task<ActionResult<object>>)method.Invoke(this, new object[] { id, entity })!;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity: {EntityType} - {Id}", entityType, id);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ActionResult<object>> UpdateGeneric<T>(string id, T updatedEntity) where T : class
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var dbSet = GetDbSet<T>();
            var existingEntity = await dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);

            if (existingEntity == null)
                return NotFound($"{typeof(T).Name} with ID {id} not found");

            // Validate ownership
            var isValid = await ValidateOwnership(existingEntity, userId);
            if (!isValid)
                return Forbid("You can only update your own entities");

            // Update properties
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanWrite && p.Name != "Id" && p.Name != "UserId" && p.Name != "CreatedAt");

            foreach (var property in properties)
            {
                var newValue = property.GetValue(updatedEntity);
                if (newValue != null)
                {
                    property.SetValue(existingEntity, newValue);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(existingEntity);
        }

        // DELETE: api/genericapi/delete/{entityType}/{id}
        [HttpDelete("delete/{entityType}/{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(string entityType, string id)
        {
            try
            {
                var type = GetEntityType(entityType);
                if (type == null)
                    return BadRequest($"Unknown entity type: {entityType}");

                var method = typeof(GenericApiController)
                    .GetMethod(nameof(DeleteGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(type);

                var result = await (Task<ActionResult<bool>>)method.Invoke(this, new object[] { id })!;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity: {EntityType} - {Id}", entityType, id);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ActionResult<bool>> DeleteGeneric<T>(string id) where T : class
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var dbSet = GetDbSet<T>();
            var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);

            if (entity == null)
                return NotFound($"{typeof(T).Name} with ID {id} not found");

            // Validate ownership
            var isValid = await ValidateOwnership(entity, userId);
            if (!isValid)
                return Forbid("You can only delete your own entities");

            dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private Type? GetEntityType(string entityType)
        {
            return entityType.ToLower() switch
            {
                "creatorprofile" => typeof(CreatorProfile),
                "affiliatepartner" => typeof(AffiliatePartner),
                "creatortoken" => typeof(CreatorToken),
                "creatorplacement" => typeof(CreatorPlacement),
                "raffle" => typeof(Raffle),
                _ => null
            };
        }

        // GET: api/genericapi/current/creatorprofile - Aktuelles User-Profil
        [HttpGet("current/creatorprofile")]
        public async Task<ActionResult<CreatorProfile>> GetCurrentUserProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var profile = await _context.CreatorProfiles
                    .Include(p => p.CreatorTokens)
                    .Include(p => p.AffiliatePartners)
                    .Include(p => p.Placements)
                    .Include(p => p.Raffles)
                    .FirstOrDefaultAsync(p => p.Id == userId);

                if (profile == null)
                    return NotFound("No CreatorProfile found for current user");

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user's CreatorProfile");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}