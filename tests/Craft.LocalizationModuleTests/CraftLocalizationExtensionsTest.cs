using Craft.LocalizationModule.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.LocalizationModuleTests;

public class CraftLocalizationExtensionsTest
{
    [Fact(
        DisplayName = "AddLocalization - Should add localization to the service collection"
    )]
    public void Test1()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        // Act
        services.AddCraftLocalization(options =>
        {
            options.SupportedCultureCodes = ["en", "de"];
            options.EndpointContextPath = "/api/custom_localizations";
            options.PolicyName = "manage_localizations";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var localConfig =
            serviceProvider.GetRequiredService<LocalizationConfiguration>();
        Assert.NotNull(localConfig);

        Assert.Equal(2, localConfig.SupportedCultureCodes.Count);
        Assert.Equal(
            "/api/custom_localizations",
            localConfig.EndpointContextPath
        );
        Assert.Equal("manage_localizations", localConfig.PolicyName);
    }
}
