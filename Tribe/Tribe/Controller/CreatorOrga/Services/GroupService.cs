using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IGroupService
    {
        Task<Group?> GetByIdAsync(string id);
        Task<IEnumerable<Group>> GetByOrganizationAsync(string organizationId);
        Task<Group> CreateAsync(Group group);
        Task<bool> UpdateAsync(string id, Group group);
        Task<bool> DeleteAsync(string id);
    }

    public class GroupService : IGroupService
    {
        private readonly OrgaDbContext _db;
        public GroupService(OrgaDbContext db) => _db = db;

        public async Task<Group?> GetByIdAsync(string id) => await _db.Groups.FindAsync(id);

        public async Task<IEnumerable<Group>> GetByOrganizationAsync(string organizationId) =>
            await _db.Groups.AsNoTracking().Where(g => g.OrganizationId == organizationId).ToListAsync();

        public async Task<Group> CreateAsync(Group group)
        {
            _db.Groups.Add(group);
            await _db.SaveChangesAsync();
            return group;
        }

        public async Task<bool> UpdateAsync(string id, Group group)
        {
            var existing = await _db.Groups.FindAsync(id);
            if (existing == null) return false;
            existing.Name = group.Name;
            existing.Description = group.Description;
            existing.LeaderId = group.LeaderId;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.Groups.FindAsync(id);
            if (existing == null) return false;
            _db.Groups.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
