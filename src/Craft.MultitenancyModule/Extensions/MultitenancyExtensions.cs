using Craft.MultitenancyModule.Filters;
using Craft.MultitenancyModule.Interfaces;
using Craft.MultitenancyModule.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.MultitenancyModule.Extensions;

public sealed class MultitenancyOtions
{
    public string PolicyName { get; set; } = "HostOnly";
    public string DatabaseConnectionString { get; set; } = string.Empty;

    public bool EnableHost { get; set; } = true;
    public Type DbContextType  { get; set; }
}



public static class MultitenancyExtensions
{
    public static IServiceCollection AddMultitenancyModule(this IServiceCollection services, Action<MultitenancyOtions> configuration)
    {
        var multitenancyConfiguration = new MultitenancyOtions();
        configuration(multitenancyConfiguration);
        services.AddSingleton(multitenancyConfiguration);
        
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<TenantEndpointFilter>();
        services.AddScoped<IDynamicDbContextAccessor, DynamicDbContextAccessor>();
        return services;
    }
}