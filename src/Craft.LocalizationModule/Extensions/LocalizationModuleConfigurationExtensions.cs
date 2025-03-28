using Craft.CraftModule.Constants;
using Craft.LocalizationModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Extensions;

/// <summary>
///     Configuration for the Localization Module database model
/// </summary>
public static class LocalizationModuleConfigurationExtensions
{
    /// <summary>
    ///     Extension method to configure the Localization Module database model
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureLocalization(this ModelBuilder builder)
    {
        const string prefix = CraftModuleConstants.PrefixAsCraft + "_Locales";

        builder.Entity<LocalizationEntity>().ToTable(prefix);

        builder.Entity<LocalizationEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired();
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.CultureCode).IsRequired();
        });
    }
}
