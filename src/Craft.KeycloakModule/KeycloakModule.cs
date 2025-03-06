using System.ComponentModel;
using System.Security.Claims;
using Craft.CraftModule.Dtos;
using Craft.KeycloakModule.Dtos;
using Craft.KeycloakModule.Enums;
using Craft.KeycloakModule.Options;
using Craft.KeycloakModule.Services;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule;

public sealed class KeycloakModule : CraftModule.CraftModule
{
    private readonly IConfiguration _configuration;
    public KeycloakModule() {}
    public KeycloakModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public override void PostInitialization(IServiceCollection services)
    {
        services.AddScoped<KeycloakAdminService>();
        
        
        var keycloakSettings = _configuration
            .GetSection("KeycloakSettings")
            .Get<KeycloakSettings>();

        services.AddDistributedMemoryCache();
        services
            .AddClientCredentialsTokenManagement()
            .AddClient(
                "adminClient",
                client =>
                {
                    client.ClientId = keycloakSettings.Admin?.ClientId;
                    client.ClientSecret = keycloakSettings.Admin?.ClientSecret;
                    client.TokenEndpoint =
                        $"{keycloakSettings.Authority}/protocol/openid-connect/token";
                }
            );

        services
            .AddKeycloakAdminHttpClient(options =>
            {
                options.AuthServerUrl = keycloakSettings.BaseUrl;
                options.Realm = keycloakSettings.Realm;
                options.Resource = keycloakSettings.Admin?.ClientId;
                options.VerifyTokenAudience = true;
                options.Credentials = new KeycloakClientInstallationCredentials
                {
                    Secret = keycloakSettings.Admin.ClientSecret,
                };
            })
            .AddClientCredentialsTokenHandler("adminClient");
    }

    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var adminEndpoint = builder
            .MapGroup("/api/keycloak/admin")
            .RequireAuthorization(nameof(KeycloakRoles.Admin));
        adminEndpoint.MapGet(
            "/users",
            async (int? page, int? limit, [Description("Search for a string contained in Username, FirstName, LastName or Email.")] string? search, KeycloakAdminService admin, CancellationToken token) =>
                Results.Ok(await admin.GetUsers(page, limit, search, token))
        ).Produces<PaginatedResponse<UserRepresentation>>()
        .WithDescription("Get all users in the realm with pagination support.");
        
        
        var userEndpoint = builder
            .MapGroup("/api/keycloak/profile")
            .RequireAuthorization(nameof(KeycloakRoles.User));

        userEndpoint.MapGet("/me",
                (ClaimsPrincipal claimsPrincipal) =>
                {
                    var claims = claimsPrincipal.Claims.ToList();
                    var profile = new Profile
                    {
                        PreferredUsername = claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
                        FullName = claims.FirstOrDefault(c => c.Type == "name")?.Value,
                        Sub = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                        Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                        IsEmailVerified =  claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true",
                    };
                    return Results.Ok(profile);
                }).Produces<Profile>()
            .WithDescription("Get the current user's profile information from the token.");
        
        return builder;
    }
}
