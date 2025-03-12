using Craft.LocalizationModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Domain.Interfaces;

public interface ILocalizationDbContext
{
    DbSet<LocalizationEntity> Localizations { get; set; }
}
