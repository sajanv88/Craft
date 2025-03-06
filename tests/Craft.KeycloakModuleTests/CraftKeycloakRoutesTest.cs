
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;

using Microsoft.AspNetCore.Builder;


using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Craft.KeycloakModuleTests;


public sealed class CraftKeycloakRoutesTest
{

    [DependsOn(typeof(KeycloakModule.KeycloakModule))]
    private sealed class TestModule: Craft.CraftModule.CraftModule {
        public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
        {
         
            return builder;
        }
    }

  

    [Fact(DisplayName = "Verify Keycloak module default endpoints are registered.")]
    public async Task Test2()
    {
        // Arrange
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "KeycloakSettings:ClientId", "test-client" },
            { "KeycloakSettings:ClientSecret", "test-secret" },
            { "KeycloakSettings:Authority", "https://keycloak.example.com" },
            { "KeycloakSettings:Realm", "test-realm" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

     
  
        var web = WebApplication.CreateBuilder();
        var services = web.Services;
        services.AddSingleton<IConfiguration>(configuration);
        services.AddEndpointsApiExplorer();
        services.AddCraftModules([typeof(TestModule)]);
        services.AddLogging();
        
        var host = web.Build();
        host.MapCraftModulesEndpoint();

        await host.StartAsync();
        
        // Act
        var endpointDataSource = services.BuildServiceProvider().GetRequiredService<EndpointDataSource>();
     
        Assert.NotNull(endpointDataSource);
        var routes = endpointDataSource.Endpoints
            .Select(e => e.DisplayName)
            .ToList();
        
        // Assert
        Assert.Contains("HTTP: GET /api/keycloak/admin/users", routes);
        Assert.Contains("HTTP: GET /api/keycloak/profile/me", routes);

        var keycloakModule = host.Services.GetRequiredService<KeycloakModule.KeycloakModule>();
        Assert.NotNull(keycloakModule);
        
        await host.StopAsync();
    }
}