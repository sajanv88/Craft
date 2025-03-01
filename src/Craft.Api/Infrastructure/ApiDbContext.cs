using Craft.Api.Domain;
using Craft.CraftModule.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Craft.Api.Infrastructure;

public class ApiDbContext(DbContextOptions<ApiDbContext> options)
    : CraftDbContext<ApiDbContext>(options)
{
    public DbSet<TodoEntity> Todos { get; set; }
}
