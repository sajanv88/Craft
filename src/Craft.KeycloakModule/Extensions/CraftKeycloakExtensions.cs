using Craft.KeycloakModule.Options;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule.Extensions;

public static class CraftKeycloakExtensions
{
    public static IServiceCollection AddCraftKeycloakAuthentication(this IServiceCollection services, Action<KeycloakSettings> settings)
    {
        var keycloakSettings = new KeycloakSettings();
        settings.Invoke(keycloakSettings);
        services.AddKeycloakWebApiAuthentication(options =>
        {
            options.Realm = keycloakSettings.Realm;
            options.Audience = keycloakSettings.Audience;
            options.AuthServerUrl = keycloakSettings.Authority;
            options.VerifyTokenAudience = keycloakSettings.VerifyTokenAudience;
        });

        services.AddKeycloakAuthorization(options =>
        {
            options.Resource = keycloakSettings.ClientId;
            options.AuthServerUrl = keycloakSettings.Authority;
            options.Realm = keycloakSettings.Realm;
        });

        services.AddAuthorizationBuilder();
        
        return services;
    }
}
