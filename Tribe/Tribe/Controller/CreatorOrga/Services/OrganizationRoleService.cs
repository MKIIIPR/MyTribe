using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IOrganizationRoleService
    {
        Task<OrganizationRole?> GetByIdAsync(string id);
        Task<IEnumerable<OrganizationRole>> GetByOrganizationAsync(string organizationId);
        Task<OrganizationRole> CreateAsync(OrganizationRole role);
        Task<bool> UpdateAsync(string id, OrganizationRole role);
        Task<bool> DeleteAsync(string id);
    }

    public class OrganizationRoleService : IOrganizationRoleService
    {
        private readonly OrgaDbContext _db;
        public OrganizationRoleService(OrgaDbContext db) => _db = db;

        public async Task<OrganizationRole?> GetByIdAsync(string id) => await _db.OrganizationRoles.FindAsync(id);

        public async Task<IEnumerable<OrganizationRole>> GetByOrganizationAsync(string organizationId) =>
            await _db.OrganizationRoles.AsNoTracking().Where(r => r.OrganizationId == organizationId).ToListAsync();

        public async Task<OrganizationRole> CreateAsync(OrganizationRole role)
        {
            _db.OrganizationRoles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task<bool> UpdateAsync(string id, OrganizationRole role)
        {
            var existing = await _db.OrganizationRoles.FindAsync(id);
            if (existing == null) return false;
            existing.Priority = role.Priority;
            existing.PermissionsJson = role.PermissionsJson;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.OrganizationRoles.FindAsync(id);
            if (existing == null) return false;
            _db.OrganizationRoles.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
