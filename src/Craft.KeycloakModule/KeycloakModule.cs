using Craft.KeycloakModule.Enums;
using Craft.KeycloakModule.Options;
using Craft.KeycloakModule.Services;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule;

public class KeycloakModule : CraftModule.CraftModule
{
    public override void PostInitialization(IServiceCollection services)
    {
        services.AddScoped<KeycloakAdminService>();
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
        var keycloakSettings = configuration.GetSection("KeycloakSettings").Get<KeycloakSettings>();
        services.AddDistributedMemoryCache();
        services.AddClientCredentialsTokenManagement()
            .AddClient("adminClient", client =>
            {
                    client.ClientId = keycloakSettings.Admin?.ClientId;
                    client.ClientSecret = keycloakSettings.Admin?.ClientSecret;
                    client.TokenEndpoint = $"{keycloakSettings.Authority}/protocol/openid-connect/token";
            });
        services.AddKeycloakAdminHttpClient(options =>
        {
            options.AuthServerUrl = keycloakSettings.BaseUrl;
            options.Realm = keycloakSettings.Realm;
            options.Resource = keycloakSettings.Admin?.ClientId;
            options.VerifyTokenAudience = true;
            options.Credentials = new KeycloakClientInstallationCredentials
            {
                Secret = keycloakSettings.Admin.ClientSecret,
            };
        }).AddClientCredentialsTokenHandler("adminClient");
    }

    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var endpoint = builder.MapGroup("/api/keycloak/admin").RequireAuthorization(nameof(KeycloakRoles.Admin));
        endpoint.MapGet("/users", async (KeycloakAdminService admin) => Results.Ok(await admin.GetUsers("dev")));
        endpoint.MapGet("/", () => "Hello from KeycloakModule Admin!");
        return builder;
    }

    public string GetModuleName() => nameof(KeycloakModule);
}