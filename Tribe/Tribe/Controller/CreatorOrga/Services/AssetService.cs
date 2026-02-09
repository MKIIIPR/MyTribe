using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IAssetService
    {
        Task<OrganizationAsset?> GetOrganizationAssetByIdAsync(string id);
        Task<IEnumerable<OrganizationAsset>> GetOrganizationAssetsAsync(string organizationId);
        Task<OrganizationAsset> CreateOrganizationAssetAsync(OrganizationAsset asset);
        Task<bool> UpdateOrganizationAssetAsync(string id, OrganizationAsset asset);
        Task<bool> DeleteOrganizationAssetAsync(string id);

        Task<MemberAsset?> GetMemberAssetByIdAsync(string id);
        Task<IEnumerable<MemberAsset>> GetMemberAssetsAsync(string memberId);
        Task<MemberAsset> CreateMemberAssetAsync(MemberAsset asset);
        Task<bool> UpdateMemberAssetAsync(string id, MemberAsset asset);
        Task<bool> DeleteMemberAssetAsync(string id);
    }

    public class AssetService : IAssetService
    {
        private readonly OrgaDbContext _db;
        public AssetService(OrgaDbContext db) => _db = db;

        public async Task<OrganizationAsset?> GetOrganizationAssetByIdAsync(string id) => await _db.OrganizationAssets.FindAsync(id);

        public async Task<IEnumerable<OrganizationAsset>> GetOrganizationAssetsAsync(string organizationId) =>
            await _db.OrganizationAssets.AsNoTracking().Where(a => a.OrganizationId == organizationId).ToListAsync();

        public async Task<OrganizationAsset> CreateOrganizationAssetAsync(OrganizationAsset asset)
        {
            _db.OrganizationAssets.Add(asset);
            await _db.SaveChangesAsync();
            return asset;
        }

        public async Task<bool> UpdateOrganizationAssetAsync(string id, OrganizationAsset asset)
        {
            var existing = await _db.OrganizationAssets.FindAsync(id);
            if (existing == null) return false;
            existing.Name = asset.Name;
            existing.Description = asset.Description;
            existing.SpecificationsJson = asset.SpecificationsJson;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrganizationAssetAsync(string id)
        {
            var existing = await _db.OrganizationAssets.FindAsync(id);
            if (existing == null) return false;
            _db.OrganizationAssets.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<MemberAsset?> GetMemberAssetByIdAsync(string id) => await _db.MemberAssets.FindAsync(id);

        public async Task<IEnumerable<MemberAsset>> GetMemberAssetsAsync(string memberId) =>
            await _db.MemberAssets.AsNoTracking().Where(a => a.MemberId == memberId).ToListAsync();

        public async Task<MemberAsset> CreateMemberAssetAsync(MemberAsset asset)
        {
            _db.MemberAssets.Add(asset);
            await _db.SaveChangesAsync();
            return asset;
        }

        public async Task<bool> UpdateMemberAssetAsync(string id, MemberAsset asset)
        {
            var existing = await _db.MemberAssets.FindAsync(id);
            if (existing == null) return false;
            existing.Name = asset.Name;
            existing.Description = asset.Description;
            existing.AttributesJson = asset.AttributesJson;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMemberAssetAsync(string id)
        {
            var existing = await _db.MemberAssets.FindAsync(id);
            if (existing == null) return false;
            _db.MemberAssets.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
