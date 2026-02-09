using Microsoft.Extensions.DependencyInjection;
using Tribe.Controller.CreatorOrga.Services;

namespace Tribe.SummeryServices
{
    public static class OrganisationServiceCollection
    {
        public static IServiceCollection AddOrganisationServices(this IServiceCollection services)
        {
            // Register implemented services (interface -> implementation)
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IRecruitmentService, RecruitmentService>();
            services.AddScoped<IOrganisationService, OrganisationService>();
            services.AddScoped<IOrganizationMemberService, OrganizationMemberService>();
            services.AddScoped<IOrganizationRoleService, OrganizationRoleService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGameProfileService, GameProfileService>();

            return services;
        }
    }
}
