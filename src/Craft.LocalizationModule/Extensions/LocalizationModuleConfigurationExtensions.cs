using Craft.LocalizationModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.LocalizationModule.Extensions;

public static class LocalizationModuleConfigurationExtensions
{
    
    public static void ConfigureLocalization(this ModelBuilder builder)
    {
        builder.Entity<LocalizationEntity>().ToTable("locales");
        
        builder.Entity<LocalizationEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired();
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.CultureCode).IsRequired();
        });
    }
}
