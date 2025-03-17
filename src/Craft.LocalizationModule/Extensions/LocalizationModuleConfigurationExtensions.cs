using Craft.CraftModule.Constants;
using Craft.LocalizationModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Extensions;

public static class LocalizationModuleConfigurationExtensions
{
    
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
