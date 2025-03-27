using Craft.CraftModule.Infrastructure;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Extensions;
using Craft.LocalizationModule.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Infrastructure;

/// <summary>
///     DbContext for the Localization Module
/// </summary>
/// <param name="options"></param>
public class LocalizationDbContext(
    DbContextOptions<LocalizationDbContext> options
) : CraftDbContext<LocalizationDbContext>(options), ILocalizationDbContext
{
    /// <summary>
    ///     Model configuration for the Localization Module
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureLocalization();
    }

    /// <summary>
    ///     DbSet for the LocalizationEntity
    /// </summary>
    public DbSet<LocalizationEntity> Localizations { get; set; }
}
