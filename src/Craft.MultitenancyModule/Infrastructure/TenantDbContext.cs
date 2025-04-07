using Craft.CraftModule.Infrastructure;
using Craft.MultitenancyModule.Domain.Entities;
using Craft.MultitenancyModule.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Craft.MultitenancyModule.Infrastructure;

public sealed class TenantDbContext(DbContextOptions<TenantDbContext> options) : CraftDbContext<TenantDbContext>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureMultitenancy();
    }
    
    public DbSet<TenantEntity> Tenants { get; set; }
}
