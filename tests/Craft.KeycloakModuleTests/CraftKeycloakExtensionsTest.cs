using Craft.KeycloakModule.Extensions;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModuleTests;

public sealed class CraftKeycloakExtensionsTest
{
    
    [Fact(DisplayName = "AddCraftKeycloakAuthorization: Should configure authorization.")]
    public void Test1()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var authorizationBuilder = services.AddCraftKeycloakAuthorization(ConfigureOptions);

        // Assert
        Assert.NotNull(authorizationBuilder);
        var serviceProvider = services.BuildServiceProvider();
        var authorizationService = serviceProvider.GetService<IAuthorizationService>();
        Assert.NotNull(authorizationService);
        return;

        void ConfigureOptions(KeycloakAuthorizationOptions options)
        {
            options.Resource = "test-resource";
        }
    }

    [Fact (DisplayName = "AddCraftKeycloakAuthentication: Should configure authentication.")]
    public void Test2()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCraftKeycloakAuthentication(ConfigureAuthOptions, ConfigureJwtOptions);
    
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var authenticationService = serviceProvider.GetService<IAuthenticationService>();
        Assert.NotNull(authenticationService);
        return;

        void ConfigureJwtOptions(JwtBearerOptions options)
        {
            options.Audience = "test-audience";
        }

        void ConfigureAuthOptions(KeycloakAuthenticationOptions options)
        {
            options.AuthServerUrl = "https://keycloak.example.com";
            options.Resource = "test-client-id";
        }
    }
}
