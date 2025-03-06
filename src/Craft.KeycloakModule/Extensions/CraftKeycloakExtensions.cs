using System.ComponentModel;
using Craft.KeycloakModule.Enums;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule.Extensions;

public static class CraftKeycloakExtensions
{
    public static AuthorizationBuilder AddCraftKeycloakAuthorization(
        this IServiceCollection services,
        Action<KeycloakAuthorizationOptions>? configureKeycloakAuthorizationOptions =
            null
    )
    {
        var options = new KeycloakAuthorizationOptions();
        configureKeycloakAuthorizationOptions?.Invoke(options);

        return services
            .AddKeycloakAuthorization(configureKeycloakAuthorizationOptions)
            .AddAuthorization()
            .AddAuthorizationBuilder()
            .AddPolicy(
                nameof(KeycloakPolicyName.User),
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireResourceRolesForClient(
                        options.Resource,
                        [nameof(KeycloakRoles.User), nameof(KeycloakRoles.Admin)]
                    );
                }
            )
            .AddPolicy(
                nameof(KeycloakPolicyName.Admin),
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireResourceRolesForClient(
                        options.Resource,
                        [nameof(KeycloakRoles.Admin)]
                    );
                    policy.RequireResourceRolesForClient(
                        "realm-management",
                        [
                            "manage-events",
                            "manage-users",
                            "manage-identity-providers",
                        ]
                    );
                }
            );
    }

    public static IServiceCollection AddCraftKeycloakAuthentication(
        this IServiceCollection services,
        Action<KeycloakAuthenticationOptions> keycloakAuthenticationOptions,
        Action<JwtBearerOptions>? configureJwtBearerOptions = null
    )
    {
        services.AddKeycloakWebApiAuthentication(
            keycloakAuthenticationOptions,
            configureJwtBearerOptions
        );
        return services;
    }
}
