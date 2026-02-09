using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IOrganizationMemberService
    {
        Task<OrganizationMember?> GetByIdAsync(string id);
        Task<IEnumerable<OrganizationMember>> GetByOrganizationAsync(string organizationId);
        Task<OrganizationMember> CreateAsync(OrganizationMember member);
        Task<bool> UpdateAsync(string id, OrganizationMember member);
        Task<bool> RemoveAsync(string id);
    }

    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly OrgaDbContext _db;
        public OrganizationMemberService(OrgaDbContext db) => _db = db;

        public async Task<OrganizationMember?> GetByIdAsync(string id) => await _db.OrganizationMembers.FindAsync(id);

        public async Task<IEnumerable<OrganizationMember>> GetByOrganizationAsync(string organizationId) =>
            await _db.OrganizationMembers.AsNoTracking().Where(m => m.OrganizationId == organizationId).ToListAsync();

        public async Task<OrganizationMember> CreateAsync(OrganizationMember member)
        {
            _db.OrganizationMembers.Add(member);
            await _db.SaveChangesAsync();
            return member;
        }

        public async Task<bool> UpdateAsync(string id, OrganizationMember member)
        {
            var existing = await _db.OrganizationMembers.FindAsync(id);
            if (existing == null) return false;
            existing.RoleId = member.RoleId;
            // keep compatibility if OrganizationMember doesn't define IsActive, check property existence at runtime not required here
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var existing = await _db.OrganizationMembers.FindAsync(id);
            if (existing == null) return false;
            _db.OrganizationMembers.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
