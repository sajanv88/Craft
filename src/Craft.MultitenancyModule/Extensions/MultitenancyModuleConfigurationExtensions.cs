using Craft.CraftModule.Constants;
using Craft.MultitenancyModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.MultitenancyModule.Extensions;

public static class MultitenancyModuleConfigurationExtensions
{
    public static void ConfigureMultitenancy(this ModelBuilder builder)
    {
        const string prefix = CraftModuleConstants.PrefixAsCraft + "_tenants";
        builder.Entity<TenantEntity>().ToTable(prefix);

        builder.Entity<TenantEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Host).HasDefaultValue(null);
            entity.Property(e => e.Description).HasDefaultValue(null);
            entity.Property(e => e.ConnectionString).HasDefaultValueSql(null);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });
    }
}
