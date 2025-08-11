using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using MyTribe.Client.Pages;
using MyTribe.Client.Services;
using MyTribe.Components;
using MyTribe.Components.Account;
using MyTribe.Data;
using MyTribe.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7006/")
});

// JWT Authentication Service registrieren
builder.Services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
// ENTFERNT: .AddAuthenticationStateSerialization() - nicht nötig für JWT

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

// JWT Configuration aus appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtSecret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = jwtSettings["Issuer"] ?? "MyTribe";
var jwtAudience = jwtSettings["Audience"] ?? "MyTribe-Client";

// Haupt-Authentication: JWT Bearer für APIs
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.FromMinutes(5) // Erlaubt 5 Min Zeitunterschied
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Token aus Cookie lesen (für Blazor Server Pages)
            if (string.IsNullOrEmpty(context.Token))
            {
                context.Token = context.Request.Cookies["jwt"];
            }

            // Token aus Authorization Header (für WASM API calls)
            var authorization = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            {
                context.Token = authorization.Substring("Bearer ".Length).Trim();
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
})
// Zusätzlich: Identity Cookies für Legacy-Support (falls nötig)
.AddIdentityCookies(options =>
{
    options.ApplicationCookie.Configure(cookieOptions =>
    {
        cookieOptions.Cookie.SameSite = SameSiteMode.Strict;
        cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        cookieOptions.Cookie.HttpOnly = true;
        cookieOptions.ExpireTimeSpan = TimeSpan.FromHours(2);
        cookieOptions.SlidingExpiration = true;
        cookieOptions.LoginPath = "/Account/Login";
        cookieOptions.LogoutPath = "/Account/Logout";
    });
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Standard Policy für alle authentifizierten Benutzer
    options.AddPolicy("RequireAuthentication", policy =>
        policy.RequireAuthenticatedUser());

    // Admin Policy (falls Sie später Rollen hinzufügen)
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireAuthenticatedUser()
               .RequireRole("Admin"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity Configuration
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Email Confirmation für Produktion
    options.SignIn.RequireConfirmedAccount = false; // Für Demo auf false
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Für Demo auf false

    // Password Requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// API Endpoints
builder.Services.AddControllers(); // Für API Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyTribe API",
        Version = "v1",
        Description = "JWT-basierte API für MyTribe Blazor App"
    });

    // JWT Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
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

// CORS für WebAssembly (wenn nötig)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebAssembly", policy =>
    {
        policy.WithOrigins("https://localhost:7006") // Ihre WebAssembly URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Security Headers
builder.Services.AddAntiforgery();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyTribe API V1");
        c.RoutePrefix = "swagger"; // Swagger UI unter /swagger
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();

    // Production Security Headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; script-src 'self' 'unsafe-eval'; style-src 'self' 'unsafe-inline'");
        await next();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowWebAssembly");

// Authentication/Authorization Pipeline
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Static Files
app.MapStaticAssets();

// Blazor Components
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MyTribe.Client._Imports).Assembly);

// API Controllers
app.MapControllers();

// Identity Endpoints (falls noch gebraucht)
app.MapAdditionalIdentityEndpoints();

app.Run();