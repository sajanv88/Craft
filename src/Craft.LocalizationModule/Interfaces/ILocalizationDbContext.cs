using Craft.LocalizationModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Interfaces;

/// <summary>
///     Interface for the Localization DbContext
/// </summary>
public interface ILocalizationDbContext
{
    /// <summary>
    ///     DbSet for the LocalizationEntity
    /// </summary>
    DbSet<LocalizationEntity> Localizations { get; set; }
}
