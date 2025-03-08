using Craft.KeycloakModule.Enums;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule.Extensions;

/// <summary>
/// Provides extension methods for configuring Keycloak-based authentication and authorization in an ASP.NET Core application.
/// </summary>
public static class CraftKeycloakExtensions
{
    /// <summary>
    /// Adds Keycloak-based authorization policies to the application's authorization system.
    /// </summary>
    /// <param name="services">The <see cref="configureKeycloakAuthorizationOptions"/> to add the authorization policies to.</param>
    /// <param name="configureKeycloakAuthorizationOptions">
    /// An optional delegate to configure the <see cref="KeycloakAuthorizationOptions"/> for Keycloak authorization.
    /// </param>
    /// <returns>
    /// An <see cref="AuthorizationBuilder"/> that can be used to further configure authorization policies.
    /// </returns>
    /// <remarks>
    /// This method configures two default policies:
    /// <list type="bullet">
    /// <item>
    /// <term>User Policy</term>
    /// <description>
    /// Requires the user to be authenticated and have either the "User" or "Admin" role for the specified resource.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Admin Policy</term>
    /// <description>
    /// Requires the user to be authenticated, have the "Admin" role for the specified resource, and have specific realm-management roles.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
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
                nameof(KeycloakPolicyName.UserPolicy),
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireResourceRolesForClient(
                        options.Resource,
                        [
                            nameof(KeycloakRoles.User),
                            nameof(KeycloakRoles.Admin),
                        ]
                    );
                }
            )
            .AddPolicy(
                nameof(KeycloakPolicyName.AdminPolicy),
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

    /// <summary>
    /// Adds Keycloak-based authentication to the application's authentication system.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the authentication services to.</param>
    /// <param name="keycloakAuthenticationOptions">
    /// A delegate to configure the <see cref="configureJwtBearerOptions"/> for Keycloak authentication.
    /// </param>
    /// <param name="configureJwtBearerOptions">
    /// An optional delegate to configure the <see cref="KeycloakAuthenticationOptions"/> for JWT Bearer authentication.
    /// </param>
    /// <returns>
    /// The <see cref="JwtBearerOptions"/> for chaining additional configuration.
    /// </returns>
    /// <remarks>
    /// This method configures Keycloak authentication using the provided options and integrates it with the ASP.NET Core authentication system.
    /// </remarks>
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
