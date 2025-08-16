using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Tribe.Client.Services;
using Tribe.Services.ClientServices;
using Tribe.Services.ClientServices.SimpleAuth;
using Tribe.Services.States;
using Tribe.Ui;
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// HTTP Client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Authentication - CascadingAuthenticationState wird im Layout verwendet
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTribeUiServices(builder.HostEnvironment.BaseAddress);
builder.Services.AddSingleton<UserState>();
builder.Services.AddScoped<IAuthService, SimplifiedAuthService>();
builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddScoped<ITokenInitializationService, TokenInitializationService>();

var app = builder.Build();

// Initialize JWT token from server cookie after login
var tokenInitService = app.Services.GetRequiredService<ITokenInitializationService>();
await tokenInitService.InitializeTokenFromCookieAsync();

await app.RunAsync();