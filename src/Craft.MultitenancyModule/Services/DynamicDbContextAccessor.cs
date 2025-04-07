using Craft.MultitenancyModule.Extensions;
using Craft.MultitenancyModule.Infrastructure;
using Craft.MultitenancyModule.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Craft.MultitenancyModule.Services;

public sealed class DynamicDbContextAccessor(
    IHttpContextAccessor httpContextAccessor,
    MultitenancyOtions multitenancyOtions,
    ILogger<DynamicDbContextAccessor> logger) : IDynamicDbContextAccessor
{
    private DbContext? _dbContext;

    public DbContext GetDbContext()
    {

        var httpContext = httpContextAccessor.HttpContext;
        var connectionString = httpContext?.Items["connectionString"]?.ToString();
        
        if (string.IsNullOrEmpty(connectionString))
        {
            logger.LogError("Connection string is not set in the HttpContext.");
            throw new InvalidOperationException("Connection string is not set.");
        }
        
        var qualifiedName = multitenancyOtions.DbContextType.AssemblyQualifiedName;
       _dbContext =  new DbContextTypeResolver().GetDbContext(qualifiedName, connectionString);
       if (_dbContext != null) return _dbContext;
       logger.LogError("Failed to create DbContext.");
        throw new InvalidOperationException("Failed to create DbContext.");

    }
    
}
