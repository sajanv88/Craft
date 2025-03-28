using Craft.Api.Domain;
using Craft.CraftModule.Infrastructure;
using Craft.LocalizationModule.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Craft.Api.Infrastructure;

public class ApiDbContext(DbContextOptions<ApiDbContext> options)
    : CraftDbContext<ApiDbContext>(options)
{
    public DbSet<TodoEntity> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureLocalization();
    }
}
