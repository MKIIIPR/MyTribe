using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IGameProfileService
    {
        Task<GameProfile?> GetByIdAsync(string id);
        Task<IEnumerable<GameProfile>> GetAllAsync();
        Task<GameProfile> CreateAsync(GameProfile profile);
        Task<bool> UpdateAsync(string id, GameProfile profile);
        Task<bool> DeleteAsync(string id);
    }

    public class GameProfileService : IGameProfileService
    {
        private readonly OrgaDbContext _db;
        public GameProfileService(OrgaDbContext db) => _db = db;

        public async Task<GameProfile?> GetByIdAsync(string id) => await _db.GameProfiles.FindAsync(id);

        public async Task<IEnumerable<GameProfile>> GetAllAsync() =>
            await _db.GameProfiles.AsNoTracking().ToListAsync();

        public async Task<GameProfile> CreateAsync(GameProfile profile)
        {
            _db.GameProfiles.Add(profile);
            await _db.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> UpdateAsync(string id, GameProfile profile)
        {
            var existing = await _db.GameProfiles.FindAsync(id);
            if (existing == null) return false;
            existing.DisplayName = profile.DisplayName;
            existing.Genre = profile.Genre;
            existing.Platform = profile.Platform;
            existing.Description = profile.Description;
            existing.MetadataJson = profile.MetadataJson;
            existing.DefaultRoleTemplateJson = profile.DefaultRoleTemplateJson;
            existing.DefaultSettingsJson = profile.DefaultSettingsJson;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.GameProfiles.FindAsync(id);
            if (existing == null) return false;
            _db.GameProfiles.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
