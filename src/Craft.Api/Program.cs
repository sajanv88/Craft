using Craft.Api.Extensions;
using Craft.Api.Infrastructure;
using Craft.CraftModule;
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;
using Craft.KeycloakModule;
using Craft.KeycloakModule.Extensions;
using Craft.KeycloakModule.Options;
using Craft.LocalizationModule;
using Craft.LocalizationModule.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var keycloakSettings = builder
    .Configuration.GetSection("KeycloakSettings")
    .Get<KeycloakSettings>();

builder.Services.AddOpenApiOauth(builder.Configuration);

builder.Services.AddCraftKeycloakAuthorization(options =>
{
    options.Realm = keycloakSettings.Realm;
    options.Resource = keycloakSettings.ClientId;
});

builder.Services.AddCraftKeycloakAuthentication(
    options =>
    {
        options.Realm = keycloakSettings.Realm;
        options.Audience = keycloakSettings.Audience;
        options.AuthServerUrl = keycloakSettings.BaseUrl;
    },
    options =>
    {
        options.MetadataAddress = keycloakSettings.MetadataAddress;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = keycloakSettings.Authority,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = c =>
            {
                Console.WriteLine(
                    $"🔴 Authentication failed: {c.Exception.Message}"
                );
                return Task.CompletedTask;
            },
            OnTokenValidated = c =>
            {
                Console.WriteLine("✅ Token successfully validated!");
                return Task.CompletedTask;
            },
        };
    }
);

builder.Services.AddCraftLocalization(options =>
{
    options.SupportedCultureCodes = ["en-US", "nl-NL", "ta-IN"];
    options.PolicyName = "api";
    options.EndpointContextPath = "/api/locales";
});

builder.Services.AddCraftModulesFromAssembly(typeof(Program).Assembly);

builder.Services.AddDbContext<ApiDbContext>(o =>
{
    var connectionString = builder.Configuration.GetConnectionString(
        "DefaultConnection"
    );
    o.UseNpgsql(connectionString);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "api",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "https://craft-ui.dev.sajankumarv.tech"
            );
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(config =>
    {
        config
            .WithPreferredScheme("OAuth2")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithOAuth2Authentication(o =>
            {
                o.Scopes = ["openid", "profile", "email", "offline_access"];
                o.ClientId = keycloakSettings.ClientId;
            });
    });
    app.MapOpenApi();
}

app.UseCraftGeneralException();

app.UseHttpsRedirection();

app.UseCors("api");

app.UseAuthentication();

app.UseAuthorization();

app.MapCraftModulesEndpoint();

app.Run();

[DependsOn(typeof(KeycloakModule))]
[DependsOn(typeof(LocalizationModule))]
public sealed class ApiModule : CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var app = builder.MapGroup("/api");
        app.MapGet("/", () => "Hello from ApiModule!");

        return builder;
    }
}
