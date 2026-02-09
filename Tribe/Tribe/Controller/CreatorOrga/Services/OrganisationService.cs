using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IOrganisationService
    {
        Task<Organization?> GetByIdAsync(string id);
        Task<IEnumerable<Organization>> GetByCreatorAsync(string creatorUserId);
        Task<Organization> CreateAsync(Organization organisation);
        Task<bool> UpdateAsync(string id, Organization organisation);
        Task<bool> DeleteAsync(string id);
    }

    public class OrganisationService : IOrganisationService
    {
        private readonly OrgaDbContext _db;

        public OrganisationService(OrgaDbContext db) => _db = db;

        public async Task<Organization?> GetByIdAsync(string id) => await _db.Organizations.FindAsync(id);

        public async Task<IEnumerable<Organization>> GetByCreatorAsync(string creatorUserId) =>
            await _db.Organizations.AsNoTracking().Where(o => o.CreatorUserId == creatorUserId).ToListAsync();

        public async Task<Organization> CreateAsync(Organization organisation)
        {
            _db.Organizations.Add(organisation);
            await _db.SaveChangesAsync();
            return organisation;
        }

        public async Task<bool> UpdateAsync(string id, Organization organisation)
        {
            var existing = await _db.Organizations.FindAsync(id);
            if (existing == null) return false;
            existing.Name = organisation.Name;
            existing.Description = organisation.Description;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.Organizations.FindAsync(id);
            if (existing == null) return false;
            _db.Organizations.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
