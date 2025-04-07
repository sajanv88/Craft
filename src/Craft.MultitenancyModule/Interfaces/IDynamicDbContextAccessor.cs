using Microsoft.EntityFrameworkCore;

namespace Craft.MultitenancyModule.Interfaces;

public interface IDynamicDbContextAccessor
{
    public DbContext GetDbContext();
}