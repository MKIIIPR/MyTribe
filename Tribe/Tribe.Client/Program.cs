using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Tribe.Client.Services;
using Tribe.Services.ClientServices;
using Tribe.Services.ClientServices.SimpleAuth;
using Tribe.Services.States;
using Tribe.Ui;
using Tribe.Services.ClientServices.ShopServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region MudBlazor
builder.Services.AddMudServices();
#endregion

#region HTTP Client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#endregion

#region Authentication
builder.Services.AddAuthorizationCore();
// Einheitlicher AuthenticationStateProvider - nur Cookie-basiert
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, SimplifiedAuthService>();
#endregion

#region API Services
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IClientApiService, ClientApiService>();
// Product client service for interacting with server-side Products API
builder.Services.AddScoped<IProductClientService, ProductClientService>();
// Category and Order client services
builder.Services.AddScoped<ICategoryClientService, CategoryClientService>();
builder.Services.AddScoped<IOrderClientService, OrderClientService>();
// Unified shop creator facade
builder.Services.AddScoped<IShopCreatorService, Tribe.Services.ClientServices.ShopServices.ShopCreatorService>();
builder.Services.AddScoped<IRaffleClientService, RaffleClientService>();
builder.Services.AddScoped<ICreatorProfileClientService, CreatorProfileClientService>();
// Shop UI service for cart + local state
builder.Services.AddSingleton<ShopService>();
#endregion

#region SignalR & State
builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddScoped<ITokenInitializationService, TokenInitializationService>();
builder.Services.AddSingleton<UserState>();
#endregion

#region Tribe UI Services
builder.Services.AddTribeUiServices(builder.HostEnvironment.BaseAddress);
#endregion

var app = builder.Build();

// Token-Initialisierung vom Server-Cookie
var tokenInitService = app.Services.GetRequiredService<ITokenInitializationService>();
await tokenInitService.InitializeTokenFromCookieAsync();

await app.RunAsync();