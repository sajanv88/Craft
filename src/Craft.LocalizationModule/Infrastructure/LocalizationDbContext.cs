using Craft.CraftModule.Infrastructure;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Domain.Interfaces;
using Craft.LocalizationModule.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Infrastructure;

public class LocalizationDbContext(DbContextOptions<LocalizationDbContext> options)
    : CraftDbContext<LocalizationDbContext>(options), ILocalizationDbContext
{
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureLocalization();
    }

    public DbSet<LocalizationEntity> Localizations { get; set; }
}

