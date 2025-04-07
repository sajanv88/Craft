using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Craft.MultitenancyModule.Infrastructure;

public sealed class DbContextTypeResolver
{
    public  DbContext GetDbContext(string dbContextQualifiedName, string newDbConnectionString)
    {
        var dbContextType = Type.GetType(dbContextQualifiedName);

        if (dbContextType is null)
        {
            throw new InvalidOperationException($"Could not resolve DbContext type from {dbContextQualifiedName}");
        }

        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
        var optionsBuilder = Activator.CreateInstance(optionsBuilderType) as DbContextOptionsBuilder;
        if (optionsBuilder is null)
        {
            throw new InvalidOperationException($"Could not create DbContextOptionsBuilder for {dbContextType}");
        }

        optionsBuilder.UseNpgsql(newDbConnectionString);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));


        var ctor = dbContextType.GetConstructor([optionsBuilder.Options.GetType()]);
        if (ctor == null)
        {
            throw new MissingMethodException(
                $"No constructor found on {dbContextType.Name} that takes DbContextOptions.");
        }


        var dbContext = ctor.Invoke([optionsBuilder.Options]) as DbContext;
        if (dbContext == null)
        {
            throw new InvalidOperationException($"Could not create DbContext of type {dbContextType}");
        }

        return dbContext;

    }

    public async Task<DbContext> MigrateAsync(string dbContextQualifiedName, string newDbConnectionString,
        CancellationToken cancellationToken = default)
    {
        
        var dbContext = GetDbContext(dbContextQualifiedName, newDbConnectionString);
        await using (dbContext)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        return dbContext;
    }
    
    
}