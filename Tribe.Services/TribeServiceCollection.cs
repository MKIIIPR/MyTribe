using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

          

            return services;
        }
    }


}
