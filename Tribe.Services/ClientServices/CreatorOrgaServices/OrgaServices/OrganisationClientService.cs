using System.Collections.Generic;
using System.Threading.Tasks;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Client.Services;

namespace Tribe.Services.ClientServices.CreatorOrgaServices.OrgaServices
{
    public class OrganisationClientService : IOrganisationClientService
    {
        private readonly IApiService _api;
        public OrganisationClientService(IApiService api) => _api = api;

        public Task<Organization?> GetByIdAsync(string id) => _api.GetAsync<Organization>($"api/organisation/{id}");
        public Task<IEnumerable<Organization>?> GetByCreatorAsync(string creatorId) => _api.GetAsync<IEnumerable<Organization>>($"api/organisation/creator/{creatorId}");
        public Task<Organization?> CreateAsync(Organization org) => _api.PostAsync<Organization, Organization>("api/organisation", org);
        public Task<bool> UpdateAsync(string id, Organization org) => _api.PutAsync($"api/organisation/{id}", org);
        public Task<bool> DeleteAsync(string id) => _api.DeleteAsync($"api/organisation/{id}");
    }
}
