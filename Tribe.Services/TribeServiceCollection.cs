using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tribe.Client.Services;
using Tribe.Services.ClientServices;
using Tribe.Services.ClientServices.SimpleAuth;

namespace Tribe.Services
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddTribeServices(this IServiceCollection services, string baseAddress)
        {
            services.AddHttpClient("TribeAPI", client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            });
            services.AddScoped<IClientApiService, ClientApiService>();
            
            
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IAuthService, SimplifiedAuthService>();
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddScoped<ITokenInitializationService, TokenInitializationService>();

            return services;
        }
    }


}
