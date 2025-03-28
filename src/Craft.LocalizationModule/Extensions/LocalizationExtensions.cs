using Microsoft.Extensions.DependencyInjection;

namespace Craft.LocalizationModule.Extensions;

/// <summary>
///  Configuration for localization
/// </summary>
public sealed class LocalizationConfiguration
{
    /// <summary>
    ///     Configure the supported culture codes. If configured, it will return only the supported culture codes
    /// </summary>
    public List<string> SupportedCultureCodes { get; set; } = [];

    /// <summary>
    ///    Provide the policy name for the localization endpoint to protect the endpoint
    /// </summary>
    public string PolicyName { get; set; } = string.Empty;

    /// <summary>
    ///    Define the context path for the localization endpoint. By default, it is /api/locales
    /// </summary>
    public string EndpointContextPath { get; set; } = "/api/locales";
}

/// <summary>
///     Localization extensions for the service collection
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    ///    Invoke the configuration for localization
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddCraftLocalization(
        this IServiceCollection services,
        Action<LocalizationConfiguration> configure
    )
    {
        var configuration = new LocalizationConfiguration();
        configure(configuration);
        services.AddSingleton(configuration);
        return services;
    }
}
