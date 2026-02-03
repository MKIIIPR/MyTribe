using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Tribe.Client.Services;
using Tribe.Services.ClientServices.SimpleAuth;

namespace Tribe.Services.ClientServices
{
    // Client/Services/ITokenInitializationService.cs
   
    public interface ITokenInitializationService
    {
        Task InitializeTokenFromCookieAsync();
    }

// Client/Services/TokenInitializationService.cs

    public class TokenInitializationService : ITokenInitializationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IAuthService _authService;
        private readonly IApiService _apiService;

        public TokenInitializationService(IJSRuntime jsRuntime, IAuthService authService, IApiService apiService)
        {
            _jsRuntime = jsRuntime;
            _authService = authService;
            _apiService = apiService;
        }

        public async Task InitializeTokenFromCookieAsync()
        {
            try
            {
                // Get JWT token from cookie set by server login
                var token = await _jsRuntime.InvokeAsync<string>("eval", "document.cookie.split('; ').find(row => row.startsWith('jwt_token='))?.split('=')[1]");

                if (!string.IsNullOrEmpty(token))
                {
                    // Store token in localStorage for client use
                    //await _authService.StoreTokenAsync(token);
                    _apiService.SetAuthToken(token);
                }
            }
            catch (Exception)
            {
                // Handle any errors silently
            }
        }
    }
}
