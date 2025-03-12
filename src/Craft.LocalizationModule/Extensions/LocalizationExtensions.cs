using Craft.CraftModule.Extensions;
using Craft.LocalizationModule.Domain.Interfaces;
using Craft.LocalizationModule.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.LocalizationModule.Extensions;

public sealed class LocalizationConfiguration
{
    public List<string> Cultures { get; set; } = [];
    
}
public static class LocalizationExtensions
{
    public static IServiceCollection AddLocalization(this IServiceCollection services,
        Action<LocalizationConfiguration> configure)
    {
        
        var configuration = new LocalizationConfiguration();
        configure(configuration);
        services.AddSingleton(configuration);
        services.AddCraftModules([typeof(LocalizationModule)]);
        return services;
    }
}
