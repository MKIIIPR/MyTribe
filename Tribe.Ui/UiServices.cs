

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

namespace FrontUI
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddUiServices(this IServiceCollection services, string baseAddress)
        {
            // 1. HttpClient für TribeAPI konfigurieren
            services.AddHttpClient("TribeAPI", client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            });

            // 2. Weitere UI-Services (z. B. MudBlazor)
            services.AddMudServices();

            return services;
        }
    }

}
