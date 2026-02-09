using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using Tribe.Middleware;

using System.Text;
using Tribe.Bib.Models.TribeRelated;
using Tribe.Client.Pages;
using Tribe.Client.Services;
using Tribe.Components;
using Tribe.Components.Account;
using Tribe.Controller.Services;
using Tribe.Data;
using Tribe.Services;
using Tribe.Services.ClientServices;
using Tribe.Services.ClientServices.ShopServices;
using Tribe.Services.ClientServices.SimpleAuth;
using Tribe.Services.Hubs;
using Tribe.Services.ServerServices;
using Tribe.Services.States;
using Tribe.Data.Seeds;

using TribeApp.Repositories;
// using Tribe.SummeryServices; (removed)

var builder = WebApplication.CreateBuilder(args);

#region Database Configuration
builder.Services.AddDbContext<ShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShopDbConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3);
        sqlOptions.CommandTimeout(30);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3);
        sqlOptions.CommandTimeout(30);
    }));
var orgaConnectionString = builder.Configuration.GetConnectionString("OrgaDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'OrgaDbContextConnection' not found.");
builder.Services.AddDbContext<OrgaDbContext>(options =>
    options.UseSqlServer(orgaConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3);
        sqlOptions.CommandTimeout(30);
    }));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
#endregion

#region Client Services (für WebAssembly)
// Diese Services werden auch auf dem Server registriert für SSR/Prerendering
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IClientApiService, ClientApiService>();
builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddScoped<ITokenInitializationService, TokenInitializationService>();
builder.Services.AddScoped<UserState>();

// Auth Service - NUR SimplifiedAuthService verwenden (einheitlicher Flow)
builder.Services.AddScoped<IAuthService, SimplifiedAuthService>();

builder.Services.AddHttpClient();
// Client side shop services for prerendering and server-side components
builder.Services.AddScoped<IProductClientService, Tribe.Services.ClientServices.ShopServices.ProductClientService>();
builder.Services.AddScoped<ICategoryClientService, Tribe.Services.ClientServices.ShopServices.CategoryClientService>();
builder.Services.AddScoped<IOrderClientService, Tribe.Services.ClientServices.ShopServices.OrderClientService>();
builder.Services.AddScoped<Tribe.Services.ClientServices.ShopServices.IRaffleClientService, Tribe.Services.ClientServices.ShopServices.RaffleClientService>();
builder.Services.AddScoped<ICreatorProfileClientService, Tribe.Services.ClientServices.ShopServices.CreatorProfileClientService>();
// Unified shop creator facade for server-side components
builder.Services.AddScoped<IShopCreatorService, Tribe.Services.ClientServices.ShopServices.ShopCreatorService>();
builder.Services.AddSingleton<Tribe.Services.ClientServices.ShopServices.ShopService>();
#endregion

#region Server Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDownloadService, DownloadService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRaffleService, RaffleService>();
builder.Services.AddScoped<IOwnProfileService, OwnProfileService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthNotificationService, AuthNotificationService>();
#endregion

#region MudBlazor & Controllers
builder.Services.AddMudServices();
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
#endregion

#region Blazor Configuration
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
#endregion

#region Authentication & Authorization
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "TribeApp",
            ValidAudience = jwtSettings["Audience"] ?? "TribeApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        // SignalR JWT Token aus Query-String oder Cookie lesen
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/authHub"))
                {
                    context.Token = accessToken;
                }

                // JWT aus Cookie lesen falls kein Authorization-Header vorhanden
                if (string.IsNullOrEmpty(context.Token))
                {
                    context.Token = context.Request.Cookies["jwt_token"];
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
#endregion

#region Identity
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
#endregion

#region SignalR
builder.Services.AddSignalR();
#region Organization Authorization
// Organization authorization handler removed
#endregion
#endregion

#region Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tribe API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tribe API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

// Add rate limiting middleware for Shop API
app.UseMiddleware<RateLimitMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Tribe.Client._Imports).Assembly)
    .AddAdditionalAssemblies(typeof(Tribe.Ui._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();
app.MapControllers();
app.MapHub<AuthHub>("/authHub");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrgaDbContext>();
    await GameProfileSeed.EnsureSeedAsync(dbContext);
}

app.Run();
