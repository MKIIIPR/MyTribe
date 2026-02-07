using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Data;
using Tribe.Services.Static;
namespace Tribe.Controller.Services
{
    public interface IOwnProfileService
    {
        Task<TribeUser> CreateAsync(TribeUser profile);
        Task<bool> ExistsAsync(string userId);
        Task<TribeUser?> GetOwnProfile(string userId);
        Task<TribeUser> UpdateAsync(TribeUser profile);
        Task<bool> CheckOutSubscription(CreatorSubscription sub);
    }
    public class OwnProfileService : IOwnProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OwnProfileService> _logger;

        public OwnProfileService(ApplicationDbContext context, ILogger<OwnProfileService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }
        public async Task<bool> CheckOutSubscription(CreatorSubscription sub)
        {
            if (sub == null)
                throw new ArgumentNullException(nameof(sub), "CreatorSubscription darf nicht null sein.");
            if (string.IsNullOrEmpty(sub.TribeProfileId))
                throw new ArgumentException("ApplicationUserId darf nicht null oder leer sein.", nameof(sub.TribeProfileId));
            try
            {
                // 1. Suche nach dem TribeProfile
                var profile = await _context.TribeUsers
                    .FirstOrDefaultAsync(p => p.Id == sub.TribeProfileId);
                // 2. Wenn kein Profil gefunden wird, erstelle ein neues
                if (profile == null)
                {
                    return false;
                }
                // 2.1. Wenn das Profil bereits eine aktive Subscription hat, breche ab
                // Paymentcheck


                // 3. Aktualisiere das Profil mit den Daten aus der Subscription
                sub.TribeProfileId = profile.Id; // Sicherstellen, dass die ID korrekt gesetzt ist
                profile.ActiveCreatorSubscription = sub;
                profile.UpdatedAt = DateTime.UtcNow;
                profile.IsCreator = true;
                profile.ProfileType = Constants.ProfileTypes.Creator;

                await EnsureCreatorProfileAsync(profile.Id, true);
                profile.ActiveCreatorSubscription.StartDate= DateTime.UtcNow; // Setze das Flag für CreatorApplication


                _context.TribeUsers.Update(profile);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CreatorApplication erfolgreich für User {UserId}", sub.TribeProfileId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei der CreatorApplication für User {UserId}", sub.TribeProfileId);
                return false;
            }
            return true;
        }
        public async Task<TribeUser?> GetOwnProfile(string profileId)
        {
            if (string.IsNullOrEmpty(profileId))
                throw new ArgumentException("Profile ID darf nicht null oder leer sein.", nameof(profileId));

            try
            {
                // Suche nach dem TribeProfile anhand der TribeUser.Id (profileId aus JWT)
                var profile = await _context.TribeUsers
                    .FirstOrDefaultAsync(p => p.Id == profileId);

                if (profile == null)
                {
                    return null;
                }

                if (profile.IsCreator)
                {
                    profile.CreatorProfile = await _context.CreatorProfiles.FindAsync(profile.Id);
                }

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Profils für ProfileId {ProfileId}", profileId);
                throw;
            }
        }
        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.TribeUsers.AnyAsync(p => p.Id == userId);
        }

        public async Task<TribeUser> CreateAsync(TribeUser profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (string.IsNullOrEmpty(profile.Id))
                throw new ArgumentException("Profil muss eine gültige ID (User-ID) haben.", nameof(profile));

            if (await ExistsAsync(profile.Id))
                throw new InvalidOperationException($"Profil mit der ID '{profile.Id}' existiert bereits.");

            try
            {
                profile.CreatedAt = DateTime.UtcNow;
                profile.UpdatedAt = DateTime.UtcNow;

                _context.TribeUsers.Add(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Profil erstellt für User {UserId}", profile.Id);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Profils für User {UserId}", profile.Id);
                throw;
            }
        }

        public async Task<TribeUser> UpdateAsync(TribeUser profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (string.IsNullOrEmpty(profile.Id))
                throw new ArgumentException("Profil muss eine ID haben.", nameof(profile));

            var existing = await _context.TribeUsers.FindAsync(profile.Id);
            if (existing == null)
                throw new InvalidOperationException($"Profil mit ID '{profile.Id}' wurde nicht gefunden.");

            try
            {
                // Alle Felder aktualisieren (außer ID, IsCreator und Navigation Properties)
                existing.DisplayName = profile.DisplayName;
                existing.Bio = profile.Bio;
                // IsCreator wird NICHT vom Client überschrieben – nur via CheckOutSubscription
                
                // Avatar-Verarbeitung
                if (!string.IsNullOrEmpty(profile.AvatarUrl))
                {
                    // Prüfe ob es Base64 ist
                    if (profile.AvatarUrl.Contains("data:image/") && profile.AvatarUrl.Contains(";base64,"))
                    {
                        // Extrahiere nur den Base64-Teil
                        var parts = profile.AvatarUrl.Split(",");
                        if (parts.Length == 2)
                        {
                            var base64Data = parts[1];
                            
                            // Prüfe die Größe des Base64-Strings
                            // Base64 ist ca. 33% größer als Binärdaten, also:
                            // Base64-Größe / 1.33 ≈ tatsächliche Größe
                            var estimatedSizeInBytes = (base64Data.Length * 3) / 4;
                            var maxSizeInBytes = 5 * 1024 * 1024; // 5 MB max
                            
                            _logger.LogInformation("Avatar-Größe: {SizeKB} KB", estimatedSizeInBytes / 1024);
                            
                            if (estimatedSizeInBytes > maxSizeInBytes)
                            {
                                _logger.LogWarning("Avatar zu groß ({SizeKB} KB). Maximum: 5 MB. Avatar wird nicht aktualisiert.", 
                                    estimatedSizeInBytes / 1024);
                                // Avatar wird NICHT aktualisiert
                            }
                            else
                            {
                                // Avatar-Größe OK → übernehm direkt als Data URL
                                existing.AvatarUrl = profile.AvatarUrl;
                                _logger.LogInformation("Avatar aktualisiert (Base64, {SizeKB} KB)", 
                                    estimatedSizeInBytes / 1024);
                            }
                        }
                    }
                    else
                    {
                        // Externe URL → direkt übernehmen
                        existing.AvatarUrl = profile.AvatarUrl;
                        _logger.LogInformation("Avatar aktualisiert (externe URL)");
                    }
                }
                else
                {
                    // Wenn Avatar leer ist, auf null setzen
                    existing.AvatarUrl = null;
                }
                
                existing.UpdatedAt = DateTime.UtcNow;

                _context.TribeUsers.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Profil aktualisiert für User {UserId}", profile.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Profils für User {UserId}", profile.Id);
                throw;
            }
        }

        private async Task EnsureCreatorProfileAsync(string userId, bool shouldExist)
        {
            var existingProfile = await _context.CreatorProfiles.FindAsync(userId);
            if (shouldExist)
            {
                if (existingProfile == null)
                {
                    _context.CreatorProfiles.Add(new CreatorProfile { Id = userId });
                }
            }
            else if (existingProfile != null)
            {
                _context.CreatorProfiles.Remove(existingProfile);
            }
        }
    }
}