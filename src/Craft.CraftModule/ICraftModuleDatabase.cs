using Microsoft.EntityFrameworkCore;

namespace Craft.CraftModule;

/// <summary>
/// Represents a database configuration for a module.
/// </summary>
public interface ICraftModuleDatabase
{
    /// <summary>
    /// Configures the main application's database context.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> for the main <see cref="DbContext"/>.</param>
    void ConfigureModelBuilder(ModelBuilder modelBuilder);
}
