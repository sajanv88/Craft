
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule;

public class KeycloakModule : CraftModule.CraftModule
{
    public override void PreInitialization(IServiceCollection services)
    {
    }

    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var endpoint = builder.MapGroup("/api/keycloak");
        endpoint.MapGet("/", () => "Hello from KeycloakModule!").RequireAuthorization();
        return builder;
    }

    public string GetModuleName() => nameof(KeycloakModule);
}
