
using Microsoft.Extensions.DependencyInjection;

namespace Craft.LocalizationModule.Extensions;

public sealed class LocalizationConfiguration
{
    public List<string> Cultures { get; set; } = [];
    public string PolicyName { get; set; } = string.Empty;

    public string EndpointContextPath { get; set; } = "/api/locales";

}
public static class LocalizationExtensions
{
    public static IServiceCollection AddLocalization(this IServiceCollection services,
        Action<LocalizationConfiguration> configure)
    {
        
        var configuration = new LocalizationConfiguration();
        configure(configuration);
        services.AddSingleton(configuration);
        return services;
    }
}
