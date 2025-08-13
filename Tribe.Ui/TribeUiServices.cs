using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tribe.Services;

namespace Tribe.Ui
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTribeUiServices(this IServiceCollection services, string baseAddress)
        {

            services.AddMudServices();
            services.AddTribeServices(baseAddress);
            return services;
        }
    }

}
