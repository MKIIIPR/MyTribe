using System.Collections.Generic;
using System.Threading.Tasks;
using Tribe.Bib.Models.CommunityManagement;

namespace Tribe.Services.ClientServices.CreatorOrgaServices.OrgaServices
{
    public interface IOrganisationClientService
    {
        Task<Organization?> GetByIdAsync(string id);
        Task<IEnumerable<Organization>?> GetByCreatorAsync(string creatorId);
        Task<Organization?> CreateAsync(Organization org);
        Task<bool> UpdateAsync(string id, Organization org);
        Task<bool> DeleteAsync(string id);
    }
}
